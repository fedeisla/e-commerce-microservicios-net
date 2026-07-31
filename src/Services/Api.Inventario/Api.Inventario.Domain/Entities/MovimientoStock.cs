using Api.Inventario.Domain.Enums;

namespace Api.Inventario.Domain.Entities;

public class MovimientoStock
{
    public Guid IdMovimiento { get; set; }
    public int Cantidad { get; set; }
    public DateTime Fecha { get; set; }
    public required string Motivo { get; set; }
    public TipoMovimientoStock TipoMovimiento { get; set; }

    public Guid IdProducto { get; set; }
    public virtual Producto? Producto { get; set; }
}