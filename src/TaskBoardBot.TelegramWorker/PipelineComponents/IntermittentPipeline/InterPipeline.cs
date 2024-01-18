using TaskBoardBot.TelegramWorker.Context;
using TaskBoardBot.TelegramWorker.Context.DbTables;
using TaskBoardBot.TelegramWorker.Services;
using Telegram.Bot.Types.Enums;

namespace TaskBoardBot.TelegramWorker.PipelineComponents.IntermittentPipeline;

public class InterPipeline(IServiceProvider iServiceProvider) : PipelineUnit {
    
    private readonly ICollection<PipelineUnit> _pipelineUnits = new List<PipelineUnit>();
    public DataBaseService GetDbService { get; } = iServiceProvider.GetService<DataBaseService>()
                                                   ?? throw new Exception("DataBaseService is null");

    public InterPipeline Add(PipelineUnit unit) {
        _pipelineUnits.Add(unit);
        return this;
    }
    
    public void Execute(PipelineContext obj) {
        obj.Parent = this;
        
        var message = obj.GetMessage();
        var callback = obj.GetCallbackQuery(); 
        Users? user = null;

        foreach (var pipelineUnit in _pipelineUnits) {
            if (obj.Type == UpdateType.Message && message != null) {
                user = GetDbService.GetUser(message.Chat.Id);
                obj = pipelineUnit.UpdateMessage(obj, message, user);
            }
            if (obj.Type == UpdateType.CallbackQuery && callback != null) {
                user = GetDbService.GetUser(callback.From.Id);
                obj = pipelineUnit.UpdateCallbackQuery(obj, callback, user);
            }
            if (!obj.IsExecute) {
                break;
            }
        }
    }
}