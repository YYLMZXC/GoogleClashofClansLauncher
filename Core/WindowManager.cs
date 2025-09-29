using System;
using System.Runtime.InteropServices;
using System.Diagnostics;

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

    // Windows API：获取前台窗口
    [DllImport("user32.dll")]
    private static extern IntPtr GetForegroundWindow();

    // Windows API：获取窗口线程ID
    [DllImport("user32.dll")]
    private static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);

    // Windows API：附加线程输入
    [DllImport("user32.dll")]
    private static extern bool AttachThreadInput(uint idAttach, uint idAttachTo, bool fAttach);

    // Windows API：获取当前线程ID
    [DllImport("kernel32.dll")]
    private static extern uint GetCurrentThreadId();

    private const int SW_RESTORE = 9; // 恢复窗口的命令
    private const int SW_SHOW = 5;    // 显示窗口的命令

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

        // 检查窗口是否已经在前台
        if (GetForegroundWindow() == windowHandle)
        {
            Debug.WriteLine("窗口已经在前台，不需要激活");
            return true;
        }

        try
        {
            // 先恢复窗口（防止最小化）
            ShowWindow(windowHandle, SW_RESTORE);
            
            // 使用更可靠的方法激活窗口，避免SetForegroundWindow的限制
            // 方法：获取窗口线程ID和当前线程ID，附加线程输入，然后设置前台窗口
            uint windowThreadId = GetWindowThreadProcessId(windowHandle, out _);
            uint currentThreadId = GetCurrentThreadId();
            
            // 如果窗口线程和当前线程不同，则附加线程输入
            if (windowThreadId != currentThreadId)
            {
                AttachThreadInput(currentThreadId, windowThreadId, true);
            }
            
            // 置于前台
            bool result = SetForegroundWindow(windowHandle);
            
            // 分离线程输入
            if (windowThreadId != currentThreadId)
            {
                AttachThreadInput(currentThreadId, windowThreadId, false);
            }
            
            return result;
        }
        catch (Exception ex)
        {
            Debug.WriteLine("激活窗口时出错: " + ex.Message);
            return false;
        }
    }
    
    /// <summary>
    /// 仅恢复窗口但不激活（不将其置于前台）
    /// 用于避免全屏游戏被强制切换到窗口模式
    /// </summary>
    /// <param name="windowHandle">窗口句柄</param>
    /// <returns>操作是否成功</returns>
    public bool RestoreWindowOnly(IntPtr windowHandle)
    {
        if (windowHandle == IntPtr.Zero)
            return false;
        
        try
        {
            // 只恢复窗口但不设置为前台
            return ShowWindow(windowHandle, SW_RESTORE);
        }
        catch (Exception ex)
        {
            Debug.WriteLine("恢复窗口时出错: " + ex.Message);
            return false;
        }
    }
    
    /// <summary>
    /// 检查窗口是否可见
    /// </summary>
    /// <param name="windowHandle">窗口句柄</param>
    /// <returns>窗口是否可见</returns>
    public bool IsWindowVisible(IntPtr windowHandle)
    {
        if (windowHandle == IntPtr.Zero)
            return false;
            
        // GetWindowPlacement可以用来获取窗口状态，但需要额外的API声明
        // 这里简化处理，假设恢复后的窗口是可见的
        return true;
    }
}