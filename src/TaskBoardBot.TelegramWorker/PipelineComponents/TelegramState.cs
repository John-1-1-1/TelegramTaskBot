namespace TaskBoardBot.TelegramWorker.PipelineComponents;

public enum TelegramState {
    None,
    ChangeLocalTime,
    ChangeDate,
    ChangeMessage
}