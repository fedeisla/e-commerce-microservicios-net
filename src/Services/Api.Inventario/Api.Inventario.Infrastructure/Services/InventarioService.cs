using Api.Inventario.Application.Services;
using Api.Inventario.Domain.Entities;
using Api.Inventario.Domain.Enums;
using Api.Inventario.Infrastructure.Data;

namespace Api.Inventario.Infrastructure.Services;

public class InventarioService : IInventarioService
{
    private readonly InventarioDbContext _context;

    public InventarioService(InventarioDbContext context)
    {
        _context = context;
    }

    public async Task<bool> AgregarStockAsync(Guid productoId, int cantidad)
    {
        var producto = await _context.Productos.FindAsync(productoId);

        if (producto == null) return false;

        // 1. Sumamos el stock
        producto.StockActual += cantidad;
        _context.Productos.Update(producto);

        // 2. Registramos el movimiento
        var movimiento = new MovimientoStock
        {
            IdProducto = productoId,
            Cantidad = cantidad,
            TipoMovimiento = TipoMovimientoStock.AjustePositivo,
            Fecha = DateTime.UtcNow,
            Motivo = "Ingreso de stock / Compensación"
        };
        _context.MovimientosStock.Add(movimiento);

        // 3. Guardamos todo
        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<(bool Exito, string Motivo)> DescontarStockAsync(Guid productoId, int cantidad)
    {
        var producto = await _context.Productos.FindAsync(productoId);

        if (producto == null) return (false, $"El producto no fue encontrado.");
        if (producto.StockActual < cantidad) return (false, $"Stock insuficiente.");

        // 1. Descontamos el stock
        producto.StockActual -= cantidad;
        _context.Productos.Update(producto);

        // 2. Registramos el movimiento 
        var movimiento = new MovimientoStock 
        {
            IdProducto = productoId,
            Cantidad = cantidad,
            TipoMovimiento = TipoMovimientoStock.AjusteNegativo, 
            Fecha = DateTime.UtcNow,
            Motivo = "Venta por Pedido" 
        };
        _context.MovimientosStock.Add(movimiento); 

        // 3. Guardamos todo junto en la misma transacción
        await _context.SaveChangesAsync();

        return (true, string.Empty);
    }
}