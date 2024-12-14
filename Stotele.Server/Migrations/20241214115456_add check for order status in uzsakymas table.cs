using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Stotele.Server.Migrations
{
    /// <inheritdoc />
    public partial class addcheckfororderstatusinuzsakymastable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Patvirtintas",
                table: "Uzsakymai",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Patvirtintas",
                table: "Uzsakymai");
        }
    }
}
