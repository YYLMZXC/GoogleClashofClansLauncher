using System;
using System.Runtime.InteropServices;

namespace GoogleClashofClansLauncher.Input;

/// <summary>
/// 鼠标模拟器类
/// 用于模拟鼠标操作
/// </summary>
public class MouseSimulator
{
    private IntPtr targetWindow = IntPtr.Zero;

    /// <summary>
    /// 设置目标窗口句柄
    /// </summary>
    /// <param name="windowHandle">窗口句柄</param>
    public void SetTargetWindow(IntPtr windowHandle)
    {
        targetWindow = windowHandle;
    }
    // Windows API：鼠标事件
    [DllImport("user32.dll", SetLastError = true)]
    private static extern void mouse_event(uint dwFlags, uint dx, uint dy, uint dwData, IntPtr dwExtraInfo);

    // 鼠标事件标志
    private const uint MOUSEEVENTF_LEFTDOWN = 0x0002;
    private const uint MOUSEEVENTF_LEFTUP = 0x0004;
    private const uint MOUSEEVENTF_RIGHTDOWN = 0x0008;
    private const uint MOUSEEVENTF_RIGHTUP = 0x0010;
    private const uint MOUSEEVENTF_MIDDLEDOWN = 0x0020;
    private const uint MOUSEEVENTF_MIDDLEUP = 0x0040;
    private const uint MOUSEEVENTF_MOVE = 0x0001;
    private const uint MOUSEEVENTF_ABSOLUTE = 0x8000;

    // Windows API：设置鼠标位置
    [DllImport("user32.dll", SetLastError = true)]
    private static extern bool SetCursorPos(int X, int Y);

    /// <summary>
    /// 移动鼠标到指定位置
    /// </summary>
    /// <param name="x">X坐标</param>
    /// <param name="y">Y坐标</param>
    public void MoveMouse(int x, int y)
    {
        SetCursorPos(x, y);
    }

    /// <summary>
    /// 模拟左键单击
    /// </summary>
    public void LeftClick()
    {
        mouse_event(MOUSEEVENTF_LEFTDOWN, 0, 0, 0, IntPtr.Zero);
        System.Threading.Thread.Sleep(50);
        mouse_event(MOUSEEVENTF_LEFTUP, 0, 0, 0, IntPtr.Zero);
    }

    /// <summary>
    /// 模拟右键单击
    /// </summary>
    public void RightClick()
    {
        mouse_event(MOUSEEVENTF_RIGHTDOWN, 0, 0, 0, IntPtr.Zero);
        System.Threading.Thread.Sleep(50);
        mouse_event(MOUSEEVENTF_RIGHTUP, 0, 0, 0, IntPtr.Zero);
    }

    /// <summary>
    /// 模拟中键单击
    /// </summary>
    public void MiddleClick()
    {
        mouse_event(MOUSEEVENTF_MIDDLEDOWN, 0, 0, 0, IntPtr.Zero);
        System.Threading.Thread.Sleep(50);
        mouse_event(MOUSEEVENTF_MIDDLEUP, 0, 0, 0, IntPtr.Zero);
    }

    /// <summary>
    /// 检查鼠标是否在目标窗口内
    /// </summary>
    /// <returns>鼠标是否在目标窗口内</returns>
    public bool IsMouseInTargetWindow()
    {
        if (targetWindow == IntPtr.Zero)
            return false;

        // 获取鼠标当前位置
        if (!GetCursorPos(out POINT cursorPos))
            return false;

        // 获取窗口位置和大小
        if (!GetWindowRect(targetWindow, out RECT windowRect))
            return false;

        // 检查鼠标位置是否在窗口矩形内
        return cursorPos.X >= windowRect.Left && cursorPos.X <= windowRect.Right &&
               cursorPos.Y >= windowRect.Top && cursorPos.Y <= windowRect.Bottom;
    }

    /// <summary>
    /// 移动鼠标到指定位置（与MoveMouse功能相同，为兼容现有代码）
    /// </summary>
    /// <param name="x">X坐标</param>
    /// <param name="y">Y坐标</param>
    public void Move(int x, int y)
    {
        MoveMouse(x, y);
    }

    // Windows API：获取鼠标位置
    [DllImport("user32.dll", SetLastError = true)]
    private static extern bool GetCursorPos(out POINT lpPoint);

    // Windows API：获取窗口矩形
    [DllImport("user32.dll", SetLastError = true)]
    private static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);

    // 点结构
    [StructLayout(LayoutKind.Sequential)]
    private struct POINT
    {
        public int X;
        public int Y;
    }

    // 矩形结构
    [StructLayout(LayoutKind.Sequential)]
    private struct RECT
    {
        public int Left;
        public int Top;
        public int Right;
        public int Bottom;
    }

    /// <summary>
    /// 移动鼠标到指定位置并点击
    /// </summary>
    /// <param name="x">X坐标</param>
    /// <param name="y">Y坐标</param>
    public void MoveAndClick(int x, int y)
    {
        SetCursorPos(x, y);
        System.Threading.Thread.Sleep(100);
        LeftClick();
    }

    /// <summary>
    /// 执行点击测试
    /// </summary>
    /// <param name="cancellationToken">取消令牌</param>
    /// <param name="progressCallback">进度回调函数</param>
    public void ExecuteClickTest(CancellationToken cancellationToken, Action<int> progressCallback = null)
    {
        int clickCount = 0;
        try
        {
            // 示例实现：连续点击5次，每次间隔1秒
            for (int i = 0; i < 5; i++)
            {
                // 检查是否已取消操作
                if (cancellationToken.IsCancellationRequested)
                {
                    return;
                }

                // 执行点击
                LeftClick();
                clickCount++;

                // 调用进度回调
                progressCallback?.Invoke(clickCount);

                // 等待下一次点击
                System.Threading.Thread.Sleep(1000);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("点击测试执行过程中发生错误: " + ex.Message);
        }
    }
}