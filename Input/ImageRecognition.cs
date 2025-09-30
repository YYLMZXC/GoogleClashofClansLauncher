using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using System.Diagnostics;
using GoogleClashofClansLauncher.Core;
using GoogleClashofClansLauncher.Core.System;
using GoogleClashofClansLauncher.Core.UI;
using System.Windows.Forms; // 添加这个引用以便使用Screen类

namespace GoogleClashofClansLauncher.Input;

/// <summary>
/// 图像识别工具类
/// 用于在屏幕上查找指定模板图像并点击对应位置
/// 【注意】：当前功能暂时禁用
/// </summary>
public class ImageRecognition
{
    // 功能控制标志 - 设置为true启用图像识别功能
    private const bool FEATURE_ENABLED = true;
    
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

    public Bitmap? CaptureWindow(IntPtr handle)
    {
        // 实际项目中需要实现窗口捕获逻辑
        return null;
    }

    public bool FindImage(Bitmap screenshot, Image template, out Point matchPoint, double threshold)
    {
        // 实际项目中需要实现图像查找逻辑
        matchPoint = Point.Empty;
        return false;
    }

    /// <summary>
    /// 找到图像中心位置
    /// 【注意】：当前功能暂时禁用
    /// </summary>
    /// <param name="templatePath">模板图像路径</param>
    /// <param name="threshold">匹配阈值 (0-1)</param>
    /// <returns>找到的图像中心位置，如果未找到则返回Point.Empty</returns>
    public Point FindImageOnScreen(string templatePath, double threshold = 0.8)
    {
        Bitmap? template = LoadTemplateImage(templatePath);
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
            template?.Dispose();
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
    /// 加载模板图像
    /// </summary>
    /// <param name="templatePath">模板图像路径</param>
    /// <returns>加载的位图，如果失败则返回null</returns>
    private Bitmap? LoadTemplateImage(string templatePath)
    {
        if (!File.Exists(templatePath))
        {
            Debug.WriteLine($"模板图像文件不存在: {templatePath}");
            return null;
        }

        try
        {
            // 从文件加载图像，然后立即关闭文件流，避免文件锁定
            using (var bmpTemp = new Bitmap(templatePath))
            {
                return new Bitmap(bmpTemp);
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"加载模板图像失败: {ex.Message}");
            return null;
        }
    }

    /// <summary>
    /// 识别图像并点击对应位置
    /// 【注意】：当前功能暂时禁用
    /// </summary>
    /// <param name="templatePath">模板图像路径</param>
    /// <returns>是否成功识别并点击</returns>
    public bool RecognizeAndClickImage(string templatePath)
    {
        // 如果功能被禁用，直接返回失败
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
    /// 【注意】：当前功能暂时禁用
    /// </summary>
    /// <param name="imageName">图片名称（不含扩展名）</param>
    /// <param name="subFolder">子文件夹名称</param>
    /// <returns>是否成功识别并点击</returns>
    public bool RecognizeAndClickResImage(string imageName, string subFolder = "1")
    {
        // 如果功能被禁用，直接返回失败

        
        // 构建图像路径
        string appDirectory = AppDomain.CurrentDomain.BaseDirectory;
        string imagePath = Path.Combine(appDirectory, "..", "..", "..", "Res", subFolder, $"{imageName}.png");
        imagePath = Path.GetFullPath(imagePath);

        Debug.WriteLine("正在查找图像: " + imagePath);
        return RecognizeAndClickImage(imagePath);
    }

    /// <summary>
    /// 点击固定位置（左下角）
    /// 【注意】：当前功能暂时禁用
    /// </summary>
    /// <param name="offsetX">X轴偏移量（相对于左下角）</param>
    /// <param name="offsetY">Y轴偏移量（相对于左下角）</param>
    /// <returns>是否成功执行点击</returns>
    public bool ClickFixedPosition(int offsetX = 100, int offsetY = 100)
    {
        // 如果功能被禁用，直接返回失败
        try
        {
            // 获取主屏幕的尺寸
            Screen? primaryScreen = Screen.PrimaryScreen;
            if (primaryScreen == null)
            {
                Debug.WriteLine("无法获取主屏幕信息");
                return false;
            }
            int screenWidth = primaryScreen.Bounds.Width;
            int screenHeight = primaryScreen.Bounds.Height;

            // 计算目标位置（左下角加上偏移量）
            int targetX = offsetX; // 从左边缘开始的X坐标
            int targetY = screenHeight - offsetY; // 从下边缘开始的Y坐标

            Debug.WriteLine("正在点击固定位置: X=" + targetX + ", Y=" + targetY);

            // 移动鼠标到目标位置
            mouseSimulator.Move(targetX, targetY);
            Thread.Sleep(100); // 等待鼠标移动到位

            // 点击一次
            mouseSimulator.LeftClick();
            Debug.WriteLine("已点击固定位置");

            // 再点击一次
            Thread.Sleep(100);
            mouseSimulator.LeftClick();
            Debug.WriteLine("已再次点击固定位置");

            return true;
        }
        catch (Exception ex)
        {
            Debug.WriteLine("点击固定位置过程中发生错误: " + ex.Message);
            return false;
        }
    }

    /// <summary>
    /// 识别图像并点击，如果识别失败则点击固定位置
    /// 【注意】：当前功能暂时禁用
    /// </summary>
    /// <param name="imageName">图片名称（不含扩展名）</param>
    /// <param name="subFolder">子文件夹名称</param>
    /// <param name="offsetX">X轴偏移量（相对于左下角）</param>
    /// <param name="offsetY">Y轴偏移量（相对于左下角）</param>
    /// <returns>是否成功执行点击</returns>
    public bool RecognizeAndClickWithFallback(string imageName, string subFolder = "1", int offsetX = 100, int offsetY = 100)
    {
        // 首先尝试图像识别
        bool imageRecognized = RecognizeAndClickResImage(imageName, subFolder);
        
        if (imageRecognized)
        {
            Debug.WriteLine("图像识别成功，已点击对应位置");
            return true;
        }
        else
        {
            // 图像识别失败，使用固定位置点击作为备选方案
            Debug.WriteLine("图像识别失败，尝试点击固定位置");
            return ClickFixedPosition(offsetX, offsetY);
        }
    }
}