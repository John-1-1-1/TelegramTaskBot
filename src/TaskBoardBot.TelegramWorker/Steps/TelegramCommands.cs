using TaskBoardBot.TelegramWorker.Context;
using TaskBoardBot.TelegramWorker.IntermittentPipeline;
using Telegram.Bot;

namespace TaskBoardBot.TelegramWorker.Steps;

public class TelegramCommands: PipelineUnit {
    public override PipelineContext Execute(PipelineContext pipelineContext) {
        pipelineContext.IsExecute = false;
        
        var chat = pipelineContext.Message?.Chat;
        var text = pipelineContext.Message?.Text;
        
        var scope = pipelineContext.ServiceProvider.CreateScope();
        
        var r = scope.ServiceProvider.GetService(typeof(ApplicationContext)) as ApplicationContext;
        
        switch (text) {
            case "/start": {
                
                r?.Users.Add(new Users() {TgId = chat!.Id, UserState = TelegramStates.None } );
                r.SaveChanges();
                
                pipelineContext.TelegramBotClient.SendTextMessageAsync(
                    chat, text);
                return pipelineContext;
            }

            case "/changeTime": {
                var user = r.Users.FirstOrDefault(u => u.TgId == chat.Id);
                user.UserState = TelegramStates.ChangeLocalTime;
                r?.Users.Update(user);
                r.SaveChanges();
                
                pipelineContext.TelegramBotClient.SendTextMessageAsync(
                    chat, "Введите ваше время.\n\n"+
                          "Пример:\n"+
                          "11 вечера\n"+
                          "23:00"); 
                 
                return pipelineContext;
            }
        }

        pipelineContext.IsExecute = true;
        return pipelineContext;
    }
}

