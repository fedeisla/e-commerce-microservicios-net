namespace Api.Pedidos.Domain.Entities;

public class DetallePedido
{
    public Guid Id { get; set; } = Guid.NewGuid();
    
    public Guid PedidoId { get; set; }
    public Pedido Pedido { get; set; } = null!;
    
    public Guid ProductoId { get; set; }
    public int Cantidad { get; set; }
    public decimal PrecioUnitario { get; set; }
}