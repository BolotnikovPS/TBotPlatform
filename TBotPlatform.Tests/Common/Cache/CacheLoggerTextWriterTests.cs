using Microsoft.Extensions.Logging;
using Moq;
using System.Text;
using TBotPlatform.Common.Cache;

namespace TBotPlatform.Tests.Common.Cache;

[TestFixture]
public class CacheLoggerTextWriterTests
{
    private Mock<ILogger> _logger = null!;
    private CacheLoggerTextWriter _writer = null!;

    [SetUp]
    public void SetUp()
    {
        _logger = new Mock<ILogger>();
        _writer = new CacheLoggerTextWriter(_logger.Object);
    }

    [TearDown]
    public void TearDown() => _writer?.Dispose();

    [Test]
    public void Encoding_ReturnsUtf8() => Assert.That(_writer.Encoding, Is.SameAs(Encoding.UTF8));

    [Test]
    public void WriteLine_WhenValueIsEmpty_DoesNotCallLog()
    {
        // CacheLoggerTextWriter only logs when value.CheckAny() is true (has content)
        _writer.WriteLine("");

        _logger.Verify(
            x => x.Log(
                It.IsAny<LogLevel>(),
                It.IsAny<EventId>(),
                It.IsAny<It.IsAnyType>(),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Never);
    }

    [Test]
    public void WriteLine_WhenValueIsWhitespace_DoesNotCallLog()
    {
        // Whitespace is considered empty by CheckAny()
        _writer.WriteLine("   ");

        _logger.Verify(
            x => x.Log(
                It.IsAny<LogLevel>(),
                It.IsAny<EventId>(),
                It.IsAny<It.IsAnyType>(),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Never);
    }

    [Test]
    public void WriteLine_WhenValueHasContent_CallsLogDebug()
    {
        // CacheLoggerTextWriter logs when value has content (CheckAny() returns true)
        _writer.WriteLine("some log message");

        _logger.Verify(
            x => x.Log(
                LogLevel.Debug,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString() == "some log message"),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Test]
    public void Write_WhenCharIsDefault_DoesNotCallLog()
    {
        // CacheLoggerTextWriter only logs when char.IsNotDefault() is true (not '\0')
        _writer.Write('\0');

        _logger.Verify(
            x => x.Log(
                It.IsAny<LogLevel>(),
                It.IsAny<EventId>(),
                It.IsAny<It.IsAnyType>(),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Never);
    }

    [Test]
    public void Write_WhenCharIsNotDefault_CallsLogDebug()
    {
        // CacheLoggerTextWriter logs when char is not default
        _writer.Write('a');

        _logger.Verify(
            x => x.Log(
                LogLevel.Debug,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString() == "a"),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }
}
