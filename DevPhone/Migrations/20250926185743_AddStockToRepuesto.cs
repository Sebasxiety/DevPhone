using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DevPhone.Migrations
{
    /// <inheritdoc />
    public partial class AddStockToRepuesto : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Stock",
                table: "Repuestos",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Stock",
                table: "Repuestos");
        }
    }
}
