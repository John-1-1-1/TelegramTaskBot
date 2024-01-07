using Telegram.Bot;
using Telegram.Bot.Types;

namespace TaskBoardBot.TelegramWorker.IntermittentPipeline;

public class PipelineContext(ITelegramBotClient telegramBotClient, Message message) {
    public bool IsExecute = true;
    public readonly ITelegramBotClient TelegramBotClient = telegramBotClient;
    public readonly Message Message = message;
    public IServiceProvider ServiceProvider;
}