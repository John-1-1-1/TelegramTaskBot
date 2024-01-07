using Telegram.Bot;
using Telegram.Bot.Types;

namespace TaskBoardBot.TelegramWorker.IntermittentPipeline;

public class PipelineContext {
    public bool IsExecute = true;
    public ITelegramBotClient TelegramBotClient;
    public Message Message;
    public IServiceProvider ServiceProvider;
    
    public PipelineContext(ITelegramBotClient telegramBotClient, Message message) {
        TelegramBotClient = telegramBotClient;
        Message = message;
    }
}