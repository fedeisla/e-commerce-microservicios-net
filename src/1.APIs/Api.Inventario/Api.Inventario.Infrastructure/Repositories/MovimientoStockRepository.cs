using Api.Inventario.Domain.Entities;
using Api.Inventario.Domain.Interfaces;
using Api.Inventario.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Api.Inventario.Infrastructure.Repositories;

public class MovimientoStockRepository : IMovimientoStockRepository
{
    private readonly InventarioDbContext _context;
    public MovimientoStockRepository(InventarioDbContext context) => _context = context;

    public async Task AddAsync(MovimientoStock movimiento) => await _context.MovimientosStock.AddAsync(movimiento);
    public async Task<IEnumerable<MovimientoStock>> GetByProductoIdAsync(Guid productoId) => await _context.MovimientosStock.Where(m => m.IdProducto == productoId).ToListAsync();
}