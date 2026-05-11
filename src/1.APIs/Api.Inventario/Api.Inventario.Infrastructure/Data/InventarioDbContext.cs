using Api.Inventario.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Api.Inventario.Infrastructure.Data;

public class InventarioDbContext : DbContext
{
    public InventarioDbContext(DbContextOptions<InventarioDbContext> options) : base(options) { }

    public DbSet<Producto> Productos => Set<Producto>();
    public DbSet<Categoria> Categorias => Set<Categoria>();
    public DbSet<MovimientoStock> MovimientosStock => Set<MovimientoStock>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Mapeo de Producto
        modelBuilder.Entity<Producto>(entity => {
            entity.HasKey(p => p.IdProducto);
            entity.HasIndex(p => p.SKU).IsUnique(); 
            entity.Property(p => p.Nombre).IsRequired().HasMaxLength(200);
        });

        // Mapeo de Categoria
        modelBuilder.Entity<Categoria>(entity => {
            entity.HasKey(c => c.IdCategoria);
            entity.Property(c => c.Nombre).IsRequired().HasMaxLength(100);
        });

        // Mapeo de MovimientoStock
        modelBuilder.Entity<MovimientoStock>(entity => {
            entity.HasKey(m => m.IdMovimiento);
            entity.Property(m => m.Motivo).IsRequired().HasMaxLength(500);
        });
    }
}