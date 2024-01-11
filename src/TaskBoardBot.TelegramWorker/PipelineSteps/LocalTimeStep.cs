using TaskBoardBot.TelegramWorker.IntermittentPipeline;

namespace TaskBoardBot.TelegramWorker.PipelineSteps;

public class LocalTimeStep: NewPipelineUnit {
    public override PipelineContext UpdateMessage(PipelineContext pipelineContext) {
        var message = pipelineContext.GetMessage();
        return pipelineContext;
    }

    public override PipelineContext UpdateCallbackQuery(PipelineContext pipelineContext) {
        throw new NotImplementedException();
    }
}