using LostAndFoundApi.Models;
using Microsoft.EntityFrameworkCore;

namespace LostAndFoundApi.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options) { }

        public DbSet<Item> Items { get; set; }
        public DbSet<Person> Persons { get; set; }
    }
}
