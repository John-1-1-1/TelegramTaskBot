using Hors;
using TaskBoardBot.TelegramWorker.Context.DbTables;
using TaskBoardBot.TelegramWorker.PipelineComponents.IntermittentPipeline;
using Telegram.Bot;
using Telegram.Bot.Types;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace TaskBoardBot.TelegramWorker.PipelineComponents.PipelineSteps.ChangeDate;

public class ChangeDateStep: PipelineUnit {
    private readonly HorsTextParser _horsTextParser = new();

    public override PipelineContext UpdateMessage(PipelineContext pipelineContext, Message message, Users? user) {

        if (user == null) {
            return pipelineContext;
        }
        
        var parseTime = _horsTextParser.Parse(message.Text, DateTime.Now.Add(user.LocalTime));
        
        List<DateTime> listDates = parseTime.Dates.Select(d => d.DateTo).ToList();

        if (listDates.Count == 0) {

            pipelineContext.TelegramBotClient.SendTextMessageAsync(
                message.Chat, "Дата не распознана!"
            );
            return pipelineContext;
        }
        user.Times = JsonSerializer.Serialize(listDates);        
        pipelineContext.Parent.GetDbService.UpdateUser(user);

        var inlineKeyboard = new MarkupBuilder().AddDates(listDates, "t",
            user.LocalTime).GetChangeButton().GetInlineKeyboardMarkup();

        pipelineContext.TelegramBotClient.SendTextMessageAsync(
            message.Chat, user.AddedText, replyMarkup: inlineKeyboard
        );
        user.UserState = TelegramState.None;
        pipelineContext.Parent.GetDbService.UpdateUser(user);
        
        pipelineContext.KillPipeline();
        return pipelineContext;
    }
}