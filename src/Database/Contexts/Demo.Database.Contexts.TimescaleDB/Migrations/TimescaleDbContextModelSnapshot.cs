﻿// <auto-generated />
using System;
using Demo.Database.Contexts.TimescaleDB;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Demo.Database.Contexts.TimescaleDB.Migrations
{
    [DbContext(typeof(TimescaleDbContext))]
    partial class TimescaleDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.5")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.HasPostgresExtension(modelBuilder, "timescaledb");
            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("Demo.Database.Models.TimeEventData", b =>
                {
                    b.Property<Guid>("EventId")
                        .HasColumnType("uuid")
                        .HasColumnName("eventid");

                    b.Property<string>("Payload")
                        .IsRequired()
                        .HasColumnType("json")
                        .HasColumnName("payload");

                    b.Property<Guid>("StudentId")
                        .HasColumnType("uuid")
                        .HasColumnName("studentid");

                    b.Property<DateTimeOffset>("Timestamp")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("timestamp");

                    b.HasIndex("EventId");

                    b.HasIndex("Timestamp", "StudentId");

                    b.ToTable("timeeventsdata", (string)null);
                });
#pragma warning restore 612, 618
        }
    }
}
