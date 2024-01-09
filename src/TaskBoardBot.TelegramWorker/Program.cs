using Microsoft.EntityFrameworkCore;
using TaskBoardBot.TelegramWorker;
using TaskBoardBot.TelegramWorker.Context;
using TaskBoardBot.TelegramWorker.IntermittentPipeline;
using TaskBoardBot.TelegramWorker.PipelineSteps.CallbackSteps;
using TaskBoardBot.TelegramWorker.PipelineSteps.MessagesSteps;

var builder = Host.CreateApplicationBuilder(args);

string? connection = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Configuration.AddJsonFile("token.json");
builder.Services.AddSingleton<TelegramBotClient>();
builder.Services.AddHostedService<Worker>();
builder.Services.AddSingleton<DataBaseService>();
builder.Services.AddSingleton<InterPipeline>(sp => 
    new InterPipeline(sp)
        .Add(new DateCallback())
        .Add(new TelegramMessageValidator())
        .Add(new TelegramCommands())
        .Add(new TelegramTextMessages()));

var optionsBuilder = new DbContextOptionsBuilder<DbContext>();

optionsBuilder.UseNpgsql(connection);

builder.Services.AddScoped<ApplicationContext>(db => new ApplicationContext(optionsBuilder.Options));

var host = builder.Build();
host.Run();