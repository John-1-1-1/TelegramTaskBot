using TaskBoardBot.TelegramWorker.TelegramBot;

namespace TaskBoardBot.TelegramWorker.Workers;

public class TelegramBotWorker(ILogger<TelegramBotWorker> logger, IServiceProvider serviceProvider)
    : BackgroundService {
    protected override Task ExecuteAsync(CancellationToken stoppingToken) {
       logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
        var telegramBotClient = serviceProvider.GetService<TelegramBotClient>() 
                                ?? throw new Exception("TelegramBotClient is null");
        telegramBotClient.Start();
        return Task.CompletedTask;
    }
}