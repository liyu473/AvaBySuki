using AvaAIChat.Extensions;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using SukiUI.Dialogs;
using SukiUI.Toasts;
using System.Threading.Tasks;

namespace AvaAIChat.ViewModels;

public partial class HomePageViewModel : ViewModelBase
{
    private readonly ILogger<HomePageViewModel> _logger;
    private readonly ISukiDialogManager _dialogManager;
    private readonly ISukiToastManager _toastManager;

    public HomePageViewModel(
        ILogger<HomePageViewModel> logger,
        ISukiDialogManager dialogManager,
        ISukiToastManager toastManager)
    {
        _logger = logger;
        _dialogManager = dialogManager;
        _toastManager = toastManager;
    }

    #region Dialog 测试

    [RelayCommand]
    private void ShowSuccessDialog()
    {
        _dialogManager.ShowSuccess("成功", "这是一个成功消息对话框！");
    }

    [RelayCommand]
    private void ShowErrorDialog()
    {
        _dialogManager.ShowError("错误", "这是一个错误消息对话框！");
    }

    [RelayCommand]
    private void ShowWarningDialog()
    {
        _dialogManager.ShowWarning("警告", "这是一个警告消息对话框！");
    }

    [RelayCommand]
    private void ShowInfoDialog()
    {
        _dialogManager.ShowInfo("提示", "这是一个信息提示对话框！");
    }

    [RelayCommand]
    private async Task ShowConfirmDialog()
    {
        var result = await _dialogManager.ShowConfirmAsync(
            "确认操作",
            "您确定要执行此操作吗？这个操作可能会影响系统设置。");

        if (result)
        {
            _toastManager.ShowSuccessShort("您点击了确定");
        }
        else
        {
            _toastManager.ShowInfoShort("您点击了取消");
        }
    }

    [RelayCommand]
    private async Task ShowDeleteConfirm()
    {
        var result = await _dialogManager.ShowDeleteConfirmAsync(
            "确认删除",
            "确定要删除这个项目吗？删除后无法恢复！");

        if (result)
        {
            _toastManager.ShowSuccess("删除成功", "项目已被删除");
        }
        else
        {
            _toastManager.ShowInfo("已取消", "删除操作已取消");
        }
    }

    #endregion Dialog 测试

    #region Toast 测试

    [RelayCommand]
    private void ShowSuccessToast()
    {
        _toastManager.ShowSuccess("操作成功", "数据保存成功！");
    }

    [RelayCommand]
    private void ShowErrorToast()
    {
        _toastManager.ShowError("操作失败", "网络连接失败，请检查您的网络设置。");
    }

    [RelayCommand]
    private void ShowWarningToast()
    {
        _toastManager.ShowWarning("注意", "您的磁盘空间不足，请及时清理。");
    }

    [RelayCommand]
    private void ShowInfoToast()
    {
        _toastManager.ShowInfo("系统提示", "有新的更新可用，建议您更新到最新版本。");
    }

    [RelayCommand]
    private void ShowOperationSuccess()
    {
        _toastManager.ShowOperationSuccess("保存");
    }

    [RelayCommand]
    private void ShowOperationError()
    {
        _toastManager.ShowOperationError("上传", "文件大小超过限制");
    }

    #endregion Toast 测试
}