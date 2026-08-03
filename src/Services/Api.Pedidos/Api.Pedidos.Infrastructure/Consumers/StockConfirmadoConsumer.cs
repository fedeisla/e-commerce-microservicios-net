using Api.Pedidos.Domain.Enums;
using Api.Pedidos.Infrastructure.Persistence;
using MassTransit;
using SharedContracts.Eventos;

namespace Api.Pedidos.Infrastructure.Consumers;

public class StockConfirmadoConsumer : IConsumer<StockConfirmadoEvent>
{
    private readonly PedidosDbContext _context;

    public StockConfirmadoConsumer(PedidosDbContext context)
    {
        _context = context;
    }

    public async Task Consume(ConsumeContext<StockConfirmadoEvent> context)
    {
        var pedido = await _context.Pedidos.FindAsync(context.Message.PedidoId);
        
        if (pedido != null)
        {
            pedido.Estado = EstadoPedido.Confirmado;
            await _context.SaveChangesAsync();
        }
    }
}