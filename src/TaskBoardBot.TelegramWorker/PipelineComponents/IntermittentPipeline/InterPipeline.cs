using TaskBoardBot.TelegramWorker.Context;
using TaskBoardBot.TelegramWorker.Context.DbTables;
using TaskBoardBot.TelegramWorker.Services;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace TaskBoardBot.TelegramWorker.PipelineComponents.IntermittentPipeline;

public class InterPipeline(IServiceProvider iServiceProvider) : PipelineUnit {
    
    private readonly Dictionary<TelegramState, ICollection<PipelineUnit>> _pipelineUnits = new();
    public DataBaseService GetDbService { get; } = iServiceProvider.GetService<DataBaseService>()
                                                   ?? throw new Exception("DataBaseService is null");

    private ICollection<PipelineUnit> _priorityPipelineUnits = new List<PipelineUnit>();
    
    public InterPipeline AddLine(PipelineLineBuilder pipelineLineBuilder) {
        _pipelineUnits[pipelineLineBuilder.State ?? throw new Exception("Null State")] = pipelineLineBuilder.PipelineUnits;
        return this;
    }
    
    public InterPipeline AddPriorityLine(PipelineLineBuilder pipelineLineBuilder) {
        _priorityPipelineUnits = pipelineLineBuilder.PipelineUnits;
        return this;
    }
    
    public void Execute(PipelineContext obj, UpdateType type) {
        obj.Parent = this;
        
        var message = obj.GetMessage();
        var callback = obj.GetCallbackQuery(); 
        
        Users? user = GetDbService.GetUser(message?.Chat.Id ?? callback?.From.Id);
        var userState = user?.UserState;


        foreach (var pipelineUnit in _priorityPipelineUnits) {
            if (type == UpdateType.Message && message != null) {
                obj = pipelineUnit.UpdateMessage(obj, message, user);
            }
            if (type == UpdateType.CallbackQuery && callback != null) {
                obj = pipelineUnit.UpdateCallbackQuery(obj, callback, user);
            }
            if (!obj.IsExecute) {
                break;
            }
        }
        
        if (userState == null) {
            return;
        }
        
        foreach (var pipelineUnit in _pipelineUnits[userState.Value]) {
            if (type == UpdateType.Message && message != null) {
                obj = pipelineUnit.UpdateMessage(obj, message, user);
            }
            if (type == UpdateType.CallbackQuery && callback != null) {
                obj = pipelineUnit.UpdateCallbackQuery(obj, callback, user);
            }
            if (!obj.IsExecute) {
                break;
            }
        }
    }
}

