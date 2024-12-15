using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Stotele.Server.Migrations
{
    /// <inheritdoc />
    public partial class damn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Kategorijos_Vadybininkai_VadybininkasId",
                table: "Kategorijos");

            migrationBuilder.AlterColumn<int>(
                name: "VadybininkasId",
                table: "Kategorijos",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Kategorijos_Vadybininkai_VadybininkasId",
                table: "Kategorijos",
                column: "VadybininkasId",
                principalTable: "Vadybininkai",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Kategorijos_Vadybininkai_VadybininkasId",
                table: "Kategorijos");

            migrationBuilder.AlterColumn<int>(
                name: "VadybininkasId",
                table: "Kategorijos",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddForeignKey(
                name: "FK_Kategorijos_Vadybininkai_VadybininkasId",
                table: "Kategorijos",
                column: "VadybininkasId",
                principalTable: "Vadybininkai",
                principalColumn: "Id");
        }
    }
}
