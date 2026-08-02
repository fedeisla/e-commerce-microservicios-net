namespace Api.Inventario.Application.Services;

public interface IInventarioService
{
    Task<(bool Exito, string Motivo)> DescontarStockAsync(Guid productoId, int cantidad);
     Task<bool> AgregarStockAsync(Guid productoId, int cantidad);
}