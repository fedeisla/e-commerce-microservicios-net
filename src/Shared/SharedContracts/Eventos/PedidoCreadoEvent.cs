namespace SharedContracts.Eventos;

public record PedidoCreadoEvent(
    Guid PedidoId,
    Guid ProductoId,
    int Cantidad
);