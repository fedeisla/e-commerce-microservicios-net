using Api.Inventario.Application.DTOs.Movimientos;

namespace Api.Inventario.Application.Interfaces;

public interface IMovimientoStockService
{
    Task<IEnumerable<MovimientoStockDto>> GetHistorialByProductoAsync(Guid idProducto);
}