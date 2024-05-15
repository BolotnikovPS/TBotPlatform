namespace TBotPlatform.Contracts.Bots;

public abstract class UserBase
{
    public long TgUserId { get; set; }

    public long ChatId { get; set; }

    public string UserName { get; set; }

    public string FirstName { get; set; }

    public string LastName { get; set; }

    public abstract bool IsAdmin();
}