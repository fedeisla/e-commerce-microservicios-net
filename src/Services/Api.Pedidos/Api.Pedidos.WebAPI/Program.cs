using Api.Pedidos.Infrastructure.Persistence;
using Api.Pedidos.Application.Interfaces;
using Api.Pedidos.Infrastructure.Services;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Api.Pedidos.Infrastructure.Consumers;
using Api.Pedidos.WebAPI.Consumers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.RateLimiting;


var builder = WebApplication.CreateBuilder(args);

// 1. Configuración de Base de Datos
builder.Services.AddDbContext<PedidosDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// 2. Inyección de Dependencias
builder.Services.AddScoped<IPedidoService, PedidoService>();
builder.Services.AddScoped<ICarritoService, CarritoService>();

// 3. Rate Limiting (Protección Anti-Spam para Checkout)
builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
    options.AddFixedWindowLimiter("CheckoutLimiter", opt =>
    {
        opt.PermitLimit = 3; // Máximo 3 intentos de checkout...
        opt.Window = TimeSpan.FromSeconds(30); // ...cada 30 segundos
        opt.QueueLimit = 0; 
    });
});

// 4. Configuración de MassTransit y RabbitMQ
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

// 5. Controladores y Serialización JSON
builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
});

// 6. Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configuración de Autenticación JWT (Zero Trust)
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
    };
});

builder.Services.AddAuthorization();

var app = builder.Build();



if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthentication();
app.UseAuthorization();
app.UseRateLimiter(); 

app.MapControllers();

app.Run();