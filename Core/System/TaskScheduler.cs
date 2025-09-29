using System;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;

namespace GoogleClashofClansLauncher.Core.System;

/// <summary>
/// 任务调度器类
/// 用于管理后台任务的执行和取消
/// </summary>
public class TaskScheduler
{
    private CancellationTokenSource? _cancellationTokenSource;
    private Task? _currentTask;

    /// <summary>
    /// 启动一个新的后台任务
    /// </summary>
    /// <param name="taskAction">要执行的任务</param>
    /// <param name="onCompleted">任务完成时的回调</param>
    /// <param name="onError">任务出错时的回调</param>
    public void StartBackgroundTask(
        Action<CancellationToken> taskAction,
        Action? onCompleted = null,
        Action<Exception>? onError = null
    )
    {
        // 如果已经有任务在运行，先取消
        if (_cancellationTokenSource != null && !_cancellationTokenSource.IsCancellationRequested)
        {
            CancelCurrentTask();
        }

        // 创建新的取消令牌
        _cancellationTokenSource = new CancellationTokenSource();
        CancellationToken token = _cancellationTokenSource.Token;

        // 在后台线程中执行任务
        _currentTask = Task.Run(() =>
        {
            try
            {
                taskAction(token);
                onCompleted?.Invoke();
            }
            catch (Exception ex)
            {
                // 忽略任务取消异常
                if (ex is TaskCanceledException || token.IsCancellationRequested)
                {
                    Debug.WriteLine("任务已被取消");
                }
                else
                {
                    Debug.WriteLine("任务执行出错: " + ex.Message);
                    onError?.Invoke(ex);
                }
            }
            finally
            {
                _currentTask = null;
                if (_cancellationTokenSource != null && _cancellationTokenSource.IsCancellationRequested)
                {
                    _cancellationTokenSource.Dispose();
                    _cancellationTokenSource = null;
                }
            }
        }, token);
    }

    /// <summary>
    /// 取消当前正在执行的任务
    /// </summary>
    public void CancelCurrentTask()
    {
        if (_cancellationTokenSource != null && !_cancellationTokenSource.IsCancellationRequested)
        {
            _cancellationTokenSource.Cancel();
            _cancellationTokenSource.Dispose();
            _cancellationTokenSource = null;
            Debug.WriteLine("任务已取消");
        }
    }

    /// <summary>
    /// 检查是否有任务正在运行
    /// </summary>
    /// <returns>是否有任务正在运行</returns>
    public bool IsTaskRunning()
    {
        return _currentTask != null && !_currentTask.IsCompleted;
    }

    /// <summary>
    /// 等待任务完成
    /// </summary>
    /// <param name="timeoutMilliseconds">超时时间（毫秒）</param>
    /// <returns>任务是否在超时前完成</returns>
    public bool WaitForTaskCompletion(int timeoutMilliseconds = -1)
    {
        if (_currentTask == null)
            return true;

        if (timeoutMilliseconds < 0)
        {
            _currentTask.Wait();
            return true;
        }
        else
            return _currentTask.Wait(timeoutMilliseconds);
    }

    /// <summary>
    /// 释放资源
    /// </summary>
    public void Dispose()
    {
        CancelCurrentTask();
        if (_cancellationTokenSource != null)
        {
            _cancellationTokenSource.Dispose();
            _cancellationTokenSource = null;
        }
    }
}