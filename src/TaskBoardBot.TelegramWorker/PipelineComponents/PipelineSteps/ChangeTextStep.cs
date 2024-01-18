using System.Globalization;
using Newtonsoft.Json;
using TaskBoardBot.TelegramWorker.Context;
using TaskBoardBot.TelegramWorker.Context.DbTables;
using TaskBoardBot.TelegramWorker.PipelineComponents.IntermittentPipeline;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace TaskBoardBot.TelegramWorker.PipelineComponents.PipelineSteps;

public class ChangeTextStep: PipelineUnit {
    public override PipelineContext UpdateMessage(PipelineContext pipelineContext, 
        Message message, Users? user) {

        if (user?.UserState != TelegramState.ChangeMessage) {
            return pipelineContext;
        }

        if (message.Text != null) {
            user.AddedText = message.Text;
            user.UserState = TelegramState.None;
            pipelineContext.Parent.GetDbService.UpdateUser(user);
            
            var buttons = new List<InlineKeyboardButton[]>();

            var parseTime = JsonSerializer.Deserialize<List<DateTime>>(user.Times);

            if (parseTime == null) {
                return pipelineContext;
            }
            
            foreach (var date in parseTime) {
                buttons.Add(new InlineKeyboardButton[] {
                    InlineKeyboardButton.WithCallbackData(date.ToString(CultureInfo.InvariantCulture),
                        "t" + date.ToFileTime())
                });
            }
            
            buttons.Add(new InlineKeyboardButton[] {
                InlineKeyboardButton.WithCallbackData("Изменить дату", "changeDate"),
                InlineKeyboardButton.WithCallbackData("Изменить текст", "changeText"),
            });
            var inlineKeyboard = new InlineKeyboardMarkup(buttons);

            

            pipelineContext.TelegramBotClient.SendTextMessageAsync(
                message.Chat, message.Text, replyMarkup: inlineKeyboard
            );
        }

        pipelineContext.KillPipeline();
        return pipelineContext;
    }
}