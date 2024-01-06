using System.Globalization;
using Hors;
using Microsoft.EntityFrameworkCore.Infrastructure;
using TaskBoardBot.TelegramWorker.IntermittentPipeline;
using TaskBoardBot.TelegramWorker.Steps;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace TaskBoardBot.TelegramWorker;

public class TelegramBotClient {

    private ITelegramBotClient _telegramBotClient;
    private ReceiverOptions _receiverOptions;
    private readonly IConfiguration _configuration;
    private readonly IServiceProvider _serviceProvider;
    private readonly InterPipeline _pipeline = new InterPipeline();

    public TelegramBotClient(ILogger<Worker> logger,IServiceProvider serviceProvider, IConfiguration configuration) {
        _configuration = configuration;
        _serviceProvider = serviceProvider;
        var token = _configuration.GetValue<String>("TelegramToken");

        if (token == null) {
            logger.LogError("TelegramToken is null");
            throw new ArgumentNullException(token);
        }
                    
        _pipeline.Add(new TelegramMessageValidator());
        _pipeline.Add(new TelegramCommands());
        _pipeline.Add(new TelegramTextMessages());
        
        _telegramBotClient = new Telegram.Bot.TelegramBotClient(token);
        _receiverOptions = new ReceiverOptions() {
            AllowedUpdates = new[] {
                UpdateType.Message,
                UpdateType.CallbackQuery 
            }
        };
    }

    public void Satrt() {
        var cts = new CancellationTokenSource();
        _telegramBotClient.StartReceiving(UpdateHandler, ErrorHandler,
            _receiverOptions, cts.Token);
    }
        
    private Task UpdateHandler(ITelegramBotClient botClient,
        Update update, CancellationToken cancellationToken) {

        //var scope = _serviceProvider.CreateScope();
        
        //var r = scope.ServiceProvider.GetService(typeof(ApplicationContext)) as ApplicationContext;
        
        try {
            switch (update.Type) {
                case UpdateType.Message: {
                    _pipeline.Execute(new PipelineContext(_telegramBotClient, update.Message));
                    break;
                }
            }
        }
        catch {
            // ignored
        }


        return Task.CompletedTask;
    }

    private Task ErrorHandler(ITelegramBotClient botClient, Exception error, CancellationToken cancellationToken) {
        
        return Task.CompletedTask;
    }
}