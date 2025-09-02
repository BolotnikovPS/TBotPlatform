#nullable enable
using Microsoft.Extensions.DependencyInjection;
using TBotPlatform.Common.Builder;
using TBotPlatform.Contracts.Abstractions.Builder;

namespace TBotPlatform.Common.Dependencies;

public static partial class DependencyInjection
{
    public static IBotPlatformBuilder AddBotPlatform(this IServiceCollection services) => new BotPlatformBuilder(services);
}