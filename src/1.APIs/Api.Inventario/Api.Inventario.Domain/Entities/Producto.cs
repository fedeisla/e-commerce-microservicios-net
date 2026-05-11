using Api.Inventario.Domain.Enums;

namespace Api.Inventario.Domain.Entities;

public class Producto
{
    public Guid IdProducto { get; set; }
    public required string Nombre { get; set; }
    public decimal Precio { get; set; }
    public required string SKU { get; set; }
    public int StockActual { get; set; }
    public ProductoEstado Estado { get; set; }

    
    public Guid IdCategoria { get; set; }
    public virtual Categoria? Categoria { get; set; }

    public virtual ICollection<MovimientoStock> Movimientos { get; set; } = new List<MovimientoStock>();
}