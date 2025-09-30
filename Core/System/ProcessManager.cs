
using System;
using System.Diagnostics;

namespace GoogleClashofClansLauncher.Core.System;

public class ProcessManager
{
    public static Process? StartProcess(string processPath)
    {
        if (string.IsNullOrEmpty(processPath)) throw new ArgumentNullException(nameof(processPath));
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
            Utils.LogError("启动进程失败", ex);
            return null;
        }
    }

    public static void CloseProcess(Process process)
    {
        if (process == null) throw new ArgumentNullException(nameof(process));
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
            Utils.LogError("关闭进程失败", ex);
        }
        finally
        {
            process.Dispose();
        }
    }
}