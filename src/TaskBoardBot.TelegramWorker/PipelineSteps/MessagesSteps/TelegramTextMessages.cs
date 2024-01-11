using System.Globalization;
using Hors;
using TaskBoardBot.TelegramWorker.IntermittentPipeline;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace TaskBoardBot.TelegramWorker.PipelineSteps.MessagesSteps;

public class TelegramTextMessages: PipelineUnit {
    
    private readonly HorsTextParser _horsTextParser = new();
    public override PipelineContext Execute(PipelineContext pipelineContext) {
        
        var user = pipelineContext.DataBaseService.GetUser(pipelineContext.Message.Chat.Id);

        if (pipelineContext.Message.Text == "Локальное время") {
            
            user.UserState = TelegramState.ChangeLocalTime;
            pipelineContext.DataBaseService.UpdateUser(user);
            pipelineContext.TelegramBotClient.SendTextMessageAsync(pipelineContext.Message.Chat.Id,
                "Введите местное время");
        }
        
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

        if (user.UserState == TelegramState.ChangeLocalTime) {
            
            var parseTime = _horsTextParser.Parse(pipelineContext.Message.Text, DateTime.Now);
            var buttons = new List<InlineKeyboardButton[]>();

            user.AddedText = parseTime.Text;
            pipelineContext.DataBaseService.UpdateUser(user);

            string textMessage = "Выберете время:/n";
            foreach (var date in parseTime.Dates) {
                buttons.Add(new InlineKeyboardButton[] {
                    InlineKeyboardButton.WithCallbackData(date.DateTo.ToString(CultureInfo.InvariantCulture),
                        "l" + date.DateTo.ToFileTime())
                });
            }
            
            var inlineKeyboard = new InlineKeyboardMarkup(buttons);
            
            pipelineContext.TelegramBotClient.SendTextMessageAsync(
                pipelineContext.Message.Chat, textMessage, replyMarkup: inlineKeyboard
            );
            
        }

        return pipelineContext;
    }
}