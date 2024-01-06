using Telegram.Bot;
using Telegram.Bot.Types;

namespace TaskBoardBot.TelegramWorker.IntermittentPipeline;

public abstract class PipelineUnit {
    public abstract PipelineContext Execute(PipelineContext pipelineContext);
    
    public PipelineContext Execute(ITelegramBotClient _telegramBotClient, Message message) {
        return Execute(new PipelineContext(_telegramBotClient, message));
    }
}