using TaskBoardBot.TelegramWorker.IntermittentPipeline;

namespace TaskBoardBot.TelegramWorker.PipelineSteps.MessagesSteps;

public class TelegramMessageValidator: PipelineUnit {
    public override PipelineContext Execute(PipelineContext pipelineContext) {
        var chat = pipelineContext.Message?.Chat;
        var text = pipelineContext.Message?.Text;
        if (chat == null || text == null) {
            pipelineContext.IsExecute = false;
        }

        return pipelineContext;
    }
}