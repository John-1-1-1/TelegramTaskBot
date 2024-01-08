using TaskBoardBot.TelegramWorker.Context;
using TaskBoardBot.TelegramWorker.IntermittentPipeline;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace TaskBoardBot.TelegramWorker;

public class TelegramBotClient {
    private ITelegramBotClient _telegramBotClient;
    private ReceiverOptions _receiverOptions;
    private readonly IConfiguration _configuration;
    private readonly IServiceProvider _serviceProvider;
    private readonly InterPipeline _pipeline;

    public TelegramBotClient(ILogger<TelegramBotClient> logger,IServiceProvider serviceProvider, IConfiguration configuration) {
        _configuration = configuration;
        _serviceProvider = serviceProvider;
        _pipeline = _serviceProvider.GetService<InterPipeline>();
        
        var token = _configuration.GetValue<String>("TelegramToken");

        if (token == null) {
            logger.LogError("TelegramToken is null");
            throw new ArgumentNullException(token);
        }
        
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
        var dataBaseService = _serviceProvider.CreateScope().
            ServiceProvider.GetService<DataBaseService>();
        
        try {
            switch (update.Type) {
                case UpdateType.Message: {
                    _pipeline.Execute(new PipelineContext(_telegramBotClient, update.Message, 
                        dataBaseService, update.CallbackQuery));
                    break;
                }
                case UpdateType.CallbackQuery: {
                    _pipeline.Execute(new PipelineContext(_telegramBotClient, update.Message,
                        dataBaseService, update.CallbackQuery));
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