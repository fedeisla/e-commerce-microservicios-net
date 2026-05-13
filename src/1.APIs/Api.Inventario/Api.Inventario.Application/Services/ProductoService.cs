using Api.Inventario.Application.DTOs.Productos;
using Api.Inventario.Application.Interfaces;
using Api.Inventario.Domain.Entities;
using Api.Inventario.Domain.Enums;
using Api.Inventario.Domain.Interfaces;

namespace Api.Inventario.Application.Services;

public class ProductoService : IProductoService
{
    private readonly IUnitOfWork _unitOfWork;

    public ProductoService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IEnumerable<ProductoDto>> GetAllAsync()
    {
        var productos = await _unitOfWork.Productos.GetAllAsync();
        
        // Mapeo manual de Entidad a DTO (Más adelante podés usar AutoMapper)
        return productos.Select(p => new ProductoDto
        {
            IdProducto = p.IdProducto,
            Nombre = p.Nombre,
            Precio = p.Precio,
            SKU = p.SKU,
            StockActual = p.StockActual,
            Estado = p.Estado.ToString(),
            CategoriaNombre = p.Categoria?.Nombre ?? "Sin Categoría"
        });
    }

    public async Task<ProductoDto?> GetByIdAsync(Guid id)
    {
        var producto = await _unitOfWork.Productos.GetByIdAsync(id);
        if (producto == null) return null;

        return new ProductoDto
        {
            IdProducto = producto.IdProducto,
            Nombre = producto.Nombre,
            Precio = producto.Precio,
            SKU = producto.SKU,
            StockActual = producto.StockActual,
            Estado = producto.Estado.ToString(),
            CategoriaNombre = producto.Categoria?.Nombre ?? "Sin Categoría"
        };
    }

    public async Task<ProductoDto> CreateAsync(ProductoCreateDto dto)
    {
        // Validación de negocio: El SKU no puede repetirse
        if (await _unitOfWork.Productos.ExisteSkuAsync(dto.SKU))
        {
            throw new Exception($"Ya existe un producto con el SKU: {dto.SKU}");
        }

        // Mapeo de DTO a Entidad
        var nuevoProducto = new Producto
        {
            IdProducto = Guid.NewGuid(),
            Nombre = dto.Nombre,
            Precio = dto.Precio,
            SKU = dto.SKU,
            IdCategoria = dto.IdCategoria,
            StockActual = 0, // Regla de negocio: arranca en 0
            Estado = ProductoEstado.Activo
        };

        await _unitOfWork.Productos.AddAsync(nuevoProducto);
        await _unitOfWork.SaveChangesAsync(); // ¡Impacta en la BD!

        // Devolvemos el producto creado (reutilizamos el método GetById para traerlo con su categoría)
        return await GetByIdAsync(nuevoProducto.IdProducto) ?? throw new Exception("Error al recuperar el producto creado.");
    }
    public async Task<ProductoDto?> UpdateAsync(Guid id, ProductoUpdateDto dto)
{
    // 1. Buscar el producto original
    var producto = await _unitOfWork.Productos.GetByIdAsync(id);
    if (producto == null)
    {
        return null; // El controlador se va a encargar de devolver un 404
    }

    // 2. Validar que la nueva categoría exista (si es que la cambió)
    if (producto.IdCategoria != dto.IdCategoria)
    {
        var categoria = await _unitOfWork.Categorias.GetByIdAsync(dto.IdCategoria);
        if (categoria == null)
        {
            throw new Exception($"La categoría con ID {dto.IdCategoria} no existe.");
        }
    }

    // 3. Aplicar los cambios a la entidad
    producto.Nombre = dto.Nombre;
    producto.Precio = dto.Precio;
    producto.IdCategoria = dto.IdCategoria;
    // Nota: El SKU y el Stock no se tocan acá.

    // 4. Guardar en base de datos
    _unitOfWork.Productos.Update(producto);
    await _unitOfWork.SaveChangesAsync();

    // 5. Devolver el producto actualizado
    return await GetByIdAsync(producto.IdProducto);
}
}