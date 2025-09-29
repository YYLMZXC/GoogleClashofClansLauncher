using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using GoogleClashofClansLauncher.Core;
using GoogleClashofClansLauncher.Core.System;

namespace GoogleClashofClansLauncher.Game;

/// <summary>
/// 游戏进程管理器
/// 负责游戏进程的启动、监控、控制和关闭
/// </summary>
public class GameProcess
{
    /// <summary>
    /// 游戏进程对象
    /// </summary>
    public Process GameProcessInstance { get; private set; } = null;

    /// <summary>
    /// 游戏进程ID
    /// </summary>
    public int ProcessId { get; private set; } = 0;

    /// <summary>
    /// 游戏进程名称
    /// </summary>
    public string ProcessName { get; set; } = "ClashOfClans";

    /// <summary>
    /// 游戏可执行文件路径
    /// </summary>
    public string GameExecutablePath { get; set; } = string.Empty;

    /// <summary>
    /// 游戏工作目录
    /// </summary>
    public string WorkingDirectory { get; set; } = string.Empty;

    /// <summary>
    /// 游戏启动参数
    /// </summary>
    public string StartArguments { get; set; } = string.Empty;

    /// <summary>
    /// 是否正在监控游戏进程
    /// </summary>
    public bool IsMonitoring { get; private set; } = false;

    /// <summary>
    /// 进程监控取消令牌
    /// </summary>
    private CancellationTokenSource _monitoringCts = null;

    /// <summary>
    /// 进程启动事件
    /// </summary>
    public event EventHandler<GameProcessEventArgs> GameProcessStarted;

    /// <summary>
    /// 进程退出事件
    /// </summary>
    public event EventHandler<GameProcessEventArgs> GameProcessExited;

    /// <summary>
    /// 进程错误事件
    /// </summary>
    public event EventHandler<GameProcessErrorEventArgs> GameProcessError;

    /// <summary>
    /// 构造函数
    /// </summary>
    public GameProcess()
    {
        Initialize();
    }

    /// <summary>
    /// 初始化游戏进程管理器
    /// </summary>
    private void Initialize()
    {
        Utils.LogDebug("游戏进程管理器已初始化", "GameProcess");
    }

    /// <summary>
    /// 检查游戏进程是否正在运行
    /// </summary>
    /// <returns>如果游戏进程正在运行，则为 true；否则为 false。</returns>
    public bool IsGameRunning()
    {
        // 检查 GameProcessInstance 是否为 null 以及 HasExited 属性
        if (GameProcessInstance != null && !GameProcessInstance.HasExited)
        {
            return true;
        }

        // 如果 GameProcessInstance 为 null，则按进程名称搜索
        Process[] processes = Process.GetProcessesByName(ProcessName);
        if (processes.Length > 0)
        {
            GameProcessInstance = processes[0];
            return true;
        }

        return false;
    }

    /// <summary>
    /// 启动游戏进程
    /// </summary>
    /// <returns>是否启动成功</returns>
    public bool StartGameProcess()
    {
        try
        {
            // 检查游戏可执行文件是否存在
            if (string.IsNullOrEmpty(GameExecutablePath) || !File.Exists(GameExecutablePath))
            {
                string errorMsg = string.IsNullOrEmpty(GameExecutablePath) ? "游戏可执行文件路径为空" : $"游戏可执行文件不存在: {GameExecutablePath}";
                Utils.LogError(errorMsg, null, "GameProcess");
                GameProcessError?.Invoke(this, new GameProcessErrorEventArgs(errorMsg));
                return false;
            }

            // 检查是否已经有游戏进程在运行
            if (IsGameRunning())
            {
                Utils.LogWarning("游戏进程已经在运行中", "GameProcess");
                return true;
            }

            Utils.LogDebug($"准备启动游戏: {GameExecutablePath}", "GameProcess");

            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                FileName = GameExecutablePath,
                WorkingDirectory = Path.GetDirectoryName(GameExecutablePath),
                UseShellExecute = true
            };

            GameProcessInstance = Process.Start(startInfo);

            if (GameProcessInstance != null)
            {
                Utils.LogDebug($"游戏进程已启动: {GameProcessInstance.Id}", "GameProcess");
                GameProcessStarted?.Invoke(this, new GameProcessEventArgs(GameProcessInstance.Id));
                return true;
            }
            else
            {
                Utils.LogError("启动游戏进程失败", null, "GameProcess");
                GameProcessError?.Invoke(this, new GameProcessErrorEventArgs("启动游戏进程失败"));
                return false;
            }
        }
        catch (Exception ex)
        {
            Utils.LogError("启动游戏进程时发生异常", ex, "GameProcess");
            GameProcessError?.Invoke(this, new GameProcessErrorEventArgs("启动游戏进程时发生异常: " + ex.Message, ex));
            return false;
        }
    }
}

/// <summary>
/// 游戏进程事件参数
/// </summary>
public class GameProcessEventArgs : EventArgs
{
    public int ProcessId { get; }

    public GameProcessEventArgs(int processId)
    {
        ProcessId = processId;
    }
}

/// <summary>
/// 游戏进程错误事件参数
/// </summary>
public class GameProcessErrorEventArgs : EventArgs
{
    public string Message { get; }
    public Exception Exception { get; }

    public GameProcessErrorEventArgs(string message, Exception ex = null)
    {
        Message = message;
        Exception = ex;
    }
}