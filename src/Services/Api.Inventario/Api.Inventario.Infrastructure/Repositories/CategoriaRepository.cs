using Api.Inventario.Domain.Entities;
using Api.Inventario.Domain.Interfaces;
using Api.Inventario.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Api.Inventario.Infrastructure.Repositories;

public class CategoriaRepository : ICategoriaRepository
{
    private readonly InventarioDbContext _context;
    public CategoriaRepository(InventarioDbContext context) => _context = context;

    public async Task<Categoria?> GetByIdAsync(Guid id) => await _context.Categorias.FindAsync(id);
    public async Task<IEnumerable<Categoria>> GetAllAsync() => await _context.Categorias.ToListAsync();
    public async Task AddAsync(Categoria categoria) => await _context.Categorias.AddAsync(categoria);
}