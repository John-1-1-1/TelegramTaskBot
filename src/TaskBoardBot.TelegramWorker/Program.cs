using Microsoft.EntityFrameworkCore;
using TaskBoardBot.TelegramWorker;
using TaskBoardBot.TelegramWorker.Context;
using TaskBoardBot.TelegramWorker.IntermittentPipeline;
using TaskBoardBot.TelegramWorker.PipelineSteps;

var builder = Host.CreateApplicationBuilder(args);

string? connection = builder.Configuration.GetConnectionString("DefaultConnection");

AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
AppContext.SetSwitch("Npgsql.DisableDateTimeInfinityConversions", true);

builder.Configuration.AddJsonFile("token.json");
builder.Services.AddSingleton<TelegramBotClient>();
builder.Services.AddHostedService<Worker>();
builder.Services.AddSingleton<DataBaseService>();
builder.Services.AddSingleton<InterPipeline>(sp => 
    new InterPipeline(sp)
        .Add(new StartStep())
        .Add(new ListTasksStep())
        .Add(new LocalTimeStep())
    
        .Add(new AddTaskStep())
    );

var optionsBuilder = new DbContextOptionsBuilder<DbContext>();

optionsBuilder.UseNpgsql(connection);

builder.Services.AddScoped<ApplicationContext>(db => new ApplicationContext(optionsBuilder.Options));

var host = builder.Build();
host.Run();