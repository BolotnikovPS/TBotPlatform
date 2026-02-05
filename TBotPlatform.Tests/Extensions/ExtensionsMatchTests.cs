using TBotPlatform.Results;
using TBotPlatform.Results.Abstractions;

namespace TBotPlatform.Tests.Extensions;

[TestFixture]
public class ExtensionsMatchTests
{
    [Test]
    public void Match_IResult_Success_CallsOnSuccess()
    {
        IResult result = Result.Success();
        var value = result.Match(() => "ok", _ => "fail");
        Assert.That(value, Is.EqualTo("ok"));
    }

    [Test]
    public void Match_IResult_Failure_CallsOnFailure()
    {
        IResult result = Result.Failure(ErrorResult.Failure("err"));
        var value = result.Match(() => "ok", e => e.Description);
        Assert.That(value, Is.EqualTo("err"));
    }

    [Test]
    public void Match_IResultT_Success_CallsOnSuccessWithValue()
    {
        IResult<int> result = ResultT<int>.Success(42);
        var value = result.Match(v => v * 2, _ => 0);
        Assert.That(value, Is.EqualTo(84));
    }

    [Test]
    public void Match_IResultT_Failure_CallsOnFailure()
    {
        IResult<int> result = ResultT<int>.Failure(ErrorResult.NotFound("nope"));
        var value = result.Match(v => v, e => -1);
        Assert.That(value, Is.EqualTo(-1));
    }
}
