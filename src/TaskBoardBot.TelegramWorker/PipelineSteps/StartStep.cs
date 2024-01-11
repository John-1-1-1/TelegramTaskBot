using TaskBoardBot.TelegramWorker.IntermittentPipeline;
using Telegram.Bot;
using Telegram.Bot.Types.ReplyMarkups;

namespace TaskBoardBot.TelegramWorker.PipelineSteps;

public class StartStep : PipelineUnit {
    public override PipelineContext UpdateMessage(PipelineContext pipelineContext) {
        
        var message = pipelineContext.GetMessage();

        var user = pipelineContext.DataBaseService.GetUser(message.Chat.Id);
        
        var replyKeyboard = new ReplyKeyboardMarkup(
            new List<KeyboardButton[]>() {
                new KeyboardButton[] {
                    new("Список дел"),
                    new("Локальное время"),
                }
            }) {
            ResizeKeyboard = true,
        }; 
        
        switch (message.Text) {
            case "/start": {
                if (user == null) {
                    pipelineContext.DataBaseService.AddUser(message.Chat.Id, TelegramState.None);
                    pipelineContext.TelegramBotClient.SendTextMessageAsync(
                        message.Chat, "Вы успешно зарегестрированы!", replyMarkup: replyKeyboard);
                }
                else {
                    pipelineContext.TelegramBotClient.SendTextMessageAsync(
                        message.Chat, "Вы уже зарегестрированы!", replyMarkup: replyKeyboard);
                }
                pipelineContext.KillPipeline();
                break;
            }
            default: {
                if (user == null) {
                    pipelineContext.TelegramBotClient.SendTextMessageAsync(
                        message.Chat, "Используйте /start для регистрации!", replyMarkup: replyKeyboard);
                    pipelineContext.KillPipeline();
                }
                break;
            }
        }
        
        return pipelineContext;
    }
}