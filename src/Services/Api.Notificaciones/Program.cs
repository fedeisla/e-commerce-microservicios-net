using MassTransit;
using Api.Notificaciones.Consumers;

var builder = WebApplication.CreateBuilder(args);

// Configuración de MassTransit (RabbitMQ)
builder.Services.AddMassTransit(x =>
{
    // Registramos nuestro Consumer
    x.AddConsumer<PedidoCreadoConsumer>();
    x.AddConsumer<UsuarioRegistradoConsumer>();

    x.UsingRabbitMq((context, cfg) =>
    {
        // Conexión al Docker de RabbitMQ local
        cfg.Host("localhost", "/", h => {
            h.Username("guest");
            h.Password("guest");
        });

        // Le decimos que escuche la cola de Notificaciones
        cfg.ReceiveEndpoint("notificaciones_pedido_creado", e =>
        {
            e.ConfigureConsumer<PedidoCreadoConsumer>(context);
             e.ConfigureConsumer<UsuarioRegistradoConsumer>(context);
        });
    });
});

var app = builder.Build();

app.MapGet("/", () => "Microservicio de Notificaciones escuchando a RabbitMQ... 🐰");

app.Run();