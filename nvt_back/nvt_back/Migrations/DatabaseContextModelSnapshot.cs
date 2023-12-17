﻿// <auto-generated />
using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using nvt_back;

#nullable disable

namespace nvt_back.Migrations
{
    [DbContext(typeof(DatabaseContext))]
    partial class DatabaseContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.13")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("nvt_back.Address", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<int>("CityId")
                        .HasColumnType("integer");

                    b.Property<double>("Lat")
                        .HasColumnType("double precision");

                    b.Property<double>("Lng")
                        .HasColumnType("double precision");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("CityId");

                    b.ToTable("Addresses", (string)null);
                });

            modelBuilder.Entity("nvt_back.City", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<int>("CountryId")
                        .HasColumnType("integer");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("PostalCode")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("CountryId");

                    b.ToTable("Cities", (string)null);
                });

            modelBuilder.Entity("nvt_back.Country", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("Countries", (string)null);
                });

            modelBuilder.Entity("nvt_back.Model.ActivationCode", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("Code")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<DateTime>("Expiration")
                        .HasColumnType("timestamp without time zone");

                    b.Property<int>("UserId")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("ActivationCodes", (string)null);
                });

            modelBuilder.Entity("nvt_back.Model.Devices.Device", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<int>("DeviceType")
                        .HasColumnType("integer");

                    b.Property<string>("Image")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<bool>("IsOnline")
                        .HasColumnType("boolean");

                    b.Property<DateTime>("LastHeartbeatTime")
                        .HasColumnType("timestamp without time zone");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<double>("PowerConsumption")
                        .HasColumnType("double precision");

                    b.Property<int>("PowerSource")
                        .HasColumnType("integer");

                    b.Property<int?>("PropertyId")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("PropertyId");

                    b.ToTable("Devices", (string)null);

                    b.UseTptMappingStrategy();
                });

            modelBuilder.Entity("nvt_back.Property", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<int>("AddressId")
                        .HasColumnType("integer");

                    b.Property<double>("Area")
                        .HasColumnType("double precision");

                    b.Property<string>("ImagePath")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("NumOfFloors")
                        .HasColumnType("integer");

                    b.Property<string>("RejectionReason")
                        .HasColumnType("text");

                    b.Property<int>("Status")
                        .HasColumnType("integer");

                    b.Property<int>("UserId")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("AddressId");

                    b.HasIndex("UserId");

                    b.ToTable("Properties", (string)null);
                });

            modelBuilder.Entity("nvt_back.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<bool>("IsActivated")
                        .HasColumnType("boolean");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("Role")
                        .HasColumnType("integer");

                    b.Property<string>("Surname")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("Users", (string)null);

                    b.HasData(
                        new
                        {
                            Id = 1,
                            Email = "bob@mail.com",
                            IsActivated = true,
                            Name = "Bob",
                            Password = "123",
                            Role = 0,
                            Surname = "Ross"
                        },
                        new
                        {
                            Id = 2,
                            Email = "rob@mail.com",
                            IsActivated = true,
                            Name = "Rob",
                            Password = "123",
                            Role = 0,
                            Surname = "Boss"
                        },
                        new
                        {
                            Id = 10,
                            Email = "lob@mail.com",
                            IsActivated = true,
                            Name = "Lob",
                            Password = "123",
                            Role = 2,
                            Surname = "Loss"
                        });
                });

            modelBuilder.Entity("nvt_back.Model.Devices.AirConditioner", b =>
                {
                    b.HasBaseType("nvt_back.Model.Devices.Device");

                    b.Property<double>("MaxTemperature")
                        .HasColumnType("double precision");

                    b.Property<double>("MinTemperature")
                        .HasColumnType("double precision");

                    b.Property<int[]>("SupportedModes")
                        .IsRequired()
                        .HasColumnType("integer[]");

                    b.ToTable("AirConditioners", (string)null);
                });

            modelBuilder.Entity("nvt_back.Model.Devices.AmbientSensor", b =>
                {
                    b.HasBaseType("nvt_back.Model.Devices.Device");

                    b.Property<double>("CurrentHumidity")
                        .HasColumnType("double precision");

                    b.Property<double>("CurrentTemperature")
                        .HasColumnType("double precision");

                    b.Property<int>("UpdateIntervalSeconds")
                        .HasColumnType("integer");

                    b.ToTable("AmbientSensors", (string)null);
                });

            modelBuilder.Entity("nvt_back.Model.Devices.EVCharger", b =>
                {
                    b.HasBaseType("nvt_back.Model.Devices.Device");

                    b.Property<double>("ChargingPower")
                        .HasColumnType("double precision");

                    b.Property<double>("ChargingThreshold")
                        .HasColumnType("double precision");

                    b.Property<int>("NumberOfPorts")
                        .HasColumnType("integer");

                    b.ToTable("EVChargers", (string)null);
                });

            modelBuilder.Entity("nvt_back.Model.Devices.HomeBattery", b =>
                {
                    b.HasBaseType("nvt_back.Model.Devices.Device");

                    b.Property<double>("Capacity")
                        .HasColumnType("double precision");

                    b.Property<double>("CurrentCharge")
                        .HasColumnType("double precision");

                    b.Property<double>("Health")
                        .HasColumnType("double precision");

                    b.ToTable("HomeBatteries", (string)null);
                });

            modelBuilder.Entity("nvt_back.Model.Devices.IrrigationSystem", b =>
                {
                    b.HasBaseType("nvt_back.Model.Devices.Device");

                    b.Property<bool>("IsOn")
                        .HasColumnType("boolean");

                    b.ToTable("IrrigationSystems", (string)null);
                });

            modelBuilder.Entity("nvt_back.Model.Devices.Lamp", b =>
                {
                    b.HasBaseType("nvt_back.Model.Devices.Device");

                    b.Property<int>("BrightnessLevel")
                        .HasColumnType("integer");

                    b.Property<int>("Color")
                        .HasColumnType("integer");

                    b.Property<bool>("IsOn")
                        .HasColumnType("boolean");

                    b.ToTable("Lamps", (string)null);
                });

            modelBuilder.Entity("nvt_back.Model.Devices.SolarPanel", b =>
                {
                    b.HasBaseType("nvt_back.Model.Devices.Device");

                    b.Property<double>("Efficiency")
                        .HasColumnType("double precision");

                    b.Property<bool>("IsOn")
                        .HasColumnType("boolean");

                    b.Property<int>("NumberOfPanels")
                        .HasColumnType("integer");

                    b.Property<double>("Size")
                        .HasColumnType("double precision");

                    b.ToTable("SolarPanels", (string)null);
                });

            modelBuilder.Entity("nvt_back.Model.Devices.VehicleGate", b =>
                {
                    b.HasBaseType("nvt_back.Model.Devices.Device");

                    b.Property<List<string>>("AllowedLicencePlates")
                        .IsRequired()
                        .HasColumnType("text[]");

                    b.Property<bool>("IsOpened")
                        .HasColumnType("boolean");

                    b.Property<bool>("IsPrivateModeOn")
                        .HasColumnType("boolean");

                    b.ToTable("VehicleGates", (string)null);
                });

            modelBuilder.Entity("nvt_back.Model.Devices.WashingMachine", b =>
                {
                    b.HasBaseType("nvt_back.Model.Devices.Device");

                    b.Property<int[]>("SupportedModes")
                        .IsRequired()
                        .HasColumnType("integer[]");

                    b.ToTable("WashingMachines", (string)null);
                });

            modelBuilder.Entity("nvt_back.Address", b =>
                {
                    b.HasOne("nvt_back.City", "City")
                        .WithMany()
                        .HasForeignKey("CityId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("City");
                });

            modelBuilder.Entity("nvt_back.City", b =>
                {
                    b.HasOne("nvt_back.Country", "Country")
                        .WithMany()
                        .HasForeignKey("CountryId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Country");
                });

            modelBuilder.Entity("nvt_back.Model.ActivationCode", b =>
                {
                    b.HasOne("nvt_back.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("nvt_back.Model.Devices.Device", b =>
                {
                    b.HasOne("nvt_back.Property", null)
                        .WithMany("Devices")
                        .HasForeignKey("PropertyId");
                });

            modelBuilder.Entity("nvt_back.Property", b =>
                {
                    b.HasOne("nvt_back.Address", "Address")
                        .WithMany()
                        .HasForeignKey("AddressId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("nvt_back.User", "Owner")
                        .WithMany("OwnedProperties")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Address");

                    b.Navigation("Owner");
                });

            modelBuilder.Entity("nvt_back.Model.Devices.AirConditioner", b =>
                {
                    b.HasOne("nvt_back.Model.Devices.Device", null)
                        .WithOne()
                        .HasForeignKey("nvt_back.Model.Devices.AirConditioner", "Id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("nvt_back.Model.Devices.AmbientSensor", b =>
                {
                    b.HasOne("nvt_back.Model.Devices.Device", null)
                        .WithOne()
                        .HasForeignKey("nvt_back.Model.Devices.AmbientSensor", "Id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("nvt_back.Model.Devices.EVCharger", b =>
                {
                    b.HasOne("nvt_back.Model.Devices.Device", null)
                        .WithOne()
                        .HasForeignKey("nvt_back.Model.Devices.EVCharger", "Id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("nvt_back.Model.Devices.HomeBattery", b =>
                {
                    b.HasOne("nvt_back.Model.Devices.Device", null)
                        .WithOne()
                        .HasForeignKey("nvt_back.Model.Devices.HomeBattery", "Id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("nvt_back.Model.Devices.IrrigationSystem", b =>
                {
                    b.HasOne("nvt_back.Model.Devices.Device", null)
                        .WithOne()
                        .HasForeignKey("nvt_back.Model.Devices.IrrigationSystem", "Id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("nvt_back.Model.Devices.Lamp", b =>
                {
                    b.HasOne("nvt_back.Model.Devices.Device", null)
                        .WithOne()
                        .HasForeignKey("nvt_back.Model.Devices.Lamp", "Id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("nvt_back.Model.Devices.SolarPanel", b =>
                {
                    b.HasOne("nvt_back.Model.Devices.Device", null)
                        .WithOne()
                        .HasForeignKey("nvt_back.Model.Devices.SolarPanel", "Id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("nvt_back.Model.Devices.VehicleGate", b =>
                {
                    b.HasOne("nvt_back.Model.Devices.Device", null)
                        .WithOne()
                        .HasForeignKey("nvt_back.Model.Devices.VehicleGate", "Id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("nvt_back.Model.Devices.WashingMachine", b =>
                {
                    b.HasOne("nvt_back.Model.Devices.Device", null)
                        .WithOne()
                        .HasForeignKey("nvt_back.Model.Devices.WashingMachine", "Id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("nvt_back.Property", b =>
                {
                    b.Navigation("Devices");
                });

            modelBuilder.Entity("nvt_back.User", b =>
                {
                    b.Navigation("OwnedProperties");
                });
#pragma warning restore 612, 618
        }
    }
}
