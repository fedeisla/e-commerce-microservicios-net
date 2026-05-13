namespace Api.Inventario.Application.DTOs.Categorias;

public class CategoriaDto
{
    public Guid IdCategoria { get; set; }
    public required string Nombre { get; set; }
}