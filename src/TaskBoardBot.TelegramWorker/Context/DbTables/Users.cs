using TaskBoardBot.TelegramWorker.PipelineComponents;

namespace TaskBoardBot.TelegramWorker.Context.DbTables;

public class Users {
    public int Id { get; set; }
    public long TgId { get; set; }
    public TimeSpan LocalTime { get; set; }
    public TelegramState UserState { get; set; }
    public string Times { get; set; } = string.Empty;
    public string AddedText { get; set; } = string.Empty;
}