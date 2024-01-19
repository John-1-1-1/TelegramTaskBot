using System.Globalization;
using TaskBoardBot.TelegramWorker.Context.DbTables;
using TaskBoardBot.TelegramWorker.PipelineComponents.IntermittentPipeline;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace TaskBoardBot.TelegramWorker.PipelineComponents.PipelineSteps.ChangeMessageStep;

public class ChangeTextStep: PipelineUnit {
    public override PipelineContext UpdateMessage(PipelineContext pipelineContext, 
        Message message, Users? user) {

        if (message.Text != null) {
            user.AddedText = message.Text;
            user.UserState = TelegramState.None;
            pipelineContext.Parent.GetDbService.UpdateUser(user);

            var parseTime = JsonSerializer.Deserialize<List<DateTime>>(user.Times);

            if (parseTime == null) {
                return pipelineContext;
            }

            var inlineMarkup = new MarkupBuilder().AddDates(parseTime, "t")
                .GetChangeButton().GetInlineKeyboardMarkup();
            

            pipelineContext.TelegramBotClient.SendTextMessageAsync(
                message.Chat, message.Text, replyMarkup: inlineMarkup
            );
        }

        pipelineContext.KillPipeline();
        return pipelineContext;
    }
}