using System.Reflection;
using Telegram.Bot;

namespace TaskBoardBot.TelegramWorker.IntermittentPipeline;

public class InterPipeline: PipelineUnit {

    private ICollection<PipelineUnit> _pipelineUnits = new List<PipelineUnit>();

    public void Add(PipelineUnit unit) {
        _pipelineUnits.Add(unit);
    }
    
    public override PipelineContext Execute(PipelineContext obj) {
        foreach (var pipelineUnit in _pipelineUnits) {
            obj = pipelineUnit.Execute(obj);
            if (!obj.IsExecute) {
                break;
            }
        }

        return obj;
    }
}