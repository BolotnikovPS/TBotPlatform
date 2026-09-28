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
/// Пользователь платформы для тестовых состояний.
/// </summary>
public sealed class BuilderTestUser : UserBase
{
    public override bool IsAdmin() => false;
}

/// <summary>
/// Меню для тестовых состояний.
/// </summary>
public sealed class BuilderTestMenuButton : IMenuButton
{
    public Task<IResult<MainButtonMassiveList>> GetMainButtons<T>(T user)
        where T : UserBase
        => Task.FromResult<IResult<MainButtonMassiveList>>(
            ResultT<MainButtonMassiveList>.Success([new MainButtonMassive { MainButtons = [new MainButton("Тест")] }]));
}

/// <summary>
/// Базовое состояние: тело обработки в тестах не используется, важна лишь регистрация типов.
/// </summary>
public abstract class BuilderTestStateBase : IState<BuilderTestUser>
{
    public Task Handle(IStateContext context, BuilderTestUser user, CancellationToken cancellationToken) => Task.CompletedTask;

    public Task HandleComplete(IStateContext context, BuilderTestUser user, CancellationToken cancellationToken) => Task.CompletedTask;

    public Task HandleError(IStateContext context, BuilderTestUser user, Exception exception, CancellationToken cancellationToken) => Task.CompletedTask;
}

/// <summary>
/// Активатор с одновременным указанием inline-состояния и меню (недопустимая комбинация).
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public sealed class InlineWithMenuTestActivatorAttribute()
    : StateActivatorBaseAttribute(isInlineState: true, menuType: typeof(BuilderTestMenuButton));

// --- Валидные состояния, участвующие в сканировании сборки -------------------------------------

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

// --- Нарушитель: атрибут есть, а IState<> не реализован (ломает сканирование сборки) -------------

[StateInlineActivator]
public sealed class AaaNotAStateBuilderTestState
{
}

// --- Состояния, доступные только другому боту ---------------------------------------------------

[StateInlineActivator(OnlyForBot = "other-bot", TextsTypes = ["builder-other-text"])]
public sealed class OnlyForOtherBotBuilderTestState : BuilderTestStateBase
{
}

// --- Дубли по ButtonsTypes / TextsTypes ---------------------------------------------------------

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

// --- Дубли по IsLockUserState / IsRegistrationState ---------------------------------------------

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

// --- Дубли команды /start -----------------------------------------------------------------------

[StateInlineActivator(OnlyForBot = "multi-start-bot", CommandsTypes = [CommandTypesConstant.StartCommand])]
public sealed class MultiStartBuilderStateOne : BuilderTestStateBase
{
}

[StateInlineActivator(OnlyForBot = "multi-start-bot", CommandsTypes = [CommandTypesConstant.StartCommand])]
public sealed class MultiStartBuilderStateTwo : BuilderTestStateBase
{
}

// --- Некорректные атрибуты ----------------------------------------------------------------------

[StateActivator(typeof(string), OnlyForBot = "bad-menu-bot")]
public sealed class BadMenuBuilderTestState : BuilderTestStateBase
{
}

[InlineWithMenuTestActivator(OnlyForBot = "inline-menu-bot")]
public sealed class InlineWithMenuBuilderTestState : BuilderTestStateBase
{
}

// --- Дубли по имени типа состояния (см. States/Duplicates) --------------------------------------

[StateInlineActivator(OnlyForBot = "duplicate-bot", TextsTypes = ["duplicate-name-text"])]
public sealed class DuplicateNamedBuilderTestState : BuilderTestStateBase
{
}
