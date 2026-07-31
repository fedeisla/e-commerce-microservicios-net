using Api.Inventario.Domain.Entities;

namespace Api.Inventario.Domain.Interfaces;

public interface IMovimientoStockRepository
{
    Task AddAsync(MovimientoStock movimiento);
    Task<IEnumerable<MovimientoStock>> GetByProductoIdAsync(Guid productoId);
}