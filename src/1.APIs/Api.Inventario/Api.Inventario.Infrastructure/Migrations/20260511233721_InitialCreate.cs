using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Api.Inventario.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Categorias",
                columns: table => new
                {
                    IdCategoria = table.Column<Guid>(type: "uuid", nullable: false),
                    Nombre = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categorias", x => x.IdCategoria);
                });

            migrationBuilder.CreateTable(
                name: "Productos",
                columns: table => new
                {
                    IdProducto = table.Column<Guid>(type: "uuid", nullable: false),
                    Nombre = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Precio = table.Column<decimal>(type: "numeric", nullable: false),
                    SKU = table.Column<string>(type: "text", nullable: false),
                    StockActual = table.Column<int>(type: "integer", nullable: false),
                    Estado = table.Column<int>(type: "integer", nullable: false),
                    IdCategoria = table.Column<Guid>(type: "uuid", nullable: false),
                    CategoriaIdCategoria = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Productos", x => x.IdProducto);
                    table.ForeignKey(
                        name: "FK_Productos_Categorias_CategoriaIdCategoria",
                        column: x => x.CategoriaIdCategoria,
                        principalTable: "Categorias",
                        principalColumn: "IdCategoria");
                });

            migrationBuilder.CreateTable(
                name: "MovimientosStock",
                columns: table => new
                {
                    IdMovimiento = table.Column<Guid>(type: "uuid", nullable: false),
                    Cantidad = table.Column<int>(type: "integer", nullable: false),
                    Fecha = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Motivo = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    TipoMovimiento = table.Column<int>(type: "integer", nullable: false),
                    IdProducto = table.Column<Guid>(type: "uuid", nullable: false),
                    ProductoIdProducto = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MovimientosStock", x => x.IdMovimiento);
                    table.ForeignKey(
                        name: "FK_MovimientosStock_Productos_ProductoIdProducto",
                        column: x => x.ProductoIdProducto,
                        principalTable: "Productos",
                        principalColumn: "IdProducto");
                });

            migrationBuilder.CreateIndex(
                name: "IX_MovimientosStock_ProductoIdProducto",
                table: "MovimientosStock",
                column: "ProductoIdProducto");

            migrationBuilder.CreateIndex(
                name: "IX_Productos_CategoriaIdCategoria",
                table: "Productos",
                column: "CategoriaIdCategoria");

            migrationBuilder.CreateIndex(
                name: "IX_Productos_SKU",
                table: "Productos",
                column: "SKU",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MovimientosStock");

            migrationBuilder.DropTable(
                name: "Productos");

            migrationBuilder.DropTable(
                name: "Categorias");
        }
    }
}
