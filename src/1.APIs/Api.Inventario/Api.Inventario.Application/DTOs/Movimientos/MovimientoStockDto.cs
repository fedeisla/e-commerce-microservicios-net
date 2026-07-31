namespace Api.Inventario.Application.DTOs.Movimientos;

public class MovimientoStockDto
{
    public Guid IdMovimiento { get; set; }
    public Guid IdProducto { get; set; }
    public required string Motivo { get; set; }
    public int Cantidad { get; set; } // Puede ser positiva (ingreso) o negativa (egreso)
    public DateTime Fecha { get; set; }
}