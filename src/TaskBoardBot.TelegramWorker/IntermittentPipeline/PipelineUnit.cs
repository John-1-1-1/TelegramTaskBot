using TaskBoardBot.TelegramWorker.Context;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace TaskBoardBot.TelegramWorker.IntermittentPipeline;

public abstract class PipelineUnit {
    public abstract PipelineContext Execute(PipelineContext pipelineContext);
}