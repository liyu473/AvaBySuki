using Avalonia.Controls.Notifications;
using SukiUI.Dialogs;
using System.Threading.Tasks;

namespace AvaAIChat.Extensions;

/// <summary>
/// 对话框扩展方法
/// </summary>
public static class DialogExtensions
{
    /// <summary>
    /// 显示信息对话框
    /// </summary>
    /// <param name="dialogManager">对话框管理器</param>
    /// <param name="title">标题</param>
    /// <param name="message">消息内容</param>
    /// <param name="type">对话框类型</param>
    /// <param name="buttonText">按钮文本</param>
    /// <param name="dismissByBackground">是否允许点击背景关闭</param>
    public static void ShowMessage(
        this ISukiDialogManager dialogManager,
        string title,
        string message,
        NotificationType type = NotificationType.Information,
        string buttonText = "确定",
        bool dismissByBackground = false)
    {
        var dialog = dialogManager.CreateDialog()
            .OfType(type)
            .WithTitle(title)
            .WithContent(message)
            .WithActionButton(buttonText, _ => { }, true);

        if (dismissByBackground)
        {
            dialog.Dismiss().ByClickingBackground();
        }

        dialog.TryShow();
    }

    /// <summary>
    /// 显示成功消息对话框
    /// </summary>
    public static void ShowSuccess(
        this ISukiDialogManager dialogManager,
        string title,
        string message,
        string buttonText = "确定")
    {
        dialogManager.ShowMessage(title, message, NotificationType.Success, buttonText);
    }

    /// <summary>
    /// 显示错误消息对话框
    /// </summary>
    public static void ShowError(
        this ISukiDialogManager dialogManager,
        string title,
        string message,
        string buttonText = "确定")
    {
        dialogManager.ShowMessage(title, message, NotificationType.Error, buttonText);
    }

    /// <summary>
    /// 显示警告消息对话框
    /// </summary>
    public static void ShowWarning(
        this ISukiDialogManager dialogManager,
        string title,
        string message,
        string buttonText = "确定")
    {
        dialogManager.ShowMessage(title, message, NotificationType.Warning, buttonText);
    }

    /// <summary>
    /// 显示信息消息对话框
    /// </summary>
    public static void ShowInfo(
        this ISukiDialogManager dialogManager,
        string title,
        string message,
        string buttonText = "确定")
    {
        dialogManager.ShowMessage(title, message, NotificationType.Information, buttonText);
    }

    /// <summary>
    /// 显示确认对话框（异步）
    /// </summary>
    /// <param name="dialogManager">对话框管理器</param>
    /// <param name="title">标题</param>
    /// <param name="message">消息内容</param>
    /// <param name="type">对话框类型</param>
    /// <param name="yesText">确认按钮文本</param>
    /// <param name="noText">取消按钮文本</param>
    /// <param name="dismissByBackground">是否允许点击背景关闭</param>
    /// <returns>用户是否点击了确认按钮</returns>
    public static async Task<bool> ShowConfirmAsync(
        this ISukiDialogManager dialogManager,
        string title,
        string message,
        NotificationType type = NotificationType.Information,
        string yesText = "确定",
        string noText = "取消",
        bool dismissByBackground = false)
    {
        var dialog = dialogManager.CreateDialog()
            .OfType(type)
            .WithTitle(title)
            .WithContent(message)
            .WithYesNoResult(yesText, noText);

        if (dismissByBackground)
        {
            dialog.Dismiss().ByClickingBackground();
        }

        var result = await dialog.TryShowAsync();
        return result;
    }

    /// <summary>
    /// 显示确认删除对话框（异步）
    /// </summary>
    public static async Task<bool> ShowDeleteConfirmAsync(
        this ISukiDialogManager dialogManager,
        string title = "确认删除",
        string message = "确定要删除吗？此操作无法撤销。")
    {
        return await dialogManager.ShowConfirmAsync(
            title,
            message,
            NotificationType.Warning,
            "删除",
            "取消",
            dismissByBackground: false);
    }

    /// <summary>
    /// 显示自定义内容对话框
    /// </summary>
    /// <param name="dialogManager">对话框管理器</param>
    /// <param name="title">标题</param>
    /// <param name="content">自定义内容（可以是任何 Control）</param>
    /// <param name="type">对话框类型</param>
    /// <param name="buttonText">按钮文本</param>
    /// <param name="dismissByBackground">是否允许点击背景关闭</param>
    public static void ShowCustomContent(
        this ISukiDialogManager dialogManager,
        string title,
        object content,
        NotificationType type = NotificationType.Information,
        string buttonText = "关闭",
        bool dismissByBackground = false)
    {
        var dialog = dialogManager.CreateDialog()
            .OfType(type)
            .WithTitle(title)
            .WithContent(content)
            .WithActionButton(buttonText, _ => { }, true);

        if (dismissByBackground)
        {
            dialog.Dismiss().ByClickingBackground();
        }

        dialog.TryShow();
    }

    /// <summary>
    /// 显示自定义内容确认对话框（异步）
    /// </summary>
    /// <param name="dialogManager">对话框管理器</param>
    /// <param name="title">标题</param>
    /// <param name="content">自定义内容（可以是任何 Control）</param>
    /// <param name="type">对话框类型</param>
    /// <param name="yesText">确认按钮文本</param>
    /// <param name="noText">取消按钮文本</param>
    /// <param name="dismissByBackground">是否允许点击背景关闭</param>
    /// <returns>用户是否点击了确认按钮</returns>
    public static async Task<bool> ShowCustomContentConfirmAsync(
        this ISukiDialogManager dialogManager,
        string title,
        object content,
        NotificationType type = NotificationType.Information,
        string yesText = "确定",
        string noText = "取消",
        bool dismissByBackground = false)
    {
        var dialog = dialogManager.CreateDialog()
            .OfType(type)
            .WithTitle(title)
            .WithContent(content)
            .WithYesNoResult(yesText, noText);

        if (dismissByBackground)
        {
            dialog.Dismiss().ByClickingBackground();
        }

        var result = await dialog.TryShowAsync();
        return result;
    }
}