using TaskBoardBot.TelegramWorker.Services;

namespace TaskBoardBot.TelegramWorker.Workers;

public class CheckerUpcomingTasksWorker(ILogger<CheckerUpcomingTasksWorker> logger,
    IServiceProvider serviceProvide) : BackgroundService {
    private ILogger<CheckerUpcomingTasksWorker> Logger { get; } = logger;

    protected override Task ExecuteAsync(CancellationToken stoppingToken) {
        TelegramBotService? telegramBotClient = serviceProvide.GetService<TelegramBotService>();
        DataBaseService? dataBaseService = serviceProvide.GetService<DataBaseService>();

        if (telegramBotClient == null || dataBaseService == null) {
            Logger.LogError("CheckerUpcomingTasksWorker dataBaseService or telegramBotClient is null");
            return Task.CompletedTask;
        }
        while (!stoppingToken.IsCancellationRequested) {
            var listTasks = dataBaseService.GetUpcomingTasks(DateTime.Now, 15);
            foreach (var task in listTasks) {
                telegramBotClient.SendUpcomingTask(task);
                task.IsActive = false;
                dataBaseService.UpdateTask(task);
            }

            Thread.Sleep(10000);
        }

        return Task.CompletedTask;
    }
}