using TaskBoardBot.TelegramWorker.Context;

namespace TaskBoardBot.TelegramWorker.IntermittentPipeline;

public class InterPipeline : PipelineUnit {

    private readonly DataBaseService _dataBaseService;
    
    public InterPipeline(IServiceProvider iServiceProvider) {
        var db = iServiceProvider.GetService<DataBaseService>();
        _dataBaseService = db ?? throw new Exception("DataBaseService is null");
    }
    
    private readonly ICollection<NewPipelineUnit> _pipelineUnits = new List<NewPipelineUnit>();

    public InterPipeline Add(NewPipelineUnit unit) {
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