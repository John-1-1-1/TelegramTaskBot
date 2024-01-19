using System.Globalization;
using Hors;
using TaskBoardBot.TelegramWorker.Context;
using TaskBoardBot.TelegramWorker.Context.DbTables;
using TaskBoardBot.TelegramWorker.PipelineComponents.IntermittentPipeline;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace TaskBoardBot.TelegramWorker.PipelineComponents.PipelineSteps;

public class LocalTimeStep: PipelineUnit {
    public override PipelineContext UpdateMessage(PipelineContext pipelineContext, Message message, Users? user) {
        var text = message.Text;

        if (user == null) {
            return pipelineContext;
        }

        if (user.LocalTime == null) {
            user.UserState = TelegramState.ChangeLocalTime;
            pipelineContext.Parent.GetDbService.UpdateUser(user);
            pipelineContext.TelegramBotClient.SendTextMessageAsync(message.Chat,
                "Введите местное время для корректной работы бота!");
            return pipelineContext;
        }

        if (text?.ToLower() == "локальное время") {
            user.UserState = TelegramState.ChangeLocalTime; 
            pipelineContext.Parent.GetDbService.UpdateUser(user); 
            pipelineContext.TelegramBotClient.SendTextMessageAsync(message.Chat, "Введите местное время!"); 
            return pipelineContext;
        }

        return pipelineContext;
    }

    public override bool IsTrueState(TelegramState? state) {
        return state == TelegramState.None;
    }
}