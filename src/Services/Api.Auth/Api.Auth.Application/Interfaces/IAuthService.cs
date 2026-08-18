using Api.Auth.Application.DTOs;

namespace Api.Auth.Application.Interfaces;

public interface IAuthService
{
    Task<(Guid UsuarioId, string Mensaje)> RegistrarUsuarioAsync(RegistroDto dto);
    Task<string> LoginAsync(LoginDto dto);
}