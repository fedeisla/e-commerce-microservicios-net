using Api.Auth.Application.Interfaces;
using Api.Auth.Application.Services;
using Api.Auth.Infrastructure.Persistence;
using Api.Auth.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.RateLimiting;
using System.Threading.RateLimiting;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers();
builder.Services.AddRateLimiter(options =>
{
    // Le decimos que devuelva un error 429 (Too Many Requests) cuando se pasen del límite
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

    // Creamos una política específica para el Login
    options.AddFixedWindowLimiter("LoginLimiter", opt =>
    {
        opt.PermitLimit = 5; 
        opt.Window = TimeSpan.FromMinutes(1); // En una ventana de 1 minuto
        opt.QueueLimit = 0; // Si se pasa de 5, rebota automáticamente sin ponerlo en cola
    });
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


builder.Services.AddDbContext<AuthDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));


builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();
builder.Services.AddScoped<IAuthService, AuthService>();

var app = builder.Build();


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();
app.UseRateLimiter();
app.MapControllers();

app.Run();