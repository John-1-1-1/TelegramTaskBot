using System.Globalization;
using Telegram.Bot.Types.ReplyMarkups;

namespace TaskBoardBot.TelegramWorker.PipelineComponents;

public class InlineButtonsBuilder {
    private List<InlineKeyboardButton[]> _buttons = new();

    public InlineButtonsBuilder AddDates(List<DateTime> dateTimes) {
        dateTimes = dateTimes.Take(5).ToList();
        foreach (var dateTime in dateTimes) {
            _buttons.Add([
                InlineKeyboardButton.WithCallbackData(
                    dateTime.ToString(CultureInfo.InvariantCulture),
                    "t" + dateTime.ToFileTime())
            ]);
        }

        return this;
    }

    public InlineButtonsBuilder AddChangeButtons() {
        _buttons.Add(new InlineKeyboardButton[] {
            InlineKeyboardButton.WithCallbackData("Изменить дату", "changeDate"),
            InlineKeyboardButton.WithCallbackData("Изменить текст", "changeText"),
        });
        return this;
    }

    public InlineKeyboardMarkup GetInlineKeyboardMarkup() {
        return new InlineKeyboardMarkup(_buttons);
    }
    
}