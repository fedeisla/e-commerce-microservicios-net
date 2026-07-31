using Api.Inventario.Domain.Interfaces;
using Api.Inventario.Infrastructure.Data;
using Api.Inventario.Infrastructure.Repositories;
using Api.Inventario.Application.Interfaces; 
using Api.Inventario.Application.Services;
using Microsoft.EntityFrameworkCore;

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

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();