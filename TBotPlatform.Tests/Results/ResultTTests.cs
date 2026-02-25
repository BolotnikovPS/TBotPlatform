using TBotPlatform.Results;
using TBotPlatform.Results.Enums;

namespace TBotPlatform.Tests.Results;

[TestFixture]
public class ResultTTests
{
    [Test]
    public void Success_WithValue_ReturnsSuccessResult()
    {
        var value = 42;
        var result = ResultT<int>.Success(value);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Value, Is.EqualTo(42));
        }
    }

    [Test]
    public void Failure_ReturnsFailedResult_AndThrowsOnValueAccess()
    {
        var error = ErrorResult.NotFound("Not found");
        var result = ResultT<string>.Failure(error);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.Error, Is.SameAs(error));
            Assert.That(() => result.Value, Throws.InvalidOperationException);
        }
    }

    [Test]
    public void ImplicitConversion_FromValue_ReturnsSuccessResult()
    {
        ResultT<int> result = 100;

        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Value, Is.EqualTo(100));
        }
    }

    [Test]
    public void ImplicitConversion_FromErrorResult_ReturnsFailedResult()
    {
        ResultT<object> result = ErrorResult.Conflict("Conflict");

        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.Error!.ErrorType, Is.EqualTo(ErrorResultType.Conflict));
        }
    }
}
