using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Stotele.Server.Migrations
{
    /// <inheritdoc />
    public partial class editparduotuves : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PrekesParduotuves_Parduotuves_ParduotuveId",
                table: "PrekesParduotuves");

            migrationBuilder.DropForeignKey(
                name: "FK_PrekesParduotuves_Prekes_PrekeId",
                table: "PrekesParduotuves");

            migrationBuilder.DropForeignKey(
                name: "FK_Vadybininkai_Parduotuves_ParduotuveId",
                table: "Vadybininkai");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PrekesParduotuves",
                table: "PrekesParduotuves");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Parduotuves",
                table: "Parduotuves");

            migrationBuilder.RenameTable(
                name: "PrekesParduotuves",
                newName: "PrekesParduotuve");

            migrationBuilder.RenameTable(
                name: "Parduotuves",
                newName: "Parduotuve");

            migrationBuilder.RenameIndex(
                name: "IX_PrekesParduotuves_PrekeId",
                table: "PrekesParduotuve",
                newName: "IX_PrekesParduotuve_PrekeId");

            migrationBuilder.RenameIndex(
                name: "IX_PrekesParduotuves_ParduotuveId",
                table: "PrekesParduotuve",
                newName: "IX_PrekesParduotuve_ParduotuveId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PrekesParduotuve",
                table: "PrekesParduotuve",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Parduotuve",
                table: "Parduotuve",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_PrekesParduotuve_Parduotuve_ParduotuveId",
                table: "PrekesParduotuve",
                column: "ParduotuveId",
                principalTable: "Parduotuve",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PrekesParduotuve_Prekes_PrekeId",
                table: "PrekesParduotuve",
                column: "PrekeId",
                principalTable: "Prekes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Vadybininkai_Parduotuve_ParduotuveId",
                table: "Vadybininkai",
                column: "ParduotuveId",
                principalTable: "Parduotuve",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PrekesParduotuve_Parduotuve_ParduotuveId",
                table: "PrekesParduotuve");

            migrationBuilder.DropForeignKey(
                name: "FK_PrekesParduotuve_Prekes_PrekeId",
                table: "PrekesParduotuve");

            migrationBuilder.DropForeignKey(
                name: "FK_Vadybininkai_Parduotuve_ParduotuveId",
                table: "Vadybininkai");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PrekesParduotuve",
                table: "PrekesParduotuve");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Parduotuve",
                table: "Parduotuve");

            migrationBuilder.RenameTable(
                name: "PrekesParduotuve",
                newName: "PrekesParduotuves");

            migrationBuilder.RenameTable(
                name: "Parduotuve",
                newName: "Parduotuves");

            migrationBuilder.RenameIndex(
                name: "IX_PrekesParduotuve_PrekeId",
                table: "PrekesParduotuves",
                newName: "IX_PrekesParduotuves_PrekeId");

            migrationBuilder.RenameIndex(
                name: "IX_PrekesParduotuve_ParduotuveId",
                table: "PrekesParduotuves",
                newName: "IX_PrekesParduotuves_ParduotuveId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PrekesParduotuves",
                table: "PrekesParduotuves",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Parduotuves",
                table: "Parduotuves",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_PrekesParduotuves_Parduotuves_ParduotuveId",
                table: "PrekesParduotuves",
                column: "ParduotuveId",
                principalTable: "Parduotuves",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PrekesParduotuves_Prekes_PrekeId",
                table: "PrekesParduotuves",
                column: "PrekeId",
                principalTable: "Prekes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Vadybininkai_Parduotuves_ParduotuveId",
                table: "Vadybininkai",
                column: "ParduotuveId",
                principalTable: "Parduotuves",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
