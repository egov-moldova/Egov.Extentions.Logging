using Egov.Extensions.Logging.Elastic;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace Egov.Extensions.Logging.Tests;

public class ElasticConsoleFormatterTests
{
    private readonly ElasticConsoleFormatter _formatter;

    public ElasticConsoleFormatterTests()
    {
        var optionsMonitor = new Mock<IOptionsMonitor<ElasticConsoleFormatterOptions>>();
        optionsMonitor.Setup(x => x.CurrentValue).Returns(new ElasticConsoleFormatterOptions
        {
            IncludeScopes = true,
            IncludeState = true,
            TimestampFormat = "yyyy-MM-dd HH:mm:ss",
            UseUtcTimestamp = true
        });

        _formatter = new ElasticConsoleFormatter(optionsMonitor.Object);
    }

    [Fact]
    public void Write_SimpleLog_ShouldOutputCorrectJson()
    {
        // Arrange
        var sw = new StringWriter();
        var logEntry = new LogEntry<string>(
            LogLevel.Information,
            "TestCategory",
            new EventId(1, "TestEvent"),
            "Test Message",
            null,
            (state, ex) => state);

        // Act
        _formatter.Write(logEntry, null, sw);
        var output = sw.ToString();

        // Assert
        var json = JsonDocument.Parse(output);
        var root = json.RootElement;

        Assert.Equal("info", root.GetProperty("event_level").GetString());
        Assert.Equal("TestCategory", root.GetProperty("event_source").GetString());
        Assert.Equal("Test Message", root.GetProperty("event_message").GetString());
        Assert.Equal(1, root.GetProperty("event_id").GetInt32());
        Assert.Equal("TestEvent", root.GetProperty("event_type").GetString());
        Assert.True(root.TryGetProperty("event_time", out _));
    }

    [Fact]
    public void Write_WithState_ShouldIncludeProperties()
    {
        // Arrange
        var sw = new StringWriter();
        var state = new List<KeyValuePair<string, object>>
        {
            new("CustomProp", "Value1"),
            new("{OriginalFormat}", "Test {CustomProp}")
        };
        var logEntry = new LogEntry<List<KeyValuePair<string, object>>>(
            LogLevel.Warning,
            "TestCategory",
            default,
            state,
            null,
            (s, ex) => "Formatted Message");

        // Act
        _formatter.Write(logEntry, null, sw);
        var output = sw.ToString();

        // Assert
        var json = JsonDocument.Parse(output);
        var root = json.RootElement;

        Assert.Equal("warning", root.GetProperty("event_level").GetString());
        Assert.Equal("Value1", root.GetProperty("CustomProp").GetString());
    }

    [Fact]
    public void Write_WithScope_ShouldIncludeProperties()
    {
        // Arrange
        var sw = new StringWriter();
        var scopeProvider = new LoggerExternalScopeProvider();
        using (scopeProvider.Push(new List<KeyValuePair<string, object>> { new("ScopeProp", "ScopeValue") }))
        {
            var logEntry = new LogEntry<string>(
                LogLevel.Error,
                "TestCategory",
                default,
                "Message",
                null,
                (s, ex) => s);

            // Act
            _formatter.Write(logEntry, scopeProvider, sw);
        }
        var output = sw.ToString();

        // Assert
        var json = JsonDocument.Parse(output);
        var root = json.RootElement;

        Assert.Equal("error", root.GetProperty("event_level").GetString());
        Assert.Equal("ScopeValue", root.GetProperty("ScopeProp").GetString());
    }
}
