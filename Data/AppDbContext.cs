using Headlight.Models;
using Microsoft.EntityFrameworkCore;

namespace Headlight.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Game> Games { get; set; }
        public DbSet<Status> Statuses { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Status>(status => {
                status
                    .HasMany(e => e.Games)
                    .WithOne(e => e.Status)
                    .HasForeignKey(e => e.StatusId)
                    .IsRequired();
                status.HasData(
                    new { Id = 1, Name = "Backlog"},
                    new { Id = 2, Name = "Playing"},
                    new { Id = 3, Name = "Finished"}
                );
            });

            modelBuilder.Entity<Platform>(platform =>
            {
                platform
                    .HasMany(e => e.Games)
                    .WithOne(e => e.Platform)
                    .HasForeignKey(e => e.PlatformId)
                    .IsRequired();
                platform.HasData(
                    new { Id = 1, Name = "PC" },
                    new { Id = 2, Name = "Steam Deck" },
                    new { Id = 3, Name = "Nintendo Switch" }
                );
            });
        }

        public List<Status> GetStatuses()
        {
            return Statuses.FromSqlInterpolated($"SELECT * FROM status;").ToList();

        }

    }
}
