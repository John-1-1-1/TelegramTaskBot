namespace TaskBoardBot.TelegramWorker.PipelineComponents;

public enum TelegramState {
    Null,
    None,
    ChangeLocalTime,
    ChangeDate,
    ChangeMessage
}