using TaskBoardBot.TelegramWorker.PipelineSteps;
using TaskBoardBot.TelegramWorker.PipelineSteps.MessagesSteps;

namespace TaskBoardBot.TelegramWorker.Context;

public class Users {
    public int Id { get; set; }
    public long TgId { get; set; }
    public DateTime LocalTime { get; set; }
    public TelegramState UserState { get; set; }
    public string AddedText { get; set; } = string.Empty;
}