
using System;
using System.Runtime.InteropServices;

namespace GoogleClashofClansLauncher.Input;

public class KeyboardSimulator
{
    [DllImport("user32.dll")] private static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, IntPtr dwExtraInfo);
    private const uint KEYDOWN = 0x0000;
    private const uint KEYUP = 0x0002;

    public void KeyDown(byte keyCode) => keybd_event(keyCode, 0, KEYDOWN, IntPtr.Zero);
    public void KeyUp(byte keyCode) => keybd_event(keyCode, 0, KEYUP, IntPtr.Zero);
    public void KeyPress(byte keyCode)
    {
        KeyDown(keyCode);
        System.Threading.Thread.Sleep(50);
        KeyUp(keyCode);
    }
    public void TypeText(string text)
    {
        foreach (char c in text)
        {
            byte k = (byte)char.ToUpper(c);
            KeyPress(k);
            System.Threading.Thread.Sleep(30);
        }
    }
}