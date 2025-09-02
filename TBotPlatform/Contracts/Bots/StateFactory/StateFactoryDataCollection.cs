using System.Collections;

namespace TBotPlatform.Contracts.Bots.StateFactory;

public class StateFactoryDataCollection(IList<StateFactoryData> list) : IReadOnlyCollection<StateFactoryData>
{
    public int Count => list.Count;

    public StateFactoryData this[int index] => list[index];

    public IEnumerator<StateFactoryData> GetEnumerator() => list.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}