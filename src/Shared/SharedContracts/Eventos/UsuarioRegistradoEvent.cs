namespace SharedContracts.Eventos;

public record UsuarioRegistradoEvent
{
    public Guid UsuarioId { get; init; }
    public string Email { get; init; } = string.Empty;
    public DateTime FechaRegistro { get; init; }
}