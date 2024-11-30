using Microsoft.EntityFrameworkCore;

namespace SleepSure.WebAPI.Models
{
    public class SleepSureContext : DbContext
    {
        public SleepSureContext(DbContextOptions<SleepSureContext> options)
            : base(options) { }

        public DbSet<Camera> Cameras { get; set; }
        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Camera>().HasData(
                new Camera { Id = 1, Name = "Garden Camera", Location = "Garden", Description = "Test"});

            modelBuilder.Entity<User>().HasData(
                new User { Id = 1, Email = "admin@gmail.com", Password = "password" });
        }
    }
}
