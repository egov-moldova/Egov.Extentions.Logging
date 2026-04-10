using Microsoft.Extensions.Logging.Console;

namespace Egov.Extensions.Logging.Elastic;

/// <summary>
/// Options for Elastic console formatter.
/// </summary>
public class ElasticConsoleFormatterOptions: ConsoleFormatterOptions
{
    /// <summary>
    /// Includes state when <see langword="true" />.
    /// </summary>
    public bool IncludeState { get; set; }

    /// <summary>
    /// Determines when to use color when logging messages.
    /// </summary>
    public LoggerColorBehavior ColorBehavior { get; set; }
}