﻿using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Stotele.Server.Migrations
{
    /// <inheritdoc />
    public partial class revert : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Kategorijos_Vadybininkai_VadybininkasId",
                table: "Kategorijos");

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

            migrationBuilder.AddForeignKey(
                name: "FK_Kategorijos_Vadybininkai_VadybininkasId",
                table: "Kategorijos",
                column: "VadybininkasId",
                principalTable: "Vadybininkai",
                principalColumn: "Id");
        }
    }
}