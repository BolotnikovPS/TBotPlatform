using TBotPlatform.Contracts.Abstractions;
using TBotPlatform.Contracts.Bots.Buttons;
using TBotPlatform.Contracts.Bots.Users;
using TBotPlatform.Results;
using TBotPlatform.Results.Abstractions;

namespace TBotPlatform.Samples.EchoBot;

internal sealed class MainMenu : IMenuButton
{
    public Task<IResult<MainButtonMassiveList>> GetMainButtons<T>(T user)
        where T : UserBase
        => Task.FromResult<IResult<MainButtonMassiveList>>(
            ResultT<MainButtonMassiveList>.Success(
                [
                    new MainButtonMassive
                    {
                        MainButtons = [new MainButton("Ping"), new MainButton("Инфо")],
                    },
                ]));
}
