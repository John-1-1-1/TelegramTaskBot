using TaskBoardBot.TelegramWorker.Context;
using TaskBoardBot.TelegramWorker.IntermittentPipeline;
using Telegram.Bot;

namespace TaskBoardBot.TelegramWorker.PipelineSteps.CallbackSteps;

public class DateCallback: PipelineUnit {
    public override PipelineContext Execute(PipelineContext pipelineContext) {

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
        }
        
        if (pipelineContext.CallbackQuery.Data[0] == 'l') {
            string message = pipelineContext.CallbackQuery.Data.Remove(0, 1);

            var user = pipelineContext.DataBaseService.GetUser(
                pipelineContext.CallbackQuery.Message.Chat.Id);
            
            Console.WriteLine(message);

            user.AddedText = string.Empty;
            pipelineContext.DataBaseService.UpdateUser(user);
            
        }

        pipelineContext.IsExecute = false;
        return pipelineContext;
    }
}