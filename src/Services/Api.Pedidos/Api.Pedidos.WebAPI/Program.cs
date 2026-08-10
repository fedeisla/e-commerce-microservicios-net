using Api.Pedidos.Infrastructure.Persistence;
using Api.Pedidos.Application.Interfaces;
using Api.Pedidos.Infrastructure.Services;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Api.Pedidos.Infrastructure.Consumers;
using Api.Pedidos.WebAPI.Consumers;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddDbContext<PedidosDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));


builder.Services.AddScoped<IPedidoService, PedidoService>();



builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<StockConfirmadoConsumer>();
    x.AddConsumer<StockRechazadoConsumer>();
    x.AddConsumer<UsuarioRegistradoConsumer>();
    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host("localhost", "/", h =>
        {
            h.Username("guest");
            h.Password("guest");
        });

        cfg.ConfigureEndpoints(context); 
    });
});

builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();
app.MapControllers();

app.Run();