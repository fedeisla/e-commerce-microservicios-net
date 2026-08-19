using MassTransit;
using Microsoft.Extensions.Logging;
using SharedContracts.Eventos;

namespace Api.Notificaciones.Consumers;

public class StockRechazadoConsumer : IConsumer<StockRechazadoEvent>
{
    private readonly ILogger<StockRechazadoConsumer> _logger;

    public StockRechazadoConsumer(ILogger<StockRechazadoConsumer> logger)
    {
        _logger = logger;
    }

    public Task Consume(ConsumeContext<StockRechazadoEvent> context)
    {
        var mensaje = context.Message;

        // Simulamos el correo de cancelación detallando el motivo
        _logger.LogWarning(
            "\n========== SIMULACIÓN DE EMAIL ==========\n" +
            "Asunto: Hubo un problema con tu pedido\n" +
            "Pedido ID: {PedidoId}\n" +
            "Mensaje: Lo sentimos, tu orden no pudo ser procesada. \n" +
            "Motivo: {MotivoRechazo}\n" , 
            mensaje.PedidoId, 
            mensaje.MotivoRechazo);

        return Task.CompletedTask;
    }
}