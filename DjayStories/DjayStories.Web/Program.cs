using DjayStories.Core.Chats;
using DjayStories.Core;
using DjayStories.Web;
using DjayStories.Web.Components;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Betalgo.Ranul.OpenAI.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddOpenAIService();

var connection1 = new SqliteConnection("DataSource=:memory:");
connection1.Open();

var connection2 = new SqliteConnection("DataSource=:memory:");
connection2.Open();


builder.Services.AddDbContext<ChatContext>(options =>
    options.UseSqlite(connection1));


builder.Services.AddDbContext<GameDbContext>(options =>
    options.UseSqlite(connection2));

builder.Services.AddScoped<ChatManager>();
builder.Services.AddScoped<IChatService, ChatService>();

builder.Services.AddScoped<GameManager>();

builder.Services.AddHostedService<GameSeederHostedService>();
builder.Services.AddHostedService<ChatSeederHostedService>();
builder.Services.AddHostedService<TimedHostedService>();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
