using System.Globalization;
using Hors;
using TaskBoardBot.TelegramWorker.Context;
using TaskBoardBot.TelegramWorker.IntermittentPipeline;
using Telegram.Bot;
using Telegram.Bot.Types.ReplyMarkups;

namespace TaskBoardBot.TelegramWorker.PipelineSteps;

public class LocalTimeStep: PipelineUnit {
    private readonly HorsTextParser _horsTextParser = new();
    
    public override PipelineContext UpdateMessage(PipelineContext pipelineContext) {
        var message = pipelineContext.GetMessage();
        var user = pipelineContext.DataBaseService.GetUser(message.Chat.Id);
        var text = message.Text!;
        
        if (user == null) {
            return pipelineContext;
        }

        if (user.UserState == TelegramState.None) {
            switch (text.ToLower()) {
                case "локальное время": {
                    user.UserState = TelegramState.ChangeLocalTime;
                    pipelineContext.DataBaseService.UpdateUser(user);
                    pipelineContext.TelegramBotClient.SendTextMessageAsync(message.Chat, "Введите своё время!");
                    return pipelineContext;
                }
            }
        }

        if (user.UserState == TelegramState.ChangeLocalTime) {
            var parseTime = _horsTextParser.Parse(message.Text, DateTime.Now);
            var buttons = new List<InlineKeyboardButton[]>();

            string textMessage = "Выберите местное время:";
            foreach (var date in parseTime.Dates) {
                buttons.Add(new InlineKeyboardButton[] {
                    InlineKeyboardButton.WithCallbackData(date.DateTo.ToString(CultureInfo.InvariantCulture),
                        "l" + date.DateTo.ToFileTime())
                });
            }
            
            var inlineKeyboard = new InlineKeyboardMarkup(buttons);

            pipelineContext.TelegramBotClient.SendTextMessageAsync(
                message.Chat, textMessage, replyMarkup: inlineKeyboard
            );
            pipelineContext.KillPipeline();
        }
        
        return pipelineContext;
    }

    public override PipelineContext UpdateCallbackQuery(PipelineContext pipelineContext) {
        if (pipelineContext.CallbackQuery == null) {
            return pipelineContext;
        }
        
        pipelineContext.TelegramBotClient.
            AnswerCallbackQueryAsync(pipelineContext.CallbackQuery.Id);
        
        if (pipelineContext.CallbackQuery.Data[0] == 'l') {
            string message = pipelineContext.CallbackQuery.Data.Remove(0, 1);

            var user = pipelineContext.DataBaseService.GetUser(
                pipelineContext.CallbackQuery.From.Id);

            user.UserState = TelegramState.None;
            user.LocalTime = DateTime.FromFileTime(long.Parse(message)).ToUniversalTime().Hour + DateTime.UtcNow.Hour;
            
            pipelineContext.DataBaseService.UpdateUser(user);
            pipelineContext.KillPipeline();
        }
        
        return pipelineContext;
    }
}