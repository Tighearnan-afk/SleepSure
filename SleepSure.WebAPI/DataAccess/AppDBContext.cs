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
    }
}
