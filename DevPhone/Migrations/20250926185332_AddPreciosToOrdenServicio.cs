using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DevPhone.Migrations
{
    /// <inheritdoc />
    public partial class AddPreciosToOrdenServicio : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "PrecioServicio",
                table: "OrdenesServicio",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "PrecioTotal",
                table: "OrdenesServicio",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PrecioServicio",
                table: "OrdenesServicio");

            migrationBuilder.DropColumn(
                name: "PrecioTotal",
                table: "OrdenesServicio");
        }
    }
}
