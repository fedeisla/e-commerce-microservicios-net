namespace Api.Inventario.Application.DTOs.Productos;

public class ProductoDto
{
    public Guid IdProducto { get; set; }
    public required string Nombre { get; set; }
    public decimal Precio { get; set; }
    public required string SKU { get; set; }
    public int StockActual { get; set; }
    public string Estado { get; set; } = string.Empty;
    public string CategoriaNombre { get; set; } = string.Empty;
}