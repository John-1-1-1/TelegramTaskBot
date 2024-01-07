using Microsoft.EntityFrameworkCore;
using TaskBoardBot.TelegramWorker.Steps;

namespace TaskBoardBot.TelegramWorker;

public sealed class ApplicationContext : DbContext {
    public DbSet<User> Users { get; set; } = null!;
    public DbSet<Tasks> Tasks { get; set; } = null!;

    public ApplicationContext(DbContextOptions<DbContext> options):  base(options) {
        Database.EnsureCreated();
    }
}

public class User {
    public int Id { get; set; }
    public long TgId { get; set; }
    public DateTime LocalTime { get; set; }
    public TelegramStates UserState { get; set; }
    public string AddedText { get; set; } = string.Empty;
    public DateTime AddedTime { get; set; }
}

public class Tasks {
    public int Id { get; set; }
    public DateTime DateTime { get; set; }
    public string Text { get; set; } = string.Empty;
    public bool IsActive { get; set; } = false;
}