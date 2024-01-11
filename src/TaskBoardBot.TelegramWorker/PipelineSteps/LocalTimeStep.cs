using TaskBoardBot.TelegramWorker.IntermittentPipeline;
using Telegram.Bot;

namespace TaskBoardBot.TelegramWorker.PipelineSteps;

public class LocalTimeStep: PipelineUnit {
    public override PipelineContext UpdateMessage(PipelineContext pipelineContext) {
        var message = pipelineContext.GetMessage();
        var user = pipelineContext.DataBaseService.GetUser(message.Chat.Id);
        var text = message.Text!;
        
        if (user == null) {
            return pipelineContext;
        }

        if (user.UserState != TelegramState.None) {
            return pipelineContext;
        }
        
        switch (text.ToLower()) {
            case "локальное время": {
                user.UserState = TelegramState.ChangeLocalTime;
                pipelineContext.DataBaseService.UpdateUser(user);
                pipelineContext.TelegramBotClient.SendTextMessageAsync(message.Chat, "Введите своё время!");
                
                break;
            }
        }
        return pipelineContext;
    }

    public override PipelineContext UpdateCallbackQuery(PipelineContext pipelineContext) {
        return base.UpdateCallbackQuery(pipelineContext);
    }
}