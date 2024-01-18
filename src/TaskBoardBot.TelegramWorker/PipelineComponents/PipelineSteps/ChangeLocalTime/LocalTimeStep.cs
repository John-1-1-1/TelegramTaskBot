using System.Globalization;
using Hors;
using TaskBoardBot.TelegramWorker.Context.DbTables;
using TaskBoardBot.TelegramWorker.PipelineComponents.IntermittentPipeline;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace TaskBoardBot.TelegramWorker.PipelineComponents.PipelineSteps.ChangeLocalTime;

public class LocalTimeStep: PipelineUnit {
    private readonly HorsTextParser _horsTextParser = new();
    
    public override PipelineContext UpdateMessage(PipelineContext pipelineContext, Message message, Users? user) {
        var text = message.Text;
        
        if (user == null) {
            return pipelineContext;
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

    public override PipelineContext UpdateCallbackQuery(PipelineContext pipelineContext,
        CallbackQuery callbackQuery, Users? user) {
        
        pipelineContext.TelegramBotClient.
            AnswerCallbackQueryAsync(callbackQuery.Id);
        
        if (callbackQuery.Data != null && callbackQuery.Data[0] == 'l') {
            string message = callbackQuery.Data.Remove(0, 1);

            if (user != null) {
                user.UserState = TelegramState.None;
                
                user.LocalTime = DateTime.FromFileTime(long.Parse(message)).Hour - 
                                 DateTime.UtcNow.Hour;

                pipelineContext.Parent.GetDbService.UpdateUser(user);
            }

            pipelineContext.KillPipeline();
        }
        
        return pipelineContext;
    }
}