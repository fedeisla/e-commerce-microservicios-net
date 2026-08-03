    using Api.Pedidos.Application.DTOs;
using Api.Pedidos.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Api.Pedidos.WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PedidosController : ControllerBase
{
    private readonly IPedidoService _pedidoService;

    public PedidosController(IPedidoService pedidoService)
    {
        _pedidoService = pedidoService;
    }

    [HttpPost]
    public async Task<IActionResult> CrearPedido([FromBody] PedidoCreateDto dto)
    {
        try
        {
            var nuevoPedido = await _pedidoService.CrearPedidoAsync(dto);
            return Ok(nuevoPedido);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
    [HttpGet("{id}")]
    public async Task<IActionResult> ObtenerPedido(Guid id)
    {
        var pedido = await _pedidoService.ObtenerPedidoPorIdAsync(id);
        if (pedido == null) return NotFound("El pedido no existe.");
        return Ok(pedido);
    }
}