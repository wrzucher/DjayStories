using DjayStories.Core;

namespace DjayStories.Web;

public class GameSeederHostedService : IHostedService
{
    private readonly IServiceProvider _serviceProvider;

    public GameSeederHostedService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        using var scope = _serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<GameDbContext>();

        await context.Database.EnsureCreatedAsync(cancellationToken);
        await LoadLoveGame(context, cancellationToken);
        await LoadGoatGame(context, cancellationToken);

        await context.SaveChangesAsync(cancellationToken);
    }

    private static async Task LoadLoveGame(GameDbContext context, CancellationToken cancellationToken)
    {
        var role1 = context.Roles.Add(new Role()
        {
            Name = "Соблазнитель",
            HowHeFeels = "Ты играешь роль сексуально озабоченного извращенца, который всех домагается. Ты сальный тип, говоришь как гопник.",
            Target = "Согласие на свидание"
        });

        var role2 = context.Roles.Add(new Role()
        {
            Name = "Обычная девушка",
            HowHeFeels = "Ты просто обычная девушка, слегка глуповатая. Говори коротко и глупо. Ты не особо разговорчивая",
            Target = "Нет цели, просто общение",
        });

        var role3 = context.Roles.Add(new Role()
        {
            Name = "Случайный посетитель",
            HowHeFeels = ".",
            Target = "Не дать соблазнить девушку",
        });

        await context.SaveChangesAsync(cancellationToken);

        var player1 = context.Players.Add(new Player()
        {
            Id = 1,
            IsReal = false,
            RoleId = role1.Entity.Id,
            Name = "Васян",
            UserId = Guid.Parse("36dcbf0f-7a97-4bb7-a14c-21e709ff62a9"),
            GameId = 1,
        });

        var player2 = context.Players.Add(new Player()
        {
            Id = 2,
            IsReal = false,
            Name = "Машка",
            RoleId = role2.Entity.Id,
            UserId = Guid.Parse("f83a9f52-3b5f-4e9b-8a2c-f5a5b1e9c471"),
            GameId = 1,
        });

        var player3 = context.Players.Add(new Player()
        {
            Id = 3,
            IsReal = true,
            Name = "Чипи",
            RoleId = role3.Entity.Id,
            UserId = Guid.Parse("e203a8b7-3db7-42de-912c-7409ae88e4b0"),
            GameId = 1,
        });

        context.Games.Add(new Game()
        {
            Id = 1,
            Name = "Чат",
            Context = "Чат где-то в интернете где все друг с другом общаются",
        });
    }

    private static async Task LoadGoatGame(GameDbContext context, CancellationToken cancellationToken)
    {
        var role1 = context.Roles.Add(new Role()
        {
            Name = "Наивный козлёнок",
            HowHeFeels = "Ты играешь наивного козлёнка, склонного всем верить.",
            Target = "Открыть дверь только матери"
        });

        var role2 = context.Roles.Add(new Role()
        {
            Name = "Умный козлёнок",
            HowHeFeels = "Ты достаточно умный козлёнок для своих лет",
            Target = "Открыть дверь только матери",
        });

        var role3 = context.Roles.Add(new Role()
        {
            Name = "Волк",
            HowHeFeels = ".",
            Target = "Съесть козлят",
        });

        await context.SaveChangesAsync(cancellationToken);

        var player1 = context.Players.Add(new Player()
        {
            Id = 4,
            IsReal = false,
            RoleId = role1.Entity.Id,
            Name = "Лупа",
            UserId = Guid.Parse("f13a9f52-3b5f-4e9b-8a2c-f5a5b1e9c471"),
            GameId = 2,
        });

        var player2 = context.Players.Add(new Player()
        {
            Id = 5,
            IsReal = false,
            Name = "Пупа",
            RoleId = role2.Entity.Id,
            UserId = Guid.Parse("31dcbf0f-7a97-4bb7-a14c-21e709ff62a9"),
            GameId = 2,
        });

        var player3 = context.Players.Add(new Player()
        {
            Id = 6,
            IsReal = true,
            Name = "Чипи",
            RoleId = role3.Entity.Id,
            UserId = Guid.Parse("e203a8b7-3db7-42de-912c-7409ae88e4b0"),
            GameId = 2,
        });

        context.Games.Add(new Game()
        {
            Id = 2,
            Name = "Сказка",
            Context = "Мама козлят ушла из дома и сказала, чтоб козлята открыли дверь только ей",
        });
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
