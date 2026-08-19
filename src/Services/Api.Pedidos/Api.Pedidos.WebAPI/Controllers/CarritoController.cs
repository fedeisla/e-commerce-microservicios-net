using Api.Pedidos.Application.DTOs;
using Api.Pedidos.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Api.Pedidos.WebAPI.Controllers;

[ApiController]
[Route("api/pedidos/carrito")]
[Authorize] 
public class CarritoController : ControllerBase
{
    private readonly ICarritoService _carritoService;

    public CarritoController(ICarritoService carritoService)
    {
        _carritoService = carritoService;
    }

    private Guid ObtenerUsuarioId()
    {
        var claimId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        
        if (!Guid.TryParse(claimId, out var usuarioId))
        {
            throw new UnauthorizedAccessException("Token inválido o sin ID de usuario.");
        }
        
        return usuarioId;
    }

    [HttpGet]
    public async Task<IActionResult> GetCarrito()
    {
        try
        {
            var usuarioId = ObtenerUsuarioId();
            var carrito = await _carritoService.ObtenerCarritoAsync(usuarioId);

            if (carrito == null)
                return NotFound(new { mensaje = "El usuario no tiene un carrito activo." });

            return Ok(carrito);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { error = ex.Message });
        }
        catch (Exception)
        {
            return StatusCode(500, new { error = "Ocurrió un problema al obtener el carrito." });
        }
    }

    [HttpPost("items")]
    public async Task<IActionResult> AgregarItem([FromBody] AgregarItemDto dto)
    {
        try
        {
            var usuarioId = ObtenerUsuarioId();
            var carrito = await _carritoService.AgregarItemAsync(usuarioId, dto);
            
            return Ok(carrito);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { error = ex.Message });
        }
        catch (DbUpdateException)
        {
            return StatusCode(500, new { error = "Error al actualizar los datos del carrito en la base de datos." });
        }
        catch (Exception)
        {
            return StatusCode(500, new { error = "Ocurrió un error interno al agregar el producto al carrito." });
        }
    }

    [HttpDelete("items/{productoId}")]
    public async Task<IActionResult> QuitarItem(Guid productoId)
    {
        try
        {
            var usuarioId = ObtenerUsuarioId();
            await _carritoService.QuitarItemAsync(usuarioId, productoId);

            return NoContent(); 
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { error = ex.Message });
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { error = ex.Message });
        }
        catch (Exception)
        {
            return StatusCode(500, new { error = "Ocurrió un error al intentar eliminar el producto." });
        }
    }

    [HttpPost("checkout")]
    [EnableRateLimiting("CheckoutLimiter")] 
    public async Task<IActionResult> Checkout()
    {
        try
        {
            var usuarioId = ObtenerUsuarioId();
            var pedidoId = await _carritoService.ProcesarCheckoutAsync(usuarioId);

            return Ok(new { mensaje = "Checkout exitoso. Pedido en proceso.", pedidoId });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { error = ex.Message });
        }
        catch (Exception)
        {
            return StatusCode(500, new { error = "Ocurrió un error al procesar el checkout. No se generó el pedido." });
        }
    }
}