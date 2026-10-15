using MassTransit;
using Api.Notificaciones.Consumers;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<PedidoCreadoConsumer>();
    x.AddConsumer<UsuarioRegistradoConsumer>();
    x.AddConsumer<StockConfirmadoConsumer>();
    x.AddConsumer<StockRechazadoConsumer>();

    x.UsingRabbitMq((context, cfg) =>
    {
        var configuration = context.GetRequiredService<IConfiguration>();
        var host = configuration["RabbitMQ:Host"] ?? "localhost";
        var username = configuration["RabbitMQ:Username"] ?? "guest";
        var password = configuration["RabbitMQ:Password"] ?? "guest";

        cfg.Host(host, "/", h =>
        {
            h.Username(username);
            h.Password(password);
        });
        cfg.ReceiveEndpoint("notificaciones-pedido-creado", e =>
        {
            e.ConfigureConsumer<PedidoCreadoConsumer>(context);
        });

        cfg.ReceiveEndpoint("notificaciones-usuario-registrado", e =>
        {
            e.ConfigureConsumer<UsuarioRegistradoConsumer>(context);
        });

        cfg.ReceiveEndpoint("notificaciones-stock-confirmado", e =>
        {
            e.ConfigureConsumer<StockConfirmadoConsumer>(context);
        });

        cfg.ReceiveEndpoint("notificaciones-stock-rechazado", e =>
        {
            e.ConfigureConsumer<StockRechazadoConsumer>(context);
        });
    });
});

var app = builder.Build();


app.MapGet("/health", () => Results.Ok(new { status = "Healthy", service = "Api.Notificaciones (Stateless)" }));

app.Run();