# Egov.Extensions.Logging

[![NuGet Version](https://img.shields.io/nuget/v/Egov.Extensions.Logging.svg)](https://www.nuget.org/packages/Egov.Extensions.Logging)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](LICENSE)

A collection of .NET logging extensions for structured and Elastic-friendly logging. This library provides a fluent API for building complex log events and a custom console formatter that outputs logs in JSON format compatible with Elastic Common Schema (ECS), designed for modern cloud-native and microservices architectures.

---

## Table of Contents

- [Features](#features)
- [Prerequisites](#prerequisites)
- [Installation](#installation)
- [Packages](#packages)
- [Configuration](#configuration)
- [Usage](#usage)
  - [Using LoggerEvent for Structured Logging](#using-loggerevent-for-structured-logging)
  - [Elastic Console Output Example](#elastic-console-output-example)
- [Testing](#testing)
- [Contributing](#contributing)
- [Code of Conduct](#code-of-conduct)
- [AI Assistance](#ai-assistance)
- [License](#license)

---

## Features

- **Structured Logging**: A powerful `LoggerEvent` class that simplifies adding metadata to logs.
- **Elastic Console Formatter**: Custom `ConsoleFormatter` producing single-line JSON logs (ECS-like).
- **Fluent API**: Easily chain properties, correlation IDs, user information, and legal context to your logs.
- **Performance Optimized**: Minimal allocations and efficient JSON generation for high-throughput environments.
- **Modern .NET**: Built for .NET 10+ leveraging the latest runtime improvements.
- **Easy Integration**: Simple extension methods for `IServiceCollection`, `IHostBuilder`, and `WebApplicationBuilder`.

---

## Prerequisites

- .NET 10.0 or later
- ASP.NET Core environment (optional, for middleware and web app integration)

---

## Installation

Install the packages via NuGet:

```bash
dotnet add package Egov.Extensions.Logging
dotnet add package Egov.Extensions.Logging.Console
```

---

## Packages

- **Egov.Extensions.Logging**: Core library containing the `LoggerEvent` helper and `ILogger` extensions.
- **Egov.Extensions.Logging.Console**: Custom console formatter for Elastic-compatible JSON output.

---

## Configuration

To use the Elastic console formatter, register it in your **Program.cs**:

```csharp
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

// Add Elastic-compatible console logging
builder.UseElasticConsoleLogging(options =>
{
    options.IncludeScopes = true;
    options.IncludeState = true;
    options.TimestampFormat = "yyyy-MM-dd HH:mm:ss.fffZ";
    options.UseUtcTimestamp = true;
});

var app = builder.Build();
```

Or using the standard `ILoggingBuilder`:

```csharp
builder.Logging.AddElasticConsole(options =>
{
    options.IncludeScopes = true;
    options.IncludeState = true;
});
```

---

## Usage

### Using LoggerEvent for Structured Logging

The `LoggerEvent` class extends `ILogger` to allow fluent building of metadata-rich logs:

```csharp
using Microsoft.Extensions.Logging;

public class MyService(ILogger<MyService> logger)
{
    public void ProcessOrder(string orderId, string userId, string correlationId)
    {
        logger.Event()
              .Correlation(correlationId)
              .User(userId)
              .Property("order_id", orderId)
              .Property("status", "processing")
              .LogInformation("Processing order {OrderId}", orderId);
    }

    public void TrackLegalContext(string entity, string reason, string basis)
    {
        logger.Event()
              .LegalEntity(entity)
              .LegalReason(reason, basis)
              .LogWarning("Accessing restricted data");
    }
}
```

### Elastic Console Output Example

The output is a single-line JSON designed for log collectors like Filebeat or Logstash:

```json
{
  "event_time": "2026-04-10 17:26:00.123Z",
  "event_level": "info",
  "event_source": "MyNamespace.MyService",
  "event_message": "Processing order ORD-12345",
  "event_id": 0,
  "event_correlation": "corr-456",
  "user": "user-789",
  "order_id": "ORD-12345",
  "status": "processing"
}
```

---

## Testing

The solution includes a comprehensive test suite using xUnit and Moq.

### Running the tests

```bash
dotnet test src/Egov.Extensions.Logging.sln
```

---

## Contributing

Contributions are welcome! Please read [CONTRIBUTING.md](CONTRIBUTING.md) for guidelines on how to get started.

---

## Code of Conduct

This project adheres to the [Contributor Covenant Code of Conduct](CODE_OF_CONDUCT.md). By participating, you are expected to uphold this code.

---

## AI Assistance

This repository contains an [AGENTS.md](AGENTS.md) file with instructions and context for AI coding agents to assist in development, ensuring consistency in code style and project structure.

---

## License

This project is licensed under the [MIT License](LICENSE).
