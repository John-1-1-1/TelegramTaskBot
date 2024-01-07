namespace TaskBoardBot.TelegramWorker.IntermittentPipeline;

public class InterPipeline(IServiceProvider serviceProvider) : PipelineUnit {

    private readonly ICollection<PipelineUnit> _pipelineUnits = new List<PipelineUnit>();

    public InterPipeline Add(PipelineUnit unit) {
        _pipelineUnits.Add(unit);
        return this;
    }
    
    public override PipelineContext Execute(PipelineContext obj) {
        obj.ServiceProvider = serviceProvider;
        foreach (var pipelineUnit in _pipelineUnits) {
            obj = pipelineUnit.Execute(obj);
            if (!obj.IsExecute) {
                break;
            }
        }
        return obj;
    }
}