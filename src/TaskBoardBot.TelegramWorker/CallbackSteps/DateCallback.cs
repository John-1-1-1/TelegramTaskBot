using TaskBoardBot.TelegramWorker.Context;
using TaskBoardBot.TelegramWorker.IntermittentPipeline;
using Telegram.Bot;

namespace TaskBoardBot.TelegramWorker.CallbackSteps;

public class DateCallback: PipelineUnit {
    public override PipelineContext Execute(PipelineContext pipelineContext) {

        if (pipelineContext.CallbackQuery == null) {
            return pipelineContext;
        }
        
        if (pipelineContext.CallbackQuery.Data[0] == 't') {
            string message = pipelineContext.CallbackQuery.Data.Remove(0, 1);
            pipelineContext.TelegramBotClient.
                AnswerCallbackQueryAsync(pipelineContext.CallbackQuery.Id);

            var user = pipelineContext.DataBaseService.GetUser(
                pipelineContext.CallbackQuery.Message.Chat.Id);
            
            var f = long.Parse(message);
            var cc = DateTime.FromFileTime(f);
            
            pipelineContext.DataBaseService.AddTasks(new Tasks() {
                DateTime = cc, TgId = user.TgId, Text = user.AddedText
            });

            user.AddedText = string.Empty;
            pipelineContext.DataBaseService.UpdateUser(user);
            
        }
        
        return pipelineContext;
    }
}