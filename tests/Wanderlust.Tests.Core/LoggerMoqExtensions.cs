using Microsoft.Extensions.Logging;
using Moq;

namespace Wanderlust.Tests.Core;

public static class LoggerMoqExtensions
{
    public static void Setup<T>(this Mock<ILogger<T>> logger)
    {
        logger.Setup(x =>
            x.Log(
                It.IsAny<LogLevel>(),
                It.IsAny<EventId>(),
                It.IsAny<object>(),
                It.IsAny<Exception>(),
                // todo: clarification
                It.Is<Func<object, Exception, string>>((v, t) => true))
        );
    }
    public static void Verify<T>(this Mock<ILogger<T>> logger, LogLevel level, Times times)
    {
        logger.Verify(
            x => x.Log(
                It.Is<LogLevel>(l => l.Equals(level)),
                It.IsAny<EventId>(),
                It.IsAny<object>(),
                It.IsAny<Exception>(),
                // todo: clarification
                It.Is<Func<object, Exception, string>>((v, t) => true)),
            times);
    }
}