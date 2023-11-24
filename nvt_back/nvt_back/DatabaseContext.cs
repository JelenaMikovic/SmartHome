﻿using Microsoft.EntityFrameworkCore;
using nvt_back.Model.Devices;

namespace nvt_back
{
    public class DatabaseContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Property> Properties { get; set; }
        public DbSet<Country> Countries { get; set; }
        public DbSet<City> Cities { get; set; }
        public DbSet<Address> Addresses { get; set; }
        public DbSet<Device> Devices { get; set; }
        public DbSet<AmbientSensor> AmbientSensors { get; set; }
        public DbSet<AirConditioner> AirConditioners { get; set; }
        public DbSet<EVCharger> EVChargers { get; set; }
        public DbSet<HomeBattery> HomeBatteries { get; set; }
        public DbSet<IrrigationSystem> IrrigationSystems { get; set; }
        public DbSet<Lamp> Lamps { get; set; }
        public DbSet<SolarPanel> SolarPanels { get; set; }
        public DbSet<VehicleGate> VehicleGates { get; set; }
        public DbSet<WashingMachine> WashingMachines { get; set; }

        public DatabaseContext(DbContextOptions options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Device>().ToTable("Devices");
            modelBuilder.Entity<AmbientSensor>().ToTable("AmbientSensors");
            modelBuilder.Entity<AirConditioner>().ToTable("AirConditioners");
            modelBuilder.Entity<EVCharger>().ToTable("EVChargers");
            modelBuilder.Entity<HomeBattery>().ToTable("HomeBatteries");
            modelBuilder.Entity<IrrigationSystem>().ToTable("IrrigationSystems");
            modelBuilder.Entity<SolarPanel>().ToTable("SolarPanels");
            modelBuilder.Entity<VehicleGate>().ToTable("VehicleGates");
            modelBuilder.Entity<WashingMachine>().ToTable("WashingMachines");
            modelBuilder.Entity<Lamp>().ToTable("Lamps");

            /*modelBuilder.Entity<Device>()
              .HasKey(x => new { x.Id, x.PropertyId });*/

            /*modelBuilder.Entity<AmbientSensor>().HasData(
                new AmbientSensor { Id = 3, IsOnline = true, Name = "test", PowerConsumption = 40, PowerSource = PowerSource.AlkalineBattery }
            ); ;*/

        }

    }
}
