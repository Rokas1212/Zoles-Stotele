using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Stotele.Server.Migrations
{
    /// <inheritdoc />
    public partial class changeparduotuveadresastostring : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Parduotuves_Adresai_AdresasId",
                table: "Parduotuves");

            migrationBuilder.DropIndex(
                name: "IX_Parduotuves_AdresasId",
                table: "Parduotuves");

            migrationBuilder.DropColumn(
                name: "AdresasId",
                table: "Parduotuves");

            migrationBuilder.AddColumn<string>(
                name: "Adresas",
                table: "Parduotuves",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Adresas",
                table: "Parduotuves");

            migrationBuilder.AddColumn<int>(
                name: "AdresasId",
                table: "Parduotuves",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Parduotuves_AdresasId",
                table: "Parduotuves",
                column: "AdresasId");

            migrationBuilder.AddForeignKey(
                name: "FK_Parduotuves_Adresai_AdresasId",
                table: "Parduotuves",
                column: "AdresasId",
                principalTable: "Adresai",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
