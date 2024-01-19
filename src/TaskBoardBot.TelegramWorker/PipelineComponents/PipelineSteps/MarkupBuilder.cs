using System.Globalization;
using Telegram.Bot.Types.ReplyMarkups;

namespace TaskBoardBot.TelegramWorker.PipelineComponents.PipelineSteps;

public class MarkupBuilder {
    List<InlineKeyboardButton[]> _buttons =
        new List<InlineKeyboardButton[]>();

    public MarkupBuilder AddDates(List<DateTime> dateTimes, string flag) {
        return AddDates(dateTimes, flag, TimeSpan.Zero);
    }

    public MarkupBuilder AddDates(List<DateTime> dateTimes, string flag, TimeSpan timeSpan) {
        foreach (var date in dateTimes.Take(5)) {
            _buttons.Add(new InlineKeyboardButton[] {
                InlineKeyboardButton.WithCallbackData(date.ToString(CultureInfo.InvariantCulture),
                    flag + date.Add(-timeSpan).ToFileTime())
            });
        }
        return this;
    }

    public MarkupBuilder GetChangeButton() {
        _buttons.Add(new [] {
            InlineKeyboardButton.WithCallbackData("Изменить дату", "changeDate"),
            InlineKeyboardButton.WithCallbackData("Изменить текст", "changeText"),
        });
        return this;
    }

    public InlineKeyboardMarkup GetInlineKeyboardMarkup() {
        return new InlineKeyboardMarkup(_buttons);
    }
}