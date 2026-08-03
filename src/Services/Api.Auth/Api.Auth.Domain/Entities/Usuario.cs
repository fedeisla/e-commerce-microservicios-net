using Api.Auth.Domain.Enums;

namespace Api.Auth.Domain.Entities;

public class Usuario
{
    public Guid Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Apellido { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    
    public string PasswordHash { get; set; } = string.Empty; 
    
    public Rol Rol { get; set; } = Rol.Cliente;
    public DateTime FechaRegistro { get; set; }
}