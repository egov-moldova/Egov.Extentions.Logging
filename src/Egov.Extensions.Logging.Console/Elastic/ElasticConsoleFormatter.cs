using System.Globalization;
using Egov.Extensions.Logging.Elastic;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Logging.Console;
using Microsoft.Extensions.Options;

namespace Egov.Extensions.Logging.Elastic;

/// <summary>
/// A <see cref="ConsoleFormatter"/> that outputs logs in Elastic-compatible JSON format.
/// </summary>
public sealed class ElasticConsoleFormatter : ConsoleFormatter, IDisposable
{
    /// <summary>
    /// The unique name for this formatter.
    /// </summary>
    public const string FormatterName = "elastic";

    private const string NullFormat = "[null]";
    private const string OriginalFormat = "{OriginalFormat}";

    private readonly IDisposable? _optionsReloadToken;
    private ElasticConsoleFormatterOptions _options = default!;
    private bool _useColors;

    /// <summary>
    /// Initializes a new instance of the <see cref="ElasticConsoleFormatter"/> class.
    /// </summary>
    /// <param name="options">The options monitor for this formatter.</param>
    public ElasticConsoleFormatter(IOptionsMonitor<ElasticConsoleFormatterOptions> options)
        : base(FormatterName)
    {
        ReloadLoggerOptions(options.CurrentValue);
        _optionsReloadToken = options.OnChange(ReloadLoggerOptions);
    }

    /// <inheritdoc/>
    public override void Write<TState>(in LogEntry<TState> logEntry, IExternalScopeProvider? scopeProvider, TextWriter textWriter)
    {
        var message = logEntry.Formatter.Invoke(logEntry.State, logEntry.Exception);

        var logLevel = logEntry.LogLevel;
        var logLevelColorSet = WriteLogLevelConsoleColor(textWriter, logLevel);

        textWriter.Write('{');

        var timestampFormat = _options.TimestampFormat ?? "O";
        var dateTimeOffset = _options.UseUtcTimestamp ? DateTimeOffset.UtcNow : DateTimeOffset.Now;
        textWriter.WriteFirstJsonPropertyUnescapedName("event_time", dateTimeOffset.ToString(timestampFormat));

        textWriter.WriteJsonPropertyUnescapedName("event_level", GetLogLevelString(logLevel));

        var eventId = logEntry.EventId;
        if (eventId.Id != default) textWriter.WriteJsonPropertyUnescapedName("event_id", eventId.Id);
        if (eventId.Name != null) textWriter.WriteJsonPropertyUnescapedName("event_type", eventId.Name);
        textWriter.WriteJsonPropertyUnescapedName("event_source", logEntry.Category);

        if (message != NullFormat)
        {
            textWriter.WriteJsonPropertyUnescapedName("event_message", message);
        }

        // exception, simple state and scopes go to details
        List<string>? details = null;

        void AddDetail(string? detail)
        {
            if (detail == null) return;
            details ??= new List<string>(4);
            details.Add(detail);
        }

        var exception = logEntry.Exception;
        if (exception != null)
        {
            details = new List<string>(4) { exception.ToString() };
        }

        HashSet<string>? seenProperties = null;

        void WritePropertyIfUnique(KeyValuePair<string, object> property)
        {
            seenProperties ??= new HashSet<string>(7, StringComparer.OrdinalIgnoreCase);
            if (!seenProperties.Add(property.Key)) return;
            textWriter.WriteJsonProperty(property.Key, property.Value);
        }

        // add state details, if enabled
        if (_options.IncludeState && (logEntry.State is IEnumerable<KeyValuePair<string, object>> stateProperties))
        {
            foreach (var stateProperty in stateProperties)
            {
                if (stateProperty.Key == OriginalFormat) continue;
                WritePropertyIfUnique(stateProperty);
            }
        }

        // add scopes details, if enabled
        if (_options.IncludeScopes && (scopeProvider != null))
        {
            scopeProvider.ForEachScope((scope, state) =>
            {
                if (scope is IEnumerable<KeyValuePair<string, object>> scopeProperties)
                {
                    foreach (var scopeProperty in scopeProperties)
                    {
                        if (scopeProperty.Key == OriginalFormat)
                        {
                            AddDetail(ToInvariantString(scope));
                            continue;
                        }
                        WritePropertyIfUnique(scopeProperty);
                    }
                }
                else if (scope != null)
                {
                    AddDetail(ToInvariantString(scope));
                }
            }, textWriter);
        }

        textWriter.WriteJsonProperty("event_details", details);

        textWriter.Write('}');
        if (logLevelColorSet) WriteResetConsoleColors(textWriter);
        textWriter.Write(Environment.NewLine);
    }

    private bool WriteLogLevelConsoleColor(TextWriter textWriter, LogLevel logLevel)
    {
        bool WriteColors(ConsoleColor foreground, ConsoleColor background)
        {
            textWriter.Write(ConsoleColors.GetForegroundColorEscapeCode(foreground));
            textWriter.Write(ConsoleColors.GetBackgroundColorEscapeCode(background));
            return true;
        }

        if (!_useColors) return false;

        return logLevel switch
        {
            LogLevel.Trace or LogLevel.Debug => WriteColors(ConsoleColor.Gray, ConsoleColor.Black),
            LogLevel.Information => WriteColors(ConsoleColor.Green, ConsoleColor.Black),
            LogLevel.Warning => WriteColors(ConsoleColor.Yellow, ConsoleColor.Black),
            LogLevel.Error => WriteColors(ConsoleColor.White, ConsoleColor.DarkRed),
            LogLevel.Critical => WriteColors(ConsoleColor.Yellow, ConsoleColor.DarkRed),
            _ => false,
        };
    }

    private void WriteResetConsoleColors(TextWriter textWriter)
    {
        if (_useColors)
        {
            textWriter.Write(ConsoleColors.DefaultForegroundColor);
            textWriter.Write(ConsoleColors.DefaultBackgroundColor);
        }
    }

    private static string GetLogLevelString(LogLevel logLevel)
    {
        return logLevel switch
        {
            LogLevel.Trace => "trace",
            LogLevel.Debug => "debug",
            LogLevel.Information => "info",
            LogLevel.Warning => "warning",
            LogLevel.Error => "error",
            LogLevel.Critical => "critical",
            _ => throw new ArgumentOutOfRangeException(nameof(logLevel))
        };
    }

    private static string? ToInvariantString(object obj) => Convert.ToString(obj, CultureInfo.InvariantCulture);

    private void ReloadLoggerOptions(ElasticConsoleFormatterOptions options)
    {
        _options = options;
        _useColors = (options.ColorBehavior == LoggerColorBehavior.Enabled) ||
                     (options.ColorBehavior == LoggerColorBehavior.Default && !Console.IsOutputRedirected);
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        _optionsReloadToken?.Dispose();
    }
}