namespace Microsoft.Extensions.Logging;

/// <summary>
/// LoggerEvent extensions.
/// </summary>
public static class LoggerExtensions
{
    /// <summary>
    /// Helper method to create a <see cref="LoggerEvent"/> for a <see cref="ILogger"/>.
    /// </summary>
    /// <param name="logger">The <see cref="ILogger"/> to be logged to.</param>
    /// <returns>The<see cref="LoggerEvent"/> so that additional calls can be chained.</returns>
    public static LoggerEvent Event(this ILogger logger)
    {
        return new LoggerEvent(logger);
    }
}
