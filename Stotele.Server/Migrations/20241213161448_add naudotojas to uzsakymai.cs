using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Stotele.Server.Migrations
{
    /// <inheritdoc />
    public partial class addnaudotojastouzsakymai : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "NaudotojasId",
                table: "Uzsakymai",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Uzsakymai_NaudotojasId",
                table: "Uzsakymai",
                column: "NaudotojasId");

            migrationBuilder.AddForeignKey(
                name: "FK_Uzsakymai_Naudotojai_NaudotojasId",
                table: "Uzsakymai",
                column: "NaudotojasId",
                principalTable: "Naudotojai",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Uzsakymai_Naudotojai_NaudotojasId",
                table: "Uzsakymai");

            migrationBuilder.DropIndex(
                name: "IX_Uzsakymai_NaudotojasId",
                table: "Uzsakymai");

            migrationBuilder.DropColumn(
                name: "NaudotojasId",
                table: "Uzsakymai");
        }
    }
}
