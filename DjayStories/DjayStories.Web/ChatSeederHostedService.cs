using Microsoft.EntityFrameworkCore;
using DjayStories.Core.Chats;

namespace DjayStories.Web;

public class ChatSeederHostedService : IHostedService
{
    private readonly IServiceProvider _serviceProvider;

    public ChatSeederHostedService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        using var scope = _serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ChatContext>();

        await context.Database.EnsureCreatedAsync(cancellationToken);

        if (await context.Users.AnyAsync(cancellationToken))
            return; // Already seeded

        // Seed Users
        var alice = new User { UserId = Guid.Parse("f83a9f52-3b5f-4e9b-8a2c-f5a5b1e9c471") };
        var bob = new User { UserId = Guid.Parse("36dcbf0f-7a97-4bb7-a14c-21e709ff62a9")};
        var charlie = new User { UserId = Guid.Parse("e203a8b7-3db7-42de-912c-7409ae88e4b0") };

        context.Users.AddRange(alice, bob, charlie);
        await context.SaveChangesAsync(cancellationToken);

        // Create a conversation room
        var conversation = new Conversation
        {
            Title = "Test Chat Room",
            IsGroup = true,
            CreatedAt = DateTime.UtcNow
        };

        context.Conversations.Add(conversation);
        await context.SaveChangesAsync(cancellationToken);

        // Add participants
        var participants = new[]
        {
            new ConversationParticipant { ConversationId = conversation.ConversationId, UserId = alice.UserId, JoinedAt = DateTime.UtcNow },
            new ConversationParticipant { ConversationId = conversation.ConversationId, UserId = bob.UserId, JoinedAt = DateTime.UtcNow },
            new ConversationParticipant { ConversationId = conversation.ConversationId, UserId = charlie.UserId, JoinedAt = DateTime.UtcNow },
        };

        context.ConversationParticipants.AddRange(participants);
        await context.SaveChangesAsync(cancellationToken);
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
