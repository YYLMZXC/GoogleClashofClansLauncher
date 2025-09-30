
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace GoogleClashofClansLauncher.Core.UI;

public class WindowManager
{
    [DllImport("user32.dll", CharSet = CharSet.Unicode)]
    private static extern IntPtr FindWindow(string? lpClassName, string lpWindowName);

    [DllImport("user32.dll")] private static extern bool SetForegroundWindow(IntPtr hWnd);
    [DllImport("user32.dll")] private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);
    [DllImport("user32.dll")] private static extern IntPtr GetForegroundWindow();
    [DllImport("user32.dll")] private static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);
    [DllImport("user32.dll")] private static extern bool AttachThreadInput(uint idAttach, uint idAttachTo, bool fAttach);
    [DllImport("kernel32.dll")] private static extern uint GetCurrentThreadId();

    private const int SW_RESTORE = 9;

    public static IntPtr FindWindowByTitle(string windowTitle) => FindWindow(null, windowTitle);

    public static bool ActivateWindow(IntPtr h)
    {
        if (h == IntPtr.Zero || GetForegroundWindow() == h) return true;
        try
        {
            ShowWindow(h, SW_RESTORE);
            uint wndThread = GetWindowThreadProcessId(h, out _);
            uint curThread = GetCurrentThreadId();
            if (wndThread != curThread) AttachThreadInput(curThread, wndThread, true);
            bool ok = SetForegroundWindow(h);
            if (wndThread != curThread) AttachThreadInput(curThread, wndThread, false);
            return ok;
        }
        catch (Exception ex)
        {
            Debug.WriteLine("激活窗口出错: " + ex.Message);
            return false;
        }
    }

    public static bool RestoreWindowOnly(IntPtr h)
    {
        if (h == IntPtr.Zero) return false;
        try { return ShowWindow(h, SW_RESTORE); }
        catch (Exception ex)
        {
            Debug.WriteLine("恢复窗口出错: " + ex.Message);
            return false;
        }
    }
}