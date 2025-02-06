namespace TBotPlatform.Contracts.Abstractions.Factories.Proxies;

public interface IStateProxyFactory
{
    /// <summary>
    /// Делает запрос в <see cref="IStateFactory"/>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="botPrefixName">Префикс имя бота</param>
    /// <param name="request">Запрос</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<T> MakeRequest<T>(string botPrefixName, Func<IStateFactory, Task<T>> request, CancellationToken cancellationToken);

    /// <summary>
    /// Делает запрос в <see cref="IStateFactory"/>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="botPrefixName">Префикс имя бота</param>
    /// <param name="request">Запрос</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<T> MakeRequest<T>(string botPrefixName, Func<IStateFactory, T> request, CancellationToken cancellationToken);

    /// <summary>
    /// Делает запрос в <see cref="IStateBindFactory"/>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="botPrefixName">Префикс имя бота</param>
    /// <param name="request">Запрос</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<T> MakeRequest<T>(string botPrefixName, Func<IStateBindFactory, Task<T>> request, CancellationToken cancellationToken);

    /// <summary>
    /// Делает запрос в <see cref="IStateBindFactory"/>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="botPrefixName">Префикс имя бота</param>
    /// <param name="request">Запрос</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<T> MakeRequest<T>(string botPrefixName, Func<IStateBindFactory, T> request, CancellationToken cancellationToken);
}