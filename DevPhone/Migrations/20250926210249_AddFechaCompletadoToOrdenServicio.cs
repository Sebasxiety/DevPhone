using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DevPhone.Migrations
{
    /// <inheritdoc />
    public partial class AddFechaCompletadoToOrdenServicio : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "FechaCompletado",
                table: "OrdenesServicio",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FechaCompletado",
                table: "OrdenesServicio");
        }
    }
}
