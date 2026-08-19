using Api.Pedidos.Application.DTOs;
using Api.Pedidos.Domain.Entities;

namespace Api.Pedidos.Application.Interfaces;

public interface ICarritoService
{
    Task<Carrito?> ObtenerCarritoAsync(Guid usuarioId);
    Task<Carrito> AgregarItemAsync(Guid usuarioId, AgregarItemDto dto);
    Task QuitarItemAsync(Guid usuarioId, Guid productoId);
    Task<Guid> ProcesarCheckoutAsync(Guid usuarioId);
}