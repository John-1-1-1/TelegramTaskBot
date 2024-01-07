using TaskBoardBot.TelegramWorker.Steps;

namespace TaskBoardBot.TelegramWorker.Context;

public class Users {
    public int Id { get; set; }
    public long TgId { get; set; }
    public DateTime LocalTime { get; set; }
    public TelegramStates UserState { get; set; }
    public string AddedText { get; set; } = string.Empty;
    public DateTime AddedTime { get; set; }
}