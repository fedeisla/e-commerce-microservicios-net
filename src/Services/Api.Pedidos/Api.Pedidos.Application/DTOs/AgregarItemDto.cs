namespace Api.Pedidos.Application.DTOs;

public class AgregarItemDto
{
    public Guid ProductoId { get; set; }
    public string ProductoNombre { get; set; } = string.Empty;
    public decimal PrecioUnitario { get; set; }
    public int Cantidad { get; set; }
}