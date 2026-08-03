using Api.Pedidos.Domain.Enums;

namespace Api.Pedidos.Domain.Entities;

public class Pedido
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid ClienteId { get; set; } 
    public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;
    public EstadoPedido Estado { get; set; } = EstadoPedido.Pendiente;
    public decimal Total { get; set; }

    public List<DetallePedido> Detalles { get; set; } = new();
}