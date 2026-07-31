using Api.Inventario.Application.DTOs.Categorias;
using Api.Inventario.Application.Interfaces;
using Api.Inventario.Domain.Entities;
using Api.Inventario.Domain.Interfaces;

namespace Api.Inventario.Application.Services;

public class CategoriaService : ICategoriaService
{
    private readonly IUnitOfWork _unitOfWork;

    public CategoriaService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IEnumerable<CategoriaDto>> GetAllAsync()
    {
        var categorias = await _unitOfWork.Categorias.GetAllAsync();
        
        return categorias.Select(c => new CategoriaDto
        {
            IdCategoria = c.IdCategoria,
            Nombre = c.Nombre
        });
    }

    public async Task<CategoriaDto?> GetByIdAsync(Guid id)
    {
        var categoria = await _unitOfWork.Categorias.GetByIdAsync(id);
        if (categoria == null) return null;

        return new CategoriaDto
        {
            IdCategoria = categoria.IdCategoria,
            Nombre = categoria.Nombre
        };
    }

    public async Task<CategoriaDto> CreateAsync(CategoriaCreateDto dto)
    {
        var nuevaCategoria = new Categoria
        {
            IdCategoria = Guid.NewGuid(),
            Nombre = dto.Nombre
        };

        await _unitOfWork.Categorias.AddAsync(nuevaCategoria);
        await _unitOfWork.SaveChangesAsync(); // Guardamos en Postgres

        return new CategoriaDto
        {
            IdCategoria = nuevaCategoria.IdCategoria,
            Nombre = nuevaCategoria.Nombre
        };
    }
}