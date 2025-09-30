
using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;

namespace GoogleClashofClansLauncher.Input;

public sealed class MouseSimulator : IDisposable
{
    [DllImport("user32.dll")] private static extern void mouse_event(uint flags, uint dx, uint dy, uint data, IntPtr extra);
    [DllImport("user32.dll")] private static extern bool SetCursorPos(int x, int y);

    private const uint MOVE = 0x0001;
    private const uint LEFTDOWN = 0x0002, LEFTUP = 0x0004;

    public static void Move(int x, int y) => SetCursorPos(x, y);
    public static void LeftClick()
    {
        mouse_event(LEFTDOWN, 0, 0, 0, IntPtr.Zero);
        Thread.Sleep(20);
        mouse_event(LEFTUP, 0, 0, 0, IntPtr.Zero);
    }
    public static void ClickTest(CancellationToken token = default)
    {
        var (cX, cY) = (SystemInformation.PrimaryMonitorSize.Width / 2,
                        SystemInformation.PrimaryMonitorSize.Height / 2);
        Move(cX, cY);
        for (int i = 0; i < 30 && !token.IsCancellationRequested; i++)
        {
            LeftClick();
            Thread.Sleep(330);
        }
    }
    public void Dispose() { }
}