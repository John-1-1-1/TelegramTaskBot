using TaskBoardBot.TelegramWorker.IntermittentPipeline;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace TaskBoardBot.TelegramWorker.Steps;

public class TelegramCommands: PipelineUnit {
    public override PipelineContext Execute(PipelineContext pipelineContext) {
        pipelineContext.IsExecute = false;
        
        var chat = pipelineContext.Message?.Chat;
        var text = pipelineContext.Message?.Text;
        if (chat == null || text == null) {
            return pipelineContext;
        }
        
        switch (text) {
            case "/start": {
                pipelineContext.TelegramBotClient.SendTextMessageAsync(
                    chat, text);
                break;
            }
        }

        pipelineContext.IsExecute = true;
        return pipelineContext;
    }
}