﻿using TBotPlatform.Extension;

namespace TBotPlatform.Contracts.Bots.Users;

public abstract class UserBase
{
    public long ChatId { get; set; }

    public long TgUserId { get; set; }

    public string UserName { get; set; }

    public string FirstName { get; set; }

    public string LastName { get; set; }

    public abstract bool IsAdmin();

    public override string ToString()
        => $"{(UserName.IsNull() ? $"{FirstName}{LastName.Insert(startIndex: 0, " ")}" : $"@{UserName}")} ({TgUserId})";
}