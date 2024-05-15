namespace TBotPlatform.Contracts.Bots.Buttons;

public class ButtonsRule(
    string button
    )
{
    /// <summary>
    /// Тип кнопки
    /// </summary>
    public string Button { get; set; } = button;
}