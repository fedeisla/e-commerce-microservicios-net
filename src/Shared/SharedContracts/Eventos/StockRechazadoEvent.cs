namespace SharedContracts.Eventos;

public record StockRechazadoEvent(
    Guid PedidoId,
    string Motivo
);