using TBotPlatform.Results;
using TBotPlatform.Results.Enums;

namespace TBotPlatform.Tests.Results;

[TestFixture]
public class ErrorResultTests
{
    [Test]
    public void None_ReturnsEmptyErrorWithNoneType()
    {
        var error = ErrorResult.None();

        Assert.Multiple(() =>
        {
            Assert.That(error.Description, Is.EqualTo(""));
            Assert.That(error.ErrorType, Is.EqualTo(ErrorResultType.None));
        });
    }

    [Test]
    public void Failure_CreatesErrorWithCorrectType()
    {
        var error = ErrorResult.Failure("Something failed");

        Assert.Multiple(() =>
        {
            Assert.That(error.Description, Is.EqualTo("Something failed"));
            Assert.That(error.ErrorType, Is.EqualTo(ErrorResultType.Failure));
        });
    }

    [Test]
    public void NotFound_CreatesErrorWithCorrectType()
    {
        var error = ErrorResult.NotFound("Resource missing");

        Assert.Multiple(() =>
        {
            Assert.That(error.Description, Is.EqualTo("Resource missing"));
            Assert.That(error.ErrorType, Is.EqualTo(ErrorResultType.NotFound));
        });
    }

    [Test]
    public void Validation_CreatesErrorWithCorrectType()
    {
        var error = ErrorResult.Validation("Invalid input");

        Assert.Multiple(() =>
        {
            Assert.That(error.Description, Is.EqualTo("Invalid input"));
            Assert.That(error.ErrorType, Is.EqualTo(ErrorResultType.Validation));
        });
    }

    [Test]
    public void Conflict_CreatesErrorWithCorrectType()
    {
        var error = ErrorResult.Conflict("Duplicate entry");

        Assert.Multiple(() =>
        {
            Assert.That(error.Description, Is.EqualTo("Duplicate entry"));
            Assert.That(error.ErrorType, Is.EqualTo(ErrorResultType.Conflict));
        });
    }

    [Test]
    public void AccessUnAuthorized_CreatesErrorWithCorrectType()
    {
        var error = ErrorResult.AccessUnAuthorized("Not authenticated");

        Assert.Multiple(() =>
        {
            Assert.That(error.Description, Is.EqualTo("Not authenticated"));
            Assert.That(error.ErrorType, Is.EqualTo(ErrorResultType.AccessUnAuthorized));
        });
    }

    [Test]
    public void AccessForbidden_CreatesErrorWithCorrectType()
    {
        var error = ErrorResult.AccessForbidden("No permission");

        Assert.Multiple(() =>
        {
            Assert.That(error.Description, Is.EqualTo("No permission"));
            Assert.That(error.ErrorType, Is.EqualTo(ErrorResultType.AccessForbidden));
        });
    }
}
