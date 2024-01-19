using System.Globalization;
using Hors;
using TaskBoardBot.TelegramWorker.Context.DbTables;
using TaskBoardBot.TelegramWorker.PipelineComponents.IntermittentPipeline;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace TaskBoardBot.TelegramWorker.PipelineComponents.PipelineSteps;

public class LocalTimeChangeTimeStep: PipelineUnit {
    
    private readonly HorsTextParser _horsTextParser = new();

    public override bool IsTrueState(TelegramState? state) {
        return state == TelegramState.ChangeLocalTime;
    }

    public override PipelineContext UpdateMessage(PipelineContext pipelineContext, Message message, Users? user) {
        
        if (user == null) {
            return pipelineContext;
        }
        
        const string textMessage = "Выберите местное время:";
        
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
}