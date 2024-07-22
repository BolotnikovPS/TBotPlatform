using System.Text;
using TBotPlatform.Contracts.Bots.Constant;
using TBotPlatform.Extension;
using Telegram.Bot.Types.ReplyMarkups;

namespace TBotPlatform.Contracts.Bots.Markups.InlineMarkups;

public class InlineMarkupState(
    string buttonName,
    string state = null,
    string data = null
    ) : InlineMarkupBase(buttonName)
{
    /// <summary>
    /// Данные на inline кнопке
    /// </summary>
    private string MarkupNextStateJson { get; } = new MarkupNextState(
        state,
        buttonName.In(PaginationsConstant.PreviosPage, PaginationsConstant.NextPage)
            ? $"{PaginationsConstant.PaginationIdentity}{data}"
            : data
        ).ToJson();

    public override InlineKeyboardButton Format()
    {
        if (MarkupNextStateJson.IsNull())
        {
            return default;
        }

        return Encoding.Default.GetBytes(MarkupNextStateJson).Length > 64
            ? default
            : InlineKeyboardButton.WithCallbackData(ButtonName, MarkupNextStateJson);
    }

    public InlineMarkupState CreateDataWithDelimiter(params string[] datas)
    {
        return new(ButtonName, state, string.Join(DelimiterConstant.DelimiterFirst, datas));
    }
}