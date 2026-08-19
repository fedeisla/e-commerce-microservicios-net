using Api.Pedidos.Domain.Enums;
using Api.Pedidos.Infrastructure.Persistence;
using MassTransit;
using Microsoft.Extensions.Logging;
using SharedContracts.Eventos;

namespace Api.Pedidos.Infrastructure.Consumers;

public class StockConfirmadoConsumer : IConsumer<StockConfirmadoEvent>
{
    private readonly PedidosDbContext _context;
    private readonly ILogger<StockConfirmadoConsumer> _logger;

    
    public StockConfirmadoConsumer(PedidosDbContext context, ILogger<StockConfirmadoConsumer> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<StockConfirmadoEvent> context)
    {
        var pedidoId = context.Message.PedidoId;
        _logger.LogInformation("Procesando confirmación de stock para el Pedido: {PedidoId}", pedidoId);

        var pedido = await _context.Pedidos.FindAsync(pedidoId);
        
        // 2. Control de nulos estricto
        if (pedido == null)
        {
            _logger.LogError("Inconsistencia: No se encontró el Pedido {PedidoId} al intentar confirmar el stock.", pedidoId);
            // Lanzar excepción asegura que MassTransit reintente o mande el mensaje a la Dead Letter Queue (DLQ)
            throw new Exception($"Pedido {pedidoId} no encontrado."); 
        }

        // 3. Idempotencia y validación de estado
        if (pedido.Estado == EstadoPedido.Confirmado)
        {
            _logger.LogInformation("Idempotencia: El Pedido {PedidoId} ya se encontraba confirmado. Ignorando evento.", pedidoId);
            return; // Salimos sin hacer ruido ni tocar la BD
        }

        if (pedido.Estado != EstadoPedido.Pendiente)
        {
            _logger.LogWarning("El Pedido {PedidoId} está en estado {Estado}, no se puede confirmar. Ignorando.", pedidoId, pedido.Estado);
            return; 
        }

        try
        {
            // 4. Delegamos el cambio a la Entidad (DDD) en vez de cambiar la propiedad directamente
            // pedido.Estado = EstadoPedido.Confirmado; <-- EVITAR ESTO
            pedido.ConfirmarStock(); // <-- USAR ESTO (Método en tu entidad Pedido)
            
            await _context.SaveChangesAsync();
            _logger.LogInformation("Stock confirmado exitosamente para el Pedido: {PedidoId}", pedidoId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error crítico al guardar la confirmación del Pedido {PedidoId}", pedidoId);
            throw; // Re-lanzar para que MassTransit haga lo suyo
        }
    }
}