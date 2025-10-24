using Avalonia.Controls.Notifications;
using SukiUI.Toasts;
using System;

namespace AvaAIChat.Extensions;

/// <summary>
/// Toast 通知扩展方法
/// </summary>
public static class ToastExtensions
{
    /// <summary>
    /// 显示 Toast 通知
    /// </summary>
    /// <param name="toastManager">Toast 管理器</param>
    /// <param name="title">标题</param>
    /// <param name="message">消息内容</param>
    /// <param name="type">通知类型</param>
    /// <param name="duration">显示时长（秒）</param>
    public static void ShowToast(
        this ISukiToastManager toastManager,
        string title,
        string message,
        NotificationType type = NotificationType.Information,
        int duration = 3)
    {
        toastManager.CreateToast()
            .OfType(type)
            .WithTitle(title)
            .WithContent(message)
            .Dismiss().After(TimeSpan.FromSeconds(duration))
            .Dismiss().ByClicking()
            .Queue();
    }

    /// <summary>
    /// 显示成功 Toast
    /// </summary>
    public static void ShowSuccess(
        this ISukiToastManager toastManager,
        string title,
        string message,
        int duration = 3)
    {
        toastManager.ShowToast(title, message, NotificationType.Success, duration);
    }

    /// <summary>
    /// 显示错误 Toast
    /// </summary>
    public static void ShowError(
        this ISukiToastManager toastManager,
        string title,
        string message,
        int duration = 5)
    {
        toastManager.ShowToast(title, message, NotificationType.Error, duration);
    }

    /// <summary>
    /// 显示警告 Toast
    /// </summary>
    public static void ShowWarning(
        this ISukiToastManager toastManager,
        string title,
        string message,
        int duration = 4)
    {
        toastManager.ShowToast(title, message, NotificationType.Warning, duration);
    }

    /// <summary>
    /// 显示信息 Toast
    /// </summary>
    public static void ShowInfo(
        this ISukiToastManager toastManager,
        string title,
        string message,
        int duration = 3)
    {
        toastManager.ShowToast(title, message, NotificationType.Information, duration);
    }

    /// <summary>
    /// 显示简短成功消息
    /// </summary>
    public static void ShowSuccessShort(
        this ISukiToastManager toastManager,
        string message)
    {
        toastManager.ShowSuccess("成功", message, 2);
    }

    /// <summary>
    /// 显示简短错误消息
    /// </summary>
    public static void ShowErrorShort(
        this ISukiToastManager toastManager,
        string message)
    {
        toastManager.ShowError("错误", message, 3);
    }

    /// <summary>
    /// 显示简短警告消息
    /// </summary>
    public static void ShowWarningShort(
        this ISukiToastManager toastManager,
        string message)
    {
        toastManager.ShowWarning("警告", message, 3);
    }

    /// <summary>
    /// 显示简短信息消息
    /// </summary>
    public static void ShowInfoShort(
        this ISukiToastManager toastManager,
        string message)
    {
        toastManager.ShowInfo("提示", message, 2);
    }

    /// <summary>
    /// 显示操作成功消息
    /// </summary>
    public static void ShowOperationSuccess(
        this ISukiToastManager toastManager,
        string operation = "操作")
    {
        toastManager.ShowSuccess("成功", $"{operation}成功", 2);
    }

    /// <summary>
    /// 显示操作失败消息
    /// </summary>
    public static void ShowOperationError(
        this ISukiToastManager toastManager,
        string operation = "操作",
        string? errorMessage = null)
    {
        var message = string.IsNullOrEmpty(errorMessage)
            ? $"{operation}失败"
            : $"{operation}失败：{errorMessage}";

        toastManager.ShowError("失败", message, 4);
    }

    /// <summary>
    /// 显示自定义内容的 Toast
    /// </summary>
    /// <param name="toastManager">Toast 管理器</param>
    /// <param name="content">自定义内容（可以是任何 Control）</param>
    /// <param name="type">通知类型</param>
    /// <param name="duration">显示时长（秒）</param>
    public static void ShowCustomContent(
        this ISukiToastManager toastManager,
        object content,
        NotificationType type = NotificationType.Information,
        int duration = 3)
    {
        toastManager.CreateToast()
            .OfType(type)
            .WithContent(content)
            .Dismiss().After(TimeSpan.FromSeconds(duration))
            .Dismiss().ByClicking()
            .Queue();
    }

    /// <summary>
    /// 显示永久 Toast（不自动关闭）
    /// </summary>
    /// <param name="toastManager">Toast 管理器</param>
    /// <param name="title">标题</param>
    /// <param name="message">消息内容</param>
    /// <param name="type">通知类型</param>
    public static void ShowPersistent(
        this ISukiToastManager toastManager,
        string title,
        string message,
        NotificationType type = NotificationType.Information)
    {
        toastManager.CreateToast()
            .OfType(type)
            .WithTitle(title)
            .WithContent(message)
            .Dismiss().ByClicking()
            .Queue();
    }
}