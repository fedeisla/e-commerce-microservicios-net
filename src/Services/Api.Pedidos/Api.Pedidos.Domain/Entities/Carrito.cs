namespace Api.Pedidos.Domain.Entities; // Ajustá el namespace a tu estructura

public class Carrito
{
    public Guid Id { get; set; }
    
    public Guid UsuarioId { get; set; } 
    
    public DateTime FechaCreacion { get; set; }
    public DateTime? FechaUltimaModificacion { get; set; }

  
    public List<CarritoItem> Items { get; set; } = new();
}