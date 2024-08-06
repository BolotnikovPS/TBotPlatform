namespace TBotPlatform.Contracts.Bots.UserBases;

public abstract class UserBase : UserMinimal
{
    public long ChatId { get; set; }

    public abstract bool IsAdmin();
}