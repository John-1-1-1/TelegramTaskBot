namespace TaskBoardBot.TelegramWorker.Context;

public class Tasks {
    public int Id { get; set; }
    public DateTime DateTime { get; set; }
    public string Text { get; set; } = string.Empty;
    public bool IsActive { get; set; } = false;
}