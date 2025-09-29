using System;
using System.Diagnostics;

namespace GoogleClashofClansLauncher.Core;

/// <summary>
/// 管理进程的启动和关闭
/// </summary>
public class ProcessManager
{
    /// <summary>
    /// 启动指定路径的进程
    /// </summary>
    /// <param name="processPath">进程可执行文件路径</param>
    /// <returns>进程实例，失败返回null</returns>
    public Process? StartProcess(string processPath)
    {
        if (string.IsNullOrEmpty(processPath))
            throw new ArgumentNullException(nameof(processPath));

        try
        {
            return Process.Start(new ProcessStartInfo
            {
                FileName = processPath,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = false
            });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"启动进程失败: {ex.Message}");
            return null;
        }
    }

    /// <summary>
    /// 关闭指定进程
    /// </summary>
    /// <param name="process">进程实例</param>
    public void CloseProcess(Process process)
    {
        if (process == null)
            throw new ArgumentNullException(nameof(process));

        try
        {
            if (!process.HasExited)
            {
                process.Kill();
                process.WaitForExit(5000);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"关闭进程失败: {ex.Message}");
        }
        finally
        {
            process.Dispose();
        }
    }
}