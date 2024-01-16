using TaskBoardBot.TelegramWorker.PipelineComponents;
using TaskBoardBot.TelegramWorker.PipelineComponents.PipelineSteps;

namespace TaskBoardBot.TelegramWorker.Context;

public class Users {
    public int Id { get; set; }
    public long TgId { get; set; }
    public int? LocalTime { get; set; }
    public TelegramState UserState { get; set; }
    public string AddedText { get; set; } = string.Empty;
}