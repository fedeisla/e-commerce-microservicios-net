using Api.Inventario.Domain.Entities;
using Api.Inventario.Domain.Interfaces;
using Api.Inventario.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Api.Inventario.Infrastructure.Repositories;

public class ProductoRepository : IProductoRepository
{
    private readonly InventarioDbContext _context;
    public ProductoRepository(InventarioDbContext context) => _context = context;

    public async Task<Producto?> GetByIdAsync(Guid id) => await _context.Productos.Include(p => p.Categoria).FirstOrDefaultAsync(p => p.IdProducto == id);
    public async Task<Producto?> GetBySkuAsync(string sku) => await _context.Productos.FirstOrDefaultAsync(p => p.SKU == sku);
    public async Task<IEnumerable<Producto>> GetAllAsync() => await _context.Productos.ToListAsync();
    public async Task AddAsync(Producto producto) => await _context.Productos.AddAsync(producto);
    public void Update(Producto producto) => _context.Productos.Update(producto);
    public async Task<bool> ExisteSkuAsync(string sku) => await _context.Productos.AnyAsync(p => p.SKU == sku);
}