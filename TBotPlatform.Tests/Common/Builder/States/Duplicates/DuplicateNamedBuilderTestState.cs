#nullable enable
using TBotPlatform.Contracts.Attributes;
using TBotPlatform.Tests.Common.Builder.States;

namespace TBotPlatform.Tests.Common.Builder.States.Duplicates;

/// <summary>
/// A state whose type name is already taken by another state in a neighboring namespace:
/// the builder must skip the duplicate by name.
/// </summary>
[StateInlineActivator(OnlyForBot = "duplicate-bot", TextsTypes = ["duplicate-name-text"])]
public sealed class DuplicateNamedBuilderTestState : BuilderTestStateBase
{
}
