#pragma warning disable CA1416
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using GoogleClashofClansLauncher.Input;

namespace GoogleClashofClansLauncher.Input;

public sealed class ImageRecognition : IDisposable
{
    #region win32
    [DllImport("user32.dll")] static extern IntPtr GetDC(IntPtr hWnd);
    [DllImport("user32.dll")] static extern int ReleaseDC(IntPtr hWnd, IntPtr hDC);
    [DllImport("gdi32.dll")] static extern IntPtr CreateCompatibleDC(IntPtr hDC);
    [DllImport("gdi32.dll")] static extern bool DeleteDC(IntPtr hDC);
    [DllImport("gdi32.dll")] static extern IntPtr CreateCompatibleBitmap(IntPtr hDC, int w, int h);
    [DllImport("gdi32.dll")] static extern bool DeleteObject(IntPtr hObj);
    [DllImport("gdi32.dll")] static extern IntPtr SelectObject(IntPtr hDC, IntPtr hObj);
    [DllImport("gdi32.dll")]
    static extern bool BitBlt(IntPtr destDC, int dx, int dy, int w, int h,
                                                         IntPtr srcDC, int sx, int sy, int rop);
    [DllImport("user32.dll")] static extern int GetSystemMetrics(int idx);
    private const int SRCCOPY = 0x00CC0020;
    private const int SM_CXSCREEN = 0;
    private const int SM_CYSCREEN = 1;
    #endregion

    private readonly MouseSimulator _mouse = new();
    private bool _disposed;

    public static Bitmap CaptureScreen()
    {
        int w = GetSystemMetrics(SM_CXSCREEN);
        int h = GetSystemMetrics(SM_CYSCREEN);
        IntPtr hScreen = GetDC(IntPtr.Zero);
        if (hScreen == IntPtr.Zero)
        {
            throw new InvalidOperationException("获取屏幕设备上下文失败！");
        }

        IntPtr hMem = CreateCompatibleDC(hScreen);
        if (hMem == IntPtr.Zero)
        {
            ReleaseDC(IntPtr.Zero, hScreen);
            throw new InvalidOperationException("创建内存设备上下文失败！");
        }

        IntPtr hBmp = CreateCompatibleBitmap(hScreen, w, h);
        if (hBmp == IntPtr.Zero)
        {
            DeleteDC(hMem);
            ReleaseDC(IntPtr.Zero, hScreen);
            throw new InvalidOperationException("创建兼容位图失败！");
        }

        IntPtr hOld = SelectObject(hMem, hBmp);
        if (hOld == IntPtr.Zero)
        {
            DeleteObject(hBmp);
            DeleteDC(hMem);
            ReleaseDC(IntPtr.Zero, hScreen);
            throw new InvalidOperationException("选择对象失败！");
        }

        if (!BitBlt(hMem, 0, 0, w, h, hScreen, 0, 0, SRCCOPY))
        {
            SelectObject(hMem, hOld);
            DeleteObject(hBmp);
            DeleteDC(hMem);
            ReleaseDC(IntPtr.Zero, hScreen);
            throw new InvalidOperationException("位块传输失败！");
        }

        SelectObject(hMem, hOld);
        DeleteDC(hMem);

        int releaseResult = ReleaseDC(IntPtr.Zero, hScreen);
        if (releaseResult == 0)
        {
            throw new InvalidOperationException("释放设备上下文失败！");
        }

        Bitmap bmp = Image.FromHbitmap(hBmp);
        DeleteObject(hBmp);
        return bmp;
    }

    public static Point FindImageOnScreen(string templatePath, double threshold = 0.8)
    {
        if (!File.Exists(templatePath)) return Point.Empty;
        using Bitmap tpl = new(templatePath);
        using Bitmap screen = CaptureScreen();
        if (screen.Width < tpl.Width || screen.Height < tpl.Height) return Point.Empty;
        for (int y = 0; y <= screen.Height - tpl.Height; y++)
            for (int x = 0; x <= screen.Width - tpl.Width; x++)
                if (ComparePixels(screen, tpl, x, y) >= threshold)
                    return new Point(x + tpl.Width / 2, y + tpl.Height / 2);
        return Point.Empty;
    }

    public static bool RecognizeAndClickImage(string templatePath)
    {
        Point pt = FindImageOnScreen(templatePath);
        if (pt == Point.Empty) return false;
        MouseSimulator.Move(pt.X, pt.Y);
        MouseSimulator.LeftClick();
        return true;
    }

    private static double ComparePixels(Bitmap src, Bitmap tpl, int ox, int oy)
    {
        int total = tpl.Width * tpl.Height;
        int match = 0;
        for (int ty = 0; ty < tpl.Height; ty++)
            for (int tx = 0; tx < tpl.Width; tx++)
            {
                Color c1 = src.GetPixel(ox + tx, oy + ty);
                Color c2 = tpl.GetPixel(tx, ty);
                if (c2.A == 0) { match++; continue; }
                double d = Math.Sqrt(Math.Pow(c1.R - c2.R, 2) +
                                     Math.Pow(c1.G - c2.G, 2) +
                                     Math.Pow(c1.B - c2.B, 2));
                if (d < 30) match++;
            }
        return (double)match / total;
    }

    public void Dispose()
    {
        if (_disposed) return;
        _mouse.Dispose();
        _disposed = true;
    }
}