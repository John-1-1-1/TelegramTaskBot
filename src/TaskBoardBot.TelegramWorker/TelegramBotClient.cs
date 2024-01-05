using Microsoft.EntityFrameworkCore.Infrastructure;
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

    public TelegramBotClient(ILogger<Worker> logger,IServiceProvider serviceProvider, IConfiguration configuration) {
        _configuration = configuration;
        _serviceProvider = serviceProvider;
        var token = _configuration.GetValue<String>("TelegramToken");

        if (token == null) {
            logger.LogError("TelegramToken is null");
            throw new ArgumentNullException(token);
        }
        
        _telegramBotClient = new Telegram.Bot.TelegramBotClient(token);
        _receiverOptions = new ReceiverOptions() {
            AllowedUpdates = new[] {
                UpdateType.Message
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
                    var message = update.Message;

                    var chat = message?.Chat;
                    var text = message?.Text;
                    if (chat == null || text == null) {
                        break;
                    }

                    botClient.SendTextMessageAsync(
                        chat, text);
                    
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