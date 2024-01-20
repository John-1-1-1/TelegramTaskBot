using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace TaskBoardBot.TelegramWorker.PipelineComponents.IntermittentPipeline;

public class PipelineContext(TelegramBotClient telegramBotClient, 
    Update? update) {
    public bool IsExecute = true;
    public readonly ITelegramBotClient TelegramBotClient = telegramBotClient;
    public InterPipeline Parent = null!;

    public Message? GetMessage() {
        return update?.Message;
    }
    
    public CallbackQuery? GetCallbackQuery() {
        return update?.CallbackQuery;
    }

    public void KillPipeline() {
        IsExecute = false;
    }
}