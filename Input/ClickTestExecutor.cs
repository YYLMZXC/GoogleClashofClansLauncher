using System;
using System.Threading;
using System.Diagnostics;
using GoogleClashofClansLauncher.Core;
using GoogleClashofClansLauncher.Core.System;
using TaskScheduler = GoogleClashofClansLauncher.Core.System.TaskScheduler;

namespace GoogleClashofClansLauncher.Input;

/// <summary>
/// 点击测试执行器类
/// 专门负责执行和管理点击测试功能
/// </summary>
public class ClickTestExecutor
{
    private readonly MouseController _mouseController;
    private CancellationTokenSource? _cancellationTokenSource;
    private int _totalClicks;
    private int _clicksPerSecond;
    private int _durationSeconds;

    /// <summary>
    /// 点击测试开始事件
    /// </summary>
    public event EventHandler<ClickTestEventArgs>? TestStarted;

    /// <summary>
    /// 点击测试进度更新事件
    /// </summary>
    public event EventHandler<ClickTestProgressEventArgs>? ProgressUpdated;

    /// <summary>
    /// 点击测试完成事件
    /// </summary>
    public event EventHandler<ClickTestCompletedEventArgs>? TestCompleted;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="mouseController">鼠标控制器实例</param>
    public ClickTestExecutor(MouseController mouseController)
    {
        _mouseController = mouseController;
        _totalClicks = 0;
        _clicksPerSecond = 3; // 默认每秒3次点击
        _durationSeconds = 10; // 默认持续10秒
    }

    /// <summary>
    /// 设置点击测试参数
    /// </summary>
    /// <param name="clicksPerSecond">每秒点击次数</param>
    /// <param name="durationSeconds">测试持续时间（秒）</param>
    public void SetTestParameters(int clicksPerSecond, int durationSeconds)
    {
        _clicksPerSecond = Math.Max(1, clicksPerSecond);
        _durationSeconds = Math.Max(1, durationSeconds);
    }

    /// <summary>
    /// 开始点击测试
    /// </summary>
    public void StartTest()
    {
        // 如果已经有测试在运行，先取消
        if (_cancellationTokenSource != null && !_cancellationTokenSource.IsCancellationRequested)
        {
            CancelTest();
        }

        // 创建新的取消令牌
        _cancellationTokenSource = new CancellationTokenSource();
        CancellationToken token = _cancellationTokenSource.Token;

        // 重置点击计数
        _totalClicks = 0;

        // 触发测试开始事件
        TestStarted?.Invoke(this, new ClickTestEventArgs(_clicksPerSecond, _durationSeconds));

        // 在后台线程中执行点击测试
        TaskScheduler taskScheduler = new TaskScheduler();
        taskScheduler.StartBackgroundTask(
            token => ExecuteClickTest(token),
            OnTestCompleted,
            OnTestError
        );
    }

    /// <summary>
    /// 取消点击测试
    /// </summary>
    public void CancelTest()
    {
        if (_cancellationTokenSource != null && !_cancellationTokenSource.IsCancellationRequested)
        {
            _cancellationTokenSource.Cancel();
            Utils.LogDebug("点击测试已取消", "ClickTestExecutor");
        }
    }

    /// <summary>
    /// 执行点击测试的核心逻辑
    /// </summary>
    /// <param name="cancellationToken">取消令牌</param>
    private void ExecuteClickTest(CancellationToken cancellationToken)
    {
        try
        {
            int clickInterval = 1000 / _clicksPerSecond; // 点击间隔（毫秒）
            int totalClicksToPerform = _clicksPerSecond * _durationSeconds;

            Utils.LogDebug($"开始点击测试: 每秒{_clicksPerSecond}次，持续{_durationSeconds}秒，总共{totalClicksToPerform}次点击", "ClickTestExecutor");

            DateTime startTime = DateTime.Now;
            int currentSecond = 0;
            int clicksInCurrentSecond = 0;

            for (int i = 0; i < totalClicksToPerform; i++)
            {
                // 检查是否取消
                cancellationToken.ThrowIfCancellationRequested();

                // 执行一次点击
                if (!_mouseController.LeftClick())
                {
                    Utils.LogError("点击操作失败", null, "ClickTestExecutor");
                    throw new Exception("点击操作执行失败");
                }

                // 更新点击计数
                _totalClicks++;
                clicksInCurrentSecond++;

                // 计算已过时间
                TimeSpan elapsed = DateTime.Now - startTime;
                int newSecond = (int)Math.Floor(elapsed.TotalSeconds);

                // 每秒更新进度
                if (newSecond > currentSecond)
                {
                    currentSecond = newSecond;
                    int progressPercentage = (int)((elapsed.TotalSeconds / _durationSeconds) * 100);
                    ProgressUpdated?.Invoke(this, new ClickTestProgressEventArgs(
                        _totalClicks,
                        progressPercentage,
                        currentSecond
                    ));
                    clicksInCurrentSecond = 0;
                }

                // 等待指定的点击间隔
                if (!Utils.Wait(clickInterval, cancellationToken))
                {
                    // 如果等待被取消，直接退出
                    cancellationToken.ThrowIfCancellationRequested();
                }
            }

            Utils.LogDebug($"点击测试完成，实际执行了{_totalClicks}次点击", "ClickTestExecutor");
        }
        catch (OperationCanceledException)
        {
            Utils.LogDebug("点击测试被用户取消", "ClickTestExecutor");
            throw; // 重新抛出，让上层处理
        }
        catch (Exception ex)
        {
            Utils.LogError("点击测试执行过程中出错", ex, "ClickTestExecutor");
            throw;
        }
    }

    /// <summary>
    /// 测试完成处理函数
    /// </summary>
    private void OnTestCompleted()
    {
        TestCompleted?.Invoke(this, new ClickTestCompletedEventArgs(_totalClicks, true));
        Cleanup();
    }

    /// <summary>
    /// 测试错误处理函数
    /// </summary>
    /// <param name="ex">异常对象</param>
    private void OnTestError(Exception ex)
    {
        bool wasCanceled = ex is TaskCanceledException || (_cancellationTokenSource != null && _cancellationTokenSource.IsCancellationRequested);
        TestCompleted?.Invoke(this, new ClickTestCompletedEventArgs(
            _totalClicks, 
            !wasCanceled && _totalClicks > 0, 
            wasCanceled ? null : ex
        ));
        Cleanup();
    }

    /// <summary>
    /// 清理资源
    /// </summary>
    private void Cleanup()
    {
        if (_cancellationTokenSource != null)
        {
            _cancellationTokenSource.Dispose();
            _cancellationTokenSource = null;
        }
    }
}

/// <summary>
/// 点击测试事件参数类
/// </summary>
public class ClickTestEventArgs : EventArgs
{
    /// <summary>
    /// 每秒点击次数
    /// </summary>
    public int ClicksPerSecond { get; }

    /// <summary>
    /// 测试持续时间（秒）
    /// </summary>
    public int DurationSeconds { get; }

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="clicksPerSecond">每秒点击次数</param>
    /// <param name="durationSeconds">测试持续时间（秒）</param>
    public ClickTestEventArgs(int clicksPerSecond, int durationSeconds)
    {
        ClicksPerSecond = clicksPerSecond;
        DurationSeconds = durationSeconds;
    }
}

/// <summary>
/// 点击测试进度事件参数类
/// </summary>
public class ClickTestProgressEventArgs : EventArgs
{
    /// <summary>
    /// 已完成的点击次数
    /// </summary>
    public int TotalClicks { get; }

    /// <summary>
    /// 进度百分比
    /// </summary>
    public int ProgressPercentage { get; }

    /// <summary>
    /// 当前已过秒数
    /// </summary>
    public int CurrentSecond { get; }

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="totalClicks">已完成的点击次数</param>
    /// <param name="progressPercentage">进度百分比</param>
    /// <param name="currentSecond">当前已过秒数</param>
    public ClickTestProgressEventArgs(int totalClicks, int progressPercentage, int currentSecond)
    {
        TotalClicks = totalClicks;
        ProgressPercentage = progressPercentage;
        CurrentSecond = currentSecond;
    }
}

/// <summary>
/// 点击测试完成事件参数类
/// </summary>
public class ClickTestCompletedEventArgs : EventArgs
{
    /// <summary>
    /// 总点击次数
    /// </summary>
    public int TotalClicks { get; }

    /// <summary>
    /// 是否成功完成
    /// </summary>
    public bool IsSuccessful { get; }

    /// <summary>
    /// 错误信息（如果有）
    /// </summary>
    public Exception? Error { get; }

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="totalClicks">总点击次数</param>
    /// <param name="isSuccessful">是否成功完成</param>
    /// <param name="error">错误信息（如果有）</param>
    public ClickTestCompletedEventArgs(int totalClicks, bool isSuccessful, Exception? error = null)
    {
        TotalClicks = totalClicks;
        IsSuccessful = isSuccessful;
        Error = error;
    }
}