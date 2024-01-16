using TaskBoardBot.TelegramWorker.Context;
using Telegram.Bot;
using TelegramBotClient = TaskBoardBot.TelegramWorker.TelegramBot.TelegramBotClient;

namespace TaskBoardBot.TelegramWorker.Services;

public class TelegramBotService(IServiceProvider serviceProvider) {
    private readonly TelegramBotClient _telegramBotClient = serviceProvider.GetService<TelegramBotClient>() ?? 
                                                            throw new Exception("telegramBotClient is empty");
    
    
    public void SendUpcomingTask(Tasks task) {
        _telegramBotClient.TelegramClient.SendTextMessageAsync(task.TgId, task.Text);
    }
}