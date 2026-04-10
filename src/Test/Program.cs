using System.Security.Claims;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

var host = Host.CreateDefaultBuilder(args)
    .UseElasticConsoleLogging(true);

var app = host.Build();

var logger = app.Services.GetRequiredService<ILogger<Program>>();

// testing null message
logger.LogInformation(null);

// testing LoggerEvent
logger.Event().User("test").LogInformation("User test");
logger.Event().User(new ClaimsPrincipal(new ClaimsIdentity([new Claim("NameIdentifier", "test")], "MPass", "NameIdentifier", null))).LogInformation("User test");

// testing nested object
logger.Event().Property("nested", new[]
{
    KeyValuePair.Create<string, object>("property1", "value1")
}).LogInformation("Nested object with one property");

logger.Event().Property("nested", new[]
{
    KeyValuePair.Create<string, object>("property1", "value1"), 
    KeyValuePair.Create<string, object>("property2", "value2")
}).LogInformation("Nested object with two properties");

logger.Event().Property("nested", new[]
{
    KeyValuePair.Create<string, object>("property1", "value1"),
    KeyValuePair.Create<string, object>("property2", "value2"),
    KeyValuePair.Create<string, object>("property3", "value3")
}).LogInformation("Nested object with three properties");

// testing list
logger.Event().Subject(new[] { "subject1", "subject2" }, "Human").LogInformation("Multiple subjects");

// testing enumerable
logger.Event().Property("enumerable", new HashSet<string> { "item1" }).LogInformation("Enumerable with one value");
logger.Event().Property("enumerable", new HashSet<string> { "item1", "item2" }).LogInformation("Enumerable with two values");
logger.Event().Property("enumerable", new HashSet<string> { "item1", "item2", "item3" }).LogInformation("Enumerable with three value");

// testing scopes
using (logger.BeginScope("Calling {user}", "user1"))
using (logger.BeginScope("Called trace: {trace}", Guid.NewGuid()))
{
    logger.LogInformation("User logged in");
}

// testing trace
logger.Event().LogTrace("Trace");

// testing warning
logger.Event().LogWarning("Warning");

// testing error
logger.Event().LogError("Error");

// testing critical
logger.Event().LogCritical("Critical");

// testing array
logger.Event().Object(new [] { Guid.NewGuid(), Guid.NewGuid() }).LogInformation("Objects");