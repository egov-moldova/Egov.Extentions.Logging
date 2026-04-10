using System.Security.Claims;


namespace Egov.Extensions.Logging.Tests;

public class LoggerEventTests
{
    private readonly Mock<ILogger> _mockLogger;
    private readonly LoggerEvent _loggerEvent;

    public LoggerEventTests()
    {
        _mockLogger = new Mock<ILogger>();
        _loggerEvent = new LoggerEvent(_mockLogger.Object);
    }

    [Fact]
    public void Event_ExtensionMethod_ShouldReturnLoggerEvent()
    {
        // Act
        var result = _mockLogger.Object.Event();

        // Assert
        Assert.IsType<LoggerEvent>(result);
    }

    [Fact]
    public void Property_ShouldAddProperty()
    {
        // Act
        _loggerEvent.Property("test_key", "test_value");
        _loggerEvent.Log(LogLevel.Information, "Test Message");

        // Assert
        _mockLogger.Verify(x => x.BeginScope(It.Is<IEnumerable<KeyValuePair<string, object>>>(s => 
            s.Any(kvp => kvp.Key == "test_key" && (string)kvp.Value == "test_value"))), Times.Once);
    }

    [Fact]
    public void Correlation_ShouldAddCorrelationProperty()
    {
        // Act
        _loggerEvent.Correlation("corr-123");
        _loggerEvent.Log(LogLevel.Information, "Test Message");

        // Assert
        _mockLogger.Verify(x => x.BeginScope(It.Is<IEnumerable<KeyValuePair<string, object>>>(s => 
            s.Any(kvp => kvp.Key == "event_correlation" && (string)kvp.Value == "corr-123"))), Times.Once);
    }

    [Fact]
    public void User_String_ShouldAddUserProperty()
    {
        // Act
        _loggerEvent.User("test_user");
        _loggerEvent.Log(LogLevel.Information, "Test Message");

        // Assert
        _mockLogger.Verify(x => x.BeginScope(It.Is<IEnumerable<KeyValuePair<string, object>>>(s => 
            s.Any(kvp => kvp.Key == "user" && (string)kvp.Value == "test_user"))), Times.Once);
    }

    [Fact]
    public void User_ClaimsPrincipal_ShouldAddUserPropertyIfAuthenticated()
    {
        // Arrange
        var claims = new[] { new Claim(ClaimTypes.Name, "authenticated_user") };
        var identity = new ClaimsIdentity(claims, "TestAuthType");
        var principal = new ClaimsPrincipal(identity);

        // Act
        _loggerEvent.User(principal);
        _loggerEvent.Log(LogLevel.Information, "Test Message");

        // Assert
        _mockLogger.Verify(x => x.BeginScope(It.Is<IEnumerable<KeyValuePair<string, object>>>(s => 
            s.Any(kvp => kvp.Key == "user" && (string)kvp.Value == "authenticated_user"))), Times.Once);
    }

    [Fact]
    public void LegalEntity_ShouldAddLegalEntityProperty()
    {
        // Act
        _loggerEvent.LegalEntity("entity-1");
        _loggerEvent.Log(LogLevel.Information, "Test Message");

        // Assert
        _mockLogger.Verify(x => x.BeginScope(It.Is<IEnumerable<KeyValuePair<string, object>>>(s => 
            s.Any(kvp => kvp.Key == "legal_entity" && (string)kvp.Value == "entity-1"))), Times.Once);
    }

    [Fact]
    public void Subject_ShouldAddSubjectProperties()
    {
        // Act
        _loggerEvent.Subject("subj-id", "subj-type", "subj-name");
        _loggerEvent.Log(LogLevel.Information, "Test Message");

        // Assert
        _mockLogger.Verify(x => x.BeginScope(It.Is<IEnumerable<KeyValuePair<string, object>>>(s => 
            s.Any(kvp => kvp.Key == "subject" && (string)kvp.Value == "subj-id") &&
            s.Any(kvp => kvp.Key == "subject_type" && (string)kvp.Value == "subj-type") &&
            s.Any(kvp => kvp.Key == "subject_name" && (string)kvp.Value == "subj-name"))), Times.Once);
    }

    [Fact]
    public void Object_ShouldAddObjectProperties()
    {
        // Act
        _loggerEvent.Object("obj-id", "obj-type", "obj-name");
        _loggerEvent.Log(LogLevel.Information, "Test Message");

        // Assert
        _mockLogger.Verify(x => x.BeginScope(It.Is<IEnumerable<KeyValuePair<string, object>>>(s => 
            s.Any(kvp => kvp.Key == "object" && (string)kvp.Value == "obj-id") &&
            s.Any(kvp => kvp.Key == "object_type" && (string)kvp.Value == "obj-type") &&
            s.Any(kvp => kvp.Key == "object_name" && (string)kvp.Value == "obj-name"))), Times.Once);
    }

    [Fact]
    public void Log_NoProperties_ShouldNotBeginScope()
    {
        // Act
        _loggerEvent.Log(LogLevel.Information, "Test Message");

        // Assert
        _mockLogger.Verify(x => x.BeginScope(It.IsAny<object>()), Times.Never);
        _mockLogger.Verify(x => x.Log(LogLevel.Information, It.IsAny<EventId>(), It.IsAny<It.IsAnyType>(), It.IsAny<Exception>(), It.IsAny<Func<It.IsAnyType, Exception?, string>>()), Times.Once);
    }
}
