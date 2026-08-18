using MassTransit;
using SharedContracts.Eventos;

namespace Api.Notificaciones.Consumers;

public class PedidoCreadoConsumer : IConsumer<PedidoCreadoEvent>
{
    private readonly ILogger<PedidoCreadoConsumer> _logger;

    public PedidoCreadoConsumer(ILogger<PedidoCreadoConsumer> logger)
    {
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<PedidoCreadoEvent> context)
    {
        var evento = context.Message;

        
      
        _logger.LogInformation("SIMULANDO ENVÍO DE EMAIL DE CONFIRMACIÓN");
        _logger.LogInformation($"Para el Usuario ID: {evento.UsuarioId}");
        _logger.LogInformation($"Pedido ID: {evento.PedidoId}");
        _logger.LogInformation($"Cantidad de productos diferentes: {evento.Items?.Count ?? 0}");


        await Task.Delay(2000); 

        _logger.LogInformation("✅ Email enviado con éxito.");
    }
}