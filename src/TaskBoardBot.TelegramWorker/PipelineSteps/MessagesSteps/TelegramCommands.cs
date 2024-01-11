using TaskBoardBot.TelegramWorker.IntermittentPipeline;
using Telegram.Bot;
using Telegram.Bot.Types.ReplyMarkups;

namespace TaskBoardBot.TelegramWorker.PipelineSteps.MessagesSteps;

public class TelegramCommands: PipelineUnit {
    public override PipelineContext Execute(PipelineContext pipelineContext) {
        pipelineContext.IsExecute = false;
        
        var chat = pipelineContext.Message.Chat;
        var text = pipelineContext.Message.Text;

        switch (text) {
            case "/start": {
                
                var replyKeyboard = new ReplyKeyboardMarkup(
                    new List<KeyboardButton[]>() {
                        new KeyboardButton[] {
                            new KeyboardButton("Список дел"),
                            new KeyboardButton("Локальное время"),
                        } }) { ResizeKeyboard = true,
                };

                var user = pipelineContext.DataBaseService.GetUser(chat.Id);
                if (user == null) {
                    pipelineContext.DataBaseService.AddUser(chat.Id, TelegramState.None);
                }
                
                pipelineContext.TelegramBotClient.SendTextMessageAsync(
                    chat, text, replyMarkup: replyKeyboard);
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

            case "Список дел": {
                var listTasks = pipelineContext.DataBaseService.GetTasksCollection(
                    pipelineContext.Message.Chat.Id);
                var listTimes = listTasks.GroupBy(t => t.DateTime);
                
                pipelineContext.TelegramBotClient.SendTextMessageAsync(
                    chat, string.Join("\n", 
                            listTimes.Select(t => "\ud83d\udccc На "+ t.First().DateTime + " \n" + 
                                                  string.Join("", t.Select( u => "\u2705 " + u.Text + "\n")) ))); 
                return pipelineContext;
            }
        }

        pipelineContext.IsExecute = true;
        return pipelineContext;
    }
}

