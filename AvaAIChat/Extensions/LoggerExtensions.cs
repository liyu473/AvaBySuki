using Serilog;
using System;
using System.Runtime.CompilerServices;

namespace AvaAIChat.Extensions;

/// <summary>
/// 日志扩展方法，自动捕获调用位置信息
/// </summary>
public static class LoggerExtensions
{
    /// <summary>
    /// 记录调试日志
    /// </summary>
    public static void Debug(
        this ILogger logger,
        string messageTemplate,
        [CallerMemberName] string memberName = "",
        [CallerFilePath] string filePath = "",
        [CallerLineNumber] int lineNumber = 0)
    {
        logger.ForContext("MemberName", memberName)
            .ForContext("FilePath", filePath)
            .ForContext("LineNumber", lineNumber)
            .Debug(messageTemplate);
    }

    /// <summary>
    /// 记录信息日志
    /// </summary>
    public static void Info(
        this ILogger logger,
        string messageTemplate,
        [CallerMemberName] string memberName = "",
        [CallerFilePath] string filePath = "",
        [CallerLineNumber] int lineNumber = 0)
    {
        logger.ForContext("MemberName", memberName)
            .ForContext("FilePath", filePath)
            .ForContext("LineNumber", lineNumber)
            .Information(messageTemplate);
    }

    /// <summary>
    /// 记录警告日志
    /// </summary>
    public static void Warn(
        this ILogger logger,
        string messageTemplate,
        [CallerMemberName] string memberName = "",
        [CallerFilePath] string filePath = "",
        [CallerLineNumber] int lineNumber = 0)
    {
        logger.ForContext("MemberName", memberName)
            .ForContext("FilePath", filePath)
            .ForContext("LineNumber", lineNumber)
            .Warning(messageTemplate);
    }

    /// <summary>
    /// 记录错误日志
    /// </summary>
    public static void Err(
        this ILogger logger,
        string messageTemplate,
        [CallerMemberName] string memberName = "",
        [CallerFilePath] string filePath = "",
        [CallerLineNumber] int lineNumber = 0)
    {
        logger.ForContext("MemberName", memberName)
            .ForContext("FilePath", filePath)
            .ForContext("LineNumber", lineNumber)
            .Error(messageTemplate);
    }

    /// <summary>
    /// 记录错误日志（带异常和行号）
    /// </summary>
    public static void Err(
        this ILogger logger,
        Exception exception,
        string messageTemplate,
        [CallerMemberName] string memberName = "",
        [CallerFilePath] string filePath = "",
        [CallerLineNumber] int lineNumber = 0)
    {
        logger.ForContext("MemberName", memberName)
            .ForContext("FilePath", filePath)
            .ForContext("LineNumber", lineNumber)
            .Error(exception, messageTemplate);
    }

    /// <summary>
    /// 记录致命错误日志（带行号）
    /// </summary>
    public static void Fatal(
        this ILogger logger,
        string messageTemplate,
        [CallerMemberName] string memberName = "",
        [CallerFilePath] string filePath = "",
        [CallerLineNumber] int lineNumber = 0)
    {
        logger.ForContext("MemberName", memberName)
            .ForContext("FilePath", filePath)
            .ForContext("LineNumber", lineNumber)
            .Fatal(messageTemplate);
    }

    /// <summary>
    /// 记录致命错误日志（带异常和行号）
    /// </summary>
    public static void Fatal(
        this ILogger logger,
        Exception exception,
        string messageTemplate,
        [CallerMemberName] string memberName = "",
        [CallerFilePath] string filePath = "",
        [CallerLineNumber] int lineNumber = 0)
    {
        logger.ForContext("MemberName", memberName)
            .ForContext("FilePath", filePath)
            .ForContext("LineNumber", lineNumber)
            .Fatal(exception, messageTemplate);
    }
}