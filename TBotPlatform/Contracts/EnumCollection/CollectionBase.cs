using System.Collections.Frozen;

namespace TBotPlatform.Contracts.EnumCollection;

public abstract class CollectionBase<T>
    where T : Enum
{
    protected abstract FrozenDictionary<T, string> DataCollection { get; }

    public string GetValueByKey(T key) => DataCollection.GetValueOrDefault(key);

    public T GetKeyByValue(string value) => DataCollection.FirstOrDefault(z => z.Value == value).Key;

    public string[] GetAllValue() => [.. DataCollection.Values,];
}