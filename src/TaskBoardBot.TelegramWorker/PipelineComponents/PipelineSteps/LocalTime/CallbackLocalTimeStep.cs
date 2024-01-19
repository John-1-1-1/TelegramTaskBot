using TaskBoardBot.TelegramWorker.Context.DbTables;
using TaskBoardBot.TelegramWorker.PipelineComponents.IntermittentPipeline;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace TaskBoardBot.TelegramWorker.PipelineComponents.PipelineSteps;

public class CallbackLocalTimeStep: PipelineUnit {
    public override PipelineContext UpdateCallbackQuery(PipelineContext pipelineContext,
        CallbackQuery callbackQuery, Users? user) {

        if (user == null) {
            return pipelineContext;
        }

        pipelineContext.TelegramBotClient.
            AnswerCallbackQueryAsync(callbackQuery.Id);
        
        if (callbackQuery.Data != null && callbackQuery.Data[0] == 'l') {
            user.UserState = TelegramState.None;
            user.LocalTime = DateTime.FromFileTime(long.Parse(callbackQuery.
                                 Data.Remove(0, 1))).Hour;
            user.Times = "";
            pipelineContext.Parent.GetDbService.UpdateUser(user);

            pipelineContext.TelegramBotClient.SendTextMessageAsync(callbackQuery.From.Id, "Ваше время сохранено!");
            
            pipelineContext.KillPipeline();
        }
        return pipelineContext;
    }

    public override bool IsTrueState(TelegramState? state) {
        return state == TelegramState.ChangeLocalTime;
    }
}