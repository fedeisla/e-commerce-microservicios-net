using Api.Pedidos.Application.DTOs;
using Api.Pedidos.Application.Interfaces;
using Api.Pedidos.Domain.Entities;
using Api.Pedidos.Infrastructure.Persistence;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using SharedContracts.Eventos;

namespace Api.Pedidos.Infrastructure.Services;

public class CarritoService : ICarritoService
{
    private readonly PedidosDbContext _dbContext;
    private readonly IPublishEndpoint _publishEndpoint;

    public CarritoService(PedidosDbContext dbContext, IPublishEndpoint publishEndpoint)
    {
        _dbContext = dbContext;
        _publishEndpoint = publishEndpoint;
    }

    public async Task<Carrito?> ObtenerCarritoAsync(Guid usuarioId)
    {
        return await _dbContext.Carritos
            .Include(c => c.Items)
            .FirstOrDefaultAsync(c => c.UsuarioId == usuarioId);
    }

    public async Task<Carrito> AgregarItemAsync(Guid usuarioId, AgregarItemDto dto)
    {
        var carrito = await ObtenerCarritoAsync(usuarioId);

        if (carrito == null)
        {
            carrito = new Carrito 
            { 
                Id = Guid.NewGuid(), 
                UsuarioId = usuarioId, 
                FechaCreacion = DateTime.UtcNow,
                Items = new List<CarritoItem>()
            };
            _dbContext.Carritos.Add(carrito);
        } 

        var itemExistente = carrito.Items.FirstOrDefault(i => i.ProductoId == dto.ProductoId);

        if (itemExistente != null)
        {
            itemExistente.Cantidad += dto.Cantidad;
        }
        else
        {
            var nuevoItem = new CarritoItem
            {
                Id = Guid.NewGuid(),
                ProductoId = dto.ProductoId,
                ProductoNombre = dto.ProductoNombre,
                PrecioUnitario = dto.PrecioUnitario,
                Cantidad = dto.Cantidad
            };
            
            carrito.Items.Add(nuevoItem);
            _dbContext.Entry(nuevoItem).State = EntityState.Added;
        }

        carrito.FechaUltimaModificacion = DateTime.UtcNow;
        await _dbContext.SaveChangesAsync();

        return carrito;
    }

    public async Task QuitarItemAsync(Guid usuarioId, Guid productoId)
    {
        var carrito = await ObtenerCarritoAsync(usuarioId);
        if (carrito == null) 
            throw new KeyNotFoundException("Carrito no encontrado.");

        var item = carrito.Items.FirstOrDefault(i => i.ProductoId == productoId);
        if (item == null) 
            throw new KeyNotFoundException("El producto no está en el carrito.");

        _dbContext.CarritoItems.Remove(item);
        carrito.FechaUltimaModificacion = DateTime.UtcNow;
        
        await _dbContext.SaveChangesAsync();
    }

    public async Task<Guid> ProcesarCheckoutAsync(Guid usuarioId)
    {
        var carrito = await ObtenerCarritoAsync(usuarioId);

        if (carrito == null || !carrito.Items.Any())
            throw new InvalidOperationException("El carrito está vacío o no existe.");

        using var transaction = await _dbContext.Database.BeginTransactionAsync();

        try
        {
            var nuevoPedido = new Pedido
            {
                Id = Guid.NewGuid(),
                ClienteId = usuarioId, 
                FechaCreacion = DateTime.UtcNow,
                // El Estado nace "Pendiente" por defecto gracias a la Entidad
                Total = carrito.Items.Sum(i => i.PrecioUnitario * i.Cantidad),
                Detalles = carrito.Items.Select(i => new DetallePedido
                {
                    Id = Guid.NewGuid(),
                    ProductoId = i.ProductoId,
                    PrecioUnitario = i.PrecioUnitario,
                    Cantidad = i.Cantidad
                }).ToList()
            };

            _dbContext.Pedidos.Add(nuevoPedido);
            _dbContext.CarritoItems.RemoveRange(carrito.Items);
            carrito.FechaUltimaModificacion = DateTime.UtcNow;

            await _dbContext.SaveChangesAsync();
            await transaction.CommitAsync();

            var evento = new PedidoCreadoEvent(
                nuevoPedido.Id,
                usuarioId,
                nuevoPedido.Detalles.Select(d => new PedidoItemEvent(d.ProductoId, d.Cantidad)).ToList()
            );

            await _publishEndpoint.Publish(evento);

            return nuevoPedido.Id;
        }
        catch (Exception)
        {
            await transaction.RollbackAsync();
            throw; 
        }
    }
}