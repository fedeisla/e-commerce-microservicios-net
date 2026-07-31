using Api.Inventario.Application.DTOs.Movimientos;
using Api.Inventario.Application.DTOs.Productos;
using Api.Inventario.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Api.Inventario.WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")] // La ruta será: api/productos
public class ProductosController : ControllerBase
{
    private readonly IProductoService _productoService;
    private readonly IMovimientoStockService _movimientoService; // 1. Declaramos el nuevo servicio

    // 2. Inyectamos ambos servicios en el constructor
    public ProductosController(IProductoService productoService, IMovimientoStockService movimientoService)
    {
        _productoService = productoService;
        _movimientoService = movimientoService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var productos = await _productoService.GetAllAsync();
        return Ok(productos); 
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var producto = await _productoService.GetByIdAsync(id);
        
        if (producto == null) 
            return NotFound(new { message = $"No se encontró el producto con ID {id}" }); 
            
        return Ok(producto);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] ProductoCreateDto dto)
    {
        try
        {
            var nuevoProducto = await _productoService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = nuevoProducto.IdProducto }, nuevoProducto);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] ProductoUpdateDto dto)
    {
        try
        {
            var productoActualizado = await _productoService.UpdateAsync(id, dto);
            
            if (productoActualizado == null)
            {
                return NotFound(new { message = $"No se encontró el producto con ID {id}" });
            }

            return Ok(productoActualizado);
        }
        catch (Exception ex)
        {
            // Atrapa el error si le mandan un IdCategoria que no existe
            return BadRequest(new { message = ex.Message }); 
        }
    }

    [HttpPost("movimientos")]
    public async Task<IActionResult> RegistrarMovimiento([FromBody] RegistrarMovimientoDto dto)
    {
        try
        {
            var productoActualizado = await _productoService.RegistrarMovimientoStockAsync(dto);
            return Ok(productoActualizado);
        }
        catch (Exception ex)
        {
            // Atrapa los errores de stock negativo o producto inexistente
            return BadRequest(new { message = ex.Message });
        }
    }

    
    [HttpGet("{id:guid}/movimientos")]
    public async Task<IActionResult> GetHistorialMovimientos(Guid id)
    {
        try
        {
            
            var producto = await _productoService.GetByIdAsync(id);
            if (producto == null)
            {
                return NotFound(new { message = $"No se encontró el producto con ID {id}" });
            }

            var historial = await _movimientoService.GetHistorialByProductoAsync(id);
            
            return Ok(historial);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error interno al obtener el historial", detalle = ex.Message });
        }
    }
}