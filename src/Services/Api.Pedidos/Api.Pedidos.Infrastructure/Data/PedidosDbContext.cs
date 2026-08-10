using Api.Pedidos.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Api.Pedidos.Infrastructure.Persistence;

public class PedidosDbContext : DbContext
{
    public PedidosDbContext(DbContextOptions<PedidosDbContext> options) : base(options)
    {
    }

    public DbSet<Pedido> Pedidos { get; set; }
    public DbSet<DetallePedido> Detalles { get; set; }
    public DbSet<Carrito> Carritos { get; set; }
    public DbSet<CarritoItem> CarritoItems { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

       
        modelBuilder.Entity<Pedido>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Total).HasPrecision(18, 2); 
            entity.Property(e => e.Estado).HasConversion<string>(); 
        });

        modelBuilder.Entity<DetallePedido>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.PrecioUnitario).HasPrecision(18, 2); 

            entity.HasOne(d => d.Pedido)
                  .WithMany(p => p.Detalles)
                  .HasForeignKey(d => d.PedidoId)
                  .OnDelete(DeleteBehavior.Cascade); 
        });

        modelBuilder.Entity<Carrito>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.UsuarioId).IsUnique(); 
        });

        
        modelBuilder.Entity<CarritoItem>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.PrecioUnitario).HasPrecision(18, 2);
            entity.Property(e => e.ProductoNombre).HasMaxLength(200);

            entity.HasOne(i => i.Carrito)
                  .WithMany(c => c.Items)
                  .HasForeignKey(i => i.CarritoId)
                  .OnDelete(DeleteBehavior.Cascade);
        });
    }
}