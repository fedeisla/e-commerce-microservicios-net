namespace Api.Pedidos.Application.DTOs;

public class DetallePedidoCreateDto
{
    public Guid ProductoId { get; set; }
    public int Cantidad { get; set; }
    public decimal PrecioUnitario { get; set; }
}