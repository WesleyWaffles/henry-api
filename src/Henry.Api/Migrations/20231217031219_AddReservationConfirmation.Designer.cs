﻿// <auto-generated />
using System;
using Henry.Api.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace Henry.Api.Migrations
{
    [DbContext(typeof(HenryDbContenxt))]
    [Migration("20231217031219_AddReservationConfirmation")]
    partial class AddReservationConfirmation
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "8.0.0");

            modelBuilder.Entity("Henry.Api.Data.Appointment", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<TimeOnly>("AppointmentFrom")
                        .HasColumnType("TEXT");

                    b.Property<DateOnly>("AppointmentOn")
                        .HasColumnType("TEXT");

                    b.Property<TimeOnly>("AppointmentTo")
                        .HasColumnType("TEXT");

                    b.Property<int>("ClientId")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("Confirmed")
                        .HasColumnType("INTEGER");

                    b.Property<int>("ProviderId")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("ReservedOn")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("ClientId");

                    b.HasIndex("ProviderId");

                    b.ToTable("Appointments");
                });

            modelBuilder.Entity("Henry.Api.Data.Client", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Email")
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("Clients");
                });

            modelBuilder.Entity("Henry.Api.Data.Provider", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("Providers");
                });

            modelBuilder.Entity("Henry.Api.Data.ProviderAvailability", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<TimeOnly>("AvailableFrom")
                        .HasColumnType("TEXT");

                    b.Property<DateOnly>("AvailableOn")
                        .HasColumnType("TEXT");

                    b.Property<TimeOnly>("AvailableTo")
                        .HasColumnType("TEXT");

                    b.Property<int>("ProviderId")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("ProviderId");

                    b.ToTable("ProviderAvailabilities");
                });

            modelBuilder.Entity("Henry.Api.Data.Appointment", b =>
                {
                    b.HasOne("Henry.Api.Data.Client", "Client")
                        .WithMany()
                        .HasForeignKey("ClientId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Henry.Api.Data.Provider", "Provider")
                        .WithMany()
                        .HasForeignKey("ProviderId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Client");

                    b.Navigation("Provider");
                });

            modelBuilder.Entity("Henry.Api.Data.ProviderAvailability", b =>
                {
                    b.HasOne("Henry.Api.Data.Provider", "Provider")
                        .WithMany()
                        .HasForeignKey("ProviderId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Provider");
                });
#pragma warning restore 612, 618
        }
    }
}
