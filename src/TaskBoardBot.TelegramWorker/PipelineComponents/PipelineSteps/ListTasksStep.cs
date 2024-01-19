using TaskBoardBot.TelegramWorker.Context;
using TaskBoardBot.TelegramWorker.Context.DbTables;
using TaskBoardBot.TelegramWorker.PipelineComponents.IntermittentPipeline;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace TaskBoardBot.TelegramWorker.PipelineComponents.PipelineSteps;

public class ListTasksStep: PipelineUnit {
    public override bool IsTrueState(TelegramState? state) {
        return state == TelegramState.None;
    }

    public override PipelineContext UpdateMessage(PipelineContext pipelineContext,
        Message message, Users? user) {

        if (user == null || message.Text?.ToLower() != "список дел") {
            return pipelineContext;
        }

        var listTasks = pipelineContext.Parent.GetDbService.
            GetTasksCollection(message.Chat.Id);

        if (listTasks.Count == 0) {
            pipelineContext.TelegramBotClient.SendTextMessageAsync(
                message.Chat, "У вас ещё нет активных задач!"); 
        }
        
        var listTimes = listTasks.GroupBy(t => t.DateTime);
                
        pipelineContext.TelegramBotClient.SendTextMessageAsync(
            message.Chat, string.Join("\n", 
                listTimes.Select(t => "\ud83d\udccc На "+ t.First().
                                          DateTime.AddHours(user.LocalTime ?? 0) + " \n" + 
                                      string.Join("", t.Select( u => "\u2705 " + u.Text + "\n")) ))); 
        pipelineContext.IsExecute = false;
        
        return pipelineContext;
    }
}