using Api.Pedidos.Application.DTOs;
using Api.Pedidos.Domain.Entities;

namespace Api.Pedidos.Application.Interfaces;

public interface IPedidoService
{
    Task<Pedido> CrearPedidoAsync(PedidoCreateDto dto);
    Task<Pedido?> ObtenerPedidoPorIdAsync(Guid id);
}