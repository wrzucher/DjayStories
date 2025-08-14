using Betalgo.Ranul.OpenAI.Interfaces;
using Betalgo.Ranul.OpenAI.ObjectModels.RequestModels;
using Betalgo.Ranul.OpenAI.ObjectModels;
using System.Text.Json;
using DjayStories.Core;
using DjayStories.Core.Chats;

namespace DjayStories.Web;

public class TimedHostedService : IHostedService, IDisposable
{
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private Timer _timer;

    private readonly JsonSerializerOptions _options = new JsonSerializerOptions
    {
        AllowTrailingCommas = true,
        PropertyNameCaseInsensitive = false,
    };

    public TimedHostedService(IServiceScopeFactory serviceScopeFactory)
    {
        _serviceScopeFactory = serviceScopeFactory;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        // Runs every 10 seconds
        _timer = new Timer(DoWork, null, TimeSpan.Zero, TimeSpan.FromSeconds(10));
        return Task.CompletedTask;
    }

    private async void DoWork(object state)
    {
        using var scope = _serviceScopeFactory.CreateScope();
        var openAIService = scope.ServiceProvider.GetRequiredService<IOpenAIService>();
        var gameManager = scope.ServiceProvider.GetRequiredService<GameManager>();
        var chatManager = scope.ServiceProvider.GetRequiredService<ChatManager>();


        var allConversations = await chatManager.GetConversationsAsync();


        foreach ( var conversation in allConversations )
        {
            var messages = await chatManager.GetMessagesAsync(conversation.ConversationId);

            var players = await gameManager.GetPlayersByGameAsync(conversation.ConversationId);
            var game = await gameManager.GetGameAsync(conversation.ConversationId);

            if (!messages.Any())
            {
                continue;
            }

            var lastPlayerId = messages.First().SenderId;
            var lastPlayer = players.First(_ => _.UserId == lastPlayerId);
            if (!lastPlayer.IsReal)
            {
                continue;
            }

            foreach (var currentPlayer in players
                .Where(_ => !_.IsReal))
            {
                var availablePlayersName = players
                    .Where(_ => _.Id != currentPlayer.Id)
                    .Select(_ => _.Name.ToUpperInvariant())
                    .ToArray();

                var resquest =  @$"
    Ты играешь роль в {game.Name} и твоё имя в пьесе {currentPlayer.Name}.
    Твоя цель: {currentPlayer.Role.Target}.
    Твое описание которому ты ДОЛЖЕН соответсвовать: {currentPlayer.Role.HowHeFeels}
    Контекст пьесы в которой ты играешь: {game.Context}

    ТРЕБУЕМЫЙ Формат ответа строго JSON следующего формата:
    {{
        Target : ""<UserName>"", -- Здесь  <UserName> Здесь должно быть User Name того, к кому ты обращаешься.
        Speach : ""<Speach>"", -- Здесь <Speach> должна быть твоя речь, что ты говоришь
        Action : ""<Action>"", -- Здесь <Action> должно быть описание твоих действий, движений, мыслей. В третьем лице единственного числа.
    }}

    Если НЕ хочешь отвечать никому то значение должно быть : SKIP
    <UserName>: В верхнем регистре, без пробелов, допустимые значения: EVERYONE, SKIP, {string.Join(",", availablePlayersName)}. 
    Если хочешь ответить всем то значение должно быть : EVERYONE
    Если НЕ хочешь отвечать никому то значение должно быть : SKIP

    Всегда проверяй, что ответ именно JSON!
    ";

                var chatMessages = new List<ChatMessage>
                {
                    ChatMessage.FromSystem(resquest)
                };

                foreach (var message in messages.OrderBy(_ => _.SentAt))
                {
                    if (message.SenderId == currentPlayer.UserId)
                    {
                        chatMessages.Add(ChatMessage.FromAssistant(message.Content));
                    }
                    else
                    {
                        var player = players.First(_ => _.UserId == message.SenderId);
                        chatMessages.Add(ChatMessage.FromUser($"{player.Name}: {message.Content}"));
                    }
                }

                var completionResult = await openAIService.ChatCompletion.CreateCompletion(new ChatCompletionCreateRequest
                {
                    Messages = chatMessages,
                    Model = Models.Gpt_4o_mini,
                });

                if (completionResult.Successful)
                {
                    var sd = completionResult.Choices.First();
                    var content = sd.Message.Content?.Replace("```json", string.Empty).Replace("```", string.Empty);

                    var result = JsonSerializer.Deserialize<Responce>(content, _options);
                    var target = players.FirstOrDefault(_ => string.Equals(_.Name, result.Target, StringComparison.InvariantCultureIgnoreCase));
                    await chatManager.SendMessageAsync(conversation.ConversationId, currentPlayer.UserId, target?.UserId, result.Speach, result.Action);
                }
            }
        }
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _timer?.Change(Timeout.Infinite, 0);
        return Task.CompletedTask;
    }

    public void Dispose()
    {
        _timer?.Dispose();
    }
}
