namespace TaskBoardBot.TelegramWorker.PipelineComponents.IntermittentPipeline;

public class PipelineLineBuilder(TelegramState state) {
    public readonly ICollection<PipelineUnit> PipelineUnits = new List<PipelineUnit>();
    public TelegramState? State { get; } = state;
    public PipelineLineBuilder Add(PipelineUnit unit) {
        PipelineUnits.Add(unit);
        return this;
    }
}