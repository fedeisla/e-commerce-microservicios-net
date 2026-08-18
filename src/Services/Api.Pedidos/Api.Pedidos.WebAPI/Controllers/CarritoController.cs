using Api.Pedidos.Application.DTOs;
using Api.Pedidos.Domain.Entities;
using Api.Pedidos.Domain.Enums;
using Api.Pedidos.Infrastructure.Persistence;
using MassTransit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SharedContracts.Eventos;
using System.Security.Claims;

namespace Api.Pedidos.WebAPI.Controllers;

[ApiController]
[Route("api/pedidos/carrito")]
[Authorize] 
public class CarritoController : ControllerBase
{
    private readonly PedidosDbContext _dbContext;
    private readonly IPublishEndpoint _publishEndpoint;

    public CarritoController(PedidosDbContext dbContext, IPublishEndpoint publishEndpoint)
    {
        _dbContext = dbContext;
        _publishEndpoint = publishEndpoint;
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

            var carrito = await _dbContext.Carritos
                .Include(c => c.Items)
                .FirstOrDefaultAsync(c => c.UsuarioId == usuarioId);

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

            var carrito = await _dbContext.Carritos
                .Include(c => c.Items)
                .FirstOrDefaultAsync(c => c.UsuarioId == usuarioId);

            if (carrito == null)
            {
                carrito = new Carrito 
                { 
                    Id = Guid.NewGuid(), 
                    UsuarioId = usuarioId, 
                    FechaCreacion = DateTime.UtcNow,
                    Items = new List<CarritoItem>()
                };
                _dbContext.Carritos.Add(carrito);
            } 

            var itemExistente = carrito.Items.FirstOrDefault(i => i.ProductoId == dto.ProductoId);

            if (itemExistente != null)
            {
                itemExistente.Cantidad += dto.Cantidad;
            }
            else
            {
                var nuevoItem = new CarritoItem
                {
                    Id = Guid.NewGuid(),
                    ProductoId = dto.ProductoId,
                    ProductoNombre = dto.ProductoNombre,
                    PrecioUnitario = dto.PrecioUnitario,
                    Cantidad = dto.Cantidad
                };
                
                carrito.Items.Add(nuevoItem);
                _dbContext.Entry(nuevoItem).State = EntityState.Added;
            }

            carrito.FechaUltimaModificacion = DateTime.UtcNow;
            
            await _dbContext.SaveChangesAsync();

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

            var carrito = await _dbContext.Carritos
                .Include(c => c.Items)
                .FirstOrDefaultAsync(c => c.UsuarioId == usuarioId);

            if (carrito == null) return NotFound(new { error = "Carrito no encontrado." });

            var item = carrito.Items.FirstOrDefault(i => i.ProductoId == productoId);
            
            if (item == null) return NotFound(new { error = "El producto no está en el carrito." });

            _dbContext.CarritoItems.Remove(item);
            
            carrito.FechaUltimaModificacion = DateTime.UtcNow;
            await _dbContext.SaveChangesAsync();

            return NoContent(); 
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
    public async Task<IActionResult> Checkout()
    {
        try
        {
            var usuarioId = ObtenerUsuarioId();

            var carrito = await _dbContext.Carritos
                .Include(c => c.Items)
                .FirstOrDefaultAsync(c => c.UsuarioId == usuarioId);

            if (carrito == null || !carrito.Items.Any())
                return BadRequest(new { error = "El carrito está vacío o no existe." });

            using var transaction = await _dbContext.Database.BeginTransactionAsync();

            try
            {
                var nuevoPedido = new Pedido
                {
                    Id = Guid.NewGuid(),
                    ClienteId = usuarioId, 
                    FechaCreacion = DateTime.UtcNow,
                    Estado = EstadoPedido.Pendiente,
                    Total = carrito.Items.Sum(i => i.PrecioUnitario * i.Cantidad),
                    Detalles = carrito.Items.Select(i => new DetallePedido
                    {
                        Id = Guid.NewGuid(),
                        ProductoId = i.ProductoId,
                        PrecioUnitario = i.PrecioUnitario,
                        Cantidad = i.Cantidad
                    }).ToList()
                };

                _dbContext.Pedidos.Add(nuevoPedido);

                _dbContext.CarritoItems.RemoveRange(carrito.Items);
                carrito.FechaUltimaModificacion = DateTime.UtcNow;

                await _dbContext.SaveChangesAsync();
                
                await transaction.CommitAsync();

                var evento = new PedidoCreadoEvent(
                    nuevoPedido.Id,
                    usuarioId,
                    nuevoPedido.Detalles.Select(d => new PedidoItemEvent(d.ProductoId, d.Cantidad)).ToList()
                );

                await _publishEndpoint.Publish(evento);

                return Ok(new { mensaje = "Checkout exitoso. Pedido en proceso.", pedidoId = nuevoPedido.Id });
            }
            catch (Exception)
            {
                // Si la base de datos falla al guardar, deshacemos la transacción
                await transaction.RollbackAsync();
                throw; // Lanzamos el error hacia el catch exterior para devolver el 500
            }
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