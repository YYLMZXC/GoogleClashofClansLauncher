using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using System.Diagnostics;
using GoogleClashofClansLauncher.Core;

namespace GoogleClashofClansLauncher.Input;

/// <summary>
/// 图像识别工具类
/// 用于在屏幕上查找指定模板图像并点击对应位置
/// </summary>
public class ImageRecognition
{
    private const int SRCCOPY = 0x00CC0020;
    private const int CAPTUREBLT = 0x40000000;
    private MouseSimulator mouseSimulator;
    private WindowManager windowManager;

    public ImageRecognition()
    {
        mouseSimulator = new MouseSimulator();
        windowManager = new WindowManager();
    }

    // Windows API：获取屏幕DC
    [DllImport("user32.dll")]
    private static extern IntPtr GetDC(IntPtr hWnd);

    // Windows API：释放DC
    [DllImport("user32.dll")]
    private static extern int ReleaseDC(IntPtr hWnd, IntPtr hDC);

    // Windows API：创建兼容DC
    [DllImport("gdi32.dll")]
    private static extern IntPtr CreateCompatibleDC(IntPtr hDC);

    // Windows API：删除DC
    [DllImport("gdi32.dll")]
    private static extern bool DeleteDC(IntPtr hDC);

    // Windows API：创建兼容位图
    [DllImport("gdi32.dll")]
    private static extern IntPtr CreateCompatibleBitmap(IntPtr hDC, int nWidth, int nHeight);

    // Windows API：删除位图
    [DllImport("gdi32.dll")]
    private static extern bool DeleteObject(IntPtr hObject);

    // Windows API：选择对象
    [DllImport("gdi32.dll")]
    private static extern IntPtr SelectObject(IntPtr hDC, IntPtr hObject);

    // Windows API：BitBlt
    [DllImport("gdi32.dll")]
    private static extern bool BitBlt(IntPtr hdcDest, int nXDest, int nYDest, int nWidth, int nHeight,
        IntPtr hdcSrc, int nXSrc, int nYSrc, int dwRop);

    // Windows API：获取屏幕尺寸
    [DllImport("user32.dll")]
    private static extern int GetSystemMetrics(int nIndex);

    private const int SM_CXSCREEN = 0;
    private const int SM_CYSCREEN = 1;

    /// <summary>
    /// 捕获整个屏幕图像
    /// </summary>
    private Bitmap CaptureScreen()
    {
        int screenWidth = GetSystemMetrics(SM_CXSCREEN);
        int screenHeight = GetSystemMetrics(SM_CYSCREEN);

        IntPtr hScreenDC = GetDC(IntPtr.Zero);
        IntPtr hMemoryDC = CreateCompatibleDC(hScreenDC);
        IntPtr hBitmap = CreateCompatibleBitmap(hScreenDC, screenWidth, screenHeight);
        IntPtr hOldBitmap = SelectObject(hMemoryDC, hBitmap);

        BitBlt(hMemoryDC, 0, 0, screenWidth, screenHeight, hScreenDC, 0, 0, SRCCOPY | CAPTUREBLT);

        SelectObject(hMemoryDC, hOldBitmap);
        DeleteDC(hMemoryDC);
        ReleaseDC(IntPtr.Zero, hScreenDC);

        Bitmap bitmap = Image.FromHbitmap(hBitmap);
        DeleteObject(hBitmap);

        return bitmap;
    }

    /// <summary>
    /// 加载模板图像
    /// </summary>
    /// <param name="imagePath">图像路径</param>
    /// <returns>加载的位图</returns>
    private Bitmap LoadTemplateImage(string imagePath)
    {
        if (!File.Exists(imagePath))
        {
            Debug.WriteLine("模板图像不存在: " + imagePath);
            return null;
        }

        try
        {
            return new Bitmap(imagePath);
        }
        catch (Exception ex)
        {
            Debug.WriteLine("加载模板图像失败: " + ex.Message);
            return null;
        }
    }

    /// <summary>
    /// 在屏幕上查找模板图像
    /// </summary>
    /// <param name="templatePath">模板图像路径</param>
    /// <param name="threshold">匹配阈值 (0-1)</param>
    /// <returns>找到的图像中心位置，如果未找到则返回Point.Empty</returns>
    public Point FindImageOnScreen(string templatePath, double threshold = 0.8)
    {
        Bitmap template = LoadTemplateImage(templatePath);
        if (template == null)
            return Point.Empty;

        try
        {
            using (Bitmap screen = CaptureScreen())
            {
                if (screen == null || template == null || 
                    screen.Width < template.Width || screen.Height < template.Height)
                {
                    return Point.Empty;
                }

                // 简单的模板匹配算法（实际项目中可以使用更复杂的算法如OpenCV）
                for (int y = 0; y <= screen.Height - template.Height; y++)
                {
                    for (int x = 0; x <= screen.Width - template.Width; x++)
                    {
                        double matchScore = ComparePixels(screen, template, x, y);
                        if (matchScore >= threshold)
                        {
                            // 返回匹配区域的中心位置
                            return new Point(
                                x + template.Width / 2,
                                y + template.Height / 2
                            );
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine("图像匹配异常: " + ex.Message);
        }
        finally
        {
            template.Dispose();
        }

        return Point.Empty;
    }

    /// <summary>
    /// 比较像素相似度
    /// </summary>
    private double ComparePixels(Bitmap screen, Bitmap template, int x, int y)
    {
        int totalPixels = template.Width * template.Height;
        int matchedPixels = 0;

        for (int ty = 0; ty < template.Height; ty++)
        {
            for (int tx = 0; tx < template.Width; tx++)
            {
                Color screenColor = screen.GetPixel(x + tx, y + ty);
                Color templateColor = template.GetPixel(tx, ty);

                // 忽略透明像素
                if (templateColor.A == 0)
                {
                    matchedPixels++;
                    continue;
                }

                // 简单的颜色比较（实际项目中可以使用更复杂的颜色空间转换和相似度计算）
                double colorDistance = Math.Sqrt(
                    Math.Pow(screenColor.R - templateColor.R, 2) +
                    Math.Pow(screenColor.G - templateColor.G, 2) +
                    Math.Pow(screenColor.B - templateColor.B, 2)
                );

                // 如果颜色距离小于30，则认为匹配
                if (colorDistance < 30)
                {
                    matchedPixels++;
                }
            }
        }

        return (double)matchedPixels / totalPixels;
    }

    /// <summary>
    /// 识别图像并点击对应位置
    /// </summary>
    /// <param name="templatePath">模板图像路径</param>
    /// <returns>是否成功识别并点击</returns>
    public bool RecognizeAndClickImage(string templatePath)
    {
        try
        {
            // 查找图像
            Point targetPosition = FindImageOnScreen(templatePath);
            if (targetPosition == Point.Empty)
            {
                Debug.WriteLine("未找到图像: " + templatePath);
                return false;
            }

            Debug.WriteLine("找到图像，位置: X=" + targetPosition.X + ", Y=" + targetPosition.Y);

            // 移动鼠标到目标位置
            mouseSimulator.Move(targetPosition.X, targetPosition.Y);
            Thread.Sleep(100); // 等待鼠标移动到位

            // 点击一次
            mouseSimulator.LeftClick();
            Debug.WriteLine("已点击图像位置");

            // 再点击一次（根据用户需求）
            Thread.Sleep(100);
            mouseSimulator.LeftClick();
            Debug.WriteLine("已再次点击");

            return true;
        }
        catch (Exception ex)
        {
            Debug.WriteLine("识别并点击图像过程中发生错误: " + ex.Message);
            return false;
        }
    }

    /// <summary>
    /// 识别并点击Res文件夹下的图片
    /// </summary>
    /// <param name="imageName">图片名称（不含扩展名）</param>
    /// <param name="subFolder">子文件夹名称</param>
    /// <returns>是否成功识别并点击</returns>
    public bool RecognizeAndClickResImage(string imageName, string subFolder = "1")
    {
        // 构建图像路径
        string appDirectory = AppDomain.CurrentDomain.BaseDirectory;
        string imagePath = Path.Combine(appDirectory, "..", "..", "..", "Res", subFolder, $"{imageName}.png");
        imagePath = Path.GetFullPath(imagePath);

        Debug.WriteLine("正在查找图像: " + imagePath);
        return RecognizeAndClickImage(imagePath);
    }
}