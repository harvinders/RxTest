﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using RxTest.Core;

namespace RxTest.Core.Migrations
{
    [DbContext(typeof(DatabaseContext))]
    [Migration("20201013040435_Initial")]
    partial class Initial
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "3.1.8");

            modelBuilder.Entity("RxTest.Article", b =>
                {
                    b.Property<string>("Url")
                        .HasColumnType("TEXT");

                    b.Property<string>("Author")
                        .HasColumnType("TEXT");

                    b.Property<string>("Description")
                        .HasColumnType("TEXT");

                    b.Property<string>("ImageUrl")
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("LastActiveDate")
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("PublicationDate")
                        .HasColumnType("date");

                    b.Property<string>("SourceUrl")
                        .HasColumnType("TEXT");

                    b.Property<string>("Title")
                        .HasColumnType("TEXT");

                    b.HasKey("Url");

                    b.HasIndex("SourceUrl");

                    b.ToTable("Articles");
                });

            modelBuilder.Entity("RxTest.NewsSource", b =>
                {
                    b.Property<string>("SourceUrl")
                        .HasColumnType("TEXT");

                    b.Property<string>("Author")
                        .HasColumnType("TEXT");

                    b.Property<string>("Description")
                        .HasColumnType("TEXT");

                    b.Property<string>("HomeUrl")
                        .HasColumnType("TEXT");

                    b.Property<string>("ImageUrl")
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("PublicationDate")
                        .HasColumnType("date");

                    b.Property<string>("Title")
                        .HasColumnType("TEXT");

                    b.HasKey("SourceUrl");

                    b.ToTable("NewsSources");
                });

            modelBuilder.Entity("RxTest.Article", b =>
                {
                    b.HasOne("RxTest.NewsSource", null)
                        .WithMany("Articles")
                        .HasForeignKey("SourceUrl")
                        .OnDelete(DeleteBehavior.Cascade);
                });
#pragma warning restore 612, 618
        }
    }
}
