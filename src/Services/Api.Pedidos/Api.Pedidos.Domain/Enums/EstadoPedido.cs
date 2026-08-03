namespace Api.Pedidos.Domain.Enums;

public enum EstadoPedido
{
    Pendiente,    // Se creó, pero Inventario no descontó stock
    Confirmado,   // Inventario dijo OK
    Rechazado,    // Inventario dijo FAIL
    Cancelado     // Cancelado por cliente o sistema
}