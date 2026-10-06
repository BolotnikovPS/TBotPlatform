#nullable enable
using TBotPlatform.Contracts.Abstractions;
using TBotPlatform.Contracts.Abstractions.Contexts.AsyncDisposable;
using TBotPlatform.Contracts.Abstractions.State;
using TBotPlatform.Contracts.Attributes;
using TBotPlatform.Contracts.Bots.Buttons;
using TBotPlatform.Contracts.Bots.Constant;
using TBotPlatform.Contracts.Bots.Users;
using TBotPlatform.Results;
using TBotPlatform.Results.Abstractions;

namespace TBotPlatform.Tests.Common.Builder.States;

/// <summary>
/// Platform user for test states.
/// </summary>
public sealed class BuilderTestUser : UserBase
{
    public override bool IsAdmin() => false;
}

/// <summary>
/// Menu for test states.
/// </summary>
public sealed class BuilderTestMenuButton : IMenuButton
{
    public Task<IResult<MainButtonMassiveList>> GetMainButtons<T>(T user)
        where T : UserBase
        => Task.FromResult<IResult<MainButtonMassiveList>>(
            ResultT<MainButtonMassiveList>.Success([new MainButtonMassive { MainButtons = [new MainButton("Тест")] }]));
}

/// <summary>
/// Base state: the handler body is not used in tests; only type registration matters.
/// </summary>
public abstract class BuilderTestStateBase : IState<BuilderTestUser>
{
    public Task Handle(IStateContext context, BuilderTestUser user, CancellationToken cancellationToken) => Task.CompletedTask;

    public Task HandleComplete(IStateContext context, BuilderTestUser user, CancellationToken cancellationToken) => Task.CompletedTask;

    public Task HandleError(IStateContext context, BuilderTestUser user, Exception exception, CancellationToken cancellationToken) => Task.CompletedTask;
}

/// <summary>
/// Activator that sets an inline state and a menu at the same time (invalid combination).
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public sealed class InlineWithMenuTestActivatorAttribute()
    : StateActivatorBaseAttribute(isInlineState: true, menuType: typeof(BuilderTestMenuButton));

// --- Valid states participating in the assembly scan ---------------------------------------------

[StateInlineActivator(CommandsTypes = [CommandTypesConstant.StartCommand], TextsTypes = ["builder-start-text"])]
public sealed class StartBuilderTestState : BuilderTestStateBase
{
}

[StateInlineActivator(TextsTypes = ["builder-named-text"])]
public sealed class NamedBuilderTestState : BuilderTestStateBase
{
}

[StateActivator(typeof(BuilderTestMenuButton))]
public sealed class MenuBuilderTestState : BuilderTestStateBase
{
}

// --- Violator: has the attribute but does not implement IState<> (breaks the assembly scan) -------

[StateInlineActivator]
public sealed class AaaNotAStateBuilderTestState
{
}

// --- States available only to another bot ----------------------------------------------------------

[StateInlineActivator(OnlyForBot = "other-bot", TextsTypes = ["builder-other-text"])]
public sealed class OnlyForOtherBotBuilderTestState : BuilderTestStateBase
{
}

// --- Duplicates by ButtonsTypes / TextsTypes ------------------------------------------------------

[StateInlineActivator(OnlyForBot = "conflict-bot", ButtonsTypes = ["conflict-button"])]
public sealed class ConflictButtonsBuilderStateOne : BuilderTestStateBase
{
}

[StateInlineActivator(OnlyForBot = "conflict-bot", ButtonsTypes = ["conflict-button"])]
public sealed class ConflictButtonsBuilderStateTwo : BuilderTestStateBase
{
}

[StateInlineActivator(OnlyForBot = "conflict-bot", TextsTypes = ["conflict-text"])]
public sealed class ConflictTextsBuilderStateOne : BuilderTestStateBase
{
}

[StateInlineActivator(OnlyForBot = "conflict-bot", TextsTypes = ["conflict-text"])]
public sealed class ConflictTextsBuilderStateTwo : BuilderTestStateBase
{
}

// --- Duplicates by IsLockUserState / IsRegistrationState ------------------------------------------

[StateInlineActivator(OnlyForBot = "lock-bot", IsLockUserState = true)]
public sealed class LockBuilderStateOne : BuilderTestStateBase
{
}

[StateInlineActivator(OnlyForBot = "lock-bot", IsLockUserState = true)]
public sealed class LockBuilderStateTwo : BuilderTestStateBase
{
}

[StateInlineActivator(OnlyForBot = "registration-bot", IsRegistrationState = true)]
public sealed class RegistrationBuilderStateOne : BuilderTestStateBase
{
}

[StateInlineActivator(OnlyForBot = "registration-bot", IsRegistrationState = true)]
public sealed class RegistrationBuilderStateTwo : BuilderTestStateBase
{
}

// --- Duplicate /start commands ---------------------------------------------------------------------

[StateInlineActivator(OnlyForBot = "multi-start-bot", CommandsTypes = [CommandTypesConstant.StartCommand])]
public sealed class MultiStartBuilderStateOne : BuilderTestStateBase
{
}

[StateInlineActivator(OnlyForBot = "multi-start-bot", CommandsTypes = [CommandTypesConstant.StartCommand])]
public sealed class MultiStartBuilderStateTwo : BuilderTestStateBase
{
}

// --- Invalid attributes ----------------------------------------------------------------------------

[StateActivator(typeof(string), OnlyForBot = "bad-menu-bot")]
public sealed class BadMenuBuilderTestState : BuilderTestStateBase
{
}

[InlineWithMenuTestActivator(OnlyForBot = "inline-menu-bot")]
public sealed class InlineWithMenuBuilderTestState : BuilderTestStateBase
{
}

// --- Duplicates by state type name (see States/Duplicates) ------------------------------------------

[StateInlineActivator(OnlyForBot = "duplicate-bot", TextsTypes = ["duplicate-name-text"])]
public sealed class DuplicateNamedBuilderTestState : BuilderTestStateBase
{
}
