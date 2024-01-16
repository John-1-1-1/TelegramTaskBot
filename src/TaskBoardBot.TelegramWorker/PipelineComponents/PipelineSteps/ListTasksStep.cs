using TaskBoardBot.TelegramWorker.PipelineComponents.IntermittentPipeline;
using Telegram.Bot;

namespace TaskBoardBot.TelegramWorker.PipelineComponents.PipelineSteps;

public class ListTasksStep: PipelineUnit {
    public override PipelineContext UpdateMessage(PipelineContext pipelineContext) {
        var message = pipelineContext.GetMessage();
        var text = message!.Text;

        if (text == null) {
            return pipelineContext;
        }
        
        var user = pipelineContext.Parent.GetDbService.GetUser(message.Chat.Id);
        
        if (user == null || user.UserState != TelegramState.None) {
            return pipelineContext;
        }
        switch (text.ToLower()) {
            case "список дел": {
                var listTasks = pipelineContext.Parent.GetDbService.
                    GetTasksCollection(message.Chat.Id);
                var listTimes = listTasks.GroupBy(t => t.DateTime);
                
                pipelineContext.TelegramBotClient.SendTextMessageAsync(
                    message.Chat, string.Join("\n", 
                        listTimes.Select(t => "\ud83d\udccc На "+ (t.First().DateTime) + " \n" + 
                                              string.Join("", t.Select( u => "\u2705 " + u.Text + "\n")) ))); 
                pipelineContext.IsExecute = false;
                break;
            }
        }

        return pipelineContext;
    }
}