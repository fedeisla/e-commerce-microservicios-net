using Api.Auth.Application.DTOs;
using Api.Auth.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
namespace Api.Auth.WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegistroDto dto)
    {
        try
        {
            var resultado = await _authService.RegistrarUsuarioAsync(dto);
            return Ok(new { mensaje = resultado });
        }
        catch (Exception ex)
        {
            // Atrapamos la excepción que lanzamos en el servicio si el mail ya existe
            return BadRequest(new { error = ex.Message });
        }
    }
    [HttpPost("login")]
    [EnableRateLimiting("LoginLimiter")]
    public async Task<IActionResult> Login([FromBody] LoginDto dto)
    {
        try
        {
            var token = await _authService.LoginAsync(dto);
            
            // Devolvemos el token en un formato JSON estándar
            return Ok(new { token = token }); 
        }
        catch (Exception ex)
        {
            // Si el servicio tira la excepción de "Credenciales inválidas", devolvemos un 401 Unauthorized
            return Unauthorized(new { error = ex.Message });
        }
    }
}