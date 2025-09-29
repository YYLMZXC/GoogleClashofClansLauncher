using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using GoogleClashofClansLauncher.Core;
using GoogleClashofClansLauncher.Core.System;

namespace GoogleClashofClansLauncher.Game;

/// <summary>
/// 游戏模板类
/// 定义游戏界面元素的模板信息
/// </summary>
public class GameTemplate
{
    /// <summary>
    /// 模板名称
    /// </summary>
    public string TemplateName { get; set; } = string.Empty;

    /// <summary>
    /// 模板文件路径
    /// </summary>
    public string TemplatePath { get; set; } = string.Empty;

    /// <summary>
    /// 模板图像
    /// </summary>
    public Bitmap? TemplateImage { get; set; } = null;

    /// <summary>
    /// 模板图像路径
    /// </summary>
    public string TemplateImagePath { get; set; } = string.Empty;

    /// <summary>
    /// 上次修改时间
    /// </summary>
    public DateTime LastModified { get; set; } = DateTime.MinValue;

    /// <summary>
    /// 识别阈值
    /// </summary>
    public double Threshold { get; set; } = 0.85;

    /// <summary>
    /// 是否启用
    /// </summary>
    public bool IsEnabled { get; set; } = true;

    /// <summary>
    /// 模板描述
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// 点击偏移量
    /// </summary>
    public Point ClickOffset { get; set; } = Point.Empty;

    /// <summary>
    /// 额外属性
    /// </summary>
    public Dictionary<string, object> ExtraProperties { get; set; } = new Dictionary<string, object>();

    /// <summary>
    /// 加载模板图像
    /// </summary>
    /// <returns>是否加载成功</returns>
    public bool LoadTemplateImage()
    {
        try
        {
            if (string.IsNullOrEmpty(TemplateImagePath))
            {
                // 如果没有指定图像路径，尝试从模板路径获取
                if (!string.IsNullOrEmpty(TemplatePath))
                {
                    string imageExtension = Path.GetExtension(TemplatePath);
                    string imagePath = TemplatePath.Replace(imageExtension, ".png");
                    
                    if (File.Exists(imagePath))
                    {
                        TemplateImagePath = imagePath;
                    }
                }
            }

            if (string.IsNullOrEmpty(TemplateImagePath) || !File.Exists(TemplateImagePath))
            {
                Utils.LogWarning($"模板图像文件不存在: {TemplateImagePath}", "GameTemplate");
                return false;
            }

            // 加载图像
            TemplateImage = new Bitmap(TemplateImagePath);
            Utils.LogDebug($"已加载模板图像: {TemplateImagePath}", "GameTemplate");
            return true;
        }
        catch (Exception ex)
        {
            Utils.LogError($"加载模板图像失败: {TemplateImagePath}", ex, "GameTemplate");
            return false;
        }
    }

    /// <summary>
    /// 保存模板到文件
    /// </summary>
    /// <returns>是否保存成功</returns>
    public bool SaveToFile()
    {
        try
        {
            if (string.IsNullOrEmpty(TemplatePath))
            {
                Utils.LogWarning("模板路径为空，无法保存", "GameTemplate");
                return false;
            }

            // 确保目录存在
            string directory = Path.GetDirectoryName(TemplatePath);
            if (directory == null)
            {
                directory = Utils.GetApplicationDataDirectory();
            }

            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            // 这里应该将模板信息序列化为JSON并保存到文件
            // 简化实现，实际应用中应该使用JSON序列化
            string templateJson = $"{{\n  \"TemplateName\": \"{TemplateName}\",\n  \"Threshold\": {Threshold},\n  \"IsEnabled\": {IsEnabled.ToString().ToLower()},\n  \"Description\": \"{Description}\",\n  \"ClickOffset\": {{\"X\": {ClickOffset.X}, \"Y\": {ClickOffset.Y}}}\n}}";

            File.WriteAllText(TemplatePath, templateJson);
            LastModified = DateTime.Now;
            
            Utils.LogDebug($"模板已保存到文件: {TemplatePath}", "GameTemplate");
            return true;
        }
        catch (Exception ex)
        {
            Utils.LogError($"保存模板到文件失败: {TemplatePath}", ex, "GameTemplate");
            return false;
        }
    }

    /// <summary>
    /// 从文件加载模板
    /// </summary>
    /// <param name="templatePath">模板文件路径</param>
    /// <returns>游戏模板对象</returns>
    public static GameTemplate? LoadFromFile(string templatePath)
    {
        try
        {
            if (string.IsNullOrEmpty(templatePath) || !File.Exists(templatePath))
            {
                Utils.LogWarning($"模板文件不存在: {templatePath}", "GameTemplate");
                return null;
            }

            GameTemplate template = new GameTemplate
            {
                TemplatePath = templatePath,
                TemplateName = Path.GetFileNameWithoutExtension(templatePath),
                LastModified = File.GetLastWriteTime(templatePath)
            };

            // 这里应该从文件中读取JSON并解析为模板对象
            // 简化实现，实际应用中应该使用JSON反序列化
            string templateJson = File.ReadAllText(templatePath);
            
            // 尝试解析基本属性
            // 实际应用中应该使用JSON解析库
            if (templateJson.Contains("\"Threshold\":"))
            {
                try
                {
                    int startIndex = templateJson.IndexOf("\"Threshold\":") + 12;
                    int endIndex = templateJson.IndexOf(",", startIndex);
                    if (endIndex == -1) endIndex = templateJson.IndexOf("}", startIndex);
                    string thresholdStr = templateJson.Substring(startIndex, endIndex - startIndex).Trim();
                    template.Threshold = double.Parse(thresholdStr);
                }
                catch { /* 忽略解析错误 */ }
            }

            // 尝试加载模板图像
            template.LoadTemplateImage();

            Utils.LogDebug($"已从文件加载模板: {templatePath}", "GameTemplate");
            return template;
        }
        catch (Exception ex)
        {
            Utils.LogError($"从文件加载模板失败: {templatePath}", ex, "GameTemplate");
            return null;
        }
    }

    /// <summary>
    /// 释放资源
    /// </summary>
    public void Dispose()
    {
        try
        {
            if (TemplateImage != null)
            {
                TemplateImage.Dispose();
                TemplateImage = null;
            }

            ExtraProperties.Clear();
            Utils.LogDebug($"模板资源已释放: {TemplateName}", "GameTemplate");
        }
        catch (Exception ex)
        {
            Utils.LogError($"释放模板资源失败: {TemplateName}", ex, "GameTemplate");
        }
    }

    /// <summary>
    /// 获取模板显示名称
    /// </summary>
    /// <returns>显示名称</returns>
    public override string ToString()
    {
        return string.IsNullOrEmpty(Description) ? TemplateName : $"{TemplateName} ({Description})";
    }

    /// <summary>
    /// 克隆模板
    /// </summary>
    /// <returns>模板的克隆</returns>
    public GameTemplate? Clone()
    {
        try
        {
            GameTemplate clone = new GameTemplate
            {
                TemplateName = this.TemplateName + "_Clone",
                TemplatePath = string.Empty, // 不复制路径，避免覆盖原文件
                TemplateImagePath = this.TemplateImagePath,
                LastModified = DateTime.Now,
                Threshold = this.Threshold,
                IsEnabled = this.IsEnabled,
                Description = this.Description,
                ClickOffset = new Point(this.ClickOffset.X, this.ClickOffset.Y)
            };

            // 深拷贝额外属性
            foreach (var kvp in this.ExtraProperties)
            {
                clone.ExtraProperties[kvp.Key] = kvp.Value;
            }

            // 克隆图像
            if (this.TemplateImage != null)
            {
                clone.TemplateImage = new Bitmap(this.TemplateImage);
            }

            return clone;
        }
        catch (Exception ex)
        {
            Utils.LogError($"克隆模板失败: {TemplateName}", ex, "GameTemplate");
            return null;
        }
    }

    /// <summary>
    /// 检查模板是否有效
    /// </summary>
    /// <returns>是否有效</returns>
    public bool IsValid()
    {
        // 检查必要属性
        if (string.IsNullOrEmpty(TemplateName))
        {
            return false;
        }

        // 检查阈值范围
        if (Threshold < 0.1 || Threshold > 1.0)
        {
            return false;
        }

        // 如果启用了模板，检查图像
        if (IsEnabled && TemplateImage == null && !string.IsNullOrEmpty(TemplateImagePath))
        {
            // 尝试加载图像
            return LoadTemplateImage();
        }

        return true;
    }

    /// <summary>
    /// 设置额外属性
    /// </summary>
    /// <typeparam name="T">属性类型</typeparam>
    /// <param name="key">属性键</param>
    /// <param name="value">属性值</param>
    public void SetExtraProperty<T>(string key, T? value)
    {
        if (string.IsNullOrEmpty(key))
        {
            return;
        }
        if (value == null)
        {
            if (ExtraProperties.ContainsKey(key))
            {
                ExtraProperties.Remove(key);
            }
            return;
        }
        ExtraProperties[key] = value;
    }

    /// <summary>
    /// 获取额外属性
    /// </summary>
    /// <typeparam name="T">属性类型</typeparam>
    /// <param name="key">属性键</param>
    /// <param name="defaultValue">默认值</param>
    /// <returns>属性值</returns>
    public T? GetExtraProperty<T>(string key, T? defaultValue = default)
    {
        if (string.IsNullOrEmpty(key) || !ExtraProperties.ContainsKey(key))
        {
            return defaultValue;
        }

        try
        {
            object value = ExtraProperties[key];
            if (value is T variable)
            {
                return variable;
            }

            return (T?)Convert.ChangeType(value, typeof(T));
        }
        catch (Exception ex)
        {
            Utils.LogWarning($"获取额外属性 '{key}' 失败: {ex.Message}", "GameTemplate");
            return defaultValue;
        }
    }
}