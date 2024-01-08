using TaskBoardBot.TelegramWorker.Steps;
using Telegram.Bot.Types;

namespace TaskBoardBot.TelegramWorker.Context;

public class DataBaseService {

    private ApplicationContext _applicationContext;
    private readonly ILogger<DataBaseService> _logger;
    
    public DataBaseService(ILogger<DataBaseService> logger, IServiceProvider serviceProvider) {
        _logger = logger;
        var scope = serviceProvider.CreateScope();
        var applicationContext = scope.ServiceProvider.GetService<ApplicationContext>();

        if (applicationContext != null) {
            _applicationContext = applicationContext;
        }
        else {
            _logger.LogError(typeof(ApplicationContext) + " is null");
            throw new Exception(typeof(ApplicationContext) + " is null");
        }
    }

    public void AddUser(long chatId, TelegramState state) {
        try {

            var user = _applicationContext.Users.FirstOrDefault(u => u.TgId == chatId);

            if (user == null) {
                _applicationContext.Users.Add(new Users() {
                    TgId = chatId, UserState = state
                });
                _applicationContext.SaveChanges();
            }
        }
        catch {
            _logger.LogError("AddUser: ApplicationContext incorrect");
        }
    }

    public void UpdateUser(Users user) { 
        try { 
            _applicationContext.Users.Update(user); 
            _applicationContext.SaveChanges();
        } catch { 
            _logger.LogError("UpdateUser: ApplicationContext incorrect"); 
        }
    }


    public Users? GetUser(long tgId) {
        try {
            return _applicationContext.Users.FirstOrDefault(u => u.TgId == tgId);
        } catch { 
            _logger.LogError("GetUser: ApplicationContext incorrect"); 
            return null;
        }
    }

    public ICollection<Tasks> GetTasksCollection(long tgId) {
        try {
            return _applicationContext.Tasks.Where(t => t.TgId == tgId).ToList();
        } catch {
            _logger.LogError("GetTasksCollection: ApplicationContext incorrect");
            return new List<Tasks>();
        }
    }

    public void AddTasks(Tasks tasks) {
        try { 
            _applicationContext.Tasks.Add(tasks); 
            _applicationContext.SaveChanges();
        } catch { 
            _logger.LogError("AddTasks: ApplicationContext incorrect"); 
        }
    }
    
}