using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using GoogleClashofClansLauncher.Config;

namespace GoogleClashofClansLauncher.Core.System;

/// <summary>
/// 错误处理器类
/// 专门处理应用程序中的异常和错误
/// </summary>
public static class ErrorHandler
{
    private static bool _isInitialized = false;
    private static string _logFilePath = string.Empty;
    private static ErrorHandlingOptions _options = new ErrorHandlingOptions();

    /// <summary>
    /// 初始化错误处理器
    /// </summary>
    /// <param name="options">错误处理选项</param>
    public static void Initialize(ErrorHandlingOptions options = null)
    {
        try
        {
            if (options != null)
            {
                _options = options;
            }

            // 设置全局异常处理器
            AppDomain.CurrentDomain.UnhandledException += OnCurrentDomainUnhandledException;
            Application.ThreadException += OnApplicationThreadException;

            // 设置日志文件路径
            _logFilePath = Path.Combine(Utils.GetApplicationDataDirectory(), "error_log.txt");
            _isInitialized = true;

            Utils.LogDebug("错误处理器初始化成功", "ErrorHandler");
        }
        catch (Exception ex)
        {
            // 这里不能使用Utils.LogError，因为可能还没有初始化
            Debug.WriteLine("错误处理器初始化失败: " + ex.Message);
        }
    }

    /// <summary>
    /// 处理未捕获的异常
    /// </summary>
    /// <param name="sender">事件发送者</param>
    /// <param name="e">事件参数</param>
    private static void OnCurrentDomainUnhandledException(object sender, UnhandledExceptionEventArgs e)
    {
        if (e.ExceptionObject is Exception ex)
        {
            HandleFatalError(ex, "未捕获的应用程序域异常");
        }
    }

    /// <summary>
    /// 处理UI线程异常
    /// </summary>
    /// <param name="sender">事件发送者</param>
    /// <param name="e">事件参数</param>
    private static void OnApplicationThreadException(object sender, ThreadExceptionEventArgs e)
    {
        HandleError(e.Exception, "UI线程异常");
    }

    /// <summary>
    /// 处理一般错误
    /// </summary>
    /// <param name="ex">异常对象</param>
    /// <param name="context">错误上下文</param>
    /// <param name="showMessageBox">是否显示消息框</param>
    public static void HandleError(Exception ex, string context = "应用程序错误", bool showMessageBox = true)
    {
        LogError(ex, context);

        if (showMessageBox && _options.ShowErrorDialogs)
        {
            string message = $"发生错误：{context}\n\n详细信息：{ex.Message}";
            if (_options.ShowDetailedErrors)
            {
                message += $"\n\n堆栈跟踪：{ex.StackTrace}";
            }

            DialogResult result = MessageBox.Show(
                message,
                "错误",
                MessageBoxButtons.OKCancel,
                MessageBoxIcon.Error
            );

            if (result == DialogResult.Cancel && _options.AllowCancelErrorHandling)
            {
                return;
            }
        }

        // 尝试恢复或继续执行
        if (_options.AutoRecoverOnError)
        {
            TryRecoverFromError(ex);
        }
    }

    /// <summary>
    /// 处理致命错误
    /// </summary>
    /// <param name="ex">异常对象</param>
    /// <param name="context">错误上下文</param>
    public static void HandleFatalError(Exception ex, string context = "致命应用程序错误")
    {
        LogError(ex, context);

        if (_options.ShowErrorDialogs)
        {
            string message = $"发生致命错误，应用程序可能无法继续运行：{context}\n\n详细信息：{ex.Message}";
            if (_options.ShowDetailedErrors)
            {
                message += $"\n\n堆栈跟踪：{ex.StackTrace}\n\n内部异常：{ex.InnerException?.Message}";
            }

            DialogResult result = MessageBox.Show(
                message,
                "致命错误",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Stop
            );

            if (result == DialogResult.Yes)
            {
                // 尝试保存日志并重启应用程序
                TryRestartApplication();
            }
        }

        // 如果配置了自动退出，则退出应用程序
        if (_options.AutoExitOnFatalError)
        {
            Application.Exit();
        }
    }

    /// <summary>
    /// 记录错误日志
    /// </summary>
    /// <param name="ex">异常对象</param>
    /// <param name="context">错误上下文</param>
    private static void LogError(Exception ex, string context)
    {
        try
        {
            // 使用Utils.LogError记录日志
            Utils.LogError(context, ex, "ErrorHandler");

            // 如果配置了额外的错误日志文件，也写入到该文件
            if (!string.IsNullOrEmpty(_logFilePath) && _options.LogToFile)
            {
                string errorLogEntry = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}] [{context}] {ex.Message}\n";
                errorLogEntry += $"堆栈跟踪：{ex.StackTrace}\n";
                if (ex.InnerException != null)
                {
                    errorLogEntry += $"内部异常：{ex.InnerException.Message}\n";
                }
                errorLogEntry += new string('-', 80) + "\n";

                // 异步写入日志，避免阻塞主线程
                Task.Run(() =>
                {
                    try
                    {
                        File.AppendAllText(_logFilePath, errorLogEntry);
                    }
                    catch { /* 静默处理日志写入错误 */ }
                });
            }
        }
        catch { /* 静默处理日志记录错误 */ }
    }

    /// <summary>
    /// 尝试从错误中恢复
    /// </summary>
    /// <param name="ex">异常对象</param>
    private static void TryRecoverFromError(Exception ex)
    {
        try
        {
            // 根据不同类型的异常尝试不同的恢复策略
            if (ex is IOException)
            {
                // IO异常恢复策略
                Utils.LogDebug("尝试从IO异常中恢复", "ErrorHandler");
                // 例如：关闭并重新打开文件
            }
            else if (ex is WebException)
            {
                // 网络异常恢复策略
                Utils.LogDebug("尝试从网络异常中恢复", "ErrorHandler");
                // 例如：重试请求或检查网络连接
            }
            else if (ex is OutOfMemoryException)
            {
                // 内存异常恢复策略
                Utils.LogDebug("尝试从内存异常中恢复", "ErrorHandler");
                // 例如：释放不必要的资源
                GC.Collect();
                GC.WaitForPendingFinalizers();
            }
            else if (ex is InvalidOperationException)
            {
                // 无效操作异常恢复策略
                Utils.LogDebug("尝试从无效操作异常中恢复", "ErrorHandler");
                // 例如：重置状态或重新初始化组件
            }

            // 通用恢复策略：尝试保存当前状态
            if (_options.AutoSaveStateOnError)
            {
                TrySaveApplicationState();
            }
        }
        catch { /* 静默处理恢复错误 */ }
    }

    /// <summary>
    /// 尝试保存应用程序状态
    /// </summary>
    private static void TrySaveApplicationState()
    {
        try
        {
            // 在实际应用中，这里应该保存应用程序的关键状态
            ConfigManager.SaveConfig();
            Utils.LogDebug("应用程序配置已保存", "ErrorHandler");
        }
        catch { /* 静默处理保存状态错误 */ }
    }

    /// <summary>
    /// 尝试重启应用程序
    /// </summary>
    private static void TryRestartApplication()
    {
        try
        {
            Utils.LogDebug("尝试重启应用程序", "ErrorHandler");
            
            // 获取当前应用程序的路径
            string appPath = Application.ExecutablePath;
            
            // 启动一个新的应用程序实例
            Process.Start(appPath);
            
            // 关闭当前应用程序
            Application.Exit();
        }
        catch (Exception ex)
        {
            // 如果重启失败，显示错误消息
            Utils.LogError("重启应用程序失败", ex, "ErrorHandler");
            MessageBox.Show(
                "无法重启应用程序，请手动启动。\n" + ex.Message,
                "重启失败",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error
            );
        }
    }

    /// <summary>
    /// 获取错误日志文件路径
    /// </summary>
    /// <returns>错误日志文件的完整路径</returns>
    public static string GetErrorLogFilePath()
    {
        return _logFilePath;
    }

    /// <summary>
    /// 检查错误处理器是否已初始化
    /// </summary>
    /// <returns>是否已初始化</returns>
    public static bool IsInitialized()
    {
        return _isInitialized;
    }

    /// <summary>
    /// 更新错误处理选项
    /// </summary>
    /// <param name="options">新的错误处理选项</param>
    public static void UpdateOptions(ErrorHandlingOptions options)
    {
        if (options != null)
        {
            _options = options;
            Utils.LogDebug("错误处理选项已更新", "ErrorHandler");
        }
    }
}

/// <summary>
/// 错误处理选项类
/// 定义错误处理器的行为
/// </summary>
public class ErrorHandlingOptions
{
    /// <summary>
    /// 是否显示错误对话框
    /// </summary>
    public bool ShowErrorDialogs { get; set; } = true;

    /// <summary>
    /// 是否显示详细的错误信息
    /// </summary>
    public bool ShowDetailedErrors { get; set; } = false;

    /// <summary>
    /// 是否在致命错误时自动退出应用程序
    /// </summary>
    public bool AutoExitOnFatalError { get; set; } = true;

    /// <summary>
    /// 是否允许用户取消错误处理
    /// </summary>
    public bool AllowCancelErrorHandling { get; set; } = true;

    /// <summary>
    /// 是否将错误记录到文件
    /// </summary>
    public bool LogToFile { get; set; } = true;

    /// <summary>
    /// 是否在错误发生时自动保存应用程序状态
    /// </summary>
    public bool AutoSaveStateOnError { get; set; } = true;

    /// <summary>
    /// 是否在错误发生时尝试自动恢复
    /// </summary>
    public bool AutoRecoverOnError { get; set; } = true;

    /// <summary>
    /// 错误日志文件的最大大小（字节）
    /// 默认：10MB
    /// </summary>
    public long MaxLogFileSize { get; set; } = 10 * 1024 * 1024;

    /// <summary>
    /// 错误重试次数
    /// </summary>
    public int MaxRetryAttempts { get; set; } = 3;

    /// <summary>
    /// 错误重试间隔（毫秒）
    /// </summary>
    public int RetryIntervalMs { get; set; } = 1000;
}

/// <summary>
/// 自定义异常基类
/// 应用程序特定的异常
/// </summary>
public class ApplicationException : Exception
{
    /// <summary>
    /// 错误代码
    /// </summary>
    public int ErrorCode { get; set; }

    /// <summary>
    /// 是否可恢复的错误
    /// </summary>
    public bool IsRecoverable { get; set; }

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="message">错误消息</param>
    public ApplicationException(string message) : base(message)
    {
        ErrorCode = 0;
        IsRecoverable = true;
    }

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="message">错误消息</param>
    /// <param name="errorCode">错误代码</param>
    public ApplicationException(string message, int errorCode) : base(message)
    {
        ErrorCode = errorCode;
        IsRecoverable = true;
    }

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="message">错误消息</param>
    /// <param name="innerException">内部异常</param>
    public ApplicationException(string message, Exception innerException) : base(message, innerException)
    {
        ErrorCode = 0;
        IsRecoverable = true;
    }

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="message">错误消息</param>
    /// <param name="errorCode">错误代码</param>
    /// <param name="isRecoverable">是否可恢复</param>
    public ApplicationException(string message, int errorCode, bool isRecoverable) : base(message)
    {
        ErrorCode = errorCode;
        IsRecoverable = isRecoverable;
    }
}

/// <summary>
/// 配置异常类
/// 配置相关的异常
/// </summary>
public class ConfigurationException : ApplicationException
{
    /// <summary>
    /// 配置项名称
    /// </summary>
    public string ConfigKey { get; set; }

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="message">错误消息</param>
    public ConfigurationException(string message) : base(message, 1001)
    {
        ConfigKey = string.Empty;
    }

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="message">错误消息</param>
    /// <param name="configKey">配置项名称</param>
    public ConfigurationException(string message, string configKey) : base(message, 1001)
    {
        ConfigKey = configKey;
    }

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="message">错误消息</param>
    /// <param name="innerException">内部异常</param>
    public ConfigurationException(string message, Exception innerException) : base(message, innerException)
    {
        ConfigKey = string.Empty;
        ErrorCode = 1001;
    }
}

/// <summary>
/// API异常类
/// API调用相关的异常
/// </summary>
public class ApiException : ApplicationException
{
    /// <summary>
    /// API状态码
    /// </summary>
    public int ApiStatusCode { get; set; }

    /// <summary>
    /// API端点
    /// </summary>
    public string ApiEndpoint { get; set; }

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="message">错误消息</param>
    public ApiException(string message) : base(message, 2001)
    {
        ApiEndpoint = string.Empty;
    }

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="message">错误消息</param>
    /// <param name="apiStatusCode">API状态码</param>
    public ApiException(string message, int apiStatusCode) : base(message, 2001)
    {
        ApiStatusCode = apiStatusCode;
        ApiEndpoint = string.Empty;
    }

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="message">错误消息</param>
    /// <param name="apiStatusCode">API状态码</param>
    /// <param name="apiEndpoint">API端点</param>
    public ApiException(string message, int apiStatusCode, string apiEndpoint) : base(message, 2001)
    {
        ApiStatusCode = apiStatusCode;
        ApiEndpoint = apiEndpoint;
    }
}