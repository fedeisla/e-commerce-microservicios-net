using Api.Auth.Domain.Entities;

namespace Api.Auth.Application.Interfaces;

public interface IUsuarioRepository
{
    Task<Usuario?> ObtenerPorEmailAsync(string email);
    Task AgregarAsync(Usuario usuario);
}