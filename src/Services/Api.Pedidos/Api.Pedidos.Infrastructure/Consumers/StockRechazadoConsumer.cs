using Api.Pedidos.Domain.Enums;
using Api.Pedidos.Infrastructure.Persistence;
using MassTransit;
using SharedContracts.Eventos;

namespace Api.Pedidos.Infrastructure.Consumers;

public class StockRechazadoConsumer : IConsumer<StockRechazadoEvent>
{
    private readonly PedidosDbContext _context;

    public StockRechazadoConsumer(PedidosDbContext context)
    {
        _context = context;
    }

    public async Task Consume(ConsumeContext<StockRechazadoEvent> context)
    {
        var pedido = await _context.Pedidos.FindAsync(context.Message.PedidoId);
        
        if (pedido != null)
        {
            pedido.Estado = EstadoPedido.Rechazado;
            // Opcional: Podrías guardar el context.Message.Motivo en la BD si agregás un campo "Notas" al Pedido
            await _context.SaveChangesAsync();
        }
    }
}