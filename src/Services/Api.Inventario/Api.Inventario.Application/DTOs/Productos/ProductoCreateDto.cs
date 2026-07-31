namespace Api.Inventario.Application.DTOs.Productos;

public class ProductoCreateDto
{
    public required string Nombre { get; set; }
    public decimal Precio { get; set; }
    public required string SKU { get; set; }
    public Guid IdCategoria { get; set; }
}