using System;
using System.Runtime.InteropServices;
using System.Threading;

namespace GoogleClashofClansLauncher.Input;

/// <summary>
/// 鼠标操作模拟器
/// </summary>
public class MouseSimulator
{
    // 鼠标事件常量
    private const uint MOUSEEVENTF_MOVE = 0x0001;
    private const uint MOUSEEVENTF_LEFTDOWN = 0x0002;
    private const uint MOUSEEVENTF_LEFTUP = 0x0004;
    private const uint MOUSEEVENTF_RIGHTDOWN = 0x0008;
    private const uint MOUSEEVENTF_RIGHTUP = 0x0010;
    
    private IntPtr _targetWindowHandle; // 目标窗口句柄
    private bool _lockToWindow; // 是否锁定到特定窗口

    /// <summary>
    /// 默认构造函数（不锁定窗口）
    /// </summary>
    public MouseSimulator()
    {
        _lockToWindow = false;
        _targetWindowHandle = IntPtr.Zero;
    }

    /// <summary>
    /// 构造函数（锁定到特定窗口）
    /// </summary>
    /// <param name="windowHandle">目标窗口句柄</param>
    public MouseSimulator(IntPtr windowHandle)
    {
        _lockToWindow = true;
        _targetWindowHandle = windowHandle;
    }

    /// <summary>
    /// 设置目标窗口句柄
    /// </summary>
    /// <param name="windowHandle">目标窗口句柄</param>
    public void SetTargetWindow(IntPtr windowHandle)
    {
        _lockToWindow = true;
        _targetWindowHandle = windowHandle;
    }

    /// <summary>
    /// 解除窗口锁定
    /// </summary>
    public void UnlockWindow()
    {
        _lockToWindow = false;
        _targetWindowHandle = IntPtr.Zero;
    }

    /// <summary>
    /// 移动鼠标到指定坐标
    /// </summary>
    /// <param name="x">目标X坐标</param>
    /// <param name="y">目标Y坐标</param>
    public void Move(int x, int y)
    {
        if (_lockToWindow && _targetWindowHandle != IntPtr.Zero)
        {
            // 向特定窗口发送鼠标移动消息
            // 注意：PostMessage的坐标是相对于窗口客户区的
            InputSimulator.PostMessage(_targetWindowHandle, InputSimulator.WM_MOUSEMOVE,
                IntPtr.Zero, MakeLParam(x, y));
        }
        else
        {
            var input = new InputSimulator.INPUT[1];
            input[0].type = InputSimulator.INPUT_MOUSE;
            input[0].u.mi.dx = x;
            input[0].u.mi.dy = y;
            input[0].u.mi.dwFlags = MOUSEEVENTF_MOVE;

            InputSimulator.SendInput(1, input, Marshal.SizeOf(typeof(InputSimulator.INPUT)));
        }
    }

    /// <summary>
    /// 左键单击
    /// </summary>
    public void LeftClick()
    {
        if (_lockToWindow && _targetWindowHandle != IntPtr.Zero)
        {
            // 向特定窗口发送鼠标点击消息
            InputSimulator.PostMessage(_targetWindowHandle, InputSimulator.WM_LBUTTONDOWN,
                (IntPtr)0x0001, GetCurrentClientCursorPos());
            Thread.Sleep(50);
            InputSimulator.PostMessage(_targetWindowHandle, InputSimulator.WM_LBUTTONUP,
                IntPtr.Zero, GetCurrentClientCursorPos());
        }
        else
        {
            // 按下左键
            var downInput = new InputSimulator.INPUT[1];
            downInput[0].type = InputSimulator.INPUT_MOUSE;
            downInput[0].u.mi.dwFlags = MOUSEEVENTF_LEFTDOWN;
            InputSimulator.SendInput(1, downInput, Marshal.SizeOf(typeof(InputSimulator.INPUT)));

            Thread.Sleep(50); // 模拟实际点击延迟

            // 释放左键
            var upInput = new InputSimulator.INPUT[1];
            upInput[0].type = InputSimulator.INPUT_MOUSE;
            upInput[0].u.mi.dwFlags = MOUSEEVENTF_LEFTUP;
            InputSimulator.SendInput(1, upInput, Marshal.SizeOf(typeof(InputSimulator.INPUT)));
        }
    }

    /// <summary>
    /// 右键单击
    /// </summary>
    public void RightClick()
    {
        if (_lockToWindow && _targetWindowHandle != IntPtr.Zero)
        {
            // 向特定窗口发送右键点击消息
            InputSimulator.PostMessage(_targetWindowHandle, InputSimulator.WM_RBUTTONDOWN,
                (IntPtr)0x0002, GetCurrentClientCursorPos());
            Thread.Sleep(50);
            InputSimulator.PostMessage(_targetWindowHandle, InputSimulator.WM_RBUTTONUP,
                IntPtr.Zero, GetCurrentClientCursorPos());
        }
        else
        {
            // 按下右键
            var downInput = new InputSimulator.INPUT[1];
            downInput[0].type = InputSimulator.INPUT_MOUSE;
            downInput[0].u.mi.dwFlags = MOUSEEVENTF_RIGHTDOWN;
            InputSimulator.SendInput(1, downInput, Marshal.SizeOf(typeof(InputSimulator.INPUT)));

            Thread.Sleep(50);

            // 释放右键
            var upInput = new InputSimulator.INPUT[1];
            upInput[0].type = InputSimulator.INPUT_MOUSE;
            upInput[0].u.mi.dwFlags = MOUSEEVENTF_RIGHTUP;
            InputSimulator.SendInput(1, upInput, Marshal.SizeOf(typeof(InputSimulator.INPUT)));
        }
    }

    /// <summary>
    /// 检查鼠标是否在目标窗口内
    /// </summary>
    /// <returns>如果鼠标在目标窗口内返回true，否则返回false</returns>
    public bool IsMouseInTargetWindow()
    {
        if (!_lockToWindow || _targetWindowHandle == IntPtr.Zero)
        {
            return true; // 未锁定窗口时默认返回true
        }

        // 获取鼠标屏幕位置
        GetCursorPos(out POINT cursorPos);
        
        // 检查鼠标位置是否在目标窗口内
        return WindowFromPoint(cursorPos) == _targetWindowHandle;
    }

    // 辅助方法：创建鼠标坐标参数
    private IntPtr MakeLParam(int low, int high)
    {
        return (IntPtr)((high << 16) | (low & 0xFFFF));
    }

    // 辅助方法：获取当前光标在窗口客户区的位置
    private IntPtr GetCurrentClientCursorPos()
    {
        if (_lockToWindow && _targetWindowHandle != IntPtr.Zero)
        {
            // 获取鼠标屏幕位置
            GetCursorPos(out POINT cursorPos);
            
            // 将屏幕坐标转换为窗口客户区坐标
            ScreenToClient(_targetWindowHandle, ref cursorPos);
            
            return MakeLParam(cursorPos.X, cursorPos.Y);
        }
        
        // 如果未锁定窗口或窗口句柄无效，返回(0,0)
        return MakeLParam(0, 0);
    }

    // Windows API声明
    [DllImport("user32.dll")]
    private static extern bool GetCursorPos(out POINT lpPoint);

    [DllImport("user32.dll")]
    private static extern bool ScreenToClient(IntPtr hWnd, ref POINT lpPoint);

    [DllImport("user32.dll")]
    private static extern IntPtr WindowFromPoint(POINT Point);

    // 点结构体
    [StructLayout(LayoutKind.Sequential)]
    private struct POINT
    {
        public int X;
        public int Y;

        public POINT(int x, int y)
        {
            this.X = x;
            this.Y = y;
        }
    }
}