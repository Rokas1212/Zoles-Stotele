using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Stotele.Server.Migrations
{
    /// <inheritdoc />
    public partial class MakeApmokejimasIdNullable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Taskai_Apmokejimai_ApmokejimasId",
                table: "Taskai");

            migrationBuilder.AlterColumn<int>(
                name: "ApmokejimasId",
                table: "Taskai",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddForeignKey(
                name: "FK_Taskai_Apmokejimai_ApmokejimasId",
                table: "Taskai",
                column: "ApmokejimasId",
                principalTable: "Apmokejimai",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Taskai_Apmokejimai_ApmokejimasId",
                table: "Taskai");

            migrationBuilder.AlterColumn<int>(
                name: "ApmokejimasId",
                table: "Taskai",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Taskai_Apmokejimai_ApmokejimasId",
                table: "Taskai",
                column: "ApmokejimasId",
                principalTable: "Apmokejimai",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
