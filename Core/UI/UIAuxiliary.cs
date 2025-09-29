using System;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;

namespace GoogleClashofClansLauncher.Core.UI;

/// <summary>
/// UI辅助类
/// 提供与UI交互相关的辅助方法
/// </summary>
public static class UIAuxiliary
{
    /// <summary>
    /// 在UI线程上执行操作
    /// </summary>
    /// <param name="control">UI控件</param>
    /// <param name="action">要执行的操作</param>
    public static void InvokeOnUIThread(this Control control, Action action)
    {
        if (control.InvokeRequired)
        {
            try
            {
                control.Invoke(action);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("在UI线程上执行操作时出错: " + ex.Message);
            }
        }
        else
        {
            action();
        }
    }

    /// <summary>
    /// 在UI线程上异步执行操作
    /// </summary>
    /// <param name="control">UI控件</param>
    /// <param name="action">要执行的操作</param>
    public static async Task InvokeOnUIThreadAsync(this Control control, Action action)
    {
        if (control.InvokeRequired)
        {
            await Task.Run(() =>
            {
                try
                {
                    control.Invoke(action);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("在UI线程上异步执行操作时出错: " + ex.Message);
                }
            });
        }
        else
        {
            action();
        }
    }

    /// <summary>
    /// 安全地更新标签文本
    /// </summary>
    /// <param name="label">标签控件</param>
    /// <param name="text">要设置的文本</param>
    public static void SafeSetText(this Label label, string text)
    {
        label.InvokeOnUIThread(() =>
        {
            try
            {
                label.Text = text;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("设置标签文本时出错: " + ex.Message);
            }
        });
    }

    /// <summary>
    /// 安全地更新按钮状态
    /// </summary>
    /// <param name="button">按钮控件</param>
    /// <param name="enabled">是否启用</param>
    /// <param name="text">可选的文本更新</param>
    public static void SafeSetButtonState(this Button button, bool enabled, string? text = null)
    {
        button.InvokeOnUIThread(() =>
        {
            try
            {
                button.Enabled = enabled;
                if (!string.IsNullOrEmpty(text))
                {
                    button.Text = text;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("设置按钮状态时出错: " + ex.Message);
            }
        });
    }

    /// <summary>
    /// 安全地更新进度条
    /// </summary>
    /// <param name="progressBar">进度条控件</param>
    /// <param name="value">进度值</param>
    public static void SafeSetProgress(this ProgressBar progressBar, int value)
    {
        progressBar.InvokeOnUIThread(() =>
        {
            try
            {
                progressBar.Value = Math.Max(progressBar.Minimum, Math.Min(progressBar.Maximum, value));
            }
            catch (Exception ex)
            {
                Debug.WriteLine("设置进度条时出错: " + ex.Message);
            }
        });
    }

    /// <summary>
    /// 显示信息消息框
    /// </summary>
    /// <param name="message">消息内容</param>
    /// <param name="title">消息框标题</param>
    public static void ShowInfoMessage(string message, string title = "信息")
    {
        try
        {
            MessageBox.Show(message, title, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        catch (Exception ex)
        {
            Debug.WriteLine("显示信息消息框时出错: " + ex.Message);
        }
    }

    /// <summary>
    /// 显示错误消息框
    /// </summary>
    /// <param name="message">错误消息</param>
    /// <param name="title">消息框标题</param>
    public static void ShowErrorMessage(string message, string title = "错误")
    {
        try
        {
            MessageBox.Show(message, title, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        catch (Exception ex)
        {
            Debug.WriteLine("显示错误消息框时出错: " + ex.Message);
        }
    }

    /// <summary>
    /// 显示警告消息框
    /// </summary>
    /// <param name="message">警告消息</param>
    /// <param name="title">消息框标题</param>
    public static void ShowWarningMessage(string message, string title = "警告")
    {
        try
        {
            MessageBox.Show(message, title, MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
        catch (Exception ex)
        {
            Debug.WriteLine("显示警告消息框时出错: " + ex.Message);
        }
    }

    /// <summary>
    /// 显示确认消息框
    /// </summary>
    /// <param name="message">确认消息</param>
    /// <param name="title">消息框标题</param>
    /// <returns>用户是否确认</returns>
    public static bool ShowConfirmationMessage(string message, string title = "确认")
    {
        try
        {
            return MessageBox.Show(message, title, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes;
        }
        catch (Exception ex)
        {
            Debug.WriteLine("显示确认消息框时出错: " + ex.Message);
            return false;
        }
    }
}