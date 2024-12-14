using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Stotele.Server.Migrations
{
    /// <inheritdoc />
    public partial class AddKainaToPrekesUzsakymas : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "Kaina",
                table: "PrekesUzsakymai",
                type: "double precision",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Kaina",
                table: "PrekesUzsakymai");
        }
    }
}
