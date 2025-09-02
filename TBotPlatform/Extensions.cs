using TBotPlatform.Contracts.Abstractions.Contexts.AsyncDisposable;

namespace TBotPlatform.Common;

public static partial class Extensions
{
    /// <summary>
    /// Устанавливает значение необходимости обновления основных кнопок
    /// </summary>
    /// <param name="context">Контекст для обработки сообщения</param>
    public static void SetNeedUpdateMarkup(this IStateContext context) => context.StateResult.IsNeedUpdateMarkup = true;

    /// <summary>
    /// Устанавливает имя следующего состояния
    /// </summary>
    /// <param name="context">Контекст для обработки сообщения</param>
    /// <param name="nextStateName">Имя состояния</param>
    public static void SetNextStateName(this IStateContext context, string nextStateName) => context.StateResult.NextStateName = nextStateName;

    /// <summary>
    /// Устанавливает возвращаемые данные
    /// </summary>
    /// <param name="context">Контекст для обработки сообщения</param>
    /// <param name="data">Данные</param>
    public static void SetData(this IStateContext context, string data) => context.StateResult.Data = data;

    /// <summary>
    /// Делит строку на массив строк по длине
    /// </summary>
    /// <param name="str">Строка</param>
    /// <param name="maxLength">Максимальная длина строки</param>
    /// <returns></returns>
    public static IEnumerable<string> SplitByLength(this string str, int maxLength)
    {
        for (var index = 0; index < str.Length; index += maxLength)
        {
            yield return str.Substring(index, Math.Min(maxLength, str.Length - index));
        }
    }
}