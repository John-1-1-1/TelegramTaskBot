using TaskBoardBot.TelegramWorker.Context.DbTables;
using TaskBoardBot.TelegramWorker.PipelineComponents.IntermittentPipeline;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace TaskBoardBot.TelegramWorker.PipelineComponents.PipelineSteps.NoneStep;

public class ListTasksStep: PipelineUnit {
    public override PipelineContext UpdateMessage(PipelineContext pipelineContext,
        Message message, Users? user) {
        
        if (user == null) {
            return pipelineContext;
        }
        
        switch (message.Text?.ToLower()) {
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