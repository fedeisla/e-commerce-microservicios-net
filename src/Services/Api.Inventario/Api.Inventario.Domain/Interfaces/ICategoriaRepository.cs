using Api.Inventario.Domain.Entities;

namespace Api.Inventario.Domain.Interfaces;

public interface ICategoriaRepository
{
    Task<Categoria?> GetByIdAsync(Guid id);
    Task<IEnumerable<Categoria>> GetAllAsync();
    Task AddAsync(Categoria categoria);
}