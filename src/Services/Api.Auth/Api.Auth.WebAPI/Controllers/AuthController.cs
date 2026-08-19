using Api.Auth.Application.DTOs;
using Api.Auth.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using MassTransit;
using SharedContracts.Eventos;  

namespace Api.Auth.WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly IPublishEndpoint _publishEndpoint;

    public AuthController(IAuthService authService, IPublishEndpoint publishEndpoint)
    {
        _authService = authService;
        _publishEndpoint = publishEndpoint;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegistroDto dto)
    {
        try
        {
           
           var (usuarioId, mensaje) = await _authService.RegistrarUsuarioAsync(dto);

            await _publishEndpoint.Publish(new UsuarioRegistradoEvent(
                usuarioId, 
                dto.Email, 
                dto.Nombre, 
                dto.Apellido
            ));

            return Ok(new { mensaje = mensaje });
        }
        catch (Exception ex)
        {
            
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