using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Stotele.Server.Migrations
{
    /// <inheritdoc />
    public partial class initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Naudotojai",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Lytis = table.Column<string>(type: "text", nullable: false),
                    ElektroninisPastas = table.Column<string>(type: "text", nullable: false),
                    Slaptazodis = table.Column<string>(type: "text", nullable: false),
                    Vardas = table.Column<string>(type: "text", nullable: false),
                    Slapyvardis = table.Column<string>(type: "text", nullable: false),
                    Pavarde = table.Column<string>(type: "text", nullable: false),
                    Administratorius = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Naudotojai", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Uzsakymai",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Data = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Suma = table.Column<double>(type: "double precision", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Uzsakymai", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Klientai",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PastoKodas = table.Column<int>(type: "integer", nullable: false),
                    Miestas = table.Column<string>(type: "text", nullable: false),
                    GimimoData = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Adresas = table.Column<string>(type: "text", nullable: false),
                    NaudotojasId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Klientai", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Klientai_Naudotojai_NaudotojasId",
                        column: x => x.NaudotojasId,
                        principalTable: "Naudotojai",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Adresai",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Gatve = table.Column<string>(type: "text", nullable: false),
                    NamoNumeris = table.Column<int>(type: "integer", nullable: false),
                    ButoNumeris = table.Column<int>(type: "integer", nullable: false),
                    PastoKodas = table.Column<string>(type: "text", nullable: false),
                    Miestas = table.Column<string>(type: "text", nullable: false),
                    Salis = table.Column<string>(type: "text", nullable: false),
                    NaudotojasId = table.Column<int>(type: "integer", nullable: false),
                    KlientasId = table.Column<int>(type: "integer", nullable: false)
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
                name: "Apmokejimai",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    SaskaitosFakturosNumeris = table.Column<int>(type: "integer", nullable: false),
                    PvmMoketojoKodas = table.Column<string>(type: "text", nullable: false),
                    GalutineSuma = table.Column<double>(type: "double precision", nullable: false),
                    PanaudotiTaskai = table.Column<int>(type: "integer", nullable: false),
                    PridetiTaskai = table.Column<int>(type: "integer", nullable: false),
                    ApmokejimoMetodas = table.Column<int>(type: "integer", nullable: false),
                    MokejimoStatusas = table.Column<int>(type: "integer", nullable: false),
                    KlientasId = table.Column<int>(type: "integer", nullable: false),
                    UzsakymasId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Apmokejimai", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Apmokejimai_Klientai_KlientasId",
                        column: x => x.KlientasId,
                        principalTable: "Klientai",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Apmokejimai_Uzsakymai_UzsakymasId",
                        column: x => x.UzsakymasId,
                        principalTable: "Uzsakymai",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Parduotuves",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    DarboLaikoPradzia = table.Column<string>(type: "text", nullable: false),
                    DarboLaikoPabaiga = table.Column<string>(type: "text", nullable: false),
                    Kvadratura = table.Column<double>(type: "double precision", nullable: false),
                    DarbuotojuKiekis = table.Column<int>(type: "integer", nullable: false),
                    TelNumeris = table.Column<string>(type: "text", nullable: false),
                    ElPastas = table.Column<string>(type: "text", nullable: false),
                    Faksas = table.Column<string>(type: "text", nullable: false),
                    AdresasId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Parduotuves", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Parduotuves_Adresai_AdresasId",
                        column: x => x.AdresasId,
                        principalTable: "Adresai",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Taskai",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PabaigosData = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Kiekis = table.Column<int>(type: "integer", nullable: false),
                    KlientasId = table.Column<int>(type: "integer", nullable: false),
                    ApmokejimasId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Taskai", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Taskai_Apmokejimai_ApmokejimasId",
                        column: x => x.ApmokejimasId,
                        principalTable: "Apmokejimai",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Taskai_Klientai_KlientasId",
                        column: x => x.KlientasId,
                        principalTable: "Klientai",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Vadybininkai",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Skyrius = table.Column<string>(type: "text", nullable: false),
                    NaudotojasId = table.Column<int>(type: "integer", nullable: false),
                    ParduotuveId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Vadybininkai", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Vadybininkai_Naudotojai_NaudotojasId",
                        column: x => x.NaudotojasId,
                        principalTable: "Naudotojai",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Vadybininkai_Parduotuves_ParduotuveId",
                        column: x => x.ParduotuveId,
                        principalTable: "Parduotuves",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Kategorijos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Pavadinimas = table.Column<string>(type: "text", nullable: false),
                    Aprasymas = table.Column<string>(type: "text", nullable: false),
                    VadybininkasId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Kategorijos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Kategorijos_Vadybininkai_VadybininkasId",
                        column: x => x.VadybininkasId,
                        principalTable: "Vadybininkai",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Prekes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Kaina = table.Column<double>(type: "double precision", nullable: false),
                    Pavadinimas = table.Column<string>(type: "text", nullable: false),
                    Kodas = table.Column<int>(type: "integer", nullable: false),
                    GaliojimoData = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Kiekis = table.Column<int>(type: "integer", nullable: false),
                    Ismatavimai = table.Column<string>(type: "text", nullable: false),
                    NuotraukosUrl = table.Column<string>(type: "text", nullable: false),
                    GarantinisLaikotarpis = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Aprasymas = table.Column<string>(type: "text", nullable: false),
                    RekomendacijosSvoris = table.Column<double>(type: "double precision", nullable: false),
                    Mase = table.Column<double>(type: "double precision", nullable: false),
                    VadybininkasId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Prekes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Prekes_Vadybininkai_VadybininkasId",
                        column: x => x.VadybininkasId,
                        principalTable: "Vadybininkai",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MegstamosKategorijos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PridejimoData = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    KategorijaId = table.Column<int>(type: "integer", nullable: false),
                    KlientasId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MegstamosKategorijos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MegstamosKategorijos_Kategorijos_KategorijaId",
                        column: x => x.KategorijaId,
                        principalTable: "Kategorijos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MegstamosKategorijos_Klientai_KlientasId",
                        column: x => x.KlientasId,
                        principalTable: "Klientai",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Atsiliepimai",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Komentaras = table.Column<string>(type: "text", nullable: false),
                    IvercioVerte = table.Column<int>(type: "integer", nullable: false),
                    Data = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Titulas = table.Column<string>(type: "text", nullable: false),
                    PrekeId = table.Column<int>(type: "integer", nullable: false),
                    KlientasId = table.Column<int>(type: "integer", nullable: false)
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

            migrationBuilder.CreateTable(
                name: "Nuolaidos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Procentai = table.Column<int>(type: "integer", nullable: false),
                    PabaigosData = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UzsakymasId = table.Column<int>(type: "integer", nullable: false),
                    PrekeId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Nuolaidos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Nuolaidos_Prekes_PrekeId",
                        column: x => x.PrekeId,
                        principalTable: "Prekes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Nuolaidos_Uzsakymai_UzsakymasId",
                        column: x => x.UzsakymasId,
                        principalTable: "Uzsakymai",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PrekesKategorijos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PrekeId = table.Column<int>(type: "integer", nullable: false),
                    KategorijaId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PrekesKategorijos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PrekesKategorijos_Kategorijos_KategorijaId",
                        column: x => x.KategorijaId,
                        principalTable: "Kategorijos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PrekesKategorijos_Prekes_PrekeId",
                        column: x => x.PrekeId,
                        principalTable: "Prekes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PrekesParduotuves",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Kiekis = table.Column<int>(type: "integer", nullable: false),
                    ParduotuveId = table.Column<int>(type: "integer", nullable: false),
                    PrekeId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PrekesParduotuves", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PrekesParduotuves_Parduotuves_ParduotuveId",
                        column: x => x.ParduotuveId,
                        principalTable: "Parduotuves",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PrekesParduotuves_Prekes_PrekeId",
                        column: x => x.PrekeId,
                        principalTable: "Prekes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PrekesPerziuros",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Data = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Kiekis = table.Column<int>(type: "integer", nullable: false),
                    PrekeId = table.Column<int>(type: "integer", nullable: false),
                    KlientasId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PrekesPerziuros", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PrekesPerziuros_Klientai_KlientasId",
                        column: x => x.KlientasId,
                        principalTable: "Klientai",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PrekesPerziuros_Prekes_PrekeId",
                        column: x => x.PrekeId,
                        principalTable: "Prekes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PrekesUzsakymai",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Kiekis = table.Column<int>(type: "integer", nullable: false),
                    PrekeId = table.Column<int>(type: "integer", nullable: false),
                    UzsakymasId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PrekesUzsakymai", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PrekesUzsakymai_Prekes_PrekeId",
                        column: x => x.PrekeId,
                        principalTable: "Prekes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PrekesUzsakymai_Uzsakymai_UzsakymasId",
                        column: x => x.UzsakymasId,
                        principalTable: "Uzsakymai",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UzblokuotosRekomendacijos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PridejimoData = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    KlientasId = table.Column<int>(type: "integer", nullable: false),
                    PrekeId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UzblokuotosRekomendacijos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UzblokuotosRekomendacijos_Klientai_KlientasId",
                        column: x => x.KlientasId,
                        principalTable: "Klientai",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UzblokuotosRekomendacijos_Prekes_PrekeId",
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
                name: "IX_Apmokejimai_KlientasId",
                table: "Apmokejimai",
                column: "KlientasId");

            migrationBuilder.CreateIndex(
                name: "IX_Apmokejimai_UzsakymasId",
                table: "Apmokejimai",
                column: "UzsakymasId");

            migrationBuilder.CreateIndex(
                name: "IX_Atsiliepimai_KlientasId",
                table: "Atsiliepimai",
                column: "KlientasId");

            migrationBuilder.CreateIndex(
                name: "IX_Atsiliepimai_PrekeId",
                table: "Atsiliepimai",
                column: "PrekeId");

            migrationBuilder.CreateIndex(
                name: "IX_Kategorijos_VadybininkasId",
                table: "Kategorijos",
                column: "VadybininkasId");

            migrationBuilder.CreateIndex(
                name: "IX_Klientai_NaudotojasId",
                table: "Klientai",
                column: "NaudotojasId");

            migrationBuilder.CreateIndex(
                name: "IX_MegstamosKategorijos_KategorijaId",
                table: "MegstamosKategorijos",
                column: "KategorijaId");

            migrationBuilder.CreateIndex(
                name: "IX_MegstamosKategorijos_KlientasId",
                table: "MegstamosKategorijos",
                column: "KlientasId");

            migrationBuilder.CreateIndex(
                name: "IX_Nuolaidos_PrekeId",
                table: "Nuolaidos",
                column: "PrekeId");

            migrationBuilder.CreateIndex(
                name: "IX_Nuolaidos_UzsakymasId",
                table: "Nuolaidos",
                column: "UzsakymasId");

            migrationBuilder.CreateIndex(
                name: "IX_Parduotuves_AdresasId",
                table: "Parduotuves",
                column: "AdresasId");

            migrationBuilder.CreateIndex(
                name: "IX_Prekes_VadybininkasId",
                table: "Prekes",
                column: "VadybininkasId");

            migrationBuilder.CreateIndex(
                name: "IX_PrekesKategorijos_KategorijaId",
                table: "PrekesKategorijos",
                column: "KategorijaId");

            migrationBuilder.CreateIndex(
                name: "IX_PrekesKategorijos_PrekeId",
                table: "PrekesKategorijos",
                column: "PrekeId");

            migrationBuilder.CreateIndex(
                name: "IX_PrekesParduotuves_ParduotuveId",
                table: "PrekesParduotuves",
                column: "ParduotuveId");

            migrationBuilder.CreateIndex(
                name: "IX_PrekesParduotuves_PrekeId",
                table: "PrekesParduotuves",
                column: "PrekeId");

            migrationBuilder.CreateIndex(
                name: "IX_PrekesPerziuros_KlientasId",
                table: "PrekesPerziuros",
                column: "KlientasId");

            migrationBuilder.CreateIndex(
                name: "IX_PrekesPerziuros_PrekeId",
                table: "PrekesPerziuros",
                column: "PrekeId");

            migrationBuilder.CreateIndex(
                name: "IX_PrekesUzsakymai_PrekeId",
                table: "PrekesUzsakymai",
                column: "PrekeId");

            migrationBuilder.CreateIndex(
                name: "IX_PrekesUzsakymai_UzsakymasId",
                table: "PrekesUzsakymai",
                column: "UzsakymasId");

            migrationBuilder.CreateIndex(
                name: "IX_Taskai_ApmokejimasId",
                table: "Taskai",
                column: "ApmokejimasId");

            migrationBuilder.CreateIndex(
                name: "IX_Taskai_KlientasId",
                table: "Taskai",
                column: "KlientasId");

            migrationBuilder.CreateIndex(
                name: "IX_UzblokuotosRekomendacijos_KlientasId",
                table: "UzblokuotosRekomendacijos",
                column: "KlientasId");

            migrationBuilder.CreateIndex(
                name: "IX_UzblokuotosRekomendacijos_PrekeId",
                table: "UzblokuotosRekomendacijos",
                column: "PrekeId");

            migrationBuilder.CreateIndex(
                name: "IX_Vadybininkai_NaudotojasId",
                table: "Vadybininkai",
                column: "NaudotojasId");

            migrationBuilder.CreateIndex(
                name: "IX_Vadybininkai_ParduotuveId",
                table: "Vadybininkai",
                column: "ParduotuveId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Atsiliepimai");

            migrationBuilder.DropTable(
                name: "MegstamosKategorijos");

            migrationBuilder.DropTable(
                name: "Nuolaidos");

            migrationBuilder.DropTable(
                name: "PrekesKategorijos");

            migrationBuilder.DropTable(
                name: "PrekesParduotuves");

            migrationBuilder.DropTable(
                name: "PrekesPerziuros");

            migrationBuilder.DropTable(
                name: "PrekesUzsakymai");

            migrationBuilder.DropTable(
                name: "Taskai");

            migrationBuilder.DropTable(
                name: "UzblokuotosRekomendacijos");

            migrationBuilder.DropTable(
                name: "Kategorijos");

            migrationBuilder.DropTable(
                name: "Apmokejimai");

            migrationBuilder.DropTable(
                name: "Prekes");

            migrationBuilder.DropTable(
                name: "Uzsakymai");

            migrationBuilder.DropTable(
                name: "Vadybininkai");

            migrationBuilder.DropTable(
                name: "Parduotuves");

            migrationBuilder.DropTable(
                name: "Adresai");

            migrationBuilder.DropTable(
                name: "Klientai");

            migrationBuilder.DropTable(
                name: "Naudotojai");
        }
    }
}
