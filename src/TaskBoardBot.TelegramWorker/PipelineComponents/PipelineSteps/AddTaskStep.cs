using Hors;
using Telegram.Bot;
using System.Text.Json;
using Telegram.Bot.Types;
using System.Globalization;
using Telegram.Bot.Types.ReplyMarkups;
using TaskBoardBot.TelegramWorker.Context;
using TaskBoardBot.TelegramWorker.Context.DbTables;
using TaskBoardBot.TelegramWorker.PipelineComponents.IntermittentPipeline;

namespace TaskBoardBot.TelegramWorker.PipelineComponents.PipelineSteps;

public class AddTaskStep: PipelineUnit {
    
    private readonly HorsTextParser _horsTextParser = new();
    public override PipelineContext UpdateMessage(PipelineContext pipelineContext, 
        Message message, Users? user) {

            if (user == null) {
                return pipelineContext;
            }
            
            var parseTime = _horsTextParser.Parse(message.Text, DateTime.Now);
                
            List<DateTime> listDates = parseTime.Dates.Select(d => d.DateTo).ToList();


            if (listDates.Count == 0) {
                pipelineContext.TelegramBotClient.SendTextMessageAsync(
                    message.Chat, "Дата не распознана!"
                );
                return pipelineContext;
            }
                
            user.AddedText = parseTime.Text;
            user.Times = JsonSerializer.Serialize(listDates);
            pipelineContext.Parent.GetDbService.UpdateUser(user);

            var inlineKeyboard = new InlineButtonsBuilder()
                .AddDates(listDates).AddChangeButtons().GetInlineKeyboardMarkup();
            
            pipelineContext.TelegramBotClient.SendTextMessageAsync(
                message.Chat, parseTime.Text, replyMarkup: inlineKeyboard);
            
            pipelineContext.IsExecute = false;
            
            return pipelineContext;
    }

    public override bool IsTrueState(TelegramState? state) {
        return state == TelegramState.None;
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
            pipelineContext.Parent.GetDbService.UpdateUser(user);
        }
        
        if (callbackQuery.Data == "changeText") {
            user.UserState = TelegramState.ChangeMessage;
            pipelineContext.Parent.GetDbService.UpdateUser(user);
        }
        
        if (callbackQuery.Data != null && callbackQuery.Data[0] == 't') {
            string message = callbackQuery.Data.Remove(0, 1);

            int? localTime = user.LocalTime;
            if (localTime != null) {

                pipelineContext.Parent.GetDbService.AddTasks(new Tasks() {
                    DateTime = DateTime.FromFileTime(long.Parse(message)).AddHours(localTime.Value),
                    TgId = user.TgId, Text = user.AddedText
                });
            }

            user.AddedText = string.Empty;
            pipelineContext.Parent.GetDbService.UpdateUser(user);
        }
        
        pipelineContext.KillPipeline();
        return pipelineContext;
    }
}