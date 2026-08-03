using Api.Pedidos.Application.DTOs;
using Api.Pedidos.Application.Interfaces;
using Api.Pedidos.Domain.Entities;
using Api.Pedidos.Domain.Enums;
using Api.Pedidos.Infrastructure.Persistence;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using SharedContracts.Eventos; 

namespace Api.Pedidos.Infrastructure.Services;

public class PedidoService : IPedidoService
{
    private readonly PedidosDbContext _context;
    private readonly IPublishEndpoint _publishEndpoint;

    public PedidoService(PedidosDbContext context, IPublishEndpoint publishEndpoint)
    {
        _context = context;
        _publishEndpoint = publishEndpoint;
    }

    public async Task<Pedido> CrearPedidoAsync(PedidoCreateDto dto)
    {
       
        var pedido = new Pedido
        {
            ClienteId = dto.ClienteId,
            Estado = EstadoPedido.Pendiente, 
            Detalles = dto.Detalles.Select(d => new DetallePedido
            {
                ProductoId = d.ProductoId,
                Cantidad = d.Cantidad,
                PrecioUnitario = d.PrecioUnitario
            }).ToList()
        };

        
        pedido.Total = pedido.Detalles.Sum(d => d.Cantidad * d.PrecioUnitario);

       
        _context.Pedidos.Add(pedido);
        await _context.SaveChangesAsync();

        // Disparamos los eventos a RabbitMQ
        // Como nuestro PedidoCreadoEvent acepta un solo ProductoId, 
        // disparamos un evento por cada producto distinto que haya en el carrito.
        foreach (var detalle in pedido.Detalles)
        {
            var evento = new PedidoCreadoEvent(
                PedidoId: pedido.Id,
                ProductoId: detalle.ProductoId,
                Cantidad: detalle.Cantidad
            );

            await _publishEndpoint.Publish(evento);
        }

        return pedido; 
    }

   public async Task<Pedido?> ObtenerPedidoPorIdAsync(Guid id)
    {
        return await _context.Pedidos
            .Include(p => p.Detalles)
            .FirstOrDefaultAsync(p => p.Id == id);
    }
}