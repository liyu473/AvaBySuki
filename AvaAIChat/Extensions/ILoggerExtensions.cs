using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace AvaAIChat.Extensions
    ;

/// <summary>
/// ILogger 扩展方法，自动捕获调用位置信息
/// </summary>
public static class ILoggerExtensions
{
    public static void LogInfo<T>(
        this ILogger<T> logger,
        string message,
        [CallerMemberName] string memberName = "",
        [CallerLineNumber] int lineNumber = 0)
    {
        using (logger.BeginScope(new Dictionary<string, object>
        {
            ["MemberName"] = memberName,
            ["LineNumber"] = lineNumber
        }))
        {
            logger.LogInformation(message);
        }
    }

    public static void LogInfo<T>(
        this ILogger<T> logger,
        string message,
        object? arg,
        [CallerMemberName] string memberName = "",
        [CallerLineNumber] int lineNumber = 0)
    {
        using (logger.BeginScope(new Dictionary<string, object>
        {
            ["MemberName"] = memberName,
            ["LineNumber"] = lineNumber
        }))
        {
            logger.LogInformation(message, arg);
        }
    }

    public static void LogWarn<T>(
        this ILogger<T> logger,
        string message,
        [CallerMemberName] string memberName = "",
        [CallerLineNumber] int lineNumber = 0)
    {
        using (logger.BeginScope(new Dictionary<string, object>
        {
            ["MemberName"] = memberName,
            ["LineNumber"] = lineNumber
        }))
        {
            logger.LogWarning(message);
        }
    }

    public static void LogErr<T>(
        this ILogger<T> logger,
        Exception exception,
        string message,
        [CallerMemberName] string memberName = "",
        [CallerLineNumber] int lineNumber = 0)
    {
        using (logger.BeginScope(new Dictionary<string, object>
        {
            ["MemberName"] = memberName,
            ["LineNumber"] = lineNumber
        }))
        {
            logger.LogError(exception, message);
        }
    }

    public static void LogFatal<T>(
        this ILogger<T> logger,
        Exception exception,
        string message,
        [CallerMemberName] string memberName = "",
        [CallerLineNumber] int lineNumber = 0)
    {
        using (logger.BeginScope(new Dictionary<string, object>
        {
            ["MemberName"] = memberName,
            ["LineNumber"] = lineNumber
        }))
        {
            logger.LogCritical(exception, message);
        }
    }
}