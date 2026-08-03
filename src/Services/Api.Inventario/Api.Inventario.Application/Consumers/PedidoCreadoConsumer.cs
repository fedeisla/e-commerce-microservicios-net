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
        
        var producto = await _productoRepository.GetByIdAsync(mensaje.ProductoId);

       
        if (producto == null || producto.StockActual < mensaje.Cantidad)
        {
            await context.Publish(new StockRechazadoEvent(
                mensaje.PedidoId, 
                "Producto inexistente o sin stock suficiente"
            ));
            return;
        }

       
        producto.StockActual -= mensaje.Cantidad;
        _productoRepository.Update(producto);

        
        var movimiento = new MovimientoStock
        {
            IdProducto = producto.IdProducto,
            Cantidad = mensaje.Cantidad,
            TipoMovimiento = TipoMovimientoStock.AjusteNegativo, 
            Fecha = DateTime.UtcNow,
            Motivo = $"Venta - Pedido {mensaje.PedidoId}" 
        };
        await _movimientoRepository.AddAsync(movimiento);

        await _unitOfWork.SaveChangesAsync();

        await context.Publish(new StockConfirmadoEvent(mensaje.PedidoId));
    }
}