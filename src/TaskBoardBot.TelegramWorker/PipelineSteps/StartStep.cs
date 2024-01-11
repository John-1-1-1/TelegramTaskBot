using TaskBoardBot.TelegramWorker.IntermittentPipeline;
using Telegram.Bot;
using Telegram.Bot.Types.ReplyMarkups;

namespace TaskBoardBot.TelegramWorker.PipelineSteps;

public class StartStep: NewPipelineUnit {
    public override PipelineContext UpdateMessage(PipelineContext pipelineContext) {
        var message = pipelineContext.GetMessage();
        if (message.Text == "/start") {
            var replyKeyboard = new ReplyKeyboardMarkup(
                new List<KeyboardButton[]>() {
                    new KeyboardButton[] {
                        new ("Список дел"),
                        new ("Локальное время"),
                    } }) { ResizeKeyboard = true,
            };

            var user = pipelineContext.DataBaseService.GetUser(message.Chat.Id);
            if (user == null) {
                pipelineContext.DataBaseService.AddUser(message.Chat.Id, TelegramState.None);
            }
                
            pipelineContext.TelegramBotClient.SendTextMessageAsync(
                message.Chat, message.Text, replyMarkup: replyKeyboard);
        }
        
        return pipelineContext;
    }
}