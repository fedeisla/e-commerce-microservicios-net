using MassTransit;
using SharedContracts.Eventos;

namespace Api.Notificaciones.Consumers;

public class UsuarioRegistradoConsumer : IConsumer<UsuarioRegistradoEvent>
{
    private readonly ILogger<UsuarioRegistradoConsumer> _logger;

    public UsuarioRegistradoConsumer(ILogger<UsuarioRegistradoConsumer> logger)
    {
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<UsuarioRegistradoEvent> context)
    {
        var evento = context.Message;

        _logger.LogInformation("SIMULANDO EMAIL DE BIENVENIDA");
        _logger.LogInformation($"Para: {evento.Email}");
        _logger.LogInformation($"Registrado el: {evento.FechaRegistro:dd/MM/yyyy HH:mm}");
        _logger.LogInformation($"Mensaje: ¡Hola! Tu cuenta fue creada exitosamente en nuestro E-Commerce.");
        

        await Task.Delay(2000); 
    }
}