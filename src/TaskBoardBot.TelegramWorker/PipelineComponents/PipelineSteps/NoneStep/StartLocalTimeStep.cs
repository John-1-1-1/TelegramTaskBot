using TaskBoardBot.TelegramWorker.Context.DbTables;
using TaskBoardBot.TelegramWorker.PipelineComponents.IntermittentPipeline;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace TaskBoardBot.TelegramWorker.PipelineComponents.PipelineSteps.NoneStep;

public class StartLocalTimeStep: PipelineUnit {
    public override PipelineContext UpdateMessage(PipelineContext pipelineContext, Message message, Users? user) {

        if (user == null) {
            return pipelineContext;
        }
        
        if (user.LocalTime == null) {
            user.UserState = TelegramState.ChangeLocalTime;
            pipelineContext.Parent.GetDbService.UpdateUser(user);
            pipelineContext.TelegramBotClient.SendTextMessageAsync(message.Chat, 
                "Введите местное время для корректной работы бота!");
        }
        
        switch (message.Text?.ToLower()) {
            case "локальное время": {
                user.UserState = TelegramState.ChangeLocalTime;
                pipelineContext.Parent.GetDbService.UpdateUser(user);
                pipelineContext.TelegramBotClient.SendTextMessageAsync(message.Chat, "Введите местное время!");
                return pipelineContext;
            }
        }
        
        return pipelineContext;
    }
}