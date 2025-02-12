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
}