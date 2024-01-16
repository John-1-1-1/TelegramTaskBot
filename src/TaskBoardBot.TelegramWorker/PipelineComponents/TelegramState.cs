namespace TaskBoardBot.TelegramWorker.PipelineComponents;

public enum TelegramState {
    None,
    ChangeLocalTime,
    GetDate,
    GetMessage
}