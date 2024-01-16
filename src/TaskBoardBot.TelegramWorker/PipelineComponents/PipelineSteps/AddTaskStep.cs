using System.Globalization;
using Hors;
using TaskBoardBot.TelegramWorker.Context;
using TaskBoardBot.TelegramWorker.PipelineComponents.IntermittentPipeline;
using Telegram.Bot;
using Telegram.Bot.Types.ReplyMarkups;

namespace TaskBoardBot.TelegramWorker.PipelineComponents.PipelineSteps;

public class AddTaskStep: PipelineUnit {
    
    private readonly HorsTextParser _horsTextParser = new();
    public override PipelineContext UpdateMessage(PipelineContext pipelineContext) {

        var message = pipelineContext.GetMessage();
        var user = pipelineContext.Parent.GetDbService.GetUser(message.Chat.Id);
        
        if (user == null || user.UserState != TelegramState.None) {
            return pipelineContext;
        }
        
        if (user.UserState == TelegramState.None) {

            var parseTime = _horsTextParser.Parse(message.Text, DateTime.Now);
            var buttons = new List<InlineKeyboardButton[]>();

            user.AddedText = parseTime.Text;
            pipelineContext.Parent.GetDbService.UpdateUser(user);

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
        var callback = pipelineContext.GetCallbackQuery();
        if (callback == null) {
            return pipelineContext;
        }
        var user = pipelineContext.Parent.GetDbService.GetUser(
            callback.From.Id);
        if (user == null) {
            return pipelineContext;
        }
        
        pipelineContext.TelegramBotClient.
            AnswerCallbackQueryAsync(callback.Id);
        
        if (callback.Data != null && callback.Data[0] == 't') {
            string message = callback.Data.Remove(0, 1);

            pipelineContext.Parent.GetDbService.AddTasks(new Tasks() {
                DateTime = DateTime.FromFileTime(long.Parse(message)),
                TgId = user.TgId, Text = user.AddedText
            });

            user.AddedText = string.Empty;
            pipelineContext.Parent.GetDbService.UpdateUser(user);
            pipelineContext.KillPipeline();
        }
        
        return pipelineContext;
    }
}