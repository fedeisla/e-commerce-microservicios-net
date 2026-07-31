namespace Api.Inventario.Application.DTOs.Movimientos;

public class RegistrarMovimientoDto
{
    public Guid IdProducto { get; set; }
    public required string Motivo { get; set; }
    public int Cantidad { get; set; }
}