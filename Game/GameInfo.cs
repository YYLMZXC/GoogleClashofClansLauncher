using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using GoogleClashofClansLauncher.Core;
using GoogleClashofClansLauncher.Core.System;
using GoogleClashofClansLauncher.Core.UI;

namespace GoogleClashofClansLauncher.Game;

/// <summary>
/// 游戏信息类
/// 管理游戏相关的信息和属性
/// </summary>
public class GameInfo
{
    #region 单例模式
    private static readonly Lazy<GameInfo> _instance = new Lazy<GameInfo>(() => new GameInfo());
    public static GameInfo Instance => _instance.Value;
    private GameInfo() { Initialize(); }
    #endregion

    // 游戏基本信息
    private string _gameName = "Clash of Clans";
    private string _gameVersion = "Unknown";
    private string _gamePath = string.Empty;
    private string _gameProcessName = "ClashofClans";
    private bool _isGameRunning = false;
    private IntPtr _gameWindowHandle = IntPtr.Zero;
    private Rectangle _gameWindowRect = Rectangle.Empty;

    private readonly WindowManager _windowManager = new WindowManager();

    private readonly WindowFinder _windowFinder = new WindowFinder();

    // 游戏分辨率信息
    private int _preferredWidth = 1024;
    private int _preferredHeight = 768;

    // 游戏模板信息
    private Dictionary<string, GameTemplate> _gameTemplates = new Dictionary<string, GameTemplate>();

    #region 属性
    /// <summary>
    /// 游戏名称
    /// </summary>
    public string GameName => _gameName;

    /// <summary>
    /// 游戏版本
    /// </summary>
    public string GameVersion
    {
        get => _gameVersion;
        set
        {
            if (_gameVersion != value)
            {
                _gameVersion = value;
                Utils.LogDebug($"游戏版本已更新: {value}", "GameInfo");
            }
        }
    }

    /// <summary>
    /// 游戏路径
    /// </summary>
    public string GamePath
    {
        get => _gamePath;
        set
        {
            if (_gamePath != value)
            {
                _gamePath = value;
                Utils.LogDebug($"游戏路径已更新: {value}", "GameInfo");
            }
        }
    }

    /// <summary>
    /// 游戏进程名称
    /// </summary>
    public string GameProcessName => _gameProcessName;

    /// <summary>
    /// 游戏是否运行
    /// </summary>
    public bool IsGameRunning
    {
        get => _isGameRunning;
        set
        {
            if (_isGameRunning != value)
            {
                _isGameRunning = value;
                Utils.LogDebug($"游戏运行状态已更新: {value}", "GameInfo");
                OnGameRunningStateChanged?.Invoke(this, EventArgs.Empty);
            }
        }
    }

    /// <summary>
    /// 游戏窗口句柄
    /// </summary>
    public IntPtr GameWindowHandle
    {
        get => _gameWindowHandle;
        set
        {
            if (_gameWindowHandle != value)
            {
                _gameWindowHandle = value;
                Utils.LogDebug($"游戏窗口句柄已更新: {value}", "GameInfo");
                OnGameWindowChanged?.Invoke(this, EventArgs.Empty);
            }
        }
    }

    /// <summary>
    /// 游戏窗口矩形
    /// </summary>
    public Rectangle GameWindowRect
    {
        get => _gameWindowRect;
        set
        {
            if (_gameWindowRect != value)
            {
                _gameWindowRect = value;
                Utils.LogDebug($"游戏窗口矩形已更新: X={value.X}, Y={value.Y}, Width={value.Width}, Height={value.Height}", "GameInfo");
                OnGameWindowResized?.Invoke(this, EventArgs.Empty);
            }
        }
    }

    /// <summary>
    /// 首选宽度
    /// </summary>
    public int PreferredWidth
    {
        get => _preferredWidth;
        set => _preferredWidth = Math.Max(800, value);
    }

    /// <summary>
    /// 首选高度
    /// </summary>
    public int PreferredHeight
    {
        get => _preferredHeight;
        set => _preferredHeight = Math.Max(600, value);
    }

    /// <summary>
    /// 游戏模板字典
    /// </summary>
    public Dictionary<string, GameTemplate> GameTemplates => _gameTemplates;
    #endregion

    #region 事件
    /// <summary>
    /// 游戏运行状态改变事件
    /// </summary>
    public event EventHandler OnGameRunningStateChanged;

    /// <summary>
    /// 游戏窗口改变事件
    /// </summary>
    public event EventHandler OnGameWindowChanged;

    /// <summary>
    /// 游戏窗口大小改变事件
    /// </summary>
    public event EventHandler OnGameWindowResized;

    /// <summary>
    /// 游戏模板加载完成事件
    /// </summary>
    public event EventHandler OnTemplatesLoaded;
    #endregion

    #region 初始化方法
    /// <summary>
    /// 初始化游戏信息
    /// </summary>
    private void Initialize()
    {
        try
        {
            LoadGameTemplates();
            UpdateGameStatus();
            Utils.LogDebug("游戏信息管理器已初始化", "GameInfo");
        }
        catch (Exception ex)
        {
            Utils.LogError("初始化游戏信息管理器失败", ex, "GameInfo");
        }
    }

    /// <summary>
    /// 加载游戏模板
    /// </summary>
    public void LoadGameTemplates()
    {
        try
        {
            _gameTemplates.Clear();
            
            // 获取模板目录
            string templatesDir = new ResourceManager().GetResourcePath(Constants.RESOURCE_TEMPLATES_FOLDER);
            
            if (!Directory.Exists(templatesDir))
            {
                Directory.CreateDirectory(templatesDir);
                Utils.LogWarning("游戏模板目录不存在，已创建", "GameInfo");
                return;
            }

            // 加载所有模板文件
            string[] templateFiles = Directory.GetFiles(templatesDir, "*.json", SearchOption.AllDirectories);
            
            foreach (string templateFile in templateFiles)
            {
                try
                {
                    string templateName = Path.GetFileNameWithoutExtension(templateFile);
                    string templateJson = File.ReadAllText(templateFile);
                    
                    // 这里应该解析JSON并创建GameTemplate对象
                    // 简化实现，实际应用中应该使用JSON序列化/反序列化
                    GameTemplate template = new GameTemplate
                    {
                        TemplateName = templateName,
                        TemplatePath = templateFile,
                        LastModified = File.GetLastWriteTime(templateFile)
                    };
                    
                    _gameTemplates[templateName] = template;
                    Utils.LogDebug($"已加载游戏模板: {templateName}", "GameInfo");
                }
                catch (Exception ex)
                {
                    Utils.LogError($"加载游戏模板失败: {Path.GetFileName(templateFile)}", ex, "GameInfo");
                }
            }

            Utils.LogDebug($"已加载 {_gameTemplates.Count} 个游戏模板", "GameInfo");
            OnTemplatesLoaded?.Invoke(this, EventArgs.Empty);
        }
        catch (Exception ex)
        {
            Utils.LogError("加载游戏模板失败", ex, "GameInfo");
        }
    }

    /// <summary>
    /// 更新游戏状态
    /// </summary>
    public void UpdateGameStatus()
    {
        try
        {
            // 查找游戏窗口
            IntPtr windowHandle = _windowFinder.FindWindowByProcessName(_gameProcessName);

            if (windowHandle != IntPtr.Zero)
            {
                IsGameRunning = true;
                GameWindowHandle = windowHandle;
                
                // 获取窗口位置和大小
                Rectangle windowRect = _windowFinder.GetWindowRect(windowHandle);
                GameWindowRect = windowRect;
            }
            else
            {
                IsGameRunning = false;
                GameWindowHandle = IntPtr.Zero;
                GameWindowRect = Rectangle.Empty;
            }
        }
        catch (Exception ex)
        {
            Utils.LogError("更新游戏状态失败", ex, "GameInfo");
        }
    }
    #endregion

    #region 游戏信息方法
    /// <summary>
    /// 获取游戏中心坐标
    /// </summary>
    /// <returns>游戏窗口中心坐标</returns>
    public Point GetGameCenter()
    {
        if (!IsGameRunning || GameWindowRect.IsEmpty)
        {
            return Point.Empty;
        }

        int centerX = GameWindowRect.Left + GameWindowRect.Width / 2;
        int centerY = GameWindowRect.Top + GameWindowRect.Height / 2;
        
        return new Point(centerX, centerY);
    }

    /// <summary>
    /// 获取游戏窗口尺寸
    /// </summary>
    /// <returns>窗口尺寸</returns>
    public Size GetGameWindowSize()
    {
        if (!IsGameRunning || GameWindowRect.IsEmpty)
        {
            return Size.Empty;
        }

        return new Size(GameWindowRect.Width, GameWindowRect.Height);
    }

    /// <summary>
    /// 检查游戏窗口是否在屏幕内
    /// </summary>
    /// <returns>是否在屏幕内</returns>
    public bool IsGameWindowVisible()
    {
        if (!IsGameRunning || GameWindowHandle == IntPtr.Zero)
        {
            return false;
        }

        // 检查窗口是否可见
        return _windowManager.IsWindowVisible(GameWindowHandle);
    }

    /// <summary>
    /// 尝试激活游戏窗口
    /// </summary>
    /// <returns>是否激活成功</returns>
    public bool ActivateGameWindow()
    {
        if (!IsGameRunning || GameWindowHandle == IntPtr.Zero)
        {
            return false;
        }

        try
        {
            // 激活窗口
            _windowManager.ActivateWindow(GameWindowHandle);
            Utils.LogDebug("已尝试激活游戏窗口", "GameInfo");
            return true;
        }
        catch (Exception ex)
        {
            Utils.LogError("激活游戏窗口失败", ex, "GameInfo");
            return false;
        }
    }

    /// <summary>
    /// 获取游戏模板
    /// </summary>
    /// <param name="templateName">模板名称</param>
    /// <returns>游戏模板对象</returns>
    public GameTemplate? GetGameTemplate(string templateName)
     {
         if (string.IsNullOrEmpty(templateName) || !_gameTemplates.ContainsKey(templateName))
         {
            return null;
        }

        return _gameTemplates[templateName];
    }

    /// <summary>
    /// 添加或更新游戏模板
    /// </summary>
    /// <param name="template">游戏模板</param>
    public void AddOrUpdateGameTemplate(GameTemplate template)
    {
        if (template == null || string.IsNullOrEmpty(template.TemplateName))
        {
            return;
        }

        _gameTemplates[template.TemplateName] = template;
        Utils.LogDebug($"添加或更新游戏模板: {template.TemplateName}", "GameInfo");
    }

    /// <summary>
    /// 删除游戏模板
    /// </summary>
    /// <param name="templateName">模板名称</param>
    public void RemoveGameTemplate(string templateName)
    {
        if (string.IsNullOrEmpty(templateName) || !_gameTemplates.ContainsKey(templateName))
        {
            return;
        }

        _gameTemplates.Remove(templateName);
        Utils.LogDebug($"删除游戏模板: {templateName}", "GameInfo");
    }
    #endregion

    #region 游戏路径相关方法
    /// <summary>
    /// 验证游戏路径是否有效
    /// </summary>
    /// <param name="path">游戏路径</param>
    /// <returns>是否有效</returns>
    public bool ValidateGamePath(string path)
    {
        if (string.IsNullOrEmpty(path))
        {
            return false;
        }

        // 检查路径是否存在
        if (!Directory.Exists(path))
        {
            return false;
        }

        // 检查是否包含游戏主程序
        string[] gameExeFiles = Directory.GetFiles(path, $"{_gameProcessName}.exe", SearchOption.AllDirectories);
        return gameExeFiles.Length > 0;
    }

    /// <summary>
    /// 查找游戏安装路径
    /// </summary>
    /// <returns>游戏安装路径</returns>
    public string FindGameInstallationPath()
    {
        try
        {
            // 检查常见安装位置
            string[] commonPaths = new string[]
            {
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), $"{_gameName}"),
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86), $"{_gameName}"),
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), $"{_gameName}"),
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), $"{_gameName}")
            };

            foreach (string path in commonPaths)
            {
                if (ValidateGamePath(path))
                {
                    return path;
                }
            }

            Utils.LogWarning("未找到游戏安装路径", "GameInfo");
            return string.Empty;
        }
        catch (Exception ex)
        {
            Utils.LogError("查找游戏安装路径失败", ex, "GameInfo");
            return string.Empty;
        }
    }
    #endregion

    #region 资源释放
    /// <summary>
    /// 释放资源
    /// </summary>
    public void Dispose()
    {
        try
        {
            _gameTemplates.Clear();
            _gameWindowHandle = IntPtr.Zero;
            _gameWindowRect = Rectangle.Empty;
            
            // 移除所有事件订阅
            if (OnGameRunningStateChanged != null)
            {
                foreach (Delegate d in OnGameRunningStateChanged.GetInvocationList())
                {
                    OnGameRunningStateChanged -= (EventHandler)d;
                }
            }
            if (OnGameWindowChanged != null)
            {
                foreach (Delegate d in OnGameWindowChanged.GetInvocationList())
                {
                    OnGameWindowChanged -= (EventHandler)d;
                }
            }
            if (OnGameWindowResized != null)
            {
                foreach (Delegate d in OnGameWindowResized.GetInvocationList())
                {
                    OnGameWindowResized -= (EventHandler)d;
                }
            }
            if (OnTemplatesLoaded != null)
            {
                foreach (Delegate d in OnTemplatesLoaded.GetInvocationList())
                {
                    OnTemplatesLoaded -= (EventHandler)d;
                }
            }
            
            Utils.LogDebug("游戏信息管理器资源已释放", "GameInfo");
        }
        catch (Exception ex)
        {
            Utils.LogError("释放游戏信息管理器资源失败", ex, "GameInfo");
        }
    }
    #endregion
}