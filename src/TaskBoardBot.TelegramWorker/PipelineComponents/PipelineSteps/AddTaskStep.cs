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

            if (user == null || user.UserState != TelegramState.None) {
                return pipelineContext;
            }
        
            if (user.UserState == TelegramState.None) {

                var parseTime = _horsTextParser.Parse(message.Text, DateTime.Now);
                var buttons = new List<InlineKeyboardButton[]>();

                user.AddedText = parseTime.Text;
                
                List<DateTime> listDates = new List<DateTime>();
                int count = 0;
                string textMessage = parseTime.Text;
                foreach (var date in parseTime.Dates) {
                    count++;
                    listDates.Add(date.DateTo);
                    buttons.Add(new InlineKeyboardButton[] {
                        InlineKeyboardButton.WithCallbackData(date.DateTo.ToString(CultureInfo.InvariantCulture),
                            "t" + date.DateTo.ToFileTime())
                    });
                    if (count > 5) {
                        break;
                    }
                }

                if (count == 0) {
                    pipelineContext.TelegramBotClient.SendTextMessageAsync(
                        message.Chat, "Дата не распознана!"
                    );
                    return pipelineContext;
                }
                
                user.Times = JsonSerializer.Serialize(listDates);
                pipelineContext.Parent.GetDbService.UpdateUser(user);
                
                buttons.Add(new InlineKeyboardButton[] {
                    InlineKeyboardButton.WithCallbackData("Изменить дату", "changeDate"),
                    InlineKeyboardButton.WithCallbackData("Изменить текст", "changeText"),
                });
                var inlineKeyboard = new InlineKeyboardMarkup(buttons);


                pipelineContext.TelegramBotClient.SendTextMessageAsync(
                    message.Chat, textMessage, replyMarkup: inlineKeyboard
                );
                pipelineContext.IsExecute = false;
            }
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