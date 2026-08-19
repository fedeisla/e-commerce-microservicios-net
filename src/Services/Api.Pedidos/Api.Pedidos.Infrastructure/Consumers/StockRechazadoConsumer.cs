using Api.Pedidos.Domain.Enums;
using Api.Pedidos.Infrastructure.Persistence;
using MassTransit;
using Microsoft.Extensions.Logging;
using SharedContracts.Eventos;

namespace Api.Pedidos.Infrastructure.Consumers;

public class StockRechazadoConsumer : IConsumer<StockRechazadoEvent>
{
    private readonly PedidosDbContext _context;
    private readonly ILogger<StockRechazadoConsumer> _logger;

    public StockRechazadoConsumer(PedidosDbContext context, ILogger<StockRechazadoConsumer> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<StockRechazadoEvent> context)
    {
        var pedidoId = context.Message.PedidoId;
        _logger.LogInformation("Procesando rechazo de stock para el Pedido: {PedidoId}. Motivo: {Motivo}", 
            pedidoId, context.Message.MotivoRechazo ?? "No especificado");

        var pedido = await _context.Pedidos.FindAsync(pedidoId);
        
        // 1. Control de nulos estricto
        if (pedido == null)
        {
            _logger.LogError("Inconsistencia: No se encontró el Pedido {PedidoId} al intentar rechazar el stock.", pedidoId);
            throw new Exception($"Pedido {pedidoId} no encontrado."); 
        }

        // 2. Idempotencia: Si ya está rechazado, no hacemos nada
        if (pedido.Estado == EstadoPedido.Rechazado)
        {
            _logger.LogInformation("Idempotencia: El Pedido {PedidoId} ya se encontraba rechazado. Ignorando evento.", pedidoId);
            return;
        }

        // 3. Control de flujo: Solo un pedido pendiente puede ser rechazado por falta de stock
        if (pedido.Estado != EstadoPedido.Pendiente)
        {
            _logger.LogWarning("El Pedido {PedidoId} está en estado {Estado}, no se puede rechazar por stock. Ignorando.", 
                pedidoId, pedido.Estado);
            return; 
        }

        try
        {
            // 4. Delegamos el cambio a la Entidad (DDD)
            pedido.RechazarStock(); 
            
            await _context.SaveChangesAsync();
            _logger.LogInformation("Pedido {PedidoId} actualizado a Rechazado exitosamente.", pedidoId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error crítico al guardar el rechazo del Pedido {PedidoId}", pedidoId);
            throw;
        }
    }
}