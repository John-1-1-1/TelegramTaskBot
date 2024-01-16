using TaskBoardBot.TelegramWorker.PipelineComponents.IntermittentPipeline;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace TaskBoardBot.TelegramWorker.TelegramBot;

public class TelegramBotClient {
    public readonly ITelegramBotClient TelegramClient;
    private readonly ReceiverOptions _receiverOptions;
    private readonly InterPipeline _pipeline;

    public TelegramBotClient(ILogger<TelegramBotClient> logger,
        IServiceProvider serviceProvider, IConfiguration configuration) {
        _pipeline = serviceProvider.GetService<InterPipeline>() ?? throw new Exception("Pipeline is empty");
        
        var token = configuration.GetValue<String>("TelegramToken");

        if (token == null) {
            logger.LogError("TelegramToken is null");
            throw new ArgumentNullException(token);
        }
        
        TelegramClient = new Telegram.Bot.TelegramBotClient(token);
        _receiverOptions = new ReceiverOptions() {
            AllowedUpdates = new[] {
                UpdateType.Message,
                UpdateType.CallbackQuery 
            }
        };
    }

    public void Start() {
        var cts = new CancellationTokenSource();
        TelegramClient.StartReceiving(UpdateHandler, ErrorHandler,
            _receiverOptions, cts.Token);
    }
        
    private Task UpdateHandler(ITelegramBotClient botClient,
        Update update, CancellationToken cancellationToken) {
        try {
            _pipeline.Execute(new PipelineContext(TelegramClient, update, update.Type));
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