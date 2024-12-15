﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using Stotele.Server.Models.ApplicationDbContexts;

#nullable disable

namespace Stotele.Server.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    partial class ApplicationDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.10")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("Stotele.Server.Models.Apmokejimas", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<int>("ApmokejimoMetodas")
                        .HasColumnType("integer");

                    b.Property<double>("GalutineSuma")
                        .HasColumnType("double precision");

                    b.Property<int>("KlientasId")
                        .HasColumnType("integer");

                    b.Property<int>("MokejimoStatusas")
                        .HasColumnType("integer");

                    b.Property<int>("PanaudotiTaskai")
                        .HasColumnType("integer");

                    b.Property<int>("PridetiTaskai")
                        .HasColumnType("integer");

                    b.Property<string>("PvmMoketojoKodas")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("SaskaitosFakturosNumeris")
                        .HasColumnType("integer");

                    b.Property<int>("UzsakymasId")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("KlientasId");

                    b.HasIndex("UzsakymasId");

                    b.ToTable("Apmokejimai");
                });

            modelBuilder.Entity("Stotele.Server.Models.Kategorija", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("Aprasymas")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Pavadinimas")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("VadybininkasId")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("VadybininkasId");

                    b.ToTable("Kategorijos");
                });

            modelBuilder.Entity("Stotele.Server.Models.Klientas", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("Adresas")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<DateTime>("GimimoData")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Miestas")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("NaudotojasId")
                        .HasColumnType("integer");

                    b.Property<int>("PastoKodas")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("NaudotojasId");

                    b.ToTable("Klientai");
                });

            modelBuilder.Entity("Stotele.Server.Models.MegstamaKategorija", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<int>("KategorijaId")
                        .HasColumnType("integer");

                    b.Property<int>("KlientasId")
                        .HasColumnType("integer");

                    b.Property<DateTime>("PridejimoData")
                        .HasColumnType("timestamp with time zone");

                    b.HasKey("Id");

                    b.HasIndex("KategorijaId");

                    b.HasIndex("KlientasId");

                    b.ToTable("MegstamosKategorijos");
                });

            modelBuilder.Entity("Stotele.Server.Models.Naudotojas", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<bool>("Administratorius")
                        .HasColumnType("boolean");

                    b.Property<string>("ElektroninisPastas")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Lytis")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Pavarde")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Slaptazodis")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Slapyvardis")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Vardas")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("Naudotojai");
                });

            modelBuilder.Entity("Stotele.Server.Models.Nuolaida", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("PabaigosData")
                        .HasColumnType("timestamp with time zone");

                    b.Property<int>("PrekeId")
                        .HasColumnType("integer");

                    b.Property<int>("Procentai")
                        .HasColumnType("integer");

                    b.Property<int?>("UzsakymasId")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("PrekeId");

                    b.HasIndex("UzsakymasId");

                    b.ToTable("Nuolaidos");
                });

            modelBuilder.Entity("Stotele.Server.Models.Parduotuve", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("Adresas")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("DarboLaikoPabaiga")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("DarboLaikoPradzia")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("DarbuotojuKiekis")
                        .HasColumnType("integer");

                    b.Property<string>("ElPastas")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Faksas")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<double>("Kvadratura")
                        .HasColumnType("double precision");

                    b.Property<string>("TelNumeris")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("Parduotuve");
                });

            modelBuilder.Entity("Stotele.Server.Models.Preke", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("Aprasymas")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<DateTime>("GaliojimoData")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTime>("GarantinisLaikotarpis")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Ismatavimai")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<double>("Kaina")
                        .HasColumnType("double precision");

                    b.Property<int>("Kiekis")
                        .HasColumnType("integer");

                    b.Property<int>("Kodas")
                        .HasColumnType("integer");

                    b.Property<double>("Mase")
                        .HasColumnType("double precision");

                    b.Property<string>("NuotraukosUrl")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Pavadinimas")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<double>("RekomendacijosSvoris")
                        .HasColumnType("double precision");

                    b.Property<int>("VadybininkasId")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("VadybininkasId");

                    b.ToTable("Prekes");
                });

            modelBuilder.Entity("Stotele.Server.Models.PrekesKategorija", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<int>("KategorijaId")
                        .HasColumnType("integer");

                    b.Property<int>("PrekeId")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("KategorijaId");

                    b.HasIndex("PrekeId");

                    b.ToTable("PrekesKategorijos");
                });

            modelBuilder.Entity("Stotele.Server.Models.PrekesParduotuve", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<int>("Kiekis")
                        .HasColumnType("integer");

                    b.Property<int>("ParduotuveId")
                        .HasColumnType("integer");

                    b.Property<int>("PrekeId")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("ParduotuveId");

                    b.HasIndex("PrekeId");

                    b.ToTable("PrekesParduotuve");
                });

            modelBuilder.Entity("Stotele.Server.Models.PrekesPerziura", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("Data")
                        .HasColumnType("timestamp with time zone");

                    b.Property<int>("Kiekis")
                        .HasColumnType("integer");

                    b.Property<int>("KlientasId")
                        .HasColumnType("integer");

                    b.Property<int>("PrekeId")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("KlientasId");

                    b.HasIndex("PrekeId");

                    b.ToTable("PrekesPerziuros");
                });

            modelBuilder.Entity("Stotele.Server.Models.PrekesUzsakymas", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<double?>("Kaina")
                        .HasColumnType("double precision");

                    b.Property<int>("Kiekis")
                        .HasColumnType("integer");

                    b.Property<int>("PrekeId")
                        .HasColumnType("integer");

                    b.Property<int>("UzsakymasId")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("PrekeId");

                    b.HasIndex("UzsakymasId");

                    b.ToTable("PrekesUzsakymai");
                });

            modelBuilder.Entity("Stotele.Server.Models.Taskai", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<int?>("ApmokejimasId")
                        .HasColumnType("integer");

                    b.Property<int>("Kiekis")
                        .HasColumnType("integer");

                    b.Property<int>("KlientasId")
                        .HasColumnType("integer");

                    b.Property<DateTime>("PabaigosData")
                        .HasColumnType("timestamp with time zone");

                    b.HasKey("Id");

                    b.HasIndex("ApmokejimasId");

                    b.HasIndex("KlientasId");

                    b.ToTable("Taskai");
                });

            modelBuilder.Entity("Stotele.Server.Models.UzblokuotaRekomendacija", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<int>("KlientasId")
                        .HasColumnType("integer");

                    b.Property<int>("PrekeId")
                        .HasColumnType("integer");

                    b.Property<DateTime>("PridejimoData")
                        .HasColumnType("timestamp with time zone");

                    b.HasKey("Id");

                    b.HasIndex("KlientasId");

                    b.HasIndex("PrekeId");

                    b.ToTable("UzblokuotosRekomendacijos");
                });

            modelBuilder.Entity("Stotele.Server.Models.Uzsakymas", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("Data")
                        .HasColumnType("timestamp with time zone");

                    b.Property<int>("NaudotojasId")
                        .HasColumnType("integer");

                    b.Property<bool>("Patvirtintas")
                        .HasColumnType("boolean");

                    b.Property<double>("Suma")
                        .HasColumnType("double precision");

                    b.HasKey("Id");

                    b.HasIndex("NaudotojasId");

                    b.ToTable("Uzsakymai");
                });

            modelBuilder.Entity("Stotele.Server.Models.Vadybininkas", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<int>("NaudotojasId")
                        .HasColumnType("integer");

                    b.Property<int>("ParduotuveId")
                        .HasColumnType("integer");

                    b.Property<string>("Skyrius")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("NaudotojasId");

                    b.HasIndex("ParduotuveId");

                    b.ToTable("Vadybininkai");
                });

            modelBuilder.Entity("Stotele.Server.Models.Apmokejimas", b =>
                {
                    b.HasOne("Stotele.Server.Models.Klientas", "Klientas")
                        .WithMany()
                        .HasForeignKey("KlientasId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Stotele.Server.Models.Uzsakymas", "Uzsakymas")
                        .WithMany()
                        .HasForeignKey("UzsakymasId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Klientas");

                    b.Navigation("Uzsakymas");
                });

            modelBuilder.Entity("Stotele.Server.Models.Kategorija", b =>
                {
                    b.HasOne("Stotele.Server.Models.Vadybininkas", "Vadybininkas")
                        .WithMany()
                        .HasForeignKey("VadybininkasId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Vadybininkas");
                });

            modelBuilder.Entity("Stotele.Server.Models.Klientas", b =>
                {
                    b.HasOne("Stotele.Server.Models.Naudotojas", "Naudotojas")
                        .WithMany()
                        .HasForeignKey("NaudotojasId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Naudotojas");
                });

            modelBuilder.Entity("Stotele.Server.Models.MegstamaKategorija", b =>
                {
                    b.HasOne("Stotele.Server.Models.Kategorija", "Kategorija")
                        .WithMany()
                        .HasForeignKey("KategorijaId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Stotele.Server.Models.Klientas", "Klientas")
                        .WithMany()
                        .HasForeignKey("KlientasId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Kategorija");

                    b.Navigation("Klientas");
                });

            modelBuilder.Entity("Stotele.Server.Models.Nuolaida", b =>
                {
                    b.HasOne("Stotele.Server.Models.Preke", "Preke")
                        .WithMany()
                        .HasForeignKey("PrekeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Stotele.Server.Models.Uzsakymas", "Uzsakymas")
                        .WithMany()
                        .HasForeignKey("UzsakymasId");

                    b.Navigation("Preke");

                    b.Navigation("Uzsakymas");
                });

            modelBuilder.Entity("Stotele.Server.Models.Preke", b =>
                {
                    b.HasOne("Stotele.Server.Models.Vadybininkas", "Vadybininkas")
                        .WithMany()
                        .HasForeignKey("VadybininkasId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Vadybininkas");
                });

            modelBuilder.Entity("Stotele.Server.Models.PrekesKategorija", b =>
                {
                    b.HasOne("Stotele.Server.Models.Kategorija", "Kategorija")
                        .WithMany()
                        .HasForeignKey("KategorijaId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Stotele.Server.Models.Preke", "Preke")
                        .WithMany()
                        .HasForeignKey("PrekeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Kategorija");

                    b.Navigation("Preke");
                });

            modelBuilder.Entity("Stotele.Server.Models.PrekesParduotuve", b =>
                {
                    b.HasOne("Stotele.Server.Models.Parduotuve", "Parduotuve")
                        .WithMany()
                        .HasForeignKey("ParduotuveId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Stotele.Server.Models.Preke", "Preke")
                        .WithMany()
                        .HasForeignKey("PrekeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Parduotuve");

                    b.Navigation("Preke");
                });

            modelBuilder.Entity("Stotele.Server.Models.PrekesPerziura", b =>
                {
                    b.HasOne("Stotele.Server.Models.Klientas", "Klientas")
                        .WithMany()
                        .HasForeignKey("KlientasId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Stotele.Server.Models.Preke", "Preke")
                        .WithMany()
                        .HasForeignKey("PrekeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Klientas");

                    b.Navigation("Preke");
                });

            modelBuilder.Entity("Stotele.Server.Models.PrekesUzsakymas", b =>
                {
                    b.HasOne("Stotele.Server.Models.Preke", "Preke")
                        .WithMany()
                        .HasForeignKey("PrekeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Stotele.Server.Models.Uzsakymas", "Uzsakymas")
                        .WithMany("PrekesUzsakymai")
                        .HasForeignKey("UzsakymasId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Preke");

                    b.Navigation("Uzsakymas");
                });

            modelBuilder.Entity("Stotele.Server.Models.Taskai", b =>
                {
                    b.HasOne("Stotele.Server.Models.Apmokejimas", "Apmokejimas")
                        .WithMany()
                        .HasForeignKey("ApmokejimasId");

                    b.HasOne("Stotele.Server.Models.Klientas", "Klientas")
                        .WithMany()
                        .HasForeignKey("KlientasId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Apmokejimas");

                    b.Navigation("Klientas");
                });

            modelBuilder.Entity("Stotele.Server.Models.UzblokuotaRekomendacija", b =>
                {
                    b.HasOne("Stotele.Server.Models.Klientas", "Klientas")
                        .WithMany()
                        .HasForeignKey("KlientasId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Stotele.Server.Models.Preke", "Preke")
                        .WithMany()
                        .HasForeignKey("PrekeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Klientas");

                    b.Navigation("Preke");
                });

            modelBuilder.Entity("Stotele.Server.Models.Uzsakymas", b =>
                {
                    b.HasOne("Stotele.Server.Models.Naudotojas", "Naudotojas")
                        .WithMany()
                        .HasForeignKey("NaudotojasId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Naudotojas");
                });

            modelBuilder.Entity("Stotele.Server.Models.Vadybininkas", b =>
                {
                    b.HasOne("Stotele.Server.Models.Naudotojas", "Naudotojas")
                        .WithMany()
                        .HasForeignKey("NaudotojasId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Stotele.Server.Models.Parduotuve", "Parduotuve")
                        .WithMany()
                        .HasForeignKey("ParduotuveId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Naudotojas");

                    b.Navigation("Parduotuve");
                });

            modelBuilder.Entity("Stotele.Server.Models.Uzsakymas", b =>
                {
                    b.Navigation("PrekesUzsakymai");
                });
#pragma warning restore 612, 618
        }
    }
}
