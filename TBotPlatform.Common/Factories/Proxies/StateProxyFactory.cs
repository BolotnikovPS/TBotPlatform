using Microsoft.Extensions.DependencyInjection;
using TBotPlatform.Contracts.Abstractions.Factories;
using TBotPlatform.Contracts.Abstractions.Factories.Proxies;

namespace TBotPlatform.Common.Factories.Proxies;

internal class StateProxyFactory(IServiceScopeFactory serviceProvider) : IStateProxyFactory
{
    public async Task<T> MakeRequest<T>(string botPrefixName, Func<IStateFactory, Task<T>> request, CancellationToken cancellationToken)
    {
        await using var scope = serviceProvider.CreateAsyncScope();
        var stateFactory = scope.ServiceProvider.GetRequiredService<StateFactory>();

        stateFactory.SetCacheKeyPrefix(botPrefixName);

        return await request.Invoke(stateFactory);
    }

    public async Task<T> MakeRequest<T>(string botPrefixName, Func<IStateFactory, T> request, CancellationToken cancellationToken)
    {
        await using var scope = serviceProvider.CreateAsyncScope();
        var stateFactory = scope.ServiceProvider.GetRequiredService<StateFactory>();

        stateFactory.SetCacheKeyPrefix(botPrefixName);

        return request.Invoke(stateFactory);
    }

    public async Task<T> MakeRequest<T>(string botPrefixName, Func<IStateBindFactory, Task<T>> request, CancellationToken cancellationToken)
    {
        await using var scope = serviceProvider.CreateAsyncScope();
        var stateFactory = scope.ServiceProvider.GetRequiredService<StateFactory>();

        stateFactory.SetCacheKeyPrefix(botPrefixName);

        return await request.Invoke(stateFactory);
    }

    public async Task<T> MakeRequest<T>(string botPrefixName, Func<IStateBindFactory, T> request, CancellationToken cancellationToken)
    {
        await using var scope = serviceProvider.CreateAsyncScope();
        var stateFactory = scope.ServiceProvider.GetRequiredService<StateFactory>();

        stateFactory.SetCacheKeyPrefix(botPrefixName);

        return request.Invoke(stateFactory);
    }
}