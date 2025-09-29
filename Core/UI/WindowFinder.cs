using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;
using System.Diagnostics;

namespace GoogleClashofClansLauncher.Core.UI;

/// <summary>
/// 窗口查找器类
/// 提供多种窗口查找策略
/// </summary>
public class WindowFinder
{
    // Windows API：枚举窗口
    [DllImport("user32.dll", CharSet = CharSet.Unicode)]
    private static extern bool EnumWindows(EnumWindowsProc enumProc, IntPtr lParam);

    // Windows API：获取窗口文本
    [DllImport("user32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    private static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

    // 委托定义
    private delegate bool EnumWindowsProc(IntPtr hWnd, IntPtr lParam);

    /// <summary>
    /// 通过窗口标题关键字查找窗口
    /// </summary>
    /// <param name="keyword">窗口标题关键字</param>
    /// <returns>窗口句柄，未找到返回IntPtr.Zero</returns>
    public IntPtr FindWindowByKeyword(string keyword)
    {
        IntPtr foundWindow = IntPtr.Zero;
        EnumWindows((hWnd, lParam) =>
        {
            StringBuilder title = new StringBuilder(512);
            GetWindowText(hWnd, title, title.Capacity);
            string windowTitle = title.ToString();

            if (!string.IsNullOrEmpty(windowTitle) && windowTitle.Contains(keyword, StringComparison.OrdinalIgnoreCase))
            {
                Debug.WriteLine("找到包含关键字的窗口: " + windowTitle);
                foundWindow = hWnd;
                return false; // 停止枚举
            }
            return true; // 继续枚举
        }, IntPtr.Zero);

        return foundWindow;
    }

    /// <summary>
    /// 通过完整的标题模式匹配查找窗口
    /// </summary>
    /// <param name="keyword">窗口标题关键字</param>
    /// <returns>窗口句柄，未找到返回IntPtr.Zero</returns>
    public IntPtr FindWindowByFullKeyword(string keyword)
    {
        IntPtr foundWindow = IntPtr.Zero;
        // 尝试匹配包含完整部落冲突标题格式的窗口
        string[] patterns = new string[]
        {
            keyword, // 直接匹配
            $"{keyword}(Clash of Clans)", // 常见格式
            $"{keyword}(Clash of Clans)*" // 带后缀的格式
        };

        EnumWindows((hWnd, lParam) =>
        {
            StringBuilder title = new StringBuilder(512);
            GetWindowText(hWnd, title, title.Capacity);
            string windowTitle = title.ToString();

            foreach (string pattern in patterns)
            {
                if (!string.IsNullOrEmpty(windowTitle) && 
                    (windowTitle.Equals(pattern, StringComparison.OrdinalIgnoreCase) ||
                     (pattern.EndsWith("*") && windowTitle.StartsWith(pattern.Substring(0, pattern.Length - 1), StringComparison.OrdinalIgnoreCase))))
                {
                    Debug.WriteLine("找到符合模式的窗口: " + windowTitle);
                    foundWindow = hWnd;
                    return false; // 停止枚举
                }
            }
            return true; // 继续枚举
        }, IntPtr.Zero);

        return foundWindow;
    }

    /// <summary>
    /// 通过进程名查找窗口
    /// </summary>
    /// <param name="processName">进程名</param>
    /// <returns>窗口句柄，未找到返回IntPtr.Zero</returns>
    public IntPtr FindWindowByProcessName(string processName)
    {
        Process[] processes = Process.GetProcessesByName(processName);
        if (processes.Length > 0)
        {
            foreach (Process process in processes)
            {
                if (process.MainWindowHandle != IntPtr.Zero)
                {
                    Debug.WriteLine($"通过进程名找到窗口，进程ID: {process.Id}");
                    return process.MainWindowHandle;
                }
            }
        }
        return IntPtr.Zero;
    }

    [DllImport("user32.dll", SetLastError = true)]
    private static extern bool GetWindowRect(IntPtr hwnd, out RECT lpRect);

    [StructLayout(LayoutKind.Sequential)]
    public struct RECT
    {
        public int Left;
        public int Top;
        public int Right;
        public int Bottom;
    }

    public static Rectangle GetWindowRect(IntPtr hWnd)
    {
        RECT rect = new RECT();
        if (GetWindowRect(hWnd, out rect))
        {
            return new Rectangle(rect.Left, rect.Top, rect.Right - rect.Left, rect.Bottom - rect.Top);
        }
        return Rectangle.Empty;
    }

    /// <summary>
    /// 综合查找策略
    /// 尝试多种方法查找游戏窗口
    /// </summary>
    /// <param name="processName">进程名</param>
    /// <param name="windowTitleKeyword">窗口标题关键字</param>
    /// <returns>窗口句柄，未找到返回IntPtr.Zero</returns>
    public IntPtr FindGameWindow(string processName, string windowTitleKeyword)
    {
        // 策略1: 先尝试通过完整标题格式查找
        IntPtr windowHandle = FindWindowByFullKeyword(windowTitleKeyword);
        if (windowHandle != IntPtr.Zero)
        {
            return windowHandle;
        }

        // 策略2: 通过关键字查找
        windowHandle = FindWindowByKeyword(windowTitleKeyword);
        if (windowHandle != IntPtr.Zero)
        {
            return windowHandle;
        }

        // 策略3: 通过进程名查找
        windowHandle = FindWindowByProcessName(processName);
        if (windowHandle != IntPtr.Zero)
        {
            return windowHandle;
        }

        // 尝试备用关键字
        string[] alternateKeywords = { "Clash of Clans", "部落冲突 COC" };
        foreach (string keyword in alternateKeywords)
        {
            windowHandle = FindWindowByKeyword(keyword);
            if (windowHandle != IntPtr.Zero)
            {
                return windowHandle;
            }
        }

        Debug.WriteLine("未找到游戏窗口");
        return IntPtr.Zero;
    }
}