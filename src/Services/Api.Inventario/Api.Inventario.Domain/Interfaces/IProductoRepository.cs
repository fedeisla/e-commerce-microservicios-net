using Api.Inventario.Domain.Entities;

namespace Api.Inventario.Domain.Interfaces;

public interface IProductoRepository
{
    Task<Producto?> GetByIdAsync(Guid id);
    Task<Producto?> GetBySkuAsync(string sku); 
    Task<IEnumerable<Producto>> GetAllAsync();
    Task AddAsync(Producto producto);
    void Update(Producto producto); 
    Task<bool> ExisteSkuAsync(string sku);
}