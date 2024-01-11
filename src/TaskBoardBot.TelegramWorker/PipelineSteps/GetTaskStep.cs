using System.Globalization;
using Hors;
using TaskBoardBot.TelegramWorker.IntermittentPipeline;
using Telegram.Bot;
using Telegram.Bot.Types.ReplyMarkups;

namespace TaskBoardBot.TelegramWorker.PipelineSteps;

public class GetTaskStep: NewPipelineUnit {
    
    private readonly HorsTextParser _horsTextParser = new();
    public override PipelineContext UpdateMessage(PipelineContext pipelineContext) {

        var message = pipelineContext.GetMessage();
        var user = pipelineContext.DataBaseService.GetUser(message.Chat.Id);
        
        if (user.UserState == TelegramState.None) {

            var parseTime = _horsTextParser.Parse(pipelineContext.Message.Text, DateTime.Now);
            var buttons = new List<InlineKeyboardButton[]>();

            user.AddedText = parseTime.Text;
            pipelineContext.DataBaseService.UpdateUser(user);

            string textMessage = parseTime.Text + "\n\n";
            foreach (var date in parseTime.Dates) {
                buttons.Add(new InlineKeyboardButton[] {
                    InlineKeyboardButton.WithCallbackData(date.DateTo.ToString(CultureInfo.InvariantCulture),
                        "t" + date.DateTo.ToFileTime())
                });
            }

            buttons.Add(new InlineKeyboardButton[] {
                InlineKeyboardButton.WithCallbackData("Изменить дату", "changeDate"),
                InlineKeyboardButton.WithCallbackData("Изменить текст", "changeText"),
            });
            var inlineKeyboard = new InlineKeyboardMarkup(buttons);


            pipelineContext.TelegramBotClient.SendTextMessageAsync(
                pipelineContext.Message.Chat, textMessage, replyMarkup: inlineKeyboard
            );
        }
        
        return pipelineContext;
    }
}