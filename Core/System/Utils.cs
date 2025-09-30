
using System;
using System.Diagnostics;
using System.IO;

namespace GoogleClashofClansLauncher.Core.System;

public static class Utils
{
    public static void LogDebug(string msg, string src = "General") =>
        Debug.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] [{src}] {msg}");

    public static void LogError(string msg, Exception? ex = null, string src = "General") =>
        Debug.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] [{src}] ERROR: {msg}{(ex == null ? "" : $"\n{ex}")}");

    public static string GetApplicationDataDirectory()
    {
        var path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                                "GoogleClashofClansLauncher");
        Directory.CreateDirectory(path);
        return path;
    }
}