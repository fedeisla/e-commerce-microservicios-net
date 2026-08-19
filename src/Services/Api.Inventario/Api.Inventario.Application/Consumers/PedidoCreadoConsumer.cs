using MassTransit;
using SharedContracts.Eventos;
using Api.Inventario.Domain.Entities; 
using Api.Inventario.Domain.Enums;
using Api.Inventario.Domain.Interfaces; 

namespace Api.Inventario.Application.Consumers;

public class PedidoCreadoConsumer : IConsumer<PedidoCreadoEvent>
{
    private readonly IProductoRepository _productoRepository;
    private readonly IMovimientoStockRepository _movimientoRepository;
    private readonly IUnitOfWork _unitOfWork;

    public PedidoCreadoConsumer(
        IProductoRepository productoRepository, 
        IMovimientoStockRepository movimientoRepository,
        IUnitOfWork unitOfWork)
    {
        _productoRepository = productoRepository;
        _movimientoRepository = movimientoRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task Consume(ConsumeContext<PedidoCreadoEvent> context)
    {
        var mensaje = context.Message;
        
        // Iteramos sobre todos los productos que el cliente tenía en su carrito
        foreach (var item in mensaje.Items)
        {
            // 1. Buscamos el producto puntual de esta vuelta del bucle
            var producto = await _productoRepository.GetByIdAsync(item.ProductoId);

            // 2. Validación de Stock 
            if (producto == null || producto.StockActual < item.Cantidad)
            {
                var motivo = producto == null 
                    ? $"El producto con ID {item.ProductoId} no existe en el catálogo."
                    : $"Stock insuficiente para el producto {item.ProductoId}. Solicitado: {item.Cantidad}, Disponible: {producto.StockActual}";

                // Si falla un solo producto, rechazamos el pedido completo y cortamos la ejecución
                await context.Publish(new StockRechazadoEvent(
                    mensaje.PedidoId,
                    motivo
                ));
                
                return; 
            }

            // 3. Descontar Stock usando tu propiedad real
            producto.StockActual -= item.Cantidad;
            _productoRepository.Update(producto);

            // 4. Registrar Movimiento 
            var movimiento = new MovimientoStock
            {
                IdMovimiento = Guid.NewGuid(), 
                IdProducto = producto.IdProducto,
                Cantidad = item.Cantidad,
                TipoMovimiento = TipoMovimientoStock.AjusteNegativo,
                Fecha = DateTime.UtcNow,
                Motivo = $"Venta - Pedido {mensaje.PedidoId}" 
            };
            
            await _movimientoRepository.AddAsync(movimiento);
        }

        // 5. Persistir cambios de forma atómica con Unit of Work
        await _unitOfWork.SaveChangesAsync();

        // 6. Confirmar éxito hacia el bus de eventos
        await context.Publish(new StockConfirmadoEvent(mensaje.PedidoId));
    }
}