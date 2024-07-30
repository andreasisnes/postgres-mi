﻿using System.Data.Common;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Azure.Core;
using Azure.Identity;
using Yuniql.AspNetCore;
using Yuniql.Extensibility;
using Yuniql.PostgreSql;

namespace Microservice.HelloCache;

public static class MigrationService
{
    public static void MigratePostgresql(this WebApplication app, Action<MigrationOptions> action)
    {
        var options = new MigrationOptions();
        action(options);

        var builder = new DbConnectionStringBuilder
        {
            ConnectionString = options.ConnectionString
        };

        if (options.UseManagedIdentity)
        {
            builder.AddAzureCredentials(options);
        }

        var traceService = new YuniqlTraceService(app.Services.GetRequiredService<ILogger>());

        app.UseYuniql(new PostgreSqlDataService(traceService), new PostgreSqlBulkImportService(traceService), traceService, new Configuration
        {
            Workspace = Path.Join(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Migration"),
            Platform = "postgresql",
            ConnectionString = builder.ToString(),
        });
    }

    private static void AddAzureCredentials(this DbConnectionStringBuilder builder, MigrationOptions options)
    {
        var credentials = new DefaultAzureCredential(new DefaultAzureCredentialOptions
        {
            ManagedIdentityClientId = options.MigrationClientId
        });

        var token = Task.Run(async () => await credentials.GetTokenAsync(new TokenRequestContext(scopes: ["https://ossrdbms-aad.database.windows.net/.default"]) { }))
            .GetAwaiter()
            .GetResult();

        builder.Add("SSLMode", "Prefer");
        builder.Add("Password", token);
    }
}

internal partial class YuniqlTraceService
    : ITraceService
{
    private readonly ILogger _logger;

    public YuniqlTraceService(ILogger logger)
    {
        _logger = logger;
    }

    public bool IsDebugEnabled { get => _logger.IsEnabled(LogLevel.Debug); set => throw new NotSupportedException(); }
    public bool IsTraceSensitiveData { get => false; set => throw new NotSupportedException(); }
    public bool IsTraceToDirectory { get => false; set => throw new NotSupportedException(); }
    public bool IsTraceToFile { get => false; set => throw new NotSupportedException(); }
    public string? TraceDirectory { get => null; set => throw new NotSupportedException(); }

    public void Debug(string message, object? payload = null)
    {
        Log.Debug(_logger, message, payload);
    }

    public void Error(string message, object? payload = null)
    {
        Log.Error(_logger, message, payload);
    }

    public void Info(string message, object? payload = null)
    {
        Log.Info(_logger, message, payload);
    }

    public void Success(string message, object? payload = null)
    {
        Log.Success(_logger, message, payload);
    }

    public void Warn(string message, object? payload = null)
    {
        Log.Warn(_logger, message, payload);
    }

    [SuppressMessage(
        "LoggerMessage",
        "LOGGEN036:A value being logged doesn't have an effective way to be converted into a string",
        Justification = "We can't really do anything more than ToString on these objects we get from yuniql")]
    private static partial class Log
    {
        [LoggerMessage(1, LogLevel.Debug, "Yuniql: {Message}. {Payload}")]
        public static partial void Debug(ILogger logger, string message, object? payload);

        [LoggerMessage(2, LogLevel.Error, "Yuniql: {Message}. {Payload}")]
        public static partial void Error(ILogger logger, string message, object? payload);

        [LoggerMessage(3, LogLevel.Information, "Yuniql: {Message}. {Payload}")]
        public static partial void Info(ILogger logger, string message, object? payload);

        [LoggerMessage(4, LogLevel.Information, "Yuniql: {Message}. {Payload}")]
        public static partial void Success(ILogger logger, string message, object? payload);

        [LoggerMessage(5, LogLevel.Warning, "Yuniql: {Message}. {Payload}")]
        public static partial void Warn(ILogger logger, string message, object? payload);
    }
}