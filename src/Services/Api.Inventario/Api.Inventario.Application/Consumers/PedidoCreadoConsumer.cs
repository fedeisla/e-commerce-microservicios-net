using Api.Inventario.Application.Services;
using MassTransit;
using Microsoft.Extensions.Logging;
using SharedContracts.Eventos;

namespace Api.Inventario.Application.Consumers;

public class PedidoCreadoConsumer : IConsumer<PedidoCreadoEvent>
{
    private readonly ILogger<PedidoCreadoConsumer> _logger;
    private readonly IInventarioService _inventarioService;

    public PedidoCreadoConsumer(
        ILogger<PedidoCreadoConsumer> logger, 
        IInventarioService inventarioService)
    {
        _logger = logger;
        _inventarioService = inventarioService;
    }

    public async Task Consume(ConsumeContext<PedidoCreadoEvent> context)
    {
        var mensaje = context.Message;
        
        _logger.LogInformation("Procesando pedido {PedidoId} para el producto {ProductoId} (Cantidad: {Cantidad})", 
            mensaje.PedidoId, mensaje.ProductoId, mensaje.Cantidad);

        // Intentamos descontar stock usando la base de datos real
        var (exito, motivo) = await _inventarioService.DescontarStockAsync(mensaje.ProductoId, mensaje.Cantidad);

        if (exito)
        {
            _logger.LogInformation("¡Stock confirmado! Emitiendo evento StockConfirmadoEvent para el pedido {PedidoId}", mensaje.PedidoId);
            await context.Publish(new StockConfirmadoEvent(mensaje.PedidoId));
        }
        else
        {
            _logger.LogWarning("Stock rechazado: {Motivo}. Emitiendo StockRechazadoEvent para el pedido {PedidoId}", motivo, mensaje.PedidoId);
            await context.Publish(new StockRechazadoEvent(mensaje.PedidoId, motivo));
        }
    }
}