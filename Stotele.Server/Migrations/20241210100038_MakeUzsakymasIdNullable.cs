using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Stotele.Server.Migrations
{
    /// <inheritdoc />
    public partial class MakeUzsakymasIdNullable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Nuolaidos_Uzsakymai_UzsakymasId",
                table: "Nuolaidos");

            migrationBuilder.AlterColumn<int>(
                name: "UzsakymasId",
                table: "Nuolaidos",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddForeignKey(
                name: "FK_Nuolaidos_Uzsakymai_UzsakymasId",
                table: "Nuolaidos",
                column: "UzsakymasId",
                principalTable: "Uzsakymai",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Nuolaidos_Uzsakymai_UzsakymasId",
                table: "Nuolaidos");

            migrationBuilder.AlterColumn<int>(
                name: "UzsakymasId",
                table: "Nuolaidos",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Nuolaidos_Uzsakymai_UzsakymasId",
                table: "Nuolaidos",
                column: "UzsakymasId",
                principalTable: "Uzsakymai",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
