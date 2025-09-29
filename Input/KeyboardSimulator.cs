using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Diagnostics;

namespace GoogleClashofClansLauncher.Input;

/// <summary>
/// 键盘操作模拟器
/// </summary>
public class KeyboardSimulator
{
    // 键盘事件常量
    private const uint KEYEVENTF_KEYDOWN = 0x0000;
    private const uint KEYEVENTF_KEYUP = 0x0002;

    /// <summary>
    /// 按下并释放指定键
    /// </summary>
    /// <param name="keyCode">虚拟键码</param>
    public void PressKey(VirtualKeyCode keyCode)
    {
        // 按下键
        var downInput = new InputSimulator.INPUT[1];
        downInput[0].type = InputSimulator.INPUT_KEYBOARD;
        downInput[0].u.ki.wVk = (ushort)keyCode;
        downInput[0].u.ki.dwFlags = KEYEVENTF_KEYDOWN;
        uint result = InputSimulator.SendInput(1, downInput, Marshal.SizeOf(typeof(InputSimulator.INPUT)));

        // 验证输入是否成功发送
        if (result == 0)
        {
            int error = Marshal.GetLastWin32Error();
            Debug.WriteLine("按键按下失败，错误码: " + error);
        }

        Thread.Sleep(10); // 降低延迟，从50ms改为10ms

        // 释放键
        var upInput = new InputSimulator.INPUT[1];
        upInput[0].type = InputSimulator.INPUT_KEYBOARD;
        upInput[0].u.ki.wVk = (ushort)keyCode;
        upInput[0].u.ki.dwFlags = KEYEVENTF_KEYUP;
        result = InputSimulator.SendInput(1, upInput, Marshal.SizeOf(typeof(InputSimulator.INPUT)));

        if (result == 0)
        {
            int error = Marshal.GetLastWin32Error();
            Debug.WriteLine("按键释放失败，错误码: " + error);
        }
    }

    /// <summary>
    /// 直接按下键（不释放）
    /// </summary>
    private void KeyDown(VirtualKeyCode keyCode)
    {
        var downInput = new InputSimulator.INPUT[1];
        downInput[0].type = InputSimulator.INPUT_KEYBOARD;
        downInput[0].u.ki.wVk = (ushort)keyCode;
        downInput[0].u.ki.dwFlags = KEYEVENTF_KEYDOWN;
        InputSimulator.SendInput(1, downInput, Marshal.SizeOf(typeof(InputSimulator.INPUT)));
    }

    /// <summary>
    /// 直接释放键
    /// </summary>
    private void KeyUp(VirtualKeyCode keyCode)
    {
        var upInput = new InputSimulator.INPUT[1];
        upInput[0].type = InputSimulator.INPUT_KEYBOARD;
        upInput[0].u.ki.wVk = (ushort)keyCode;
        upInput[0].u.ki.dwFlags = KEYEVENTF_KEYUP;
        InputSimulator.SendInput(1, upInput, Marshal.SizeOf(typeof(InputSimulator.INPUT)));
    }

    /// <summary>
    /// 输入文本（仅支持基本字符）
    /// </summary>
    /// <param name="text">要输入的文本</param>
    public void TypeText(string text)
    {
        foreach (var c in text)
        {
            var keyCode = GetVirtualKeyCode(c);
            if (keyCode == VirtualKeyCode.NONE)
                continue;

            // 判断是否需要Shift键（大写或特殊符号）
            var needShift = char.IsUpper(c) || IsShiftRequiredSymbol(c);

            if (needShift)
                PressKeyWithShift(keyCode);
            else
                PressKey(keyCode);

            Thread.Sleep(10); // 降低延迟，从50ms改为10ms
        }
    }

    /// <summary>
    /// 获取字符对应的虚拟键码
    /// </summary>
    private VirtualKeyCode GetVirtualKeyCode(char c)
    {
        return char.ToUpper(c) switch
        {
            'A' => VirtualKeyCode.VK_A,
            'B' => VirtualKeyCode.VK_B,
            'C' => VirtualKeyCode.VK_C,
            'D' => VirtualKeyCode.VK_D,
            'E' => VirtualKeyCode.VK_E,
            'F' => VirtualKeyCode.VK_F,
            'G' => VirtualKeyCode.VK_G,
            'H' => VirtualKeyCode.VK_H,
            'I' => VirtualKeyCode.VK_I,
            'J' => VirtualKeyCode.VK_J,
            'K' => VirtualKeyCode.VK_K,
            'L' => VirtualKeyCode.VK_L,
            'M' => VirtualKeyCode.VK_M,
            'N' => VirtualKeyCode.VK_N,
            'O' => VirtualKeyCode.VK_O,
            'P' => VirtualKeyCode.VK_P,
            'Q' => VirtualKeyCode.VK_Q,
            'R' => VirtualKeyCode.VK_R,
            'S' => VirtualKeyCode.VK_S,
            'T' => VirtualKeyCode.VK_T,
            'U' => VirtualKeyCode.VK_U,
            'V' => VirtualKeyCode.VK_V,
            'W' => VirtualKeyCode.VK_W,
            'X' => VirtualKeyCode.VK_X,
            'Y' => VirtualKeyCode.VK_Y,
            'Z' => VirtualKeyCode.VK_Z,
            '0' => VirtualKeyCode.VK_0,
            '1' => VirtualKeyCode.VK_1,
            '2' => VirtualKeyCode.VK_2,
            '3' => VirtualKeyCode.VK_3,
            '4' => VirtualKeyCode.VK_4,
            '5' => VirtualKeyCode.VK_5,
            '6' => VirtualKeyCode.VK_6,
            '7' => VirtualKeyCode.VK_7,
            '8' => VirtualKeyCode.VK_8,
            '9' => VirtualKeyCode.VK_9,
            ' ' => VirtualKeyCode.SPACE,
            '.' => VirtualKeyCode.OEM_PERIOD,
            ',' => VirtualKeyCode.OEM_COMMA,
            _ => VirtualKeyCode.NONE
        };
    }

    /// <summary>
    /// 判断符号是否需要Shift键
    /// </summary>
    private bool IsShiftRequiredSymbol(char c)
    {
        return "!@#$%^&*()_+{}:\"<>?~|".Contains(c);
    }

    /// <summary>
    /// 按住Shift键按下指定键
    /// </summary>
    private void PressKeyWithShift(VirtualKeyCode keyCode)
    {
        // 修正的实现：先按下Shift，然后按下目标键，最后释放Shift
        KeyDown(VirtualKeyCode.SHIFT);
        Thread.Sleep(5);
        KeyDown(keyCode);
        Thread.Sleep(10);
        KeyUp(keyCode);
        Thread.Sleep(5);
        KeyUp(VirtualKeyCode.SHIFT);
    }
}

/// <summary>
/// Windows虚拟键码枚举
/// </summary>
public enum VirtualKeyCode : ushort
{
    NONE = 0,
    VK_A = 0x41,
    VK_B = 0x42,
    VK_C = 0x43,
    VK_D = 0x44,
    VK_E = 0x45,
    VK_F = 0x46,
    VK_G = 0x47,
    VK_H = 0x48,
    VK_I = 0x49,
    VK_J = 0x4A,
    VK_K = 0x4B,
    VK_L = 0x4C,
    VK_M = 0x4D,
    VK_N = 0x4E,
    VK_O = 0x4F,
    VK_P = 0x50,
    VK_Q = 0x51,
    VK_R = 0x52,
    VK_S = 0x53,
    VK_T = 0x54,
    VK_U = 0x55,
    VK_V = 0x56,
    VK_W = 0x57,
    VK_X = 0x58,
    VK_Y = 0x59,
    VK_Z = 0x5A,
    VK_0 = 0x30,
    VK_1 = 0x31,
    VK_2 = 0x32,
    VK_3 = 0x33,
    VK_4 = 0x34,
    VK_5 = 0x35,
    VK_6 = 0x36,
    VK_7 = 0x37,
    VK_8 = 0x38,
    VK_9 = 0x39,
    RETURN = 0x0D,
    SHIFT = 0x10,
    SPACE = 0x20,
    OEM_PERIOD = 0xBE,
    OEM_COMMA = 0xBC
}