using System;
using System.Runtime.InteropServices;

namespace GoogleClashofClansLauncher.Input;

/// <summary>
/// 键盘模拟器类
/// 用于模拟键盘操作
/// </summary>
public class KeyboardSimulator
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
    // Windows API：键盘事件
    [DllImport("user32.dll", SetLastError = true)]
    private static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, IntPtr dwExtraInfo);

    // 键盘事件标志
    private const uint KEYEVENTF_KEYDOWN = 0x0000;
    private const uint KEYEVENTF_KEYUP = 0x0002;

    /// <summary>
    /// 模拟按键按下
    /// </summary>
    /// <param name="keyCode">按键代码</param>
    public void KeyDown(byte keyCode)
    {
        keybd_event(keyCode, 0, KEYEVENTF_KEYDOWN, IntPtr.Zero);
    }

    /// <summary>
    /// 模拟按键释放
    /// </summary>
    /// <param name="keyCode">按键代码</param>
    public void KeyUp(byte keyCode)
    {
        keybd_event(keyCode, 0, KEYEVENTF_KEYUP, IntPtr.Zero);
    }

    /// <summary>
    /// 模拟按键单击
    /// </summary>
    /// <param name="keyCode">按键代码</param>
    public void KeyPress(byte keyCode)
    {
        KeyDown(keyCode);
        System.Threading.Thread.Sleep(50);
        KeyUp(keyCode);
    }

    /// <summary>
    /// 模拟组合键按下
    /// </summary>
    /// <param name="keyCodes">按键代码数组</param>
    public void CombinationKeyPress(params byte[] keyCodes)
    {
        // 按下所有键
        foreach (byte keyCode in keyCodes)
        {
            KeyDown(keyCode);
        }

        // 短暂延迟
        System.Threading.Thread.Sleep(100);

        // 释放所有键（反向顺序释放）
        for (int i = keyCodes.Length - 1; i >= 0; i--)
        {
            KeyUp(keyCodes[i]);
        }
    }

    /// <summary>
    /// 模拟输入文本
    /// </summary>
    /// <param name="text">要输入的文本</param>
    public void TypeText(string text)
    {
        foreach (char c in text)
        {
            // 这里简化处理，实际应用中可能需要根据字符映射到正确的按键代码
            // 可以使用 ToAscii 等 API 实现更复杂的字符转换
            byte keyCode = (byte)char.ToUpper(c);
            KeyPress(keyCode);
            System.Threading.Thread.Sleep(30);
        }
    }
}