namespace Api.Inventario.Domain.Interfaces;

public interface IUnitOfWork : IDisposable
{
    // Acceso a los repositorios (opcional, pero muy cómodo)
    IProductoRepository Productos { get; }
    ICategoriaRepository Categorias { get; }
    IMovimientoStockRepository Movimientos { get; }

    // El método que confirma todo en una sola transacción
    Task<int> SaveChangesAsync();
}