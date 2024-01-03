﻿// <auto-generated />
using System;
using LocalDatabase.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace LocalDatabase.Migrations
{
    [DbContext(typeof(DataContext))]
    [Migration("20240103153405_InitialCreate")]
    partial class InitialCreate
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "7.0.14");

            modelBuilder.Entity("CustomerProductGroup", b =>
                {
                    b.Property<int>("CustomersId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("ProductGroupsId")
                        .HasColumnType("INTEGER");

                    b.HasKey("CustomersId", "ProductGroupsId");

                    b.HasIndex("ProductGroupsId");

                    b.ToTable("CustomerProductGroup");
                });

            modelBuilder.Entity("LocalDatabase.Models.Business", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("City")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("HouseNumber")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("PostalCode")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Street")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("Businesses");
                });

            modelBuilder.Entity("LocalDatabase.Models.Customer", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int?>("BusinessId")
                        .HasColumnType("INTEGER");

                    b.Property<string>("City")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("FirstName")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("HouseNumber")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("LastName")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<int?>("PictureId")
                        .HasColumnType("INTEGER");

                    b.Property<string>("PostalCode")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Street")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("BusinessId");

                    b.HasIndex("PictureId")
                        .IsUnique();

                    b.ToTable("Customers");
                });

            modelBuilder.Entity("LocalDatabase.Models.Picture", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<byte[]>("Data")
                        .HasColumnType("BLOB");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("Pictures");
                });

            modelBuilder.Entity("LocalDatabase.Models.ProductGroup", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("Name")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.ToTable("ProductGroups");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            Name = 1
                        },
                        new
                        {
                            Id = 2,
                            Name = 2
                        },
                        new
                        {
                            Id = 3,
                            Name = 3
                        },
                        new
                        {
                            Id = 4,
                            Name = 4
                        },
                        new
                        {
                            Id = 5,
                            Name = 5
                        });
                });

            modelBuilder.Entity("CustomerProductGroup", b =>
                {
                    b.HasOne("LocalDatabase.Models.Customer", null)
                        .WithMany()
                        .HasForeignKey("CustomersId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("LocalDatabase.Models.ProductGroup", null)
                        .WithMany()
                        .HasForeignKey("ProductGroupsId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("LocalDatabase.Models.Customer", b =>
                {
                    b.HasOne("LocalDatabase.Models.Business", "Business")
                        .WithMany()
                        .HasForeignKey("BusinessId");

                    b.HasOne("LocalDatabase.Models.Picture", "Picture")
                        .WithOne("Customer")
                        .HasForeignKey("LocalDatabase.Models.Customer", "PictureId");

                    b.Navigation("Business");

                    b.Navigation("Picture");
                });

            modelBuilder.Entity("LocalDatabase.Models.Picture", b =>
                {
                    b.Navigation("Customer");
                });
#pragma warning restore 612, 618
        }
    }
}
