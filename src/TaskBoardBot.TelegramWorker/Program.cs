using Microsoft.EntityFrameworkCore;
using TaskBoardBot.TelegramWorker.Context;
using TaskBoardBot.TelegramWorker.PipelineComponents.IntermittentPipeline;
using TaskBoardBot.TelegramWorker.PipelineComponents.PipelineSteps;
using TaskBoardBot.TelegramWorker.Services;
using TaskBoardBot.TelegramWorker.TelegramBot;
using TaskBoardBot.TelegramWorker.Workers;

var builder = Host.CreateApplicationBuilder(args);

string? connection = builder.Configuration.GetConnectionString("DefaultConnection");

AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
AppContext.SetSwitch("Npgsql.DisableDateTimeInfinityConversions", true);

builder.Configuration.AddJsonFile("token.json");
builder.Services.AddSingleton<TelegramBotClient>();
builder.Services.AddSingleton<DataBaseService>();
builder.Services.AddSingleton<TelegramBotService>();

builder.Services.AddSingleton<InterPipeline>(sp => 
    new InterPipeline(sp)   
        .Add(new StartCommandStep())
        .Add(new ListTasksStep())
        .Add(new LocalTimeStep())
        .Add(new AddTaskStep())
    );

builder.Services.AddHostedService<TelegramBotWorker>();
builder.Services.AddHostedService<CheckerUpcomingTasksWorker>();

var optionsBuilder = new DbContextOptionsBuilder<DbContext>();

optionsBuilder.UseNpgsql(connection);

builder.Services.AddScoped<ApplicationContext>(db => new ApplicationContext(optionsBuilder.Options));

var host = builder.Build();
host.Run();