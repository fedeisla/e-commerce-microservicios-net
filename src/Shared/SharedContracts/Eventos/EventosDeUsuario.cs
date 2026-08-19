namespace SharedContracts.Eventos;


public record UsuarioRegistradoEvent(
    Guid UsuarioId, 
    string Email, 
    string Nombre, 
    string Apellido
);

public record RolUsuarioActualizadoEvent(
    Guid UsuarioId, 
    string NuevoRol
);