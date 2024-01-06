using Microsoft.EntityFrameworkCore;

namespace TaskBoardBot.TelegramWorker;

public sealed class ApplicationContext : DbContext {
    public DbSet<User> Users { get; set; } = null!;

    public ApplicationContext(DbContextOptions<DbContext> options) : base(options) {
        
    }
}

public class User {
    public int Id { get; set; }
    public string Name { get; set; }
    public int Age { get; set; }
}