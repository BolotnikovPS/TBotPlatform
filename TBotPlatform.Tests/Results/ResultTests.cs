using TBotPlatform.Results;
using TBotPlatform.Results.Enums;

namespace TBotPlatform.Tests.Results;

[TestFixture]
public class ResultTests
{
    [Test]
    public void Success_ReturnsSuccessResult()
    {
        var result = Result.Success();

        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Error, Is.Not.Null);
        }
        Assert.That(result.Error!.ErrorType, Is.EqualTo(ErrorResultType.None));
    }

    [Test]
    public void Failure_ReturnsFailedResult()
    {
        var error = ErrorResult.Failure("Test error");
        var result = Result.Failure(error);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.Error, Is.SameAs(error));
        }
        Assert.That(result.Error!.Description, Is.EqualTo("Test error"));
    }

    [Test]
    public void ImplicitConversion_FromErrorResult_ReturnsFailedResult()
    {
        Result result = ErrorResult.Validation("Validation failed");

        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.Error!.ErrorType, Is.EqualTo(ErrorResultType.Validation));
        }
    }
}
