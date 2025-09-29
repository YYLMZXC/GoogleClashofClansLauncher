using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;
using GoogleClashofClansLauncher.Core;
using GoogleClashofClansLauncher.Core.System;

namespace GoogleClashofClansLauncher.Game;

/// <summary>
/// 游戏窗口管理器
/// 负责游戏窗口的查找、定位、大小调整和操作
/// </summary>
public class GameWindow
{
    /// <summary>
    /// 游戏窗口句柄
    /// </summary>
    public IntPtr WindowHandle { get; private set; } = IntPtr.Zero;

    /// <summary>
    /// 游戏窗口标题
    /// </summary>
    public string WindowTitle { get; set; } = "Clash of Clans";

    /// <summary>
    /// 窗口查找超时时间(毫秒)
    /// </summary>
    public int FindWindowTimeout { get; set; } = 10000;

    /// <summary>
    /// 窗口更新间隔(毫秒)
    /// </summary>
    public int WindowUpdateInterval { get; set; } = 500;

    /// <summary>
    /// 是否自动调整窗口大小
    /// </summary>
    public bool AutoResizeWindow { get; set; } = true;

    /// <summary>
    /// 首选窗口大小
    /// </summary>
    public Size PreferredWindowSize { get; set; } = new Size(1024, 768);

    /// <summary>
    /// 窗口位置
    /// </summary>
    public Point WindowPosition { get; private set; } = Point.Empty;

    /// <summary>
    /// 窗口大小
    /// </summary>
    public Size WindowSize { get; private set; } = Size.Empty;

    /// <summary>
    /// 窗口区域
    /// </summary>
    public Rectangle WindowRectangle { get; private set; } = Rectangle.Empty;

    /// <summary>
    /// 窗口状态改变事件
    /// </summary>
    public event EventHandler<GameWindowEventArgs> WindowStateChanged;

    /// <summary>
    /// 窗口位置改变事件
    /// </summary>
    public event EventHandler<GameWindowEventArgs> WindowPositionChanged;

    /// <summary>
    /// 窗口大小改变事件
    /// </summary>
    public event EventHandler<GameWindowEventArgs> WindowSizeChanged;

    /// <summary>
    /// 构造函数
    /// </summary>
    public GameWindow()
    {
        Initialize();
    }

    /// <summary>
    /// 初始化游戏窗口管理器
    /// </summary>
    private void Initialize()
    {
        Utils.LogDebug("游戏窗口管理器已初始化", "GameWindow");
    }

    /// <summary>
    /// 查找游戏窗口
    /// </summary>
    /// <returns>是否找到窗口</returns>
    public bool FindGameWindow()
    {
        try
        {
            Utils.LogDebug($"正在查找游戏窗口: {WindowTitle}", "GameWindow");

            // 尝试通过标题查找窗口
            IntPtr handle = FindWindow(null, WindowTitle);
            if (handle == IntPtr.Zero)
            {
                // 如果没有找到，尝试通过类名查找(如果有必要)
                Utils.LogWarning($"未找到标题为 '{WindowTitle}' 的窗口，尝试其他方式", "GameWindow");
                return false;
            }

            WindowHandle = handle;
            Utils.LogDebug($"已找到游戏窗口，句柄: {WindowHandle}", "GameWindow");

            // 更新窗口信息
            UpdateWindowInfo();

            // 触发窗口状态改变事件
            OnWindowStateChanged(new GameWindowEventArgs(WindowHandle, true));
            return true;
        }
        catch (Exception ex)
        {
            Utils.LogError("查找游戏窗口时发生异常", ex, "GameWindow");
            return false;
        }
    }

    /// <summary>
    /// 等待游戏窗口出现
    /// </summary>
    /// <returns>是否找到窗口</returns>
    public bool WaitForGameWindow()
    {
        try
        {
            Utils.LogDebug($"等待游戏窗口出现，超时时间: {FindWindowTimeout}毫秒", "GameWindow");

            DateTime startTime = DateTime.Now;
            while ((DateTime.Now - startTime).TotalMilliseconds < FindWindowTimeout)
            {
                if (FindGameWindow())
                {
                    return true;
                }

                // 短暂休眠后继续查找
                System.Threading.Thread.Sleep(100);
            }

            Utils.LogError($"等待游戏窗口超时，超过{FindWindowTimeout}毫秒", "GameWindow");
            return false;
        }
        catch (Exception ex)
        {
            Utils.LogError("等待游戏窗口时发生异常", ex, "GameWindow");
            return false;
        }
    }

    /// <summary>
    /// 激活游戏窗口
    /// </summary>
    /// <returns>是否激活成功</returns>
    public bool ActivateGameWindow()
    {
        try
        {
            if (!IsWindowValid())
            {
                Utils.LogWarning("无法激活窗口，窗口句柄无效", "GameWindow");
                return false;
            }

            // 确保窗口可见
            if (!IsWindowVisible(WindowHandle))
            {
                ShowWindow(WindowHandle, SW_SHOW);
                Utils.LogDebug("已显示隐藏的游戏窗口", "GameWindow");
            }

            // 激活窗口并设置焦点
            bool activated = SetForegroundWindow(WindowHandle);
            if (activated)
            {
                Utils.LogDebug("游戏窗口已激活", "GameWindow");
                // 触发窗口状态改变事件
                OnWindowStateChanged(new GameWindowEventArgs(WindowHandle, true));
            }
            else
            {
                Utils.LogWarning("游戏窗口激活失败", "GameWindow");
            }

            return activated;
        }
        catch (Exception ex)
        {
            Utils.LogError("激活游戏窗口时发生异常", ex, "GameWindow");
            return false;
        }
    }

    /// <summary>
    /// 关闭游戏窗口
    /// </summary>
    /// <returns>是否关闭成功</returns>
    public bool CloseGameWindow()
    {
        try
        {
            if (!IsWindowValid())
            {
                Utils.LogWarning("无法关闭窗口，窗口句柄无效", "GameWindow");
                return true; // 认为成功，因为窗口不存在
            }

            bool closed = CloseWindow(WindowHandle);
            if (closed)
            {
                Utils.LogDebug("游戏窗口已关闭", "GameWindow");
                WindowHandle = IntPtr.Zero;
                // 触发窗口状态改变事件
                OnWindowStateChanged(new GameWindowEventArgs(IntPtr.Zero, false));
            }
            else
            {
                Utils.LogWarning("游戏窗口关闭失败", "GameWindow");
            }

            return closed;
        }
        catch (Exception ex)
        {
            Utils.LogError("关闭游戏窗口时发生异常", ex, "GameWindow");
            return false;
        }
    }

    /// <summary>
    /// 调整游戏窗口大小
    /// </summary>
    /// <param name="width">宽度</param>
    /// <param name="height">高度</param>
    /// <returns>是否调整成功</returns>
    public bool ResizeGameWindow(int width, int height)
    {
        try
        {
            if (!IsWindowValid())
            {
                Utils.LogWarning("无法调整窗口大小，窗口句柄无效", "GameWindow");
                return false;
            }

            // 检查参数有效性
            if (width <= 0 || height <= 0)
            {
                Utils.LogWarning("窗口大小参数无效", "GameWindow");
                return false;
            }

            // 调整窗口大小
            bool resized = SetWindowPos(WindowHandle, IntPtr.Zero, 
                WindowPosition.X, WindowPosition.Y, width, height, 
                SWP_NOZORDER | SWP_NOACTIVATE);

            if (resized)
            {
                Utils.LogDebug($"游戏窗口大小已调整为: {width}x{height}", "GameWindow");
                // 更新窗口信息
                UpdateWindowInfo();
                // 触发窗口大小改变事件
                OnWindowSizeChanged(new GameWindowEventArgs(WindowHandle, true));
            }
            else
            {
                Utils.LogWarning("游戏窗口大小调整失败", "GameWindow");
            }

            return resized;
        }
        catch (Exception ex)
        {
            Utils.LogError("调整游戏窗口大小时发生异常", ex, "GameWindow");
            return false;
        }
    }

    /// <summary>
    /// 移动游戏窗口
    /// </summary>
    /// <param name="x">X坐标</param>
    /// <param name="y">Y坐标</param>
    /// <returns>是否移动成功</returns>
    public bool MoveGameWindow(int x, int y)
    {
        try
        {
            if (!IsWindowValid())
            {
                Utils.LogWarning("无法移动窗口，窗口句柄无效", "GameWindow");
                return false;
            }

            // 移动窗口
            bool moved = SetWindowPos(WindowHandle, IntPtr.Zero, 
                x, y, WindowSize.Width, WindowSize.Height, 
                SWP_NOZORDER | SWP_NOACTIVATE);

            if (moved)
            {
                Utils.LogDebug($"游戏窗口位置已移动到: ({x}, {y})", "GameWindow");
                // 更新窗口信息
                UpdateWindowInfo();
                // 触发窗口位置改变事件
                OnWindowPositionChanged(new GameWindowEventArgs(WindowHandle, true));
            }
            else
            {
                Utils.LogWarning("游戏窗口移动失败", "GameWindow");
            }

            return moved;
        }
        catch (Exception ex)
        {
            Utils.LogError("移动游戏窗口时发生异常", ex, "GameWindow");
            return false;
        }
    }

    /// <summary>
    /// 获取游戏窗口中心坐标
    /// </summary>
    /// <returns>中心坐标</returns>
    public Point GetWindowCenter()
    {
        if (!IsWindowValid() || WindowSize.IsEmpty)
        {
            return Point.Empty;
        }

        int centerX = WindowPosition.X + (WindowSize.Width / 2);
        int centerY = WindowPosition.Y + (WindowSize.Height / 2);
        
        Utils.LogDebug($"游戏窗口中心坐标: ({centerX}, {centerY})", "GameWindow");
        return new Point(centerX, centerY);
    }

    /// <summary>
    /// 检查窗口是否有效
    /// </summary>
    /// <returns>窗口是否有效</returns>
    public bool IsWindowValid()
    {
        return WindowHandle != IntPtr.Zero && IsWindow(WindowHandle);
    }

    /// <summary>
    /// 检查窗口是否可见
    /// </summary>
    /// <returns>窗口是否可见</returns>
    public bool IsWindowVisible()
    {
        return IsWindowValid() && IsWindowVisible(WindowHandle);
    }

    /// <summary>
    /// 更新窗口信息
    /// </summary>
    private void UpdateWindowInfo()
    {
        try
        {
            if (!IsWindowValid())
            {
                WindowPosition = Point.Empty;
                WindowSize = Size.Empty;
                WindowRectangle = Rectangle.Empty;
                return;
            }

            // 获取窗口矩形
            RECT rect = new RECT();
            if (GetWindowRect(WindowHandle, ref rect))
            {
                WindowPosition = new Point(rect.Left, rect.Top);
                WindowSize = new Size(rect.Right - rect.Left, rect.Bottom - rect.Top);
                WindowRectangle = new Rectangle(rect.Left, rect.Top, rect.Right - rect.Left, rect.Bottom - rect.Top);

                Utils.LogDebug($"更新窗口信息 - 位置: ({WindowPosition.X}, {WindowPosition.Y}), 大小: {WindowSize.Width}x{WindowSize.Height}", "GameWindow");
            }
        }
        catch (Exception ex)
        {
            Utils.LogError("更新窗口信息时发生异常", ex, "GameWindow");
        }
    }

    /// <summary>
    /// 刷新窗口信息
    /// </summary>
    public void RefreshWindowInfo()
    {
        UpdateWindowInfo();
    }

    /// <summary>
    /// 获取窗口标题
    /// </summary>
    /// <returns>窗口标题</returns>
    public string GetWindowTitle()
    {
        try
        {
            if (!IsWindowValid())
            {
                return string.Empty;
            }

            // 获取窗口标题长度
            int titleLength = GetWindowTextLength(WindowHandle);
            if (titleLength <= 0)
            {
                return string.Empty;
            }

            // 创建缓冲区并获取标题
            StringBuilder titleBuilder = new StringBuilder(titleLength + 1);
            GetWindowText(WindowHandle, titleBuilder, titleBuilder.Capacity);

            string title = titleBuilder.ToString();
            Utils.LogDebug($"获取窗口标题: {title}", "GameWindow");
            return title;
        }
        catch (Exception ex)
        {
            Utils.LogError("获取窗口标题时发生异常", ex, "GameWindow");
            return string.Empty;
        }
    }

    /// <summary>
    /// 设置窗口标题
    /// </summary>
    /// <param name="title">新标题</param>
    /// <returns>是否设置成功</returns>
    public bool SetWindowTitle(string title)
    {
        try
        {
            if (!IsWindowValid() || string.IsNullOrEmpty(title))
            {
                Utils.LogWarning("无法设置窗口标题，窗口句柄无效或标题为空", "GameWindow");
                return false;
            }

            bool success = SetWindowText(WindowHandle, title);
            if (success)
            {
                WindowTitle = title;
                Utils.LogDebug($"游戏窗口标题已设置为: {title}", "GameWindow");
            }
            else
            {
                Utils.LogWarning("游戏窗口标题设置失败", "GameWindow");
            }

            return success;
        }
        catch (Exception ex)
        {
            Utils.LogError("设置窗口标题时发生异常", ex, "GameWindow");
            return false;
        }
    }

    /// <summary>
    /// 获取窗口进程ID
    /// </summary>
    /// <returns>进程ID</returns>
    public int GetWindowProcessId()
    {
        try
        {
            if (!IsWindowValid())
            {
                return 0;
            }

            int processId;
            GetWindowThreadProcessId(WindowHandle, out processId);
            
            Utils.LogDebug($"窗口进程ID: {processId}", "GameWindow");
            return processId;
        }
        catch (Exception ex)
        {
            Utils.LogError("获取窗口进程ID时发生异常", ex, "GameWindow");
            return 0;
        }
    }

    /// <summary>
    /// 触发窗口状态改变事件
    /// </summary>
    /// <param name="e">事件参数</param>
    protected virtual void OnWindowStateChanged(GameWindowEventArgs e)
    {
        WindowStateChanged?.Invoke(this, e);
    }

    /// <summary>
    /// 触发窗口位置改变事件
    /// </summary>
    /// <param name="e">事件参数</param>
    protected virtual void OnWindowPositionChanged(GameWindowEventArgs e)
    {
        WindowPositionChanged?.Invoke(this, e);
    }

    /// <summary>
    /// 触发窗口大小改变事件
    /// </summary>
    /// <param name="e">事件参数</param>
    protected virtual void OnWindowSizeChanged(GameWindowEventArgs e)
    {
        WindowSizeChanged?.Invoke(this, e);
    }

    #region Windows API 导入

    [DllImport("user32.dll", SetLastError = true)]
    private static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

    [DllImport("user32.dll", SetLastError = true)]
    private static extern bool SetForegroundWindow(IntPtr hWnd);

    [DllImport("user32.dll", SetLastError = true)]
    private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

    [DllImport("user32.dll", SetLastError = true)]
    private static extern bool CloseWindow(IntPtr hWnd);

    [DllImport("user32.dll", SetLastError = true)]
    private static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, 
        int X, int Y, int cx, int cy, uint uFlags);

    [DllImport("user32.dll", SetLastError = true)]
    private static extern bool GetWindowRect(IntPtr hWnd, ref RECT lpRect);

    [DllImport("user32.dll", SetLastError = true)]
    private static extern int GetWindowTextLength(IntPtr hWnd);

    [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
    private static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

    [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
    private static extern bool SetWindowText(IntPtr hWnd, string lpString);

    [DllImport("user32.dll", SetLastError = true)]
    private static extern bool IsWindowVisible(IntPtr hWnd);

    [DllImport("user32.dll", SetLastError = true)]
    private static extern bool IsWindow(IntPtr hWnd);

    [DllImport("user32.dll", SetLastError = true)]
    private static extern uint GetWindowThreadProcessId(IntPtr hWnd, out int lpdwProcessId);

    private const int SW_SHOW = 5;
    private const uint SWP_NOZORDER = 0x0004;
    private const uint SWP_NOACTIVATE = 0x0010;

    [StructLayout(LayoutKind.Sequential)]
    private struct RECT
    {
        public int Left;
        public int Top;
        public int Right;
        public int Bottom;
    }

    #endregion
}

/// <summary>
/// 游戏窗口事件参数
/// </summary>
public class GameWindowEventArgs : EventArgs
{
    /// <summary>
    /// 窗口句柄
    /// </summary>
    public IntPtr WindowHandle { get; }

    /// <summary>
    /// 窗口是否有效
    /// </summary>
    public bool IsValid { get; }

    /// <summary>
    /// 事件时间
    /// </summary>
    public DateTime EventTime { get; }

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="windowHandle">窗口句柄</param>
    /// <param name="isValid">窗口是否有效</param>
    public GameWindowEventArgs(IntPtr windowHandle, bool isValid)
    {
        WindowHandle = windowHandle;
        IsValid = isValid;
        EventTime = DateTime.Now;
    }
}