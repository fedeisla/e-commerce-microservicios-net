using Api.Inventario.Domain.Interfaces;
using Api.Inventario.Infrastructure.Data;

namespace Api.Inventario.Infrastructure.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly InventarioDbContext _context;
    public IProductoRepository Productos { get; }
    public ICategoriaRepository Categorias { get; }
    public IMovimientoStockRepository Movimientos { get; }

    public UnitOfWork(InventarioDbContext context)
    {
        _context = context;
        Productos = new ProductoRepository(_context);
        Categorias = new CategoriaRepository(_context);
        Movimientos = new MovimientoStockRepository(_context);
    }

    public async Task<int> SaveChangesAsync() => await _context.SaveChangesAsync();
    public void Dispose() => _context.Dispose();
}