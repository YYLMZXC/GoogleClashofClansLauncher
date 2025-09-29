using System;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using GoogleClashofClansLauncher.Core;
using GoogleClashofClansLauncher.Core.System;

namespace GoogleClashofClansLauncher.Config;

/// <summary>
/// 配置管理器类
/// 负责应用程序配置的加载、保存和管理
/// </summary>
public class ConfigManager
{
    private const string DEFAULT_CONFIG_FILE_NAME = "app_config.json";
    private static string _configFilePath;
    private static AppConfig _appConfig;
    private static readonly object _lockObject = new object();

    /// <summary>
    /// 静态构造函数
    /// 初始化配置文件路径
    /// </summary>
    static ConfigManager()
    {
        string appDataDir = Utils.GetAppDataDirectory();
        _configFilePath = Path.Combine(appDataDir, DEFAULT_CONFIG_FILE_NAME);
    }

    /// <summary>
    /// 获取当前应用配置
    /// </summary>
    /// <returns>应用配置对象</returns>
    public static AppConfig GetConfig()
    {
        lock (_lockObject)
        {
            if (_appConfig == null)
            {
                LoadConfig();
            }
            return _appConfig;
        }
    }

    /// <summary>
    /// 加载配置文件
    /// </summary>
    public static void LoadConfig()
    {
        lock (_lockObject)
        {
            try
            {
                if (!File.Exists(_configFilePath))
                {
                    // 如果配置文件不存在，创建默认配置
                    _appConfig = CreateDefaultConfig();
                    SaveConfig();
                    Utils.LogDebug("配置文件不存在，已创建默认配置", "ConfigManager");
                    return;
                }

                string jsonContent = File.ReadAllText(_configFilePath);
                var appConfig = JsonSerializer.Deserialize<AppConfig>(jsonContent);

                if (appConfig == null)
                {
                    _appConfig = CreateDefaultConfig();
                    Utils.LogWarning("配置文件内容无效，已使用默认配置", "ConfigManager");
                }
                else
                {
                    _appConfig = appConfig;
                    Utils.LogDebug("配置文件加载成功", "ConfigManager");
                }
            }
            catch (Exception ex)
            {
                _appConfig = CreateDefaultConfig();
                Utils.LogError("加载配置文件失败", ex, "ConfigManager");
            }
        }
    }

    /// <summary>
    /// 保存配置文件
    /// </summary>
    public static void SaveConfig()
    {
        lock (_lockObject)
        {
            try
            {
                if (_appConfig == null)
                {
                    _appConfig = CreateDefaultConfig();
                }

                // 确保配置目录存在
                string configDir = Path.GetDirectoryName(_configFilePath);
                if (configDir != null && !Directory.Exists(configDir))
                {
                    Directory.CreateDirectory(configDir);
                }

                // 序列化配置对象
                string jsonContent = JsonSerializer.Serialize(
                    _appConfig,
                    new JsonSerializerOptions
                    {
                        WriteIndented = true,
                        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
                    }
                );

                // 写入文件
                File.WriteAllText(_configFilePath, jsonContent);
                Utils.LogDebug("配置文件保存成功", "ConfigManager");
            }
            catch (Exception ex)
            {
                Utils.LogError("保存配置文件失败", ex, "ConfigManager");
            }
        }
    }

    /// <summary>
    /// 创建默认配置
    /// </summary>
    /// <returns>默认配置对象</returns>
    private static AppConfig CreateDefaultConfig()
    {
        return new AppConfig
        {
            General = new GeneralConfig
            {
                AutoStart = false,
                CheckForUpdates = true,
                Language = "zh-CN",
                Theme = "Light"
            },
            Game = new GameConfig
            {
                GameProcessName = "clashofclans",
                GameWindowTitle = "Clash of Clans",
                GameResolution = "1280x720",
                GameInteractionDelay = 1000
            },
            ImageRecognition = new ImageRecognitionConfig
            {
                MatchThreshold = 0.8,
                SearchRegion = "fullscreen",
                EnableImageEnhancement = true
            },
            API = new APIConfig
            {
                ApiKey = string.Empty,
                ApiUrl = "https://api.example.com",
                Timeout = 30000,
                EnableLogging = false
            }
        };
    }

    /// <summary>
    /// 获取配置文件路径
    /// </summary>
    /// <returns>配置文件的完整路径</returns>
    public static string GetConfigFilePath()
    {
        return _configFilePath;
    }

    /// <summary>
    /// 重置配置为默认值
    /// </summary>
    public static void ResetConfig()
    {
        lock (_lockObject)
        {
            _appConfig = CreateDefaultConfig();
            SaveConfig();
            Utils.LogDebug("配置已重置为默认值", "ConfigManager");
        }
    }

    /// <summary>
    /// 更新配置项
    /// </summary>
    /// <typeparam name="T">配置项类型</typeparam>
    /// <param name="updater">配置更新函数</param>
    public static void UpdateConfig<T>(Action<AppConfig> updater)
    {
        lock (_lockObject)
        {
            try
            {
                if (_appConfig == null)
                {
                    _appConfig = CreateDefaultConfig();
                }

                updater(_appConfig);
                SaveConfig();
            }
            catch (Exception ex)
            {
                Utils.LogError("更新配置失败", ex, "ConfigManager");
            }
        }
    }
}

/// <summary>
/// 应用程序配置类
/// 包含所有配置项
/// </summary>
public class AppConfig
{
    /// <summary>
    /// 通用配置
    /// </summary>
    public GeneralConfig General { get; set; }

    /// <summary>
    /// 游戏相关配置
    /// </summary>
    public GameConfig Game { get; set; }

    /// <summary>
    /// 图像识别配置
    /// </summary>
    public ImageRecognitionConfig ImageRecognition { get; set; }

    /// <summary>
    /// API相关配置
    /// </summary>
    public APIConfig API { get; set; }

    /// <summary>
    /// 配置版本号
    /// 用于配置迁移
    /// </summary>
    public string Version { get; set; } = "1.0.0";

    /// <summary>
    /// 上次修改时间
    /// </summary>
    [JsonIgnore]
    public DateTime LastModified { get; set; } = DateTime.Now;
}

/// <summary>
/// 通用配置类
/// </summary>
public class GeneralConfig
{
    /// <summary>
    /// 是否自动启动
    /// </summary>
    public bool AutoStart { get; set; }

    /// <summary>
    /// 是否检查更新
    /// </summary>
    public bool CheckForUpdates { get; set; }

    /// <summary>
    /// 语言设置
    /// </summary>
    public string Language { get; set; }

    /// <summary>
    /// 主题设置
    /// </summary>
    public string Theme { get; set; }
}

/// <summary>
/// 游戏相关配置类
/// </summary>
public class GameConfig
{
    /// <summary>
    /// 游戏进程名称
    /// </summary>
    public string GameProcessName { get; set; }

    /// <summary>
    /// 游戏窗口标题
    /// </summary>
    public string GameWindowTitle { get; set; }

    /// <summary>
    /// 游戏分辨率
    /// </summary>
    public string GameResolution { get; set; }

    /// <summary>
    /// 游戏交互延迟(毫秒)
    /// </summary>
    public int GameInteractionDelay { get; set; }
}

/// <summary>
/// 图像识别配置类
/// </summary>
public class ImageRecognitionConfig
{
    /// <summary>
    /// 匹配阈值
    /// 范围0.0-1.0
    /// </summary>
    public double MatchThreshold { get; set; }

    /// <summary>
    /// 搜索区域
    /// </summary>
    public string SearchRegion { get; set; }

    /// <summary>
    /// 是否启用图像增强
    /// </summary>
    public bool EnableImageEnhancement { get; set; }
}

/// <summary>
/// API相关配置类
/// </summary>
public class APIConfig
{
    /// <summary>
    /// API密钥
    /// </summary>
    [JsonIgnore]
    public string ApiKey { get; set; }

    /// <summary>
    /// API URL
    /// </summary>
    public string ApiUrl { get; set; }

    /// <summary>
    /// 请求超时时间(毫秒)
    /// </summary>
    public int Timeout { get; set; }

    /// <summary>
    /// 是否启用API日志
    /// </summary>
    public bool EnableLogging { get; set; }
}