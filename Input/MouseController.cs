using System;
using System.Drawing;
using System.Runtime.InteropServices;
using GoogleClashofClansLauncher.Core;
using GoogleClashofClansLauncher.Core.System;

namespace GoogleClashofClansLauncher.Input;

/// <summary>
/// 鼠标控制器类
/// 提供所有鼠标操作的控制功能
/// </summary>
public class MouseController
{
    // Windows API 常量
    private const int MOUSEEVENTF_MOVE = 0x0001;
    private const int MOUSEEVENTF_LEFTDOWN = 0x0002;
    private const int MOUSEEVENTF_LEFTUP = 0x0004;
    private const int MOUSEEVENTF_RIGHTDOWN = 0x0008;
    private const int MOUSEEVENTF_RIGHTUP = 0x0010;
    private const int MOUSEEVENTF_MIDDLEDOWN = 0x0020;
    private const int MOUSEEVENTF_MIDDLEUP = 0x0040;
    private const int MOUSEEVENTF_ABSOLUTE = 0x8000;

    // Windows API 导入
    [DllImport("user32.dll")]
    private static extern bool SetCursorPos(int x, int y);

    [DllImport("user32.dll")]
    private static extern bool GetCursorPos(out Point lpPoint);

    [DllImport("user32.dll")]
    private static extern void mouse_event(uint dwFlags, uint dx, uint dy, uint dwData, int dwExtraInfo);

    [DllImport("user32.dll")]
    private static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

    /// <summary>
    /// 获取当前鼠标位置
    /// </summary>
    /// <returns>鼠标位置点</returns>
    public Point GetMousePosition()
    {
        Point position;
        if (GetCursorPos(out position))
        {
            return position;
        }
        return Point.Empty;
    }

    /// <summary>
    /// 设置鼠标位置
    /// </summary>
    /// <param name="x">X坐标</param>
    /// <param name="y">Y坐标</param>
    /// <returns>操作是否成功</returns>
    public bool SetMousePosition(int x, int y)
    {
        return SetMousePosition(new Point(x, y));
    }

    /// <summary>
    /// 设置鼠标位置
    /// </summary>
    /// <param name="position">位置点</param>
    /// <returns>操作是否成功</returns>
    public bool SetMousePosition(Point position)
    {
        return Utils.SafeExecute(() => SetCursorPos(position.X, position.Y), "设置鼠标位置失败", "MouseController");
    }

    /// <summary>
    /// 相对于当前位置移动鼠标
    /// </summary>
    /// <param name="deltaX">X轴移动距离</param>
    /// <param name="deltaY">Y轴移动距离</param>
    /// <returns>操作是否成功</returns>
    public bool MoveMouseRelative(int deltaX, int deltaY)
    {
        Point currentPos = GetMousePosition();
        if (currentPos == Point.Empty)
            return false;

        return SetMousePosition(currentPos.X + deltaX, currentPos.Y + deltaY);
    }

    /// <summary>
    /// 执行左键单击
    /// </summary>
    /// <returns>操作是否成功</returns>
    public bool LeftClick()
    {
        return Utils.SafeExecute(() =>
        {
            mouse_event(MOUSEEVENTF_LEFTDOWN, 0, 0, 0, 0);
            Utils.Wait(Constants.DefaultClickDelay);
            mouse_event(MOUSEEVENTF_LEFTUP, 0, 0, 0, 0);
        }, "执行左键单击失败", "MouseController");
    }

    /// <summary>
    /// 执行右键单击
    /// </summary>
    /// <returns>操作是否成功</returns>
    public bool RightClick()
    {
        return Utils.SafeExecute(() =>
        {
            mouse_event(MOUSEEVENTF_RIGHTDOWN, 0, 0, 0, 0);
            Utils.Wait(Constants.DefaultClickDelay);
            mouse_event(MOUSEEVENTF_RIGHTUP, 0, 0, 0, 0);
        }, "执行右键单击失败", "MouseController");
    }

    /// <summary>
    /// 执行中键单击
    /// </summary>
    /// <returns>操作是否成功</returns>
    public bool MiddleClick()
    {
        return Utils.SafeExecute(() =>
        {
            mouse_event(MOUSEEVENTF_MIDDLEDOWN, 0, 0, 0, 0);
            Utils.Wait(Constants.DefaultClickDelay);
            mouse_event(MOUSEEVENTF_MIDDLEUP, 0, 0, 0, 0);
        }, "执行中键单击失败", "MouseController");
    }

    /// <summary>
    /// 在指定位置执行左键单击
    /// </summary>
    /// <param name="x">X坐标</param>
    /// <param name="y">Y坐标</param>
    /// <returns>操作是否成功</returns>
    public bool LeftClickAt(int x, int y)
    {
        return LeftClickAt(new Point(x, y));
    }

    /// <summary>
    /// 在指定位置执行左键单击
    /// </summary>
    /// <param name="position">位置点</param>
    /// <returns>操作是否成功</returns>
    public bool LeftClickAt(Point position)
    {
        Point originalPos = GetMousePosition();
        if (originalPos == Point.Empty)
            return false;

        if (!SetMousePosition(position))
            return false;

        if (!LeftClick())
        {
            SetMousePosition(originalPos); // 尝试恢复位置
            return false;
        }

        // 可选：点击后恢复到原始位置
        SetMousePosition(originalPos);
        return true;
    }

    /// <summary>
    /// 在指定窗口内执行点击
    /// </summary>
    /// <param name="hWnd">窗口句柄</param>
    /// <param name="x">相对于窗口的X坐标</param>
    /// <param name="y">相对于窗口的Y坐标</param>
    /// <returns>操作是否成功</returns>
    public bool ClickInWindow(IntPtr hWnd, int x, int y)
    {
        return Utils.SafeExecute(() =>
        {
            // 计算相对于窗口的点击位置
            int lParam = y << 16 | x;
            SendMessage(hWnd, 0x0201, IntPtr.Zero, (IntPtr)lParam); // WM_LBUTTONDOWN
            Utils.Wait(Constants.DefaultClickDelay);
            SendMessage(hWnd, 0x0202, IntPtr.Zero, (IntPtr)lParam); // WM_LBUTTONUP
        }, "在窗口内执行点击失败", "MouseController");
    }

    /// <summary>
    /// 执行双击操作
    /// </summary>
    /// <returns>操作是否成功</returns>
    public bool DoubleClick()
    {
        return Utils.SafeExecute(() =>
        {
            LeftClick();
            Utils.Wait(Constants.DefaultDoubleClickDelay);
            LeftClick();
        }, "执行双击操作失败", "MouseController");
    }

    /// <summary>
    /// 执行连续多次点击
    /// </summary>
    /// <param name="count">点击次数</param>
    /// <param name="intervalMs">点击间隔毫秒数</param>
    /// <returns>操作是否成功</returns>
    public bool ClickMultipleTimes(int count, int intervalMs = 1000)
    {
        if (count <= 0)
            return true;

        return Utils.SafeExecute(() =>
        {
            for (int i = 0; i < count; i++)
            {
                if (!LeftClick())
                {
                    // If LeftClick fails, SafeExecute in LeftClick would have already logged the error.
                    // We can throw an exception to stop the execution of this SafeExecute block.
                    throw new Exception("LeftClick failed during multiple clicks.");
                }

                if (i < count - 1)
                    Utils.Wait(intervalMs);
            }
        }, "执行连续点击失败", "MouseController");
    }

    /// <summary>
    /// 执行拖拽操作
    /// </summary>
    /// <param name="fromPoint">起始点</param>
    /// <param name="toPoint">结束点</param>
    /// <returns>操作是否成功</returns>
    public bool DragAndDrop(Point fromPoint, Point toPoint)
    {
        Point originalPos = GetMousePosition();
        if (originalPos == Point.Empty)
            return false;

        return Utils.SafeExecute(() =>
        {
            // 移动到起始位置
            SetMousePosition(fromPoint);
            Utils.Wait(Constants.DefaultDragDelay);
            
            // 按下左键
            mouse_event(MOUSEEVENTF_LEFTDOWN, 0, 0, 0, 0);
            Utils.Wait(Constants.DefaultDragDelay);
            
            // 移动到结束位置
            SetMousePosition(toPoint);
            Utils.Wait(Constants.DefaultDragDelay);
            
            // 释放左键
            mouse_event(MOUSEEVENTF_LEFTUP, 0, 0, 0, 0);
        }, "执行拖拽操作失败", "MouseController");
    }
}