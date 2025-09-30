using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using System.Diagnostics;
using GoogleClashofClansLauncher.Core;
using GoogleClashofClansLauncher.Core.System;
using GoogleClashofClansLauncher.Core.UI;

namespace GoogleClashofClansLauncher.Input
{
    /// <summary>
    /// 图像识别工具类
    /// 当前仅支持：屏幕截图、模板匹配、点击匹配位置
    /// </summary>
    public sealed class ImageRecognition : IDisposable
    {
        #region Win32 API
        private const int SRCCOPY = 0x00CC0020;
        private const int CAPTUREBLT = 0x40000000;
        private const int SM_CXSCREEN = 0;
        private const int SM_CYSCREEN = 1;

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
        #endregion

        private readonly MouseSimulator _mouse = new();
        private bool _disposed;

        /// <summary>
        /// 截取当前屏幕
        /// </summary>
        public Bitmap CaptureScreen()
        {
            int w = GetSystemMetrics(SM_CXSCREEN);
            int h = GetSystemMetrics(SM_CYSCREEN);

            IntPtr hScreen = GetDC(IntPtr.Zero);
            IntPtr hMem = CreateCompatibleDC(hScreen);
            IntPtr hBmp = CreateCompatibleBitmap(hScreen, w, h);
            IntPtr hOld = SelectObject(hMem, hBmp);

            BitBlt(hMem, 0, 0, w, h, hScreen, 0, 0, SRCCOPY | CAPTUREBLT);

            SelectObject(hMem, hOld);
            DeleteDC(hMem);
            ReleaseDC(IntPtr.Zero, hScreen);

            Bitmap bmp = Image.FromHbitmap(hBmp);
            DeleteObject(hBmp);
            return bmp;
        }

        /// <summary>
        /// 在屏幕中查找模板并返回中心点；未找到返回 Empty
        /// </summary>
        /// <param name="templatePath">模板路径（png/jpg）</param>
        /// <param name="threshold">匹配度阈值 0~1</param>
        public Point FindImageOnScreen(string templatePath, double threshold = 0.8)
        {
            if (!File.Exists(templatePath)) return Point.Empty;

            using Bitmap tpl = new(templatePath);
            using Bitmap screen = CaptureScreen();

            if (screen.Width < tpl.Width || screen.Height < tpl.Height) return Point.Empty;

            for (int y = 0; y <= screen.Height - tpl.Height; y++)
            {
                for (int x = 0; x <= screen.Width - tpl.Width; x++)
                {
                    if (ComparePixels(screen, tpl, x, y) >= threshold)
                        return new Point(x + tpl.Width / 2, y + tpl.Height / 2);
                }
            }
            return Point.Empty;
        }

        /// <summary>
        /// 查找并点击一次；成功返回 true
        /// </summary>
        public bool RecognizeAndClickImage(string templatePath)
        {
            Point pt = FindImageOnScreen(templatePath);
            if (pt == Point.Empty) return false;

            _mouse.Move(pt.X, pt.Y);
            _mouse.LeftClick();
            return true;
        }

        #region private helper
        private static double ComparePixels(Bitmap src, Bitmap tpl, int ox, int oy)
        {
            int total = tpl.Width * tpl.Height;
            int match = 0;

            for (int ty = 0; ty < tpl.Height; ty++)
            {
                for (int tx = 0; tx < tpl.Width; tx++)
                {
                    Color c1 = src.GetPixel(ox + tx, oy + ty);
                    Color c2 = tpl.GetPixel(tx, ty);

                    if (c2.A == 0) { match++; continue; }

                    double d = Math.Sqrt(
                        Math.Pow(c1.R - c2.R, 2) +
                        Math.Pow(c1.G - c2.G, 2) +
                        Math.Pow(c1.B - c2.B, 2));
                    if (d < 30) match++;
                }
            }
            return (double)match / total;
        }
        #endregion

        #region IDisposable
        public void Dispose()
        {
            if (_disposed) return;
            _mouse?.Dispose();
            _disposed = true;
        }
        #endregion
    }
}