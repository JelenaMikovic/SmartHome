using Microsoft.EntityFrameworkCore;
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


        public DatabaseContext(DbContextOptions options) : base(options)
        {
            Console.WriteLine("tu");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            /*modelBuilder.Entity<AmbientSensor>().HasData(
                new AmbientSensor { Id = 3, IsOnline = true, Name = "test", PowerConsumption = 40, PowerSource = PowerSource.AlkalineBattery }
            ); ;*/

        }

    }
}
