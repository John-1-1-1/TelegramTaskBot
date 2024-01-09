namespace TaskBoardBot.TelegramWorker.IntermittentPipeline;

public abstract class PipelineUnit {
    public abstract PipelineContext Execute(PipelineContext pipelineContext);
}