using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Stotele.Server.Migrations
{
    /// <inheritdoc />
    public partial class remodeldb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Adresai");

            migrationBuilder.DropTable(
                name: "Atsiliepimai");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Adresai",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    KlientasId = table.Column<int>(type: "integer", nullable: false),
                    NaudotojasId = table.Column<int>(type: "integer", nullable: false),
                    ButoNumeris = table.Column<int>(type: "integer", nullable: false),
                    Gatve = table.Column<string>(type: "text", nullable: false),
                    Miestas = table.Column<string>(type: "text", nullable: false),
                    NamoNumeris = table.Column<int>(type: "integer", nullable: false),
                    PastoKodas = table.Column<string>(type: "text", nullable: false),
                    Salis = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Adresai", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Adresai_Klientai_KlientasId",
                        column: x => x.KlientasId,
                        principalTable: "Klientai",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Adresai_Naudotojai_NaudotojasId",
                        column: x => x.NaudotojasId,
                        principalTable: "Naudotojai",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Atsiliepimai",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    KlientasId = table.Column<int>(type: "integer", nullable: false),
                    PrekeId = table.Column<int>(type: "integer", nullable: false),
                    Data = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IvercioVerte = table.Column<int>(type: "integer", nullable: false),
                    Komentaras = table.Column<string>(type: "text", nullable: false),
                    Titulas = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Atsiliepimai", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Atsiliepimai_Klientai_KlientasId",
                        column: x => x.KlientasId,
                        principalTable: "Klientai",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Atsiliepimai_Prekes_PrekeId",
                        column: x => x.PrekeId,
                        principalTable: "Prekes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Adresai_KlientasId",
                table: "Adresai",
                column: "KlientasId");

            migrationBuilder.CreateIndex(
                name: "IX_Adresai_NaudotojasId",
                table: "Adresai",
                column: "NaudotojasId");

            migrationBuilder.CreateIndex(
                name: "IX_Atsiliepimai_KlientasId",
                table: "Atsiliepimai",
                column: "KlientasId");

            migrationBuilder.CreateIndex(
                name: "IX_Atsiliepimai_PrekeId",
                table: "Atsiliepimai",
                column: "PrekeId");
        }
    }
}
