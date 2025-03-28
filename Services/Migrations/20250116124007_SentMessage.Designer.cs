﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using Services.Data;

#nullable disable

namespace CoreNotify.API.Migrations
{
	[DbContext(typeof(ApplicationDbContext))]
    [Migration("20250116124007_SentMessage")]
    partial class SentMessage
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "9.0.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("CoreNotify.API.Data.Entities.Account", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("ApiKey")
                        .IsRequired()
                        .HasMaxLength(32)
                        .HasColumnType("character varying(32)");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp without time zone");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("character varying(50)");

                    b.Property<bool>("EmailConfirmed")
                        .HasColumnType("boolean");

                    b.Property<DateTime>("RenewalDate")
                        .HasColumnType("timestamp without time zone");

                    b.Property<DateTime?>("UpdatedAt")
                        .HasColumnType("timestamp without time zone");

                    b.HasKey("Id");

                    b.HasIndex("Email")
                        .IsUnique();

                    b.ToTable("Accounts");
                });

            modelBuilder.Entity("CoreNotify.API.Data.Entities.DailyUsage", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<int>("AccountId")
                        .HasColumnType("integer");

                    b.Property<int>("Confirmations")
                        .HasColumnType("integer");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp without time zone");

                    b.Property<DateOnly>("Date")
                        .HasColumnType("date");

                    b.Property<int>("ResetCodes")
                        .HasColumnType("integer");

                    b.Property<int>("ResetLinks")
                        .HasColumnType("integer");

                    b.Property<DateTime?>("UpdatedAt")
                        .HasColumnType("timestamp without time zone");

                    b.HasKey("Id");

                    b.HasIndex("AccountId", "Date")
                        .IsUnique();

                    b.ToTable("DailyUsage");
                });

            modelBuilder.Entity("CoreNotify.API.Data.Entities.SentMessage", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<int>("AccountId")
                        .HasColumnType("integer");

                    b.Property<DateTime?>("BounceDateTime")
                        .HasColumnType("timestamp without time zone");

                    b.Property<bool>("Bounced")
                        .HasColumnType("boolean");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp without time zone");

                    b.Property<string>("MessageId")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("character varying(50)");

                    b.Property<int>("MessageType")
                        .HasColumnType("integer");

                    b.Property<string>("Recipient")
                        .IsRequired()
                        .HasMaxLength(64)
                        .HasColumnType("character varying(64)");

                    b.Property<DateTime>("Timestamp")
                        .HasColumnType("timestamp without time zone");

                    b.Property<DateTime?>("UpdatedAt")
                        .HasColumnType("timestamp without time zone");

                    b.HasKey("Id");

                    b.HasAlternateKey("MessageId");

                    b.HasIndex("AccountId");

                    b.ToTable("SentMessage");
                });

            modelBuilder.Entity("CoreNotify.API.Data.Entities.DailyUsage", b =>
                {
                    b.HasOne("CoreNotify.API.Data.Entities.Account", "Account")
                        .WithMany("DailyUsage")
                        .HasForeignKey("AccountId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Account");
                });

            modelBuilder.Entity("CoreNotify.API.Data.Entities.SentMessage", b =>
                {
                    b.HasOne("CoreNotify.API.Data.Entities.Account", "Account")
                        .WithMany("SentMessages")
                        .HasForeignKey("AccountId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Account");
                });

            modelBuilder.Entity("CoreNotify.API.Data.Entities.Account", b =>
                {
                    b.Navigation("DailyUsage");

                    b.Navigation("SentMessages");
                });
#pragma warning restore 612, 618
        }
    }
}
