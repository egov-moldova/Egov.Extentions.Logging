using System.Collections;
using System.Security.Claims;

namespace Microsoft.Extensions.Logging;

/// <summary>
/// Represents a structured logger event.
/// </summary>
public class LoggerEvent : ILogger
{
    /// <summary>
    /// The <see cref="ILogger"/> to be logged to.
    /// </summary>
    protected readonly ILogger Logger;

    /// <summary>
    /// A list of structured properties to be logged.
    /// </summary>
    protected readonly List<KeyValuePair<string, object>> Properties = new(8);

    /// <summary>
    /// Constructs a structured logger event.
    /// </summary>
    /// <param name="logger">The <see cref="ILogger"/> to be logged to.</param>
    public LoggerEvent(ILogger logger)
    {
        Logger = logger;
    }

    /// <summary>
    /// Adds an arbitrary property to logger event.
    /// </summary>
    /// <param name="name">The name of the property to be added.</param>
    /// <param name="value">
    /// <para>The value of the property to be added.</para>
    /// <para>A special serialization treatment is applied for values of type:</para>
    /// <para>- <see cref="IEnumerable{T}"/> of <see cref="KeyValuePair{TKey, TValue}"/> are logged as nested objects.</para>
    /// <para>- <see cref="IList"/> and <see cref="IEnumerable"/> are logged as arrays when containing more than one value.</para>
    /// </param>
    /// <returns>The<see cref="LoggerEvent"/> so that additional calls can be chained.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="name"/> is null or empty.</exception>
    public LoggerEvent Property(string name, object value)
    {
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentOutOfRangeException(nameof(name));
        Properties.Add(new KeyValuePair<string, object>(name, value));
        return this;
    }

    /// <summary>
    /// Adds "event_correlation" property to logger event.
    /// </summary>
    /// <param name="correlation">The value of event correlation.</param>
    /// <returns>The<see cref="LoggerEvent"/> so that additional calls can be chained.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="correlation"/> is empty.</exception>
    public LoggerEvent Correlation(object correlation)
    {
        ArgumentNullException.ThrowIfNull(correlation);
        Properties.Add(new KeyValuePair<string, object>("event_correlation", correlation));
        return this;
    }

    /// <summary>
    /// Adds "legal_entity" property to logger event, if non-empty.
    /// </summary>
    /// <param name="entity">The value of legal entity.</param>
    /// <returns>The<see cref="LoggerEvent"/> so that additional calls can be chained.</returns>
    public LoggerEvent LegalEntity(string? entity)
    {
        if (!string.IsNullOrWhiteSpace(entity))
        {
            Properties.Add(new KeyValuePair<string, object>("legal_entity", entity));
        }
        return this;
    }

    /// <summary>
    /// Adds "legal_basis" property to logger event, if non-empty.
    /// </summary>
    /// <param name="basis">The value of legal basis.</param>
    /// <returns>The<see cref="LoggerEvent"/> so that additional calls can be chained.</returns>
    public LoggerEvent LegalBasis(string? basis)
    {
        if (!string.IsNullOrWhiteSpace(basis))
        {
            Properties.Add(new KeyValuePair<string, object>("legal_basis", basis));
        }
        return this;
    }

    /// <summary>
    /// Adds "legal_reason" and "legal_basis" properties to logger event, if non-empty.
    /// </summary>
    /// <param name="reason">The value of legal reason.</param>
    /// <param name="basis">The value of legal basis.</param>
    /// <returns>The<see cref="LoggerEvent"/> so that additional calls can be chained.</returns>
    public LoggerEvent LegalReason(string? reason, string? basis = null)
    {
        if (!string.IsNullOrWhiteSpace(reason))
        {
            Properties.Add(new KeyValuePair<string, object>("legal_reason", reason));
        }
        return LegalBasis(basis);
    }

    /// <summary>
    /// Adds "user" property to logger event, if non-empty.
    /// </summary>
    /// <param name="user">The value of user.</param>
    /// <returns>The<see cref="LoggerEvent"/> so that additional calls can be chained.</returns>
    public LoggerEvent User(string? user)
    {
        if (!string.IsNullOrWhiteSpace(user))
        {
            Properties.Add(new KeyValuePair<string, object>("user", user));
        }
        return this;
    }

    /// <summary>
    /// Adds "user" property to logger event, if authenticated.
    /// </summary>
    /// <param name="principal">The <see cref="ClaimsPrincipal"/> to check for the authenticated user.</param>
    /// <returns>The<see cref="LoggerEvent"/> so that additional calls can be chained.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="principal"/> is null.</exception>
    public LoggerEvent User(ClaimsPrincipal principal)
    {
        ArgumentNullException.ThrowIfNull(principal);
        if (principal.Identity?.IsAuthenticated ?? false)
        {
            return User(principal.Identity.Name);
        }
        return this;
    }

    /// <summary>
    /// Adds "user_session" property to logger event, if non-empty.
    /// </summary>
    /// <param name="session">The value of session.</param>
    /// <returns>The<see cref="LoggerEvent"/> so that additional calls can be chained.</returns>
    public LoggerEvent UserSession(string? session)
    {
        if (!string.IsNullOrWhiteSpace(session))
        {
            Properties.Add(new KeyValuePair<string, object>("user_session", session));
        }
        return this;
    }

    /// <summary>
    /// Adds "user_address" property to logger event, if non-empty.
    /// </summary>
    /// <param name="address">The value of address.</param>
    /// <returns>The<see cref="LoggerEvent"/> so that additional calls can be chained.</returns>
    public LoggerEvent UserAddress(string? address)
    {
        if (!string.IsNullOrWhiteSpace(address))
        {
            Properties.Add(new KeyValuePair<string, object>("user_address", address));
        }
        return this;
    }

    /// <summary>
    /// Adds "subject", "subject_type" and "subject_name" properties to logger event, if non-empty.
    /// </summary>
    /// <param name="subject">The value of subject.</param>
    /// <param name="subjectType">The value of subject type.</param>
    /// <param name="subjectName">The value of subject name.</param>
    /// <returns>The<see cref="LoggerEvent"/> so that additional calls can be chained.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="subject"/> is null.</exception>
    public LoggerEvent Subject(object subject, object? subjectType = null, string? subjectName = null)
    {
        ArgumentNullException.ThrowIfNull(subject);
        Properties.Add(new KeyValuePair<string, object>("subject", subject));

        if (subjectType != null)
        {
            Properties.Add(new KeyValuePair<string, object>("subject_type", subjectType));
        }
        if (!string.IsNullOrWhiteSpace(subjectName))
        {
            Properties.Add(new KeyValuePair<string, object>("subject_name", subjectName));
        }

        return this;
    }

    /// <summary>
    /// Adds "object", "object_type" and "object_name" properties to logger event, if non-empty.
    /// </summary>
    /// <param name="object">The value of object.</param>
    /// <param name="objectType">The value of object type.</param>
    /// <param name="objectName">The value of object name.</param>
    /// <returns>The<see cref="LoggerEvent"/> so that additional calls can be chained.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="object"/> is null.</exception>
    public LoggerEvent Object(object @object, object? objectType = null, string? objectName = null)
    {
        ArgumentNullException.ThrowIfNull(@object);
        Properties.Add(new KeyValuePair<string, object>("object", @object));

        if (objectType != null)
        {
            Properties.Add(new KeyValuePair<string, object>("object_type", objectType));
        }
        if (!string.IsNullOrWhiteSpace(objectName))
        {
            Properties.Add(new KeyValuePair<string, object>("object_name", objectName));
        }

        return this;
    }

    /// <inheritdoc/>
    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
    {
        if (Properties.Count == 0)
        {
            Logger.Log(logLevel, eventId, state, exception, formatter);
            return;
        }

        using (Logger.BeginScope(Properties))
        {
            Logger.Log(logLevel, eventId, state, exception, formatter);
        }
    }

    /// <inheritdoc/>
    public bool IsEnabled(LogLevel logLevel)
    {
        return Logger.IsEnabled(logLevel);
    }

    /// <inheritdoc/>
    public IDisposable? BeginScope<TState>(TState state) where TState : notnull
    {
        return Logger.BeginScope(state);
    }
}
