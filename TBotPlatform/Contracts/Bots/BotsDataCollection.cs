using System.Collections;

namespace TBotPlatform.Contracts.Bots;

public class BotsDataCollection(IList<string> list) : IReadOnlyCollection<string>
{
    public int Count => list.Count;

    public string this[int index] => list[index];

    public IEnumerator<string> GetEnumerator() => list.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}