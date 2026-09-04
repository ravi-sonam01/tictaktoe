using TicTacToe.Api.Services;

var builder = WebApplication.CreateBuilder(args);

// 1. Add CORS policy
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngularFrontend",
        policy =>
        {
            policy.WithOrigins("http://localhost:4200") // Default Angular port
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        });
});

builder.Services.AddSingleton<IGameService, GameService>();
builder.Services.AddControllers();

var app = builder.Build();

app.UseCors("AllowAngularFrontend");

app.UseAuthorization();
app.MapControllers();
app.Run();