using System;
using System.Diagnostics;
using System.Text;
using System.Reflection;
using System.IO;
using System.Drawing;
using System.Threading;

namespace GoogleClashofClansLauncher.Core.System
{
    public static class Utils
{
    /// <summary>
    /// 记录调试信息到输出窗口
    /// </summary>
    /// <param name="message">调试消息</param>
    /// <param name="source">消息来源</param>
    public static void LogDebug(string message, string source = "General")
    {
        try
        {
            Debug.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] [{source}] {message}");
        }
        catch { }
    }

    /// <summary>
    /// 记录错误信息到输出窗口
    /// </summary>
    /// <param name="message">错误消息</param>
    /// <param name="ex">异常对象</param>
    /// <param name="source">消息来源</param>
    public static void LogError(string message, Exception? ex = null, string source = "General")
    {
        try
        {
            if (ex == null)
            {
                Debug.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] [{source}] ERROR: {message}");
            }
            else
            {
                Debug.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] [{source}] ERROR: {message}\nException: {ex.Message}\nStack Trace: {ex.StackTrace}");
            }
        }
        catch { }
    }

    /// <summary>
    /// 记录警告信息到输出窗口
    /// </summary>
    /// <param name="message">警告消息</param>
    /// <param name="source">消息来源</param>
    public static void LogWarning(string message, string source = "General")
    {
        try
        {
            Debug.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] [{source}] WARNING: {message}");
        }
        catch { }
    }

    /// <summary>
    /// 记录一般信息到输出窗口
    /// </summary>
    /// <param name="message">信息</param>
    /// <param name="source">消息来源</param>
    public static void LogInformation(string message, string source = "General")
    {
        try
        {
            Debug.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] [{source}] INFO: {message}");
        }
        catch { }
    }

    /// <summary>
    /// 安全地执行操作并捕获异常
    /// </summary>
    /// <param name="action">要执行的操作</param>
    /// <param name="errorMessage">错误消息</param>
    /// <param name="source">操作来源</param>
    /// <returns>是否成功执行</returns>
    public static bool SafeExecute(Action action, string errorMessage = "操作执行失败", string source = "General")
    {
        try
        {
            action();
            return true;
        }
        catch (Exception ex)
        {
            LogError(errorMessage, ex, source);
            return false;
        }
    }

    /// <summary>
    /// 安全地执行函数并捕获异常
    /// </summary>
    /// <typeparam name="T">返回值类型</typeparam>
    /// <param name="function">要执行的函数</param>
    /// <param name="defaultValue">默认返回值</param>
    /// <param name="errorMessage">错误消息</param>
    /// <param name="source">函数来源</param>
    /// <returns>函数的返回值或默认值</returns>
    public static T SafeExecute<T>(Func<T> function, T defaultValue = default!, string errorMessage = "函数执行失败", string source = "General")
    {
        try
        {
            return function();
        }
        catch (Exception ex)
        {
            LogError(errorMessage, ex, source);
            return defaultValue;
        }
    }

    /// <summary>
    /// 获取应用程序版本信息
    /// </summary>
    /// <returns>应用程序版本字符串</returns>
    public static string GetApplicationVersion()
    {
        try
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            Version version = assembly.GetName().Version;
            return version.ToString();
        }
        catch
        {
            return "未知版本";
        }
    }

    /// <summary>
    /// 创建目录（如果不存在）
    /// </summary>
    /// <param name="directoryPath">目录路径</param>
    /// <returns>是否创建成功</returns>
    public static bool CreateDirectoryIfNotExists(string directoryPath)
    {
        try
        {
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }
            return true;
        }
        catch (Exception ex)
        {
            LogError("创建目录失败", ex, "Utils");
            return false;
        }
    }

    /// <summary>
    /// 计算两点之间的距离
    /// </summary>
    /// <param name="point1">第一个点</param>
    /// <param name="point2">第二个点</param>
    /// <returns>两点之间的距离</returns>
    public static double CalculateDistance(Point point1, Point point2)
    {
        int dx = point1.X - point2.X;
        int dy = point1.Y - point2.Y;
        return Math.Sqrt(dx * dx + dy * dy);
    }

    /// <summary>
    /// 生成指定范围内的随机整数
    /// </summary>
    /// <param name="minValue">最小值（包含）</param>
    /// <param name="maxValue">最大值（包含）</param>
    /// <returns>随机整数</returns>
    public static int GetRandomNumber(int minValue, int maxValue)
    {
        Random random = new Random();
        return random.Next(minValue, maxValue + 1);
    }

    /// <summary>
    /// 等待指定的时间
    /// </summary>
    /// <param name="milliseconds">等待的毫秒数</param>
    public static void Wait(int milliseconds)
    {
        global::System.Threading.Thread.Sleep(milliseconds);
    }

    /// <summary>
    /// 获取应用程序数据目录
    /// </summary>
    /// <returns>应用程序数据目录路径</returns>
    public static string GetApplicationDataDirectory()
    {
        try
        {
            string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string appFolder = Path.Combine(appDataPath, Constants.ApplicationName);
            CreateDirectoryIfNotExists(appFolder);
            return appFolder;
        }
        catch (Exception ex)
        {
            LogError("获取应用程序数据目录失败", ex, "Utils");
            return Environment.CurrentDirectory;
        }
    }

    /// <summary>
    /// 格式化文件大小
    /// </summary>
    /// <param name="bytes">字节数</param>
    /// <returns>格式化后的文件大小字符串</returns>
    public static string FormatFileSize(long bytes)
    {
        string[] suffix = { "B", "KB", "MB", "GB", "TB" };
        int i = 0;
        double dblBytes = bytes;

        if (bytes > 0)
        {
            i = Convert.ToInt32(Math.Floor(Math.Log(bytes, 1024)));
            dblBytes = bytes / Math.Pow(1024, i);
        }

        return $"{dblBytes:0.##} {suffix[i]}";
    }

    /// <summary>
    /// 等待指定的毫秒数，并支持取消
    /// </summary>
    /// <param name="millisecondsDelay">延迟毫秒数</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>是否完成等待（未被取消）</returns>
    public static bool Wait(int millisecondsDelay, global::System.Threading.CancellationToken cancellationToken = default)
    {
        try
        {
            if (cancellationToken.CanBeCanceled)
            {
                // WaitOne returns true if the handle is signaled (cancellation)
                // and false if the timeout is reached.
                // We want to return true if wait completed (timeout), false if cancelled.
                return !cancellationToken.WaitHandle.WaitOne(millisecondsDelay);
            }
            else
            {
                // If token cannot be canceled, just sleep.
                global::System.Threading.Thread.Sleep(millisecondsDelay);
                return true;
            }
        }
        catch (Exception ex)
        {
            LogError("等待操作失败", ex, "Utils");
            return false;
        }
    }
}
}