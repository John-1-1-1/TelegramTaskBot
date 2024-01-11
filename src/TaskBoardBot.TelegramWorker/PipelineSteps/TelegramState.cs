namespace TaskBoardBot.TelegramWorker.PipelineSteps;

public enum TelegramState {
    None,
    ChangeLocalTime,
    GetDate,
    GetMessage
}