using Hors;
using TaskBoardBot.TelegramWorker.Context.DbTables;
using TaskBoardBot.TelegramWorker.PipelineComponents.IntermittentPipeline;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace TaskBoardBot.TelegramWorker.PipelineComponents.PipelineSteps.ChangeLocalTime;

public class LocalTimeStep: PipelineUnit {
    private readonly HorsTextParser _horsTextParser = new();
    
    public override PipelineContext UpdateMessage(PipelineContext pipelineContext, Message message, Users? user) {
        
        if (user == null) {
            return pipelineContext;
        }

        const string textMessage = "Выберите местное время:";
        
        var parseTime = _horsTextParser.Parse(message.Text, DateTime.Now);
        
        var inlineKeyboard = new MarkupBuilder()
            .AddDates(parseTime.Dates.Select(d => d.DateTo).ToList(), "l").GetInlineKeyboardMarkup();

        pipelineContext.TelegramBotClient.SendTextMessageAsync(
            message.Chat, textMessage, replyMarkup: inlineKeyboard
        );
        pipelineContext.KillPipeline();

        return pipelineContext;
    }

    public override PipelineContext UpdateCallbackQuery(PipelineContext pipelineContext,
        CallbackQuery callbackQuery, Users? user) {
        
        pipelineContext.TelegramBotClient.
            AnswerCallbackQueryAsync(callbackQuery.Id);
        
        if (callbackQuery.Data != null && callbackQuery.Data[0] == 'l') {
            string message = callbackQuery.Data.Remove(0, 1);

            if (user != null) {
                user.UserState = TelegramState.None;
                
                user.LocalTime = DateTime.FromFileTime(long.Parse(message)).ToUniversalTime() - 
                                              DateTime.UtcNow;
                pipelineContext.TelegramBotClient.SendTextMessageAsync(callbackQuery.From.Id, "Ваше время: " + 
                    DateTime.FromFileTime(long.Parse(message)));
                pipelineContext.Parent.GetDbService.UpdateUser(user);
            }

            pipelineContext.KillPipeline(); 
        }
        
        return pipelineContext;
    }
}