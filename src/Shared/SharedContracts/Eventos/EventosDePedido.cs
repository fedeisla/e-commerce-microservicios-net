namespace SharedContracts.Eventos;
public record PedidoItemEvent(
    Guid ProductoId, 
    int Cantidad
);



public record PedidoCreadoEvent(
    Guid PedidoId,
    Guid UsuarioId,
    List<PedidoItemEvent> Items
);

public record StockConfirmadoEvent(
    Guid PedidoId
);

public record StockRechazadoEvent(
    Guid PedidoId, 
    string MotivoRechazo
);