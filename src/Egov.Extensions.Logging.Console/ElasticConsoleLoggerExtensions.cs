using Egov.Extensions.Logging.Elastic;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Dependency injection extensions.
/// </summary>
public static class ElasticConsoleLoggerExtensions
{
    /// <summary>
    /// Adds ElasticSearch compatible console logger with default options.
    /// </summary>
    /// <param name="builder">The <see cref="ILoggingBuilder"/> to use.</param>
    /// <returns>The<see cref="ILoggingBuilder"/> so that additional calls can be chained.</returns>
    public static ILoggingBuilder AddElasticConsole(this ILoggingBuilder builder)
        => builder.AddElasticConsole(_ => { });

    /// <summary>
    /// Adds ElasticSearch compatible console logger.
    /// </summary>
    /// <param name="builder">The <see cref="ILoggingBuilder"/> to use.</param>
    /// <param name="configure">A delegate to configure the console logger options for the elastic log formatter.</param>
    /// <returns>The<see cref="ILoggingBuilder"/> so that additional calls can be chained.</returns>
    public static ILoggingBuilder AddElasticConsole(this ILoggingBuilder builder, Action<ElasticConsoleFormatterOptions> configure)
    {
        builder.AddElasticConsoleFormatter(configure);

        return builder.AddConsole(options =>
        {
            options.FormatterName = ElasticConsoleFormatter.FormatterName;
        });
    }

    /// <summary>
    /// Adds ElasticSearch compatible console logger formatter with default options.
    /// </summary>
    /// <param name="builder">The <see cref="ILoggingBuilder"/> to use.</param>
    /// <returns>The<see cref="ILoggingBuilder"/> so that additional calls can be chained.</returns>
    public static ILoggingBuilder AddElasticConsoleFormatter(this ILoggingBuilder builder)
        => builder.AddElasticConsoleFormatter(_ => { });

    /// <summary>
    /// Adds ElasticSearch compatible console logger formatter.
    /// </summary>
    /// <param name="builder">The <see cref="ILoggingBuilder"/> to use.</param>
    /// <param name="configure">A delegate to configure the console logger options for the elastic log formatter.</param>
    /// <returns>The<see cref="ILoggingBuilder"/> so that additional calls can be chained.</returns>
    public static ILoggingBuilder AddElasticConsoleFormatter(this ILoggingBuilder builder, Action<ElasticConsoleFormatterOptions> configure)
        => builder.AddConsoleFormatter<ElasticConsoleFormatter, ElasticConsoleFormatterOptions>(configure);

    /// <summary>
    /// Use ElasticSearch compatible console formatter.
    /// </summary>
    /// <param name="builder">The <see cref="IHostBuilder"/> to use.</param>
    /// <param name="includingInDevelopment">If <c>true</c>, use Console JSON logging in Development environment also.</param>
    /// <returns>The<see cref="IHostBuilder"/> so that additional calls can be chained.</returns>
    public static IHostBuilder UseElasticConsoleLogging(this IHostBuilder builder, bool includingInDevelopment = false)
    {
        return builder.ConfigureLogging((context, loggingBuilder) =>
        {
            if (includingInDevelopment || !context.HostingEnvironment.IsDevelopment())
            {
                loggingBuilder.AddElasticConsole();
            }
        });
    }

    /// <summary>
    /// Use ElasticSearch compatible console formatter.
    /// </summary>
    /// <param name="builder">The <see cref="WebApplicationBuilder"/> to use.</param>
    /// <param name="includingInDevelopment">If <c>true</c>, use Console JSON logging in Development environment also.</param>
    public static void UseElasticConsoleLogging(this WebApplicationBuilder builder, bool includingInDevelopment = false)
    {
        if (includingInDevelopment || !builder.Environment.IsDevelopment())
        {
            builder.Logging.AddElasticConsole();
        }
    }
}