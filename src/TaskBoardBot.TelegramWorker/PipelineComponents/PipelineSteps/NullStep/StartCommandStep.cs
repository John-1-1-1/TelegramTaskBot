using TaskBoardBot.TelegramWorker.Context;
using TaskBoardBot.TelegramWorker.Context.DbTables;
using TaskBoardBot.TelegramWorker.PipelineComponents.IntermittentPipeline;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace TaskBoardBot.TelegramWorker.PipelineComponents.PipelineSteps;

public class StartCommandStep : PipelineUnit {
    public override PipelineContext UpdateMessage(PipelineContext pipelineContext, 
        Message message, Users? user) {

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
                        pipelineContext.Parent.GetDbService.AddUser(message.Chat.Id, TelegramState.None);
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