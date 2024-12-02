using Microsoft.EntityFrameworkCore;
using SleepSure.WebAPI.Models;

namespace SleepSure.WebAPI.DataAccess
{
    public class AppDBContext : DbContext
    {
        public AppDBContext(DbContextOptions<AppDBContext> options)
            : base(options) { }

        public DbSet<DeviceLocation> Locations { get; set; }
        public DbSet<Camera> Cameras { get; set; }
        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<DeviceLocation>().HasData(
                new DeviceLocation { Id = 1, LocationName = "Kitchen" },
                new DeviceLocation { Id = 2, LocationName = "Garden" });

            modelBuilder.Entity<Camera>().HasData(
                new Camera { Id = 1, Name = "Garden Camera", Location = "Garden", Description = "Test", DeviceLocationId = 2});

            modelBuilder.Entity<User>().HasData(
                new User { Id = 1, Email = "admin@gmail.com", Password = "password" });
        }
    }
}
