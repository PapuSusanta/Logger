# Custom Logger

This is a custom logging utility designed to provide a flexible and efficient way to log events, errors, and debug information in your application.

## Features

- Supports multiple log levels, including DEBUG, INFO, WARNING, ERROR, and CRITICAL.
- Allows for customization of log formats, output destinations, and log levels.
- Designed to be thread-safe, ensuring that logs are written correctly even in concurrent environments.

## Usage

### Basic Example

```csharp
using web.Logging;

// Create a new logger instance with the specified name
var logger = new FileLogger(configuration, category => true);

// Log a debug message
logger.Debug("This is a debug message");

// Log an info message
logger.Info("This is an info message");

// Log a warning message
logger.Warning("This is a warning message");

// Log an error message
logger.Error("This is an error message");

// Log a critical message
logger.Critical("This is a critical message");
```
