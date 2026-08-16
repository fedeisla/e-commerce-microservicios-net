using Api.Pedidos.Application.DTOs;
using Api.Pedidos.Domain.Entities;
using Api.Pedidos.Domain.Enums;
using Api.Pedidos.Infrastructure.Persistence;
using MassTransit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SharedContracts.Eventos;
using System.Security.Claims;

namespace Api.Pedidos.WebAPI.Controllers;

[ApiController]
[Route("api/pedidos/carrito")]
[Authorize] 
public class CarritoController : ControllerBase
{
    private readonly PedidosDbContext _dbContext;
    private readonly IPublishEndpoint _publishEndpoint;

    public CarritoController(PedidosDbContext dbContext, IPublishEndpoint publishEndpoint)
    {
        _dbContext = dbContext;
        _publishEndpoint = publishEndpoint;
    }

    private Guid ObtenerUsuarioId()
    {
        var claimId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        
        if (!Guid.TryParse(claimId, out var usuarioId))
        {
            throw new UnauthorizedAccessException("Token inválido o sin ID de usuario.");
        }
        
        return usuarioId;
    }

    [HttpGet]
    public async Task<IActionResult> GetCarrito()
    {
        var usuarioId = ObtenerUsuarioId();

        var carrito = await _dbContext.Carritos
            .Include(c => c.Items)
            .FirstOrDefaultAsync(c => c.UsuarioId == usuarioId);

        if (carrito == null)
            return NotFound(new { mensaje = "El usuario no tiene un carrito activo." });

        return Ok(carrito);
    }

    [HttpPost("items")]
    public async Task<IActionResult> AgregarItem([FromBody] AgregarItemDto dto)
    {
        var usuarioId = ObtenerUsuarioId();

        var carrito = await _dbContext.Carritos
            .Include(c => c.Items)
            .FirstOrDefaultAsync(c => c.UsuarioId == usuarioId);

        if (carrito == null)
        {
            carrito = new Carrito 
            { 
                Id = Guid.NewGuid(), 
                UsuarioId = usuarioId, 
                FechaCreacion = DateTime.UtcNow 
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
            carrito.Items.Add(new CarritoItem
            {
                Id = Guid.NewGuid(),
                CarritoId = carrito.Id,
                ProductoId = dto.ProductoId,
                ProductoNombre = dto.ProductoNombre,
                PrecioUnitario = dto.PrecioUnitario,
                Cantidad = dto.Cantidad
            });
        }

        carrito.FechaUltimaModificacion = DateTime.UtcNow;
        await _dbContext.SaveChangesAsync();

        return Ok(carrito);
    }

    [HttpDelete("items/{productoId}")]
    public async Task<IActionResult> QuitarItem(Guid productoId)
    {
        var usuarioId = ObtenerUsuarioId();

        var carrito = await _dbContext.Carritos
            .Include(c => c.Items)
            .FirstOrDefaultAsync(c => c.UsuarioId == usuarioId);

        if (carrito == null) return NotFound("Carrito no encontrado.");

        var item = carrito.Items.FirstOrDefault(i => i.ProductoId == productoId);
        
        if (item == null) return NotFound("El producto no está en el carrito.");

        _dbContext.CarritoItems.Remove(item);
        
        carrito.FechaUltimaModificacion = DateTime.UtcNow;
        await _dbContext.SaveChangesAsync();

        return NoContent(); 
    }

    [HttpPost("checkout")]
    public async Task<IActionResult> Checkout()
    {
        var usuarioId = ObtenerUsuarioId();

        // 1. Traemos el carrito con sus ítems
        var carrito = await _dbContext.Carritos
            .Include(c => c.Items)
            .FirstOrDefaultAsync(c => c.UsuarioId == usuarioId);

        if (carrito == null || !carrito.Items.Any())
            return BadRequest("El carrito está vacío o no existe.");

       
        using var transaction = await _dbContext.Database.BeginTransactionAsync();

        try
        {
            // 2. Convertir Carrito en Pedido Oficial
            var nuevoPedido = new Pedido
            {
                Id = Guid.NewGuid(),
                ClienteId = usuarioId, 
                FechaCreacion = DateTime.UtcNow,
                Estado = EstadoPedido.Pendiente,
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

            // 3. Vaciamos el carrito eliminando sus ítems
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

            return Ok(new { mensaje = "Checkout exitoso. Pedido en proceso.", pedidoId = nuevoPedido.Id });
        }
        catch (Exception)
        {
            await transaction.RollbackAsync();
            throw; 
        }
    }
}