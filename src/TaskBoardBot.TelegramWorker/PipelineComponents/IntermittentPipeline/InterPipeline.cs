using TaskBoardBot.TelegramWorker.Context;
using TaskBoardBot.TelegramWorker.Context.DbTables;
using TaskBoardBot.TelegramWorker.Services;
using Telegram.Bot.Types.Enums;

namespace TaskBoardBot.TelegramWorker.PipelineComponents.IntermittentPipeline;

public class InterPipeline(IServiceProvider iServiceProvider) {
    
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
        Users? user = GetDbService.GetUser(message?.Chat.Id ?? callback?.From.Id);

        foreach (var pipelineUnit in _pipelineUnits) {
            
            if (!pipelineUnit.IsTrueState(user?.UserState)) {
                continue;
            }

            if (obj.Type == UpdateType.Message && message != null) {
                obj = pipelineUnit.UpdateMessage(obj, message, user);
            }
            if (obj.Type == UpdateType.CallbackQuery && callback != null) {
                obj = pipelineUnit.UpdateCallbackQuery(obj, callback, user);
            }
            if (!obj.IsExecute) {
                break;
            }
        }
    }
}