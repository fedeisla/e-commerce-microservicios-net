namespace Api.Pedidos.Domain.Entities;

public class CarritoItem
{
    public Guid Id { get; set; }
    
   
    public Guid CarritoId { get; set; }
    public Carrito Carrito { get; set; } = null!; 

    public Guid ProductoId { get; set; } 

    public string ProductoNombre { get; set; } = string.Empty;
    public decimal PrecioUnitario { get; set; }
    public int Cantidad { get; set; }
}