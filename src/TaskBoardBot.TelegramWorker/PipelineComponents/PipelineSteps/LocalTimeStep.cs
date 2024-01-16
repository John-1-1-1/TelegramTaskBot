using System.Globalization;
using Hors;
using TaskBoardBot.TelegramWorker.PipelineComponents.IntermittentPipeline;
using Telegram.Bot;
using Telegram.Bot.Types.ReplyMarkups;

namespace TaskBoardBot.TelegramWorker.PipelineComponents.PipelineSteps;

public class LocalTimeStep: PipelineUnit {
    private readonly HorsTextParser _horsTextParser = new();
    
    public override PipelineContext UpdateMessage(PipelineContext pipelineContext) {
        var message = pipelineContext.GetMessage();
        
        if (message == null) {
            return pipelineContext;
        }
        
        var user = pipelineContext.Parent.GetDbService.GetUser(message.Chat.Id);
        var text = message.Text;
        
        if (text == null) {
            return pipelineContext;
        }
        
        if (user == null) {
            return pipelineContext;
        }

        if (user.UserState == TelegramState.None) {
            switch (text.ToLower()) {
                case "локальное время": {
                    user.UserState = TelegramState.ChangeLocalTime;
                    pipelineContext.Parent.GetDbService.UpdateUser(user);
                    pipelineContext.TelegramBotClient.SendTextMessageAsync(message.Chat, "Введите своё время!");
                    return pipelineContext;
                }
            }
        }

        const string textMessage = "Выберите местное время:";
        if (user.UserState != TelegramState.ChangeLocalTime) {
            return pipelineContext;
        }
        
        var parseTime = _horsTextParser.Parse(message.Text, DateTime.Now);
        var buttons = new List<InlineKeyboardButton[]>();

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

        return pipelineContext;
    }

    public override PipelineContext UpdateCallbackQuery(PipelineContext pipelineContext) {
        var callback = pipelineContext.GetCallbackQuery();
        if (callback == null) {
            return pipelineContext;
        }
        
        pipelineContext.TelegramBotClient.
            AnswerCallbackQueryAsync(callback.Id);
        
        if (callback.Data != null && callback.Data[0] == 'l') {
            string message = callback.Data.Remove(0, 1);

            var user = pipelineContext.Parent.GetDbService.GetUser(
                callback.From.Id);

            if (user != null) {
                user.UserState = TelegramState.None;
                user.LocalTime = DateTime.FromFileTime(long.Parse(message)).ToUniversalTime().Hour +
                                 DateTime.UtcNow.Hour;

                pipelineContext.Parent.GetDbService.UpdateUser(user);
            }

            pipelineContext.KillPipeline();
        }
        
        return pipelineContext;
    }
}