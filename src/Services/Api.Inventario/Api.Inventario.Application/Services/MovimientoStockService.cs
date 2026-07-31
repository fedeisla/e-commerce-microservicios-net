using Api.Inventario.Application.DTOs.Movimientos;
using Api.Inventario.Application.Interfaces;
using Api.Inventario.Domain.Interfaces;

namespace Api.Inventario.Application.Services;

public class MovimientoStockService : IMovimientoStockService
{
    private readonly IUnitOfWork _unitOfWork;

    public MovimientoStockService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IEnumerable<MovimientoStockDto>> GetHistorialByProductoAsync(Guid idProducto)
    {
        var historial = await _unitOfWork.Movimientos.GetByProductoIdAsync(idProducto);

        return historial.Select(m => new MovimientoStockDto
        {
            IdMovimiento = m.IdMovimiento,
            IdProducto = m.IdProducto,
            Motivo = m.Motivo,
            Cantidad = m.Cantidad,
            Fecha = m.Fecha
        }).OrderByDescending(m => m.Fecha);
    }
}