using Microsoft.EntityFrameworkCore;
using TaskBoardBot.TelegramWorker.Context.DbTables;

namespace TaskBoardBot.TelegramWorker.Context;

public sealed class ApplicationContext : DbContext {
    public DbSet<Users> Users { get; set; } = null!;
    public DbSet<Tasks> Tasks { get; set; } = null!;

    public ApplicationContext(DbContextOptions<DbContext> options):  base(options) {
        Database.EnsureCreated();
    }
}