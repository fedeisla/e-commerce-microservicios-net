using Api.Auth.Application.Interfaces;
using Api.Auth.Domain.Entities;
using Api.Auth.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Api.Auth.Infrastructure.Repositories;

public class UsuarioRepository : IUsuarioRepository
{
    private readonly AuthDbContext _context;

    public UsuarioRepository(AuthDbContext context)
    {
        _context = context;
    }

    public async Task<Usuario?> ObtenerPorEmailAsync(string email)
    {
        return await _context.Usuarios.FirstOrDefaultAsync(u => u.Email == email);
    }

    public async Task AgregarAsync(Usuario usuario)
    {
        await _context.Usuarios.AddAsync(usuario);
        await _context.SaveChangesAsync(); 
    }
}