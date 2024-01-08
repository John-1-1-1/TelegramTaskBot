using TaskBoardBot.TelegramWorker.Context;

namespace TaskBoardBot.TelegramWorker.IntermittentPipeline;

public class InterPipeline : PipelineUnit {

    private readonly DataBaseService _dataBaseService;
    
    public InterPipeline(IServiceProvider iServiceProvider) {
        
        _dataBaseService = iServiceProvider.GetService<DataBaseService>();
    }
    
    private readonly ICollection<PipelineUnit> _pipelineUnits = new List<PipelineUnit>();

    public InterPipeline Add(PipelineUnit unit) {
        _pipelineUnits.Add(unit);
        return this;
    }
    
    public override PipelineContext Execute(PipelineContext obj) {
        obj.DataBaseService = _dataBaseService;
        foreach (var pipelineUnit in _pipelineUnits) {
            obj = pipelineUnit.Execute(obj);
            if (!obj.IsExecute) {
                break;
            }
        }
        return obj;
    }
}