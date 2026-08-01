using Api.Inventario.Application.Services;
using Api.Inventario.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Api.Inventario.Infrastructure.Services;

public class InventarioService : IInventarioService
{
    private readonly InventarioDbContext _context;

    public InventarioService(InventarioDbContext context)
    {
        _context = context;
    }

    public async Task<(bool Exito, string Motivo)> DescontarStockAsync(Guid productoId, int cantidad)
    {
        var producto = await _context.Productos.FindAsync(productoId);

        if (producto == null)
        {
            return (false, $"El producto con ID {productoId} no fue encontrado.");
        }

        if (producto.StockActual < cantidad)
        {
            return (false, $"Stock insuficiente. Disponible: {producto.StockActual}, Solicitado: {cantidad}");
        }

        // Descontamos el stock de la entidad
        producto.StockActual -= cantidad;

        _context.Productos.Update(producto);
        await _context.SaveChangesAsync();

        return (true, string.Empty);
    }
}