using Api.Inventario.Application.DTOs.Movimientos;
using Api.Inventario.Application.DTOs.Productos;

namespace Api.Inventario.Application.Interfaces;

public interface IProductoService
{
    Task<IEnumerable<ProductoDto>> GetAllAsync();
    Task<ProductoDto?> GetByIdAsync(Guid id);
    Task<ProductoDto> CreateAsync(ProductoCreateDto dto);
    Task<ProductoDto?> UpdateAsync(Guid id, ProductoUpdateDto dto);
    Task<ProductoDto> RegistrarMovimientoStockAsync(RegistrarMovimientoDto dto);
}