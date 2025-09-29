using System;
using System.IO;
using System.Drawing;
using System.Diagnostics;
using System.Windows.Forms;

namespace GoogleClashofClansLauncher.Core.System;

/// <summary>
/// 资源管理器类
/// 负责加载和管理应用程序的资源文件
/// </summary>
public class ResourceManager
{
    private readonly string _applicationDirectory;
    private readonly string _resourceDirectory;

    /// <summary>
    /// 构造函数
    /// </summary>
    public ResourceManager()
    {
        _applicationDirectory = Path.GetDirectoryName(Application.ExecutablePath) ?? AppDomain.CurrentDomain.BaseDirectory;
        _resourceDirectory = Path.Combine(_applicationDirectory, Constants.ResourceDirectory);
    }

    /// <summary>
    /// 查找项目根目录
    /// </summary>
    /// <param name="maxDepth">最大查找深度</param>
    /// <returns>项目根目录路径</returns>
    public string FindProjectRoot(int maxDepth = 3)
    {
        string currentDir = _applicationDirectory;
        
        for (int i = 0; i < maxDepth; i++)
        {
            if (Directory.Exists(Path.Combine(currentDir, Constants.ResourceDirectory)))
            {
                return currentDir;
            }

            string? parentDir = Path.GetDirectoryName(currentDir);
            if (string.IsNullOrEmpty(parentDir))
            {
                break;
            }

            currentDir = parentDir;
        }

        return _applicationDirectory; // 如果找不到，返回应用程序目录
    }

    /// <summary>
    /// 获取资源文件的完整路径
    /// </summary>
    /// <param name="relativePath">资源文件的相对路径</param>
    /// <returns>资源文件的完整路径</returns>
    public string GetResourcePath(params string[] relativePath)
    {
        string projectDir = FindProjectRoot();
        return Path.Combine(new string[] { projectDir, Constants.ResourceDirectory }.Concat(relativePath).ToArray());
    }

    /// <summary>
    /// 加载图标资源
    /// </summary>
    /// <param name="iconName">图标文件名</param>
    /// <param name="subDirectory">子目录</param>
    /// <returns>加载的图标，如果失败返回null</returns>
    public Icon? LoadIcon(string iconName, string? subDirectory = null)
    {
        try
        {
            string iconPath;
            if (!string.IsNullOrEmpty(subDirectory))
            {
                iconPath = GetResourcePath(subDirectory, iconName);
            }
            else
            {
                iconPath = GetResourcePath(iconName);
            }

            if (File.Exists(iconPath))
            {
                using (Icon icon = new Icon(iconPath))
                {
                    return (Icon)icon.Clone(); // 克隆以避免文件锁定
                }
            }
            
            // 尝试备用路径
            string altIconPath = Path.Combine(Application.StartupPath, Constants.ResourceDirectory);
            if (!string.IsNullOrEmpty(subDirectory))
            {
                altIconPath = Path.Combine(altIconPath, subDirectory);
            }
            altIconPath = Path.Combine(altIconPath, iconName);
            
            if (File.Exists(altIconPath))
            {
                using (Icon icon = new Icon(altIconPath))
                {
                    return (Icon)icon.Clone();
                }
            }
            
            Debug.WriteLine("图标文件不存在: " + iconPath);
            return null;
        }
        catch (Exception ex)
        {
            Debug.WriteLine("加载图标时发生错误: " + ex.Message);
            return null;
        }
    }

    /// <summary>
    /// 加载图像资源
    /// </summary>
    /// <param name="imageName">图像文件名</param>
    /// <param name="subDirectory">子目录</param>
    /// <returns>加载的图像，如果失败返回null</returns>
    public Image? LoadImage(string imageName, string? subDirectory = null)
    {
        try
        {
            string imagePath;
            if (!string.IsNullOrEmpty(subDirectory))
            {
                imagePath = GetResourcePath(subDirectory, imageName);
            }
            else
            {
                imagePath = GetResourcePath(imageName);
            }

            if (File.Exists(imagePath))
            {
                using (Image image = Image.FromFile(imagePath))
                {
                    return (Image)image.Clone(); // 克隆以避免文件锁定
                }
            }
            
            Debug.WriteLine("图像文件不存在: " + imagePath);
            return null;
        }
        catch (Exception ex)
        {
            Debug.WriteLine("加载图像时发生错误: " + ex.Message);
            return null;
        }
    }

    /// <summary>
    /// 检查资源文件是否存在
    /// </summary>
    /// <param name="resourceName">资源文件名</param>
    /// <param name="subDirectory">子目录</param>
    /// <returns>资源文件是否存在</returns>
    public bool ResourceExists(string resourceName, string? subDirectory = null)
    {
        string resourcePath;
        if (!string.IsNullOrEmpty(subDirectory))
        {
            resourcePath = GetResourcePath(subDirectory, resourceName);
        }
        else
        {
            resourcePath = GetResourcePath(resourceName);
        }

        return File.Exists(resourcePath);
    }
}