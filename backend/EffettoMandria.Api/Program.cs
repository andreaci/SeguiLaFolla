using EffettoMandria.Api;
using EffettoMandria.Api.Hubs;
using EffettoMandria.Api.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<InMemoryStore>();
builder.Services.AddSingleton<QuestionService>();
builder.Services.AddSingleton<AuthService>();
builder.Services.AddSingleton<ScoringService>();
builder.Services.AddSingleton<GameService>();
builder.Services.AddSignalR();
builder.Services.AddControllers();

var corsOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>()
    ?? ["http://localhost:5173", "http://127.0.0.1:5173"];

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins(corsOrigins)
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

var app = builder.Build();

app.UseCors();
app.UseDefaultFiles();
app.UseStaticFiles();
app.MapControllers();
app.MapHub<GameHub>("/hubs/game");
app.MapFallbackToFile("index.html");


app.Run();
