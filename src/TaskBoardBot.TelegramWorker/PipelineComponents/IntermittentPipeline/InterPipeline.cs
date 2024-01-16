using TaskBoardBot.TelegramWorker.Services;

namespace TaskBoardBot.TelegramWorker.PipelineComponents.IntermittentPipeline;

public class InterPipeline(IServiceProvider iServiceProvider) : PipelineUnit {
    
    private readonly ICollection<PipelineUnit> _pipelineUnits = new List<PipelineUnit>();
    public DataBaseService GetDbService { get; } = iServiceProvider.GetService<DataBaseService>()
                                                   ?? throw new Exception("DataBaseService is null");

    public InterPipeline Add(PipelineUnit unit) {
        _pipelineUnits.Add(unit);
        return this;
    }
    
    public override PipelineContext Execute(PipelineContext obj) {
        obj.Parent = this;
        foreach (var pipelineUnit in _pipelineUnits) {
            obj = pipelineUnit.Execute(obj);
            if (!obj.IsExecute) {
                break;
            }
        }
        return obj;
    }
}