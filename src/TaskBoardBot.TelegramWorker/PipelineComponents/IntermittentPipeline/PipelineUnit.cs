using Telegram.Bot.Types.Enums;

namespace TaskBoardBot.TelegramWorker.PipelineComponents.IntermittentPipeline;

public abstract class PipelineUnit: PipelineMethods {
    public override PipelineContext Execute(PipelineContext pipelineContext) {
        switch (pipelineContext.Type) {
            case UpdateType.Message: {
                return UpdateMessage(pipelineContext);
            }
            case UpdateType.CallbackQuery: {
                return UpdateCallbackQuery(pipelineContext);
            }
        }
        
        return pipelineContext;
    }

    public override PipelineContext UpdateCallbackQuery(PipelineContext pipelineContext) {
        return pipelineContext;
    }

    public override PipelineContext UpdateMessage(PipelineContext pipelineContext) {
        return pipelineContext;
    }
}

public abstract class PipelineMethods {
    public abstract PipelineContext Execute(PipelineContext pipelineContext);
    public abstract PipelineContext UpdateMessage(PipelineContext pipelineContext); 
    public abstract PipelineContext UpdateCallbackQuery(PipelineContext pipelineContext);
}