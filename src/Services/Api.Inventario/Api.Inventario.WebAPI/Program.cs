using Api.Inventario.Domain.Interfaces;
using Api.Inventario.Infrastructure.Data;
using Api.Inventario.Infrastructure.Repositories;
using Api.Inventario.Application.Interfaces; 
using Api.Inventario.Application.Services;
using Microsoft.EntityFrameworkCore;
using MassTransit;
using Api.Inventario.Infrastructure.Services;

var builder = WebApplication.CreateBuilder(args);

// 1. Configurar Entity Framework Core con PostgreSQL
builder.Services.AddDbContext<InventarioDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// 2. Inyectar el Unit of Work (El director de orquesta)
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// 3. Inyectar los Servicios de Aplicación (La lógica de negocio)
builder.Services.AddScoped<IProductoService, ProductoService>();
builder.Services.AddScoped<ICategoriaService, CategoriaService>();
builder.Services.AddScoped<IMovimientoStockService, MovimientoStockService>(); 
builder.Services.AddScoped<IInventarioService, InventarioService>();



builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<Api.Inventario.Application.Consumers.PedidoCreadoConsumer>();
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

        // 2. Creamos la cola específica en RabbitMQ y le enchufamos el Consumer
        cfg.ReceiveEndpoint("inventario-pedido-creado", e =>
        {
            e.ConfigureConsumer<Api.Inventario.Application.Consumers.PedidoCreadoConsumer>(context);
        });
    });
});

// 4. Configuración de Controladores y Swagger
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// 5. Configurar el pipeline HTTP
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
// Endpoint test para probar RabbitMQ
app.MapPost("/api/test-rabbitmq", async (IPublishEndpoint publishEndpoint) =>
{
    var eventoPrueba = new SharedContracts.Eventos.PedidoCreadoEvent(
        PedidoId: Guid.NewGuid(),
        ProductoId: Guid.Parse("4fa641ee-fc22-4bfd-a68d-293fa5459e09"), // El ID de tu producto en la DB
        Cantidad: 2
    );

    await publishEndpoint.Publish(eventoPrueba);
    
    return Results.Ok("¡Evento de pedido creado enviado a RabbitMQ!");
});

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();