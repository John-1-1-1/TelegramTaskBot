using TaskBoardBot.TelegramWorker.Context;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace TaskBoardBot.TelegramWorker.IntermittentPipeline;

public class PipelineContext(ITelegramBotClient telegramBotClient, Message? message,
    DataBaseService? dataBaseService, CallbackQuery? callbackQuery) {
    public bool IsExecute = true;
    public readonly ITelegramBotClient TelegramBotClient = telegramBotClient;
    public readonly Message? Message = message;
    public DataBaseService? DataBaseService = dataBaseService;
    public readonly CallbackQuery? CallbackQuery = callbackQuery;
}