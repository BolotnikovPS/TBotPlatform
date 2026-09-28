#nullable enable
using TBotPlatform.Contracts.Attributes;
using TBotPlatform.Tests.Common.Builder.States;

namespace TBotPlatform.Tests.Common.Builder.States.Duplicates;

/// <summary>
/// Состояние с именем типа, уже занятым другим состоянием в соседнем пространстве имён:
/// сборщик обязан пропустить дубль по имени.
/// </summary>
[StateInlineActivator(OnlyForBot = "duplicate-bot", TextsTypes = ["duplicate-name-text"])]
public sealed class DuplicateNamedBuilderTestState : BuilderTestStateBase
{
}
