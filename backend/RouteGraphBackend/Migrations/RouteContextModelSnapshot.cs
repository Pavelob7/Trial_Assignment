﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using RouteGraphBackend.Data;

#nullable disable

namespace RouteGraphBackend.Migrations
{
    [DbContext(typeof(RouteGraphBackend.Data.RouteContext))]
    partial class RouteContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.18")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("RouteGraphBackend.Models.Point", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<double>("Height")
                        .HasColumnType("double precision");

                    b.Property<double>("Latitude")
                        .HasColumnType("double precision");

                    b.Property<double>("Longitude")
                        .HasColumnType("double precision");

                    b.Property<int?>("RouteId")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("RouteId");

                    b.ToTable("Points");
                });

            modelBuilder.Entity("RouteGraphBackend.Models.Route", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("Routes");
                });

            modelBuilder.Entity("RouteGraphBackend.Models.Track", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<double>("Distance")
                        .HasColumnType("double precision");

                    b.Property<int>("FirstId")
                        .HasColumnType("integer");

                    b.Property<double>("MaxSpeed")
                        .HasColumnType("double precision");

                    b.Property<int?>("RouteId")
                        .HasColumnType("integer");

                    b.Property<int>("SecondId")
                        .HasColumnType("integer");

                    b.Property<string>("Surface")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("RouteId");

                    b.ToTable("Tracks");
                });

            modelBuilder.Entity("RouteGraphBackend.Models.Point", b =>
                {
                    b.HasOne("RouteGraphBackend.Models.Route", null)
                        .WithMany("Points")
                        .HasForeignKey("RouteId");
                });

            modelBuilder.Entity("RouteGraphBackend.Models.Track", b =>
                {
                    b.HasOne("RouteGraphBackend.Models.Route", null)
                        .WithMany("Tracks")
                        .HasForeignKey("RouteId");
                });

            modelBuilder.Entity("RouteGraphBackend.Models.Route", b =>
                {
                    b.Navigation("Points");

                    b.Navigation("Tracks");
                });
#pragma warning restore 612, 618
        }
    }
}
