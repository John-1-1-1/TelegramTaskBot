using System.Text.Json;
using Hors;
using TaskBoardBot.TelegramWorker.Context;
using TaskBoardBot.TelegramWorker.Context.DbTables;
using TaskBoardBot.TelegramWorker.PipelineComponents.IntermittentPipeline;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace TaskBoardBot.TelegramWorker.PipelineComponents.PipelineSteps.NoneStep;

public class AddTaskStep: PipelineUnit {
    
    private readonly HorsTextParser _horsTextParser = new();
    public override PipelineContext UpdateMessage(PipelineContext pipelineContext, 
        Message message, Users? user) {

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
            
            var inlineKeyboard = new MarkupBuilder().AddDates(listDates, "t",
                user.LocalTime).GetChangeButton().GetInlineKeyboardMarkup();

            pipelineContext.TelegramBotClient.SendTextMessageAsync(
                message.Chat, parseTime.Text, replyMarkup: inlineKeyboard
            );
            
            user.AddedText = parseTime.Text;
            user.Times = JsonSerializer.Serialize(listDates);        
            user.UserState = TelegramState.None;
            pipelineContext.Parent.GetDbService.UpdateUser(user);
            
            pipelineContext.KillPipeline();
            
            return pipelineContext;
    }

    public override PipelineContext UpdateCallbackQuery(PipelineContext pipelineContext, 
        CallbackQuery callbackQuery, Users? user) {
        
        if (user == null) {
            return pipelineContext;
        }
        
        pipelineContext.TelegramBotClient.
            AnswerCallbackQueryAsync(callbackQuery.Id);

        if (callbackQuery.Data == "changeDate") {
            user.UserState = TelegramState.ChangeDate;
            pipelineContext.TelegramBotClient.SendTextMessageAsync(
                callbackQuery.From.Id, "Введите новую дату!"
            );
            pipelineContext.Parent.GetDbService.UpdateUser(user);
        }
        
        if (callbackQuery.Data == "changeText") {
            user.UserState = TelegramState.ChangeMessage;
            pipelineContext.TelegramBotClient.SendTextMessageAsync(
                callbackQuery.From.Id, "Введите новый текст!"
            );
            pipelineContext.Parent.GetDbService.UpdateUser(user);
        }
        
        if (callbackQuery.Data != null && callbackQuery.Data[0] == 't') {
            string message = callbackQuery.Data.Remove(0, 1);
            
            pipelineContext.Parent.GetDbService.AddTasks(new Tasks() {
                DateTime = DateTime.FromFileTime(long.Parse(message)),
                TgId = user.TgId, Text = user.AddedText
            });

            pipelineContext.TelegramBotClient.SendTextMessageAsync(user.TgId, "Задача успешно добавлена");

            user.AddedText = string.Empty;
            user.Times = string.Empty; 
            pipelineContext.Parent.GetDbService.UpdateUser(user);
        }
        
        pipelineContext.KillPipeline();
        return pipelineContext;
    }
}