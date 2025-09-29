using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Diagnostics;

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
    private int _defaultClickDelay = 333; // 默认点击延迟（毫秒）

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
            InputSimulator.PostMessage(_targetWindowHandle, (uint)InputSimulator.WM_MOUSEMOVE,
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
        LeftClick(_defaultClickDelay);
    }

    /// <summary>
    /// 左键单击（带延迟参数）
    /// </summary>
    /// <param name="delayAfterClick">点击后延迟时间（毫秒）</param>
    public void LeftClick(int delayAfterClick)
    {
        if (_lockToWindow && _targetWindowHandle != IntPtr.Zero)
        {
            // 向特定窗口发送鼠标点击消息
            InputSimulator.PostMessage(_targetWindowHandle, (uint)InputSimulator.WM_LBUTTONDOWN,
                (IntPtr)0x0001, GetCurrentClientCursorPos());
            Thread.Sleep(50);
            InputSimulator.PostMessage(_targetWindowHandle, (uint)InputSimulator.WM_LBUTTONUP,
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
        
        // 点击后延迟
        if (delayAfterClick > 0)
        {
            Thread.Sleep(delayAfterClick);
        }
    }

    /// <summary>
    /// 在指定位置左键单击
    /// </summary>
    /// <param name="x">目标X坐标</param>
    /// <param name="y">目标Y坐标</param>
    public void LeftClickAtPosition(int x, int y)
    {
        LeftClickAtPosition(x, y, _defaultClickDelay);
    }

    /// <summary>
    /// 在指定位置左键单击（带延迟参数）
    /// </summary>
    /// <param name="x">目标X坐标</param>
    /// <param name="y">目标Y坐标</param>
    /// <param name="delayAfterClick">点击后延迟时间（毫秒）</param>
    public void LeftClickAtPosition(int x, int y, int delayAfterClick)
    {
        if (_lockToWindow && _targetWindowHandle != IntPtr.Zero)
        {
            // 向特定窗口的指定位置发送鼠标点击消息
            InputSimulator.PostMessage(_targetWindowHandle, (uint)InputSimulator.WM_LBUTTONDOWN,
                (IntPtr)0x0001, MakeLParam(x, y));
            Thread.Sleep(50);
            InputSimulator.PostMessage(_targetWindowHandle, (uint)InputSimulator.WM_LBUTTONUP,
                IntPtr.Zero, MakeLParam(x, y));
        }
        else
        {
            // 移动到指定位置
            Move(x, y);
            Thread.Sleep(50); // 移动后稍作停顿
            
            // 点击操作
            var downInput = new InputSimulator.INPUT[1];
            downInput[0].type = InputSimulator.INPUT_MOUSE;
            downInput[0].u.mi.dwFlags = MOUSEEVENTF_LEFTDOWN;
            InputSimulator.SendInput(1, downInput, Marshal.SizeOf(typeof(InputSimulator.INPUT)));

            Thread.Sleep(50);

            var upInput = new InputSimulator.INPUT[1];
            upInput[0].type = InputSimulator.INPUT_MOUSE;
            upInput[0].u.mi.dwFlags = MOUSEEVENTF_LEFTUP;
            InputSimulator.SendInput(1, upInput, Marshal.SizeOf(typeof(InputSimulator.INPUT)));
        }
        
        // 点击后延迟
        if (delayAfterClick > 0)
        {
            Thread.Sleep(delayAfterClick);
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
            InputSimulator.PostMessage(_targetWindowHandle, (uint)InputSimulator.WM_RBUTTONDOWN,
                (IntPtr)0x0002, GetCurrentClientCursorPos());
            Thread.Sleep(50);
            InputSimulator.PostMessage(_targetWindowHandle, (uint)InputSimulator.WM_RBUTTONUP,
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

    /// <summary>
    /// 点击游戏窗口中心位置
    /// </summary>
    public void ClickGameCenter()
    {
        if (_lockToWindow && _targetWindowHandle != IntPtr.Zero)
        {
            // 获取窗口大小
            RECT windowRect;
            GetWindowRect(_targetWindowHandle, out windowRect);
            
            // 计算窗口中心坐标（相对于窗口客户区）
            int centerX = (windowRect.right - windowRect.left) / 2;
            int centerY = (windowRect.bottom - windowRect.top) / 2;
            
            // 点击中心位置
            LeftClickAtPosition(centerX, centerY);
        }
    }

    /// <summary>
    /// 执行点击模拟测试（1秒3次，持续10秒）
    /// </summary>
    /// <param name="token">取消令牌</param>
    /// <param name="progressCallback">进度回调函数</param>
    public void ExecuteClickTest(CancellationToken token, Action<int> progressCallback = null)
    {
        // 记录开始时间
        DateTime startTime = DateTime.Now;
        int clickCount = 0;
        
        // 持续10秒或直到取消
        while ((DateTime.Now - startTime).TotalSeconds < 10 && !token.IsCancellationRequested)
        {
            // 1秒内点击3次（间隔约333毫秒）
            for (int i = 0; i < 3; i++)
            {
                if (token.IsCancellationRequested) break;
                
                // 每次点击前检查鼠标是否在目标窗口内
                if (!IsMouseInTargetWindow())
                {
                    // 等待最多2秒，让用户有时间将鼠标移回窗口
                    DateTime waitStart = DateTime.Now;
                    while (!IsMouseInTargetWindow() && 
                           (DateTime.Now - waitStart).TotalSeconds < 2 && 
                           !token.IsCancellationRequested)
                    {
                        Thread.Sleep(100);
                    }
                    
                    if (token.IsCancellationRequested) break;
                    if (!IsMouseInTargetWindow())
                    {
                        break;
                    }
                }
                
                // 点击游戏屏幕中心
                ClickGameCenter();
                clickCount++;
                
                // 报告进度
                progressCallback?.Invoke(clickCount);
            }
        }
    }

    /// <summary>
    /// 设置默认点击延迟
    /// </summary>
    /// <param name="delay">延迟时间（毫秒）</param>
    public void SetDefaultClickDelay(int delay)
    {
        if (delay >= 0)
        {
            _defaultClickDelay = delay;
        }
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

    [DllImport("user32.dll")]
    private static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);

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

    // 矩形结构体
    [StructLayout(LayoutKind.Sequential)]
    private struct RECT
    {
        public int left;
        public int top;
        public int right;
        public int bottom;
    }
}