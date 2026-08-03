namespace Api.Pedidos.Application.DTOs;

public class PedidoCreateDto
{
    public Guid ClienteId { get; set; }
    public List<DetallePedidoCreateDto> Detalles { get; set; } = new();
}