using MassTransit;
using Api.Pedidos.Domain.Entities;
using Api.Pedidos.Infrastructure.Persistence; 
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SharedContracts.Eventos;

namespace Api.Pedidos.WebAPI.Consumers;

public class UsuarioRegistradoConsumer : IConsumer<UsuarioRegistradoEvent>
{
    private readonly ILogger<UsuarioRegistradoConsumer> _logger;
    private readonly PedidosDbContext _dbContext;
    public UsuarioRegistradoConsumer(ILogger<UsuarioRegistradoConsumer> logger, PedidosDbContext dbContext)
    {
        _logger = logger;
        _dbContext = dbContext;
    }

    public async Task Consume(ConsumeContext<UsuarioRegistradoEvent> context)
    {
        var mensaje = context.Message;

        _logger.LogInformation(" Procesando evento de registro para el usuario: {Email}", mensaje.Email);

        try
        {
            // Verificamos que el usuario no tenga ya un carrito (por si el evento se llega a procesar dos veces por error)
            bool carritoExiste = await _dbContext.Carritos
                .AnyAsync(c => c.UsuarioId == mensaje.UsuarioId);

            if (carritoExiste)
            {
                _logger.LogWarning(" El usuario {UsuarioId} ya tiene un carrito. Se omite la creación.", mensaje.UsuarioId);
                return; 
            }

            // Instanciamos el nuevo carrito vacío
            var nuevoCarrito = new Carrito
            {
                Id = Guid.NewGuid(),
                UsuarioId = mensaje.UsuarioId,
                FechaCreacion = DateTime.UtcNow
            };

            // Lo guardamos en la base de datos de Pedidos
            _dbContext.Carritos.Add(nuevoCarrito);
            await _dbContext.SaveChangesAsync();

            _logger.LogInformation(" [ÉXITO] Carrito vacío creado en la BD para el usuario {Email} (ID: {UsuarioId})", 
                mensaje.Email, mensaje.UsuarioId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, " Error al crear el carrito para el usuario {UsuarioId}", mensaje.UsuarioId);
            throw; 
        }
    }
}