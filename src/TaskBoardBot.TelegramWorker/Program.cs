using Microsoft.EntityFrameworkCore;
using TaskBoardBot.TelegramWorker.Context;
using TaskBoardBot.TelegramWorker.PipelineComponents;
using TaskBoardBot.TelegramWorker.PipelineComponents.IntermittentPipeline;
using TaskBoardBot.TelegramWorker.PipelineComponents.PipelineSteps;
using TaskBoardBot.TelegramWorker.PipelineComponents.PipelineSteps.ChangeDate;
using TaskBoardBot.TelegramWorker.PipelineComponents.PipelineSteps.ChangeLocalTime;
using TaskBoardBot.TelegramWorker.PipelineComponents.PipelineSteps.ChangeMessageStep;
using TaskBoardBot.TelegramWorker.PipelineComponents.PipelineSteps.NoneStep;
using TaskBoardBot.TelegramWorker.PipelineComponents.PipelineSteps.NullStep;
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
    new InterPipeline(sp).AddLine(
        
        new PipelineLineBuilder(TelegramState.Null)
            .Add(new StartCommandStep())
        ).AddLine(
        
        new PipelineLineBuilder(TelegramState.None)
            .Add(new StartLocalTimeStep())
            .Add(new ListTasksStep())
            .Add(new AddTaskStep())
        ).AddLine(
        
        new PipelineLineBuilder(TelegramState.ChangeMessage)
            .Add(new ChangeTextStep())
        ).AddLine(
        
        new PipelineLineBuilder(TelegramState.ChangeDate) 
            .Add(new ChangeDateStep())
        ).AddLine(
        
        new PipelineLineBuilder(TelegramState.ChangeLocalTime)
            .Add(new LocalTimeStep())
        )
    );

builder.Services.AddHostedService<TelegramBotWorker>();
builder.Services.AddHostedService<CheckerUpcomingTasksWorker>();

var optionsBuilder = new DbContextOptionsBuilder<DbContext>();

optionsBuilder.UseNpgsql(connection);

builder.Services.AddScoped<ApplicationContext>(db => new ApplicationContext(optionsBuilder.Options));

var host = builder.Build();
host.Run();