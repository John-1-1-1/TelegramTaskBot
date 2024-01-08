using Microsoft.EntityFrameworkCore;

namespace TaskBoardBot.TelegramWorker.Context;

public sealed class ApplicationContext : DbContext {
    public DbSet<Users> Users { get; set; } = null!;
    public DbSet<Tasks> Tasks { get; set; } = null!;

    public ApplicationContext(DbContextOptions<DbContext> options):  base(options) {
        Database.EnsureCreated();
    }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Tasks>().HasData(
            new Tasks() {Id = 1,  DateTime = DateTime.Now.Date.ToUniversalTime(), 
                Text = "Text", TgId = 123});
    }
}