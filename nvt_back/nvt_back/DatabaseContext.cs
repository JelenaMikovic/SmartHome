using Microsoft.EntityFrameworkCore;
using nvt_back.Model;

namespace nvt_back
{
    public class DatabaseContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Property> Properties { get; set; }
        public DbSet<Country> Countries { get; set; }
        public DbSet<City> Cities { get; set; }
        public DbSet<Address> Addresses { get; set; }
        public DbSet<ActivationCode> ActivationCodes { get; set; }


        public DatabaseContext(DbContextOptions options) : base(options)
        {
            Console.WriteLine("tu");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>().HasData(
                new User { Id = 1, Name = "Bob", Surname = "Ross", Email = "bob@mail.com", Password = "123", IsActivated = true, Role = UserRole.USER }
            );

        }

    }
}
