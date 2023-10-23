using Microsoft.EntityFrameworkCore;
using System.Reflection.Metadata;

namespace CityWebApiNotDecoupled.Models
{
    public class DatabaseContext : DbContext
    {
        public DatabaseContext(DbContextOptions<DatabaseContext> options)
        : base(options)
        {
        }

        public DbSet<City> Cities { get; set; } = null!;
        public DbSet<Country> Countries { get; set; } = null!;
    }
}
