namespace TaskBoardBot.TelegramWorker.IntermittentPipeline;

public class InterPipeline: PipelineUnit {

    private ICollection<PipelineUnit> _pipelineUnits = new List<PipelineUnit>();
    private IServiceProvider _serviceProvider;

    public InterPipeline(IServiceProvider serviceProvider) {
        _serviceProvider = serviceProvider;
    }
    

    public InterPipeline Add(PipelineUnit unit) {
        _pipelineUnits.Add(unit);
        return this;
    }
    
    public override PipelineContext Execute(PipelineContext obj) {
        obj.ServiceProvider = _serviceProvider;
        foreach (var pipelineUnit in _pipelineUnits) {
            obj = pipelineUnit.Execute(obj);
            if (!obj.IsExecute) {
                break;
            }
        }

        return obj;
    }
}