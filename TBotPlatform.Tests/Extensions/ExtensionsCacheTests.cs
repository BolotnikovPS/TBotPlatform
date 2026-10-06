using Microsoft.Extensions.DependencyInjection;
using TBotPlatform.Contracts.Abstractions.Cache;
using TBotPlatform.Results.Enums;
using ZiggyCreatures.Caching.Fusion;

namespace TBotPlatform.Tests.Extensions;

/// <summary>
/// Tests the <see cref="Extensions"/> methods on <see cref="IFusionCache"/>:
/// single values, tagged collections, and handling of missing data.
/// </summary>
[TestFixture]
public class ExtensionsCacheTests
{
    private ServiceProvider _provider = null!;

    private IFusionCache FusionCache => _provider.GetRequiredService<IFusionCache>();

    [SetUp]
    public void SetUp()
    {
        var services = new ServiceCollection();
        services.AddFusionCache();
        _provider = services.BuildServiceProvider();
    }

    [TearDown]
    public async Task TearDown() => await _provider.DisposeAsync();

    [Test]
    public async Task SetValue_ThenGetValue_ReturnsStoredValue()
    {
        var value = new TestCacheValue { Key = "single-1", Value = "hello" };

        var setResult = await FusionCache.SetValue(value);
        var getResult = await FusionCache.GetValue<TestCacheValue>(value.Key);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(setResult.IsSuccess, Is.True);
            Assert.That(getResult.IsSuccess, Is.True);
            Assert.That(getResult.Value.Value, Is.EqualTo("hello"));
        }
    }

    [Test]
    public async Task SetValue_WithExpiry_KeepsValueAvailable()
    {
        var value = new TestCacheValue { Key = "single-2", Value = "with-expiry" };

        var setResult = await FusionCache.SetValue(value, TimeSpan.FromMinutes(1));
        var getResult = await FusionCache.GetValue<TestCacheValue>(value.Key);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(setResult.IsSuccess, Is.True);
            Assert.That(getResult.Value.Value, Is.EqualTo("with-expiry"));
        }
    }

    [Test]
    public async Task GetValue_WhenKeyIsMissing_ReturnsNotFoundFailure()
    {
        var result = await FusionCache.GetValue<TestCacheValue>("missing-key");

        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.Error, Is.Not.Null);
            Assert.That(result.Error!.ErrorType, Is.EqualTo(ErrorResultType.NotFound));
        }
    }

    [Test]
    public async Task RemoveValue_ThenGetValue_ReturnsNotFound()
    {
        var value = new TestCacheValue { Key = "single-3", Value = "to-remove" };
        await FusionCache.SetValue(value);

        var removeResult = await FusionCache.RemoveValue(value.Key);
        var getResult = await FusionCache.GetValue<TestCacheValue>(value.Key);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(removeResult.IsSuccess, Is.True);
            Assert.That(getResult.IsSuccess, Is.False);
        }
    }

    [Test]
    public async Task KeyExists_WhenMissingKey_ReturnsFalse()
    {
        var result = await FusionCache.KeyExists("missing-key-exists");

        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Value, Is.False);
        }
    }

    [Test]
    public async Task KeyExists_AfterSetValue_ReturnsTrue()
    {
        var value = new TestCacheValue { Key = "single-4", Value = "exists" };
        await FusionCache.SetValue(value);

        var result = await FusionCache.KeyExists(value.Key);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Value, Is.True);
        }
    }

    [Test]
    public async Task AddValueToCollection_ThenGetValueFromCollection_ReturnsValue()
    {
        var value = new TestCacheValue { Key = "item-1", Value = "in-collection" };

        var addResult = await FusionCache.AddValueToCollection("users", value);
        var getResult = await FusionCache.GetValueFromCollection<TestCacheValue>("users", value.Key);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(addResult.IsSuccess, Is.True);
            Assert.That(getResult.IsSuccess, Is.True);
            Assert.That(getResult.Value.Value, Is.EqualTo("in-collection"));
        }
    }

    [Test]
    public async Task GetValueFromCollection_WhenItemIsMissing_ReturnsNotFoundFailure()
    {
        var result = await FusionCache.GetValueFromCollection<TestCacheValue>("users", "absent");

        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.Error!.ErrorType, Is.EqualTo(ErrorResultType.NotFound));
        }
    }

    [Test]
    public async Task RemoveValueFromCollection_RemovesOnlyThatItem()
    {
        var first = new TestCacheValue { Key = "item-2", Value = "first" };
        var second = new TestCacheValue { Key = "item-3", Value = "second" };
        await FusionCache.AddValueToCollection("users", first);
        await FusionCache.AddValueToCollection("users", second);

        var removeResult = await FusionCache.RemoveValueFromCollection("users", first.Key);
        var removed = await FusionCache.GetValueFromCollection<TestCacheValue>("users", first.Key);
        var kept = await FusionCache.GetValueFromCollection<TestCacheValue>("users", second.Key);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(removeResult.IsSuccess, Is.True);
            Assert.That(removed.IsSuccess, Is.False);
            Assert.That(kept.IsSuccess, Is.True);
            Assert.That(kept.Value.Value, Is.EqualTo("second"));
        }
    }

    [Test]
    public async Task RemoveCollection_RemovesEveryTaggedValue()
    {
        var first = new TestCacheValue { Key = "item-4", Value = "first" };
        var second = new TestCacheValue { Key = "item-5", Value = "second" };
        await FusionCache.AddValueToCollection("chats", first);
        await FusionCache.AddValueToCollection("chats", second);

        var removeResult = await FusionCache.RemoveCollection("chats");
        var afterFirst = await FusionCache.GetValueFromCollection<TestCacheValue>("chats", first.Key);
        var afterSecond = await FusionCache.GetValueFromCollection<TestCacheValue>("chats", second.Key);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(removeResult.IsSuccess, Is.True);
            Assert.That(afterFirst.IsSuccess, Is.False);
            Assert.That(afterSecond.IsSuccess, Is.False);
        }
    }

    [Test]
    public async Task Collections_WithSameItemKey_AreIsolated()
    {
        await FusionCache.AddValueToCollection("first-collection", new TestCacheValue { Key = "shared", Value = "one" });
        await FusionCache.AddValueToCollection("second-collection", new TestCacheValue { Key = "shared", Value = "two" });

        var fromFirst = await FusionCache.GetValueFromCollection<TestCacheValue>("first-collection", "shared");
        var fromSecond = await FusionCache.GetValueFromCollection<TestCacheValue>("second-collection", "shared");

        using (Assert.EnterMultipleScope())
        {
            Assert.That(fromFirst.Value.Value, Is.EqualTo("one"));
            Assert.That(fromSecond.Value.Value, Is.EqualTo("two"));
        }
    }

    private sealed class TestCacheValue : IKeyInCache
    {
        public string Key { get; init; } = string.Empty;

        public string Value { get; init; } = string.Empty;
    }
}
