using System.Text;
using TBotPlatform.Contracts.Bots.Constant;
using TBotPlatform.Contracts.Bots.Markups.Enums;
using TBotPlatform.Extension;
using Telegram.Bot.Types.ReplyMarkups;

namespace TBotPlatform.Contracts.Bots.Markups.InlineMarkups;

public class InlineMarkupState(string buttonName, string state, string data = null)
    : InlineMarkupBase(buttonName, EInlineMarkupType.CallbackData)
{
    public InlineMarkupState(string buttonName, MarkupNextState markupNextState)
        : this(buttonName, markupNextState.State, markupNextState.Data)
    {
    }

    /// <summary>
    /// Данные на inline кнопке
    /// </summary>
    private string MarkupNextStateJson { get; } = new MarkupNextState(
        state,
        buttonName.In(PaginationsConstant.PreviousPage, PaginationsConstant.NextPage)
            ? $"{PaginationsConstant.PaginationIdentity}{data}"
            : data
        ).ToJson();

    public override InlineKeyboardButton Format()
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(ButtonName);
        ArgumentException.ThrowIfNullOrWhiteSpace(state);

        if (MarkupNextStateJson.IsNull())
        {
            return null;
        }

        return Encoding.Default.GetBytes(MarkupNextStateJson).Length > InlineMarkupConstant.MarkupNextStateJsonLength
            ? null
            : InlineKeyboardButton.WithCallbackData(ButtonName, MarkupNextStateJson);
    }

    public InlineMarkupState CreateDataWithDelimiter(params string[] datas) => new(ButtonName, state, string.Join(DelimiterConstant.DelimiterFirst, datas));
}