using System.Globalization;
using Hors;
using TaskBoardBot.TelegramWorker.Context;
using TaskBoardBot.TelegramWorker.IntermittentPipeline;
using Telegram.Bot;
using Telegram.Bot.Types.ReplyMarkups;

namespace TaskBoardBot.TelegramWorker.Steps;

public class TelegramTextMessages: PipelineUnit {
    
    private readonly HorsTextParser _horsTextParser = new HorsTextParser();
    public override PipelineContext Execute(PipelineContext pipelineContext) {
        
        var parseTime = _horsTextParser.Parse(pipelineContext.Message.Text, DateTime.Now);
        var buttons = new List<InlineKeyboardButton[]>();

        var user = pipelineContext.DataBaseService.GetUser(pipelineContext.Message.Chat.Id);
        user.AddedText = parseTime.Text;
        pipelineContext.DataBaseService.UpdateUser(user);

        string textMessage = parseTime.Text + "\n\n";
        foreach (var date in parseTime.Dates) {
            buttons.Add(new InlineKeyboardButton[] {
                InlineKeyboardButton.WithCallbackData(date.DateTo.ToString(CultureInfo.InvariantCulture),
                    "t"+date.DateTo.Date.ToFileTime() )
            });
        }
        buttons.Add(new InlineKeyboardButton[] {
            InlineKeyboardButton.WithCallbackData("Изменить дату","changeDate"),
            InlineKeyboardButton.WithCallbackData("Изменить текст","changeText"),
        });
        var inlineKeyboard = new InlineKeyboardMarkup(buttons);
                   
                    
        pipelineContext.TelegramBotClient.SendTextMessageAsync(
            pipelineContext.Message.Chat, textMessage, replyMarkup: inlineKeyboard
        );
        
        return pipelineContext;
    }
}