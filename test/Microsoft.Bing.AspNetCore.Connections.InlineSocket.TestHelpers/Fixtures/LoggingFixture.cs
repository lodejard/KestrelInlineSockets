// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Microsoft.Bing.AspNetCore.Connections.InlineSocket.Tests.Fixtures
{
    public class LoggingFixture : IDisposable
    {
        public List<LogItem> LogItems { get; } = new List<LogItem>();

        public void ConfigureLogging(ILoggingBuilder builder)
        {
            builder.AddProvider(new LoggerProvider(this));
            builder.Services.Configure<LoggerFilterOptions>(options =>
            {
                options.Rules.Add(new LoggerFilterRule(
                    typeof(LoggerProvider).FullName,
                    null,
                    LogLevel.Trace,
                    (providerName, categoryName, logLevel) => true));
            });
        }

        public void Dispose()
        {
        }

        public void Record(string categoryName, LogLevel logLevel, EventId eventId, IReadOnlyCollection<KeyValuePair<string, object>> properties, Exception exception, string message)
        {
            LogItems.Add(new LogItem
            {
                CategoryName = categoryName,
                LogLevel = logLevel,
                EventId = eventId,
                Properties = properties.ToDictionary(kv => kv.Key, kv => kv.Value),
                Exception = exception,
                Message = message,
            });
        }

        public class LoggerProvider : ILoggerProvider
        {
            private readonly LoggingFixture _fixture;

            public LoggerProvider(LoggingFixture fixture)
            {
                _fixture = fixture;
            }

            public ILogger CreateLogger(string categoryName)
            {
                return new Logger(_fixture, categoryName);
            }

            public void Dispose()
            {
            }
        }

        public class Logger : ILogger
        {
            private LoggingFixture _fixture;
            private string _categoryName;

            public Logger(LoggingFixture fixture, string categoryName)
            {
                _fixture = fixture;
                _categoryName = categoryName;
            }

            public IDisposable BeginScope<TState>(TState state)
            {
                return new LoggerScope();
            }

            public bool IsEnabled(LogLevel logLevel)
            {
                return true;
            }

            public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
            {
                _fixture.Record(_categoryName, logLevel, eventId, state as IReadOnlyCollection<KeyValuePair<string, object>>, exception, formatter(state, exception));
            }
        }

        public class LoggerScope : IDisposable
        {
            public void Dispose()
            {
            }
        }

        public class LogItem
        {
            public string CategoryName { get; internal set; }

            public LogLevel LogLevel { get; internal set; }

            public EventId EventId { get; internal set; }

            public Exception Exception { get; internal set; }

            public string Message { get; internal set; }

            public IDictionary<string, object> Properties { get; internal set; }

            public override string ToString()
            {
                return $"{CategoryName}.{EventId} {LogLevel} {Message}";
            }
        }
    }
}
