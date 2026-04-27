using LostAndFound.WPF.Model;
using Microsoft.EntityFrameworkCore;

namespace Server.EF_Core
{
    public class Context : DbContext
    {
        public Context(DbContextOptions<Context> options) : base(options) { }

        public DbSet<Item> Items { get; set; } = null!;
        public DbSet<Claim> Claims { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Item → Claims (1:n), Cascade Delete
            modelBuilder.Entity<Claim>()
                .HasOne<Item>()
                .WithMany()
                .HasForeignKey(c => c.ItemId)
                .OnDelete(DeleteBehavior.Cascade);

            // String-Suche in SQLite: LIKE ist case-insensitiv per Default ✓
            base.OnModelCreating(modelBuilder);
        }
    }
}
