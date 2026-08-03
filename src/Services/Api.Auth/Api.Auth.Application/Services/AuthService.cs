using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Api.Auth.Application.DTOs;
using Api.Auth.Application.Interfaces;
using Api.Auth.Domain.Entities;
using Api.Auth.Domain.Enums;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Api.Auth.Application.Services;

public class AuthService : IAuthService
{
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly IConfiguration _configuration;

    public AuthService(IUsuarioRepository usuarioRepository,IConfiguration configuration)
    {
        _usuarioRepository = usuarioRepository;
        _configuration = configuration;
    }

   public async Task<string> LoginAsync(LoginDto dto)
    {
        // Buscar al usuario por Email
        var usuario = await _usuarioRepository.ObtenerPorEmailAsync(dto.Email);
        
        
        if (usuario == null)
            throw new Exception("Credenciales inválidas.");

        // Verificar la contraseña ingresada contra el Hash de la base de datos
        bool passwordValida = BCrypt.Net.BCrypt.Verify(dto.Password, usuario.PasswordHash);
        if (!passwordValida)
            throw new Exception("Credenciales inválidas.");

        // Crear los Claims (la información pública que viaja dentro del token)
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, usuario.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, usuario.Email),
            new Claim(ClaimTypes.Role, usuario.Rol.ToString()), 
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        // Leer la clave y configuración desde el appsettings.json
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var expiracion = DateTime.UtcNow.AddMinutes(double.Parse(_configuration["Jwt:ExpireMinutes"]!));

        // Fabricar el token
        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: expiracion,
            signingCredentials: creds
        );

        
        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public async Task<string> RegistrarUsuarioAsync(RegistroDto dto)
    {
        //Verificar si el email ya existe
        var usuarioExistente = await _usuarioRepository.ObtenerPorEmailAsync(dto.Email);
        if (usuarioExistente != null)
        {
            throw new Exception("El email ya está registrado.");
        }

        //Encriptar la contraseña
        string passwordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);

        // mapeamos la entidad
        var nuevoUsuario = new Usuario
        {
            Id = Guid.NewGuid(),
            Nombre = dto.Nombre,
            Apellido = dto.Apellido,
            Email = dto.Email,
            PasswordHash = passwordHash,
            Rol = Rol.Cliente, 
            FechaRegistro = DateTime.UtcNow
        };

        
        await _usuarioRepository.AgregarAsync(nuevoUsuario);

        return "Usuario registrado con éxito.";
    }
}