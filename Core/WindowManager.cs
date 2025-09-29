using System;
using System.Runtime.InteropServices;

namespace GoogleClashofClansLauncher.Core;

/// <summary>
/// 管理窗口的查找和激活
/// </summary>
public class WindowManager
{
    // Windows API：查找窗口
    [DllImport("user32.dll", CharSet = CharSet.Unicode)]
    private static extern IntPtr FindWindow(string? lpClassName, string lpWindowName);

    // Windows API：设置窗口为前台
    [DllImport("user32.dll")]
    private static extern bool SetForegroundWindow(IntPtr hWnd);

    // Windows API：恢复窗口（从最小化状态）
    [DllImport("user32.dll")]
    private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

    private const int SW_RESTORE = 9; // 恢复窗口的命令

    /// <summary>
    /// 根据窗口标题查找窗口句柄
    /// </summary>
    /// <param name="windowTitle">窗口标题（精确匹配）</param>
    /// <returns>窗口句柄，未找到返回IntPtr.Zero</returns>
    public IntPtr FindWindowByTitle(string windowTitle)
    {
        return FindWindow(null, windowTitle);
    }

    /// <summary>
    /// 激活窗口（置于前台）
    /// </summary>
    /// <param name="windowHandle">窗口句柄</param>
    /// <returns>操作是否成功</returns>
    public bool ActivateWindow(IntPtr windowHandle)
    {
        if (windowHandle == IntPtr.Zero)
            return false;

        // 先恢复窗口（防止最小化）
        ShowWindow(windowHandle, SW_RESTORE);
        // 置于前台
        return SetForegroundWindow(windowHandle);
    }
}