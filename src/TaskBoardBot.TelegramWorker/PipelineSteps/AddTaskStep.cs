using System.Globalization;
using Hors;
using TaskBoardBot.TelegramWorker.Context;
using TaskBoardBot.TelegramWorker.IntermittentPipeline;
using Telegram.Bot;
using Telegram.Bot.Types.ReplyMarkups;

namespace TaskBoardBot.TelegramWorker.PipelineSteps;

public class AddTaskStep: PipelineUnit {
    
    private readonly HorsTextParser _horsTextParser = new();
    public override PipelineContext UpdateMessage(PipelineContext pipelineContext) {

        var message = pipelineContext.GetMessage();
        var user = pipelineContext.DataBaseService.GetUser(message.Chat.Id);
        
        if (user == null || user.UserState != TelegramState.None) {
            return pipelineContext;
        }
        
        if (user.UserState == TelegramState.None) {

            var parseTime = _horsTextParser.Parse(message.Text, DateTime.Now);
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
                message.Chat, textMessage, replyMarkup: inlineKeyboard
            );
            pipelineContext.IsExecute = false;
        }
        return pipelineContext;
    }

    public override PipelineContext UpdateCallbackQuery(PipelineContext pipelineContext) {
        if (pipelineContext.CallbackQuery == null) {
            return pipelineContext;
        }
        
        pipelineContext.TelegramBotClient.
            AnswerCallbackQueryAsync(pipelineContext.CallbackQuery.Id);
        
        if (pipelineContext.CallbackQuery.Data[0] == 't') {
            string message = pipelineContext.CallbackQuery.Data.Remove(0, 1);

            var user = pipelineContext.DataBaseService.GetUser(
                pipelineContext.CallbackQuery.Message.Chat.Id);
            
            pipelineContext.DataBaseService.AddTasks(new Tasks() {
                DateTime =  DateTime.FromFileTime(long.Parse(message)).ToUniversalTime(),
                TgId = user.TgId, Text = user.AddedText
            });

            user.AddedText = string.Empty;
            pipelineContext.DataBaseService.UpdateUser(user);
            pipelineContext.KillPipeline();
        }
        
        return pipelineContext;
    }
}