using TaskBoardBot.TelegramWorker.Context;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace TaskBoardBot.TelegramWorker.IntermittentPipeline;

public class PipelineContext(ITelegramBotClient telegramBotClient, Message? message,
    DataBaseService dataBaseService, CallbackQuery? callbackQuery, UpdateType type) {
    public bool IsExecute = true;
    public readonly ITelegramBotClient TelegramBotClient = telegramBotClient;
    public readonly Message? Message = message;
    public DataBaseService DataBaseService = dataBaseService;
    public readonly CallbackQuery? CallbackQuery = callbackQuery;
    public UpdateType Type = type;

    public Message GetMessage() {
        if (message == null) {
            throw new Exception();
        }

        return message;
    }
    
    public CallbackQuery GetCallbackQuery() {
        if (callbackQuery == null) {
            throw new Exception();
        }

        return callbackQuery;
    }
}