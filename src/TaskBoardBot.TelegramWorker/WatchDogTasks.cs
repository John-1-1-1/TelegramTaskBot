using TaskBoardBot.TelegramWorker.Context;

namespace TaskBoardBot.TelegramWorker;

public class WatchDogTasks(ILogger<WatchDogTasks> logger, IServiceProvider serviceProvide) {
    private readonly IServiceProvider _serviceProvide = serviceProvide;
    private ILogger<WatchDogTasks> Logger { get; } = logger;

    public async void Run(CancellationToken stoppingToken) {

        TelegramBotClient telegramBotClient = serviceProvide.GetService<TelegramBotClient>();
        DataBaseService dataBaseService = serviceProvide.GetService<DataBaseService>();
        
        while (!stoppingToken.IsCancellationRequested) {
            var listTasks = dataBaseService.GetUpcomingTasks(DateTime.Now, 15);
            foreach (var task in listTasks) {
                telegramBotClient.SendUpcomingTask(task);
                task.IsActive = false;
                dataBaseService.UpdateTask(task);
            }
            Thread.Sleep(10000);
        }
    }
}