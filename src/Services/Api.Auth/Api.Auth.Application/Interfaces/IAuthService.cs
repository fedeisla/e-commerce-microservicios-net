using Api.Auth.Application.DTOs;

namespace Api.Auth.Application.Interfaces;

public interface IAuthService
{
    Task<string> RegistrarUsuarioAsync(RegistroDto dto);
    Task<string> LoginAsync(LoginDto dto);
}