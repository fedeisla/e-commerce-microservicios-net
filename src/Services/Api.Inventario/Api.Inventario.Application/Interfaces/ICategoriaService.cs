using Api.Inventario.Application.DTOs.Categorias;

namespace Api.Inventario.Application.Interfaces;

public interface ICategoriaService
{
    Task<IEnumerable<CategoriaDto>> GetAllAsync();
    Task<CategoriaDto?> GetByIdAsync(Guid id);
    Task<CategoriaDto> CreateAsync(CategoriaCreateDto dto);
}