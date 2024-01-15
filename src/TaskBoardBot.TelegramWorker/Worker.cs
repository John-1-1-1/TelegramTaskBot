using TaskBoardBot.TelegramWorker.Context;

namespace TaskBoardBot.TelegramWorker;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly IServiceProvider _serviceProvider;
    public Worker(ILogger<Worker> logger, IServiceProvider serviceProvider) {
        _serviceProvider = serviceProvider;
        _logger = logger;

        var r = _serviceProvider.GetService<DataBaseService>();
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken) {
        WatchDogTasks watchDogTasks = _serviceProvider.GetService<WatchDogTasks>();
        _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
        var telegramBotClient = _serviceProvider.GetService<TelegramBotClient>();
        telegramBotClient.Satrt();
        watchDogTasks.Run(stoppingToken);
    }
}