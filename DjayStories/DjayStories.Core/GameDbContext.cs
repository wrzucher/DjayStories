using Microsoft.EntityFrameworkCore;
namespace DjayStories.Core;

public class GameDbContext : DbContext
{
    public DbSet<Game> Games { get; set; }
    public DbSet<Player> Players { get; set; }
    public DbSet<Role> Roles { get; set; }
    public GameDbContext(DbContextOptions<GameDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {

        modelBuilder.Entity<Game>()
            .HasKey(p => p.Id);

        // Player composite key
        modelBuilder.Entity<Player>()
            .HasKey(p => new { p.GameId, p.Id });

        modelBuilder.Entity<Player>()
            .HasOne(p => p.Game)
            .WithMany(g => g.Players)
            .HasForeignKey(p => p.GameId);

        modelBuilder.Entity<Player>()
            .HasOne(p => p.Role)
            .WithMany(r => r.Players)
            .HasForeignKey(p => p.RoleId);
    }
}
