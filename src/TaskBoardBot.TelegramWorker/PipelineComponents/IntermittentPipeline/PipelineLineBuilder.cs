namespace TaskBoardBot.TelegramWorker.PipelineComponents.IntermittentPipeline;

public class PipelineLineBuilder {
    public PipelineLineBuilder() { }

    public PipelineLineBuilder(TelegramState state) {
        State = state;
    }
    public readonly ICollection<PipelineUnit> PipelineUnits = new List<PipelineUnit>();
    public TelegramState? State { get; }

    public PipelineLineBuilder Add(PipelineUnit unit) {
        PipelineUnits.Add(unit);
        return this;
    }
}