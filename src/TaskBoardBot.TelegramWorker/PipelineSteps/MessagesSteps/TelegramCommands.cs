using TaskBoardBot.TelegramWorker.IntermittentPipeline;
using Telegram.Bot;

namespace TaskBoardBot.TelegramWorker.PipelineSteps.MessagesSteps;

public class TelegramCommands: PipelineUnit {
    public override PipelineContext Execute(PipelineContext pipelineContext) {
        pipelineContext.IsExecute = false;
        
        var chat = pipelineContext.Message.Chat;
        var text = pipelineContext.Message.Text;

        switch (text) {
            case "/start": {
                pipelineContext.DataBaseService.AddUser(chat.Id, TelegramState.None);
                pipelineContext.TelegramBotClient.SendTextMessageAsync(
                    chat, text);
                return pipelineContext;
            }

            case "/changeTime": {
                var user = pipelineContext.DataBaseService.GetUser(chat.Id);
                if (user != null) {
                    user.UserState = TelegramState.ChangeLocalTime;
                    pipelineContext.DataBaseService.UpdateUser(user);
                }

                pipelineContext.TelegramBotClient.SendTextMessageAsync(
                    chat, "Введите ваше время.\n\n"+
                          "Пример:\n"+
                          "11 вечера\n"+
                          "23:00"); 
                 
                return pipelineContext;
            }
        }

        pipelineContext.IsExecute = true;
        return pipelineContext;
    }
}

