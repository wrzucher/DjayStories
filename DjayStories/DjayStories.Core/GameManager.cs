using Microsoft.EntityFrameworkCore;
namespace DjayStories.Core;

public class GameManager
{
    private readonly DbContextOptions<GameDbContext> _options;

    public GameManager(DbContextOptions<GameDbContext> options)
    {
        _options = options;
    }

    // Get player by GameId and UserId with Role and Relationships
    public async Task<Player?> GetPlayerAsync(int gameId, Guid userId)
    {
        var context = new GameDbContext(_options);
        return await context.Players
            .Include(p => p.Role)
            .FirstOrDefaultAsync(p => p.GameId == gameId && p.UserId == userId);
    }

    // Get a game by Id with all players
    public async Task<Game?> GetGameAsync(int gameId)
    {
        var context = new GameDbContext(_options);
        return await context.Games
            .Include(g => g.Players)
            .FirstOrDefaultAsync(g => g.Id == gameId);
    }

    // Get all players in a game with roles and relationships
    public async Task<List<Player>> GetPlayersByGameAsync(int gameId)
    {
        var context = new GameDbContext(_options);
        return await context.Players
            .Where(p => p.GameId == gameId)
            .Include(p => p.Role)
            .ToListAsync();
    }
}