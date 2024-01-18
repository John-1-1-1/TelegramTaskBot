using TaskBoardBot.TelegramWorker.Context;
using TaskBoardBot.TelegramWorker.Context.DbTables;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace TaskBoardBot.TelegramWorker.PipelineComponents.IntermittentPipeline;

public abstract class PipelineUnit: PipelineMethods {

    public override PipelineContext UpdateCallbackQuery(PipelineContext pipelineContext, 
        CallbackQuery callbackQuery, Users? user) {
        return pipelineContext;
    }

    public override PipelineContext UpdateMessage(PipelineContext pipelineContext, 
        Message message, Users? user) {
        return pipelineContext;
    }
}

public abstract class PipelineMethods {
    public abstract PipelineContext UpdateMessage(PipelineContext pipelineContext, 
        Message message, Users? user); 
    public abstract PipelineContext UpdateCallbackQuery(PipelineContext pipelineContext, 
        CallbackQuery callbackQuery, Users? user);
}