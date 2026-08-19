namespace SharedContracts.Eventos;

public record ProductoCreadoEvent(
    Guid ProductoId,
    string Sku,
    string Nombre,
    decimal PrecioBase
);

public record StockActualizadoEvent(
    Guid ProductoId,
    int NuevoStockDisponible
);

// si el stock cae por debajo de un umbral (ej. quedan menos de 5 unidades)
public record AlertaStockBajoEvent(
    Guid ProductoId,
    string NombreProducto,
    int StockRestante
);