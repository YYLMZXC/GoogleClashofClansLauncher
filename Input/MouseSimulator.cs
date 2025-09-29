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

    /// <summary>
    /// 移动鼠标到指定坐标
    /// </summary>
    /// <param name="x">目标X坐标</param>
    /// <param name="y">目标Y坐标</param>
    public void Move(int x, int y)
    {
        var input = new InputSimulator.INPUT[1];
        input[0].type = InputSimulator.INPUT_MOUSE;
        input[0].u.mi.dx = x;
        input[0].u.mi.dy = y;
        input[0].u.mi.dwFlags = MOUSEEVENTF_MOVE;

        InputSimulator.SendInput(1, input, Marshal.SizeOf(typeof(InputSimulator.INPUT)));
    }

    /// <summary>
    /// 左键单击
    /// </summary>
    public void LeftClick()
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

    /// <summary>
    /// 右键单击
    /// </summary>
    public void RightClick()
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