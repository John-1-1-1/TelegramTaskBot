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
    public override bool IsTrueState(TelegramState? state) {
        return state == TelegramState.ChangeMessage;
    }

    public override PipelineContext UpdateMessage(PipelineContext pipelineContext, 
        Message message, Users? user) {

        if (user == null || message.Text == null) {
            return pipelineContext;
        }

        user.AddedText = message.Text;
        user.UserState = TelegramState.None;
        pipelineContext.Parent.GetDbService.UpdateUser(user);

        var parseTime = JsonSerializer.Deserialize<List<DateTime>>(user.Times);

        if (parseTime == null) {
            pipelineContext.TelegramBotClient.SendTextMessageAsync(
                message.Chat, "Нечего изменять. Создайте задачу!");
            return pipelineContext;
        }

        var inlineKeyboard = new InlineButtonsBuilder().AddDates(parseTime).AddChangeButtons()
            .GetInlineKeyboardMarkup();
            

        pipelineContext.TelegramBotClient.SendTextMessageAsync(
            message.Chat, message.Text, replyMarkup: inlineKeyboard);

        pipelineContext.KillPipeline();
        return pipelineContext;
    }
}