using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using GoogleClashofClansLauncher.Config;
using GoogleClashofClansLauncher.Core;
using GoogleClashofClansLauncher.Core.System;
using ApplicationException = GoogleClashofClansLauncher.Core.System.ApplicationException;

namespace GoogleClashofClansLauncher.Settings;

/// <summary>
/// 设置管理器类
/// 专门管理应用程序的各种设置
/// </summary>
public class SettingsManager : INotifyPropertyChanged
{
    private static SettingsManager? _instance;
    private static readonly object _lockObject = new object();
    private AppConfig? _config;
    private Dictionary<string, SettingInfo> _settingsCache = new Dictionary<string, SettingInfo>();
    private bool _isLoading = false;

    /// <summary>
    /// 单例实例
    /// </summary>
    public static SettingsManager Instance
    {
        get
        {
            lock (_lockObject)
            {
                if (_instance == null)
                {
                    _instance = new SettingsManager();
                }
                return _instance;
            }
        }
    }

    /// <summary>
    /// 配置对象
    /// </summary>
    public AppConfig? Config
    {
        get { return _config; }
        private set { _config = value; }
    }

    /// <summary>
    /// 属性更改事件
    /// </summary>
    public event PropertyChangedEventHandler? PropertyChanged;

    /// <summary>
    /// 构造函数
    /// </summary>
    private SettingsManager()
    {
        LoadSettings();
        InitializeSettingsCache();
    }

    /// <summary>
    /// 加载设置
    /// </summary>
    public void LoadSettings()
    {
        try
        {
            _isLoading = true;
            Config = ConfigManager.GetConfig();
            Utils.LogDebug("设置加载成功", "SettingsManager");
        }
        catch (Exception ex)
        {
            Utils.LogError("加载设置失败", ex, "SettingsManager");
        }
        finally
        {
            _isLoading = false;
        }
    }

    /// <summary>
    /// 保存设置
    /// </summary>
    public void SaveSettings()
    {
        try
        {
            ConfigManager.SaveConfig();
            Utils.LogDebug("设置保存成功", "SettingsManager");
        }
        catch (Exception ex)
        {
            Utils.LogError("保存设置失败", ex, "SettingsManager");
            throw new ApplicationException("保存设置失败，请检查权限或磁盘空间", ex);
        }
    }

    /// <summary>
    /// 重置设置为默认值
    /// </summary>
    public void ResetSettings()
    {
        try
        {
            _isLoading = true;
            ConfigManager.ResetConfig();
            Config = ConfigManager.GetConfig();
            InitializeSettingsCache();
            Utils.LogDebug("设置已重置为默认值", "SettingsManager");
            
            // 通知所有属性更改
            OnPropertyChanged(null);
        }
        catch (Exception ex)
        {
            Utils.LogError("重置设置失败", ex, "SettingsManager");
            throw new ApplicationException("重置设置失败", ex);
        }
        finally
        {
            _isLoading = false;
        }
    }

    /// <summary>
    /// 获取设置值
    /// </summary>
    /// <typeparam name="T">设置类型</typeparam>
    /// <param name="settingPath">设置路径，格式为 "Section.PropertyName"</param>
    /// <param name="defaultValue">默认值</param>
    /// <returns>设置值</returns>
    public T? GetSetting<T>(string settingPath, T? defaultValue = default)
    {
        try
        {
            if (string.IsNullOrEmpty(settingPath))
            {
                throw new ArgumentNullException(nameof(settingPath));
            }

            // 检查缓存
            if (_settingsCache.TryGetValue(settingPath, out SettingInfo? settingInfo) && settingInfo != null)
            {
                return ConvertSettingValue<T>(settingInfo.Value, defaultValue);
            }

            // 解析设置路径
            string[] pathParts = settingPath.Split('.');
            if (pathParts.Length < 2)
            {
                throw new ArgumentException("设置路径格式无效，应为 'Section.PropertyName'", nameof(settingPath));
            }

            string sectionName = pathParts[0];
            string propertyName = string.Join(".", pathParts.Skip(1));

            // 获取设置值
            object? section = GetConfigSection(sectionName);
            if (section == null)
            {
                Utils.LogWarning($"未找到设置节 '{sectionName}'", "SettingsManager");
                return defaultValue;
            }

            var (parent, property) = GetNestedPropertyAndParent(section, propertyName);
            if (property == null || parent == null)
            {
                Utils.LogWarning($"未找到设置属性 '{settingPath}'", "SettingsManager");
                return defaultValue;
            }

            // 获取属性值
            object? value = property.GetValue(parent);
            
            // 更新缓存
            settingInfo = new SettingInfo
            {
                Section = parent,
                Property = property,
                Value = value
            };
            _settingsCache[settingPath] = settingInfo;

            return ConvertSettingValue<T>(value, defaultValue);
        }
        catch (Exception ex)
        {
            Utils.LogError($"获取设置 '{settingPath}' 失败", ex, "SettingsManager");
            return defaultValue;
        }
    }

    /// <summary>
    /// 设置设置值
    /// </summary>
    /// <typeparam name="T">设置类型</typeparam>
    /// <param name="settingPath">设置路径</param>
    /// <param name="value">设置值</param>
    /// <returns>是否设置成功</returns>
    public bool SetSetting<T>(string settingPath, T value)
    {
        try
        {
            if (string.IsNullOrEmpty(settingPath))
            {
                throw new ArgumentNullException(nameof(settingPath));
            }

            // 检查缓存
            if (!_settingsCache.TryGetValue(settingPath, out SettingInfo? settingInfo) || settingInfo == null)
            {
                // 解析设置路径
                string[] pathParts = settingPath.Split('.');
                if (pathParts.Length < 2)
                {
                    throw new ArgumentException("设置路径格式无效，应为 'Section.PropertyName'", nameof(settingPath));
                }

                string sectionName = pathParts[0];
                string propertyName = string.Join(".", pathParts.Skip(1));

                // 获取设置节和属性
                object? section = GetConfigSection(sectionName);
                if (section == null)
                {
                    Utils.LogWarning($"未找到设置节 '{sectionName}'", "SettingsManager");
                    return false;
                }

                var (parent, property) = GetNestedPropertyAndParent(section, propertyName);
                if (property == null || parent == null)
                {
                    Utils.LogWarning($"未找到设置属性 '{settingPath}'", "SettingsManager");
                    return false;
                }

                // 创建缓存项
                settingInfo = new SettingInfo
                {
                    Section = parent,
                    Property = property
                };
                _settingsCache[settingPath] = settingInfo;
            }

            // 转换值类型
            object? convertedValue = value != null ? Convert.ChangeType(value, settingInfo.Property.PropertyType) : null;
            
            // 设置属性值
            settingInfo.Property.SetValue(settingInfo.Section, convertedValue);
            settingInfo.Value = convertedValue;

            // 通知属性更改
            if (!_isLoading)
            {
                OnPropertyChanged(settingPath);
            }

            Utils.LogDebug($"设置 '{settingPath}' 已更新为 '{value}'", "SettingsManager");
            return true;
        }
        catch (Exception ex)
        {
            Utils.LogError($"设置 '{settingPath}' 失败", ex, "SettingsManager");
            return false;
        }
    }

    /// <summary>
    /// 获取配置节
    /// </summary>
    /// <param name="sectionName">节名称</param>
    /// <returns>配置节对象</returns>
    private object? GetConfigSection(string sectionName)
    {
        try
        {
            if (Config == null) return null;
            
            PropertyInfo? sectionProperty = typeof(AppConfig).GetProperty(sectionName, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
            if (sectionProperty == null)
            {
                return null;
            }

            return sectionProperty.GetValue(Config);
        }
        catch (Exception ex)
        {
            Utils.LogError($"获取配置节 '{sectionName}' 失败", ex, "SettingsManager");
            return null;
        }
    }

    /// <summary>
    /// 获取嵌套属性
    /// </summary>
    /// <param name="obj">对象</param>
    /// <param name="propertyPath">属性路径</param>
    /// <returns>属性信息</returns>
    private (object? parent, PropertyInfo? property) GetNestedPropertyAndParent(object obj, string propertyPath)
    {
        try
        {
            var pathParts = propertyPath.Split('.');
            object? currentObj = obj;

            for (int i = 0; i < pathParts.Length; i++)
            {
                if (currentObj == null) return (null, null);

                var property = currentObj.GetType().GetProperty(pathParts[i], BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
                if (property == null) return (null, null);

                if (i == pathParts.Length - 1)
                {
                    return (currentObj, property);
                }
                else
                {
                    currentObj = property.GetValue(currentObj);
                }
            }

            return (null, null);
        }
        catch (Exception ex)
        {
            Utils.LogError($"获取嵌套属性 '{propertyPath}' 失败", ex, "SettingsManager");
            return (null, null);
        }
    }

    /// <summary>
    /// 转换设置值
    /// </summary>
    /// <typeparam name="T">目标类型</typeparam>
    /// <param name="value">原始值</param>
    /// <param name="defaultValue">默认值</param>
    /// <returns>转换后的值</returns>
    private T? ConvertSettingValue<T>(object? value, T? defaultValue)
    {
        try
        {
            if (value == null || value == DBNull.Value)
            {
                return defaultValue;
            }

            if (value is T tValue)
            {
                return tValue;
            }

            return (T)Convert.ChangeType(value, typeof(T));
        }
        catch (Exception ex)
        {
            Utils.LogWarning($"转换设置值为类型 '{typeof(T).Name}' 失败: {ex.Message}", "SettingsManager");
            return defaultValue;
        }
    }

    /// <summary>
    /// 初始化设置缓存
    /// </summary>
    private void InitializeSettingsCache()
    {
        _settingsCache.Clear();
        
        // 在实际应用中，可以预加载常用设置到缓存
        // 这里简化处理
        Utils.LogDebug("设置缓存已初始化", "SettingsManager");
    }

    /// <summary>
    /// 触发属性更改事件
    /// </summary>
    /// <param name="propertyName">属性名称</param>
    protected virtual void OnPropertyChanged(string? propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    /// <summary>
    /// 获取所有设置节
    /// </summary>
    /// <returns>设置节列表</returns>
    public List<string> GetAllSections()
    {
        try
        {
            return typeof(AppConfig)
                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(p => p.PropertyType.IsClass && p.PropertyType != typeof(string))
                .Select(p => p.Name)
                .ToList();
        }
        catch (Exception ex)
        {
            Utils.LogError("获取所有设置节失败", ex, "SettingsManager");
            return new List<string>();
        }
    }

    /// <summary>
    /// 获取设置节的所有属性
    /// </summary>
    /// <param name="sectionName">节名称</param>
    /// <returns>属性列表</returns>
    public List<string> GetSectionProperties(string sectionName)
    {
        try
        {
            object? section = GetConfigSection(sectionName);
            if (section == null)
            {
                return new List<string>();
            }

            return section.GetType()
                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Select(p => p.Name)
                .ToList();
        }
        catch (Exception ex)
        {
            Utils.LogError($"获取设置节 '{sectionName}' 的属性失败", ex, "SettingsManager");
            return new List<string>();
        }
    }

    /// <summary>
    /// 检查设置是否存在
    /// </summary>
    /// <param name="settingPath">设置路径</param>
    /// <returns>是否存在</returns>
    public bool SettingExists(string settingPath)
    {
        try
        {
            if (string.IsNullOrEmpty(settingPath))
            {
                return false;
            }

            // 检查缓存
            if (_settingsCache.ContainsKey(settingPath))
            {
                return true;
            }

            // 解析设置路径
            string[] pathParts = settingPath.Split('.');
            if (pathParts.Length < 2)
            {
                return false;
            }

            string sectionName = pathParts[0];
            string propertyName = string.Join(".", pathParts.Skip(1));

            // 检查设置节和属性
            object? section = GetConfigSection(sectionName);
            if (section == null)
            {
                return false;
            }

            var (parent, property) = GetNestedPropertyAndParent(section, propertyName);

            return property != null;
        }
        catch (Exception)
        {
            return false;
        }
    }

    /// <summary>
    /// 设置信息类
    /// 用于缓存设置信息
    /// </summary>
    private class SettingInfo
    {
        /// <summary>
        /// 配置节对象
        /// </summary>
        public required object Section { get; set; }

        /// <summary>
        /// 属性信息
        /// </summary>
        public required PropertyInfo Property { get; set; }

        /// <summary>
        /// 属性值
        /// </summary>
        public object? Value { get; set; }
    }
}