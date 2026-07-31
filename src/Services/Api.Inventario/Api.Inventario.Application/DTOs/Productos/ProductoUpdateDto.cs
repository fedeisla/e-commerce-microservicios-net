namespace Api.Inventario.Application.DTOs.Productos;

public class ProductoUpdateDto
{
    public required string Nombre { get; set; }
    public decimal Precio { get; set; }
    public Guid IdCategoria { get; set; }
}