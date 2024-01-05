using Microsoft.EntityFrameworkCore;
using TaskBoardBot.TelegramWorker;

var builder = Host.CreateApplicationBuilder(args);

string? connection = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Configuration.AddJsonFile("token.json");
builder.Services.AddSingleton<TelegramBotClient>();
builder.Services.AddHostedService<Worker>();
var optionsBuilder = new DbContextOptionsBuilder<DbContext>();

optionsBuilder.UseNpgsql(connection);

builder.Services.AddScoped<ApplicationContext>(db => new ApplicationContext(optionsBuilder.Options));


var host = builder.Build();
host.Run();