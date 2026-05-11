namespace Api.Inventario.Domain.Entities;

public class Categoria
{
    public Guid IdCategoria { get; set; }
    public required string Nombre { get; set; }

    public virtual ICollection<Producto> Productos { get; set; } = new List<Producto>();
}