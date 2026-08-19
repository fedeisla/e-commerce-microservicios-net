using MassTransit;
using Microsoft.Extensions.Logging;
using SharedContracts.Eventos;

namespace Api.Notificaciones.Consumers;

public class StockConfirmadoConsumer : IConsumer<StockConfirmadoEvent>
{
    private readonly ILogger<StockConfirmadoConsumer> _logger;

    public StockConfirmadoConsumer(ILogger<StockConfirmadoConsumer> logger)
    {
        _logger = logger;
    }

    public Task Consume(ConsumeContext<StockConfirmadoEvent> context)
    {
        var pedidoId = context.Message.PedidoId;

        // Simulamos el envío del email a través de los logs
        _logger.LogInformation(
            "\n========== SIMULACIÓN DE EMAIL ==========\n" +
            "Asunto: ¡Tu pedido ha sido confirmado!\n" +
            "Pedido ID: {PedidoId}\n" +
            "Mensaje: Buenas noticias. Hemos verificado el stock y estamos preparando tu orden para el envío.\n" +
            pedidoId);
        return Task.CompletedTask;
    }
}