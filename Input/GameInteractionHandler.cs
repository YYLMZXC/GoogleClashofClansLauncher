using System;
using System.Drawing;
using System.Windows.Forms;
using GoogleClashofClansLauncher.Core;
using GoogleClashofClansLauncher.Core.System;
using GoogleClashofClansLauncher.Core.UI;
using GoogleClashofClansLauncher.Game;

namespace GoogleClashofClansLauncher.Input;

/// <summary>
/// 游戏交互处理器类
/// 专门处理与游戏界面的交互逻辑
/// </summary>
public class GameInteractionHandler
{
    private readonly MouseController _mouseController;
    private readonly ImageRecognition _imageRecognition;
    private readonly WindowFinder _windowFinder;
    private readonly WindowManager _windowManager;
    private readonly ResourceManager _resourceManager;
    private readonly GameWindow _gameWindow;

    private IntPtr _gameWindowHandle = IntPtr.Zero;
    private bool _isInteractionActive = false;

    // Constants for game window identification
    private const string ProcessName = "crosvm";
    private const string WindowTitleKeyword = "部落冲突";

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="mouseController">鼠标控制器实例</param>
    /// <param name="imageRecognition">图像识别实例</param>
    /// <param name="windowFinder">窗口查找器实例</param>
    /// <param name="windowManager">窗口管理器实例</param>
    /// <param name="resourceManager">资源管理器实例</param>
    /// <param name="gameWindow">游戏窗口实例</param>
    public GameInteractionHandler(
        MouseController mouseController,
        ImageRecognition imageRecognition,
        WindowFinder windowFinder,
        WindowManager windowManager,
        ResourceManager resourceManager,
        GameWindow gameWindow
    )
    {
        _mouseController = mouseController;
        _imageRecognition = imageRecognition;
        _windowFinder = windowFinder;
        _windowManager = windowManager;
        _resourceManager = resourceManager;
        _gameWindow = gameWindow;
    }

    /// <summary>
    /// 初始化游戏交互
    /// </summary>
    /// <returns>初始化是否成功</returns>
    public bool Initialize()
    {
        try
        {
            // 加载必要的游戏模板图像
            LoadGameTemplates();
            Utils.LogDebug("游戏交互处理器初始化成功", "GameInteractionHandler");
            return true;
        }
        catch (Exception ex)
        {
            Utils.LogError("游戏交互处理器初始化失败", ex, "GameInteractionHandler");
            return false;
        }
    }

    /// <summary>
    /// 加载游戏模板图像
    /// </summary>
    private void LoadGameTemplates()
    {
        try
        {
            // 加载游戏中常用的模板图像
            // 这里应该根据实际需要添加具体的模板加载代码
            Utils.LogDebug("游戏模板图像加载完成", "GameInteractionHandler");
        }
        catch (Exception ex)
        {
            Utils.LogError("游戏模板图像加载失败", ex, "GameInteractionHandler");
        }
    }

    /// <summary>
    /// 查找并激活游戏窗口
    /// </summary>
    /// <returns>是否成功</returns>
    public bool FindAndActivateGameWindow()
    {
        try
        {
            _gameWindowHandle = _windowFinder.FindGameWindow(ProcessName, WindowTitleKeyword);
            if (_gameWindowHandle == IntPtr.Zero)
            {
                Utils.LogError("未找到游戏窗口", null, "GameInteractionHandler");
                return false;
            }

            // 激活游戏窗口
            if (!_windowManager.ActivateWindow(_gameWindowHandle))
            {
                Utils.LogError("激活游戏窗口失败", null, "GameInteractionHandler");
                return false;
            }

            Utils.LogDebug("游戏窗口已成功找到并激活", "GameInteractionHandler");
            return true;
        }
        catch (Exception ex)
        {
            Utils.LogError("查找和激活游戏窗口过程中出错", ex, "GameInteractionHandler");
            return false;
        }
    }

    /// <summary>
    /// 开始游戏交互
    /// </summary>
    public void StartInteraction()
    {
        if (_isInteractionActive)
        {
            Utils.LogDebug("游戏交互已经处于活动状态", "GameInteractionHandler");
            return;
        }

        try
        {
            // 确保游戏窗口已经找到并激活
            if (_gameWindowHandle == IntPtr.Zero)
            {
                if (!FindAndActivateGameWindow())
                {
                    Utils.LogError("无法启动游戏交互：未找到游戏窗口", null, "GameInteractionHandler");
                    return;
                }
            }

            _isInteractionActive = true;
            Utils.LogDebug("游戏交互已启动", "GameInteractionHandler");
        }
        catch (Exception ex)
        {
            Utils.LogError("启动游戏交互过程中出错", ex, "GameInteractionHandler");
            _isInteractionActive = false;
        }
    }

    /// <summary>
    /// 停止游戏交互
    /// </summary>
    public void StopInteraction()
    {
        if (!_isInteractionActive)
        {
            Utils.LogDebug("游戏交互已经处于停止状态", "GameInteractionHandler");
            return;
        }

        try
        {
            _isInteractionActive = false;
            Utils.LogDebug("游戏交互已停止", "GameInteractionHandler");
        }
        catch (Exception ex)
        {
            Utils.LogError("停止游戏交互过程中出错", ex, "GameInteractionHandler");
        }
    }

    /// <summary>
    /// 在游戏中心执行点击操作
    /// </summary>
    /// <param name="times">点击次数</param>
    /// <returns>操作是否成功</returns>
    public bool ClickGameCenter(int times = 1)
    {
        if (!_isInteractionActive || _gameWindowHandle == IntPtr.Zero)
        {
            Utils.LogError("游戏交互未激活或未找到游戏窗口", null, "GameInteractionHandler");
            return false;
        }

        try
        {
            Utils.LogDebug($"在游戏中心执行{times}次点击操作", "GameInteractionHandler");
            
            // 获取游戏窗口位置
            Rectangle windowRect = _gameWindow.WindowRectangle;
            if (windowRect.IsEmpty)
            {
                Utils.LogError("获取游戏窗口位置失败", null, "GameInteractionHandler");
                return false;
            }

            // 计算游戏中心位置
            int centerX = windowRect.Left + windowRect.Width / 2;
            int centerY = windowRect.Top + windowRect.Height / 2;

            // 执行点击操作
            for (int i = 0; i < times; i++)
            {
                if (!_mouseController.LeftClickAt(centerX, centerY))
                {
                    Utils.LogError($"第{i + 1}次点击操作失败", null, "GameInteractionHandler");
                    return false;
                }

                // 两次点击之间的延迟
                if (i < times - 1)
                {
                    Utils.Wait(Constants.DefaultGameInteractionDelay);
                }
            }

            Utils.LogDebug("游戏中心点击操作完成", "GameInteractionHandler");
            return true;
        }
        catch (Exception ex)
        {
            Utils.LogError("执行游戏中心点击操作过程中出错", ex, "GameInteractionHandler");
            return false;
        }
    }

    /// <summary>
    /// 使用图像识别查找并点击指定按钮
    /// </summary>
    /// <param name="templateName">模板图像名称</param>
    /// <param name="threshold">匹配阈值</param>
    /// <returns>操作是否成功</returns>
    public bool FindAndClickButton(string templateName, double threshold = 0.8)
    {
        if (!_isInteractionActive || _gameWindowHandle == IntPtr.Zero)
        {
            Utils.LogError("游戏交互未激活或未找到游戏窗口", null, "GameInteractionHandler");
            return false;
        }

        try
        {
            Utils.LogDebug($"尝试查找并点击模板为 '{templateName}' 的按钮", "GameInteractionHandler");

            // 获取游戏窗口截图
            Bitmap? screenshot = _imageRecognition.CaptureWindow(_gameWindowHandle);
            if (screenshot == null)
            {
                Utils.LogError("获取游戏窗口截图失败", null, "GameInteractionHandler");
                return false;
            }

            // 加载模板图像
            Image? template = _resourceManager.LoadImage(templateName, Constants.ImageTemplateDirectory);
            if (template == null)
            {
                Utils.LogError($"加载模板图像 '{templateName}' 失败", null, "GameInteractionHandler");
                screenshot.Dispose();
                return false;
            }

            // 执行图像匹配
            Point matchPoint;
            bool found = _imageRecognition.FindImage(screenshot, template, out matchPoint, threshold);
            
            // 释放资源
            screenshot.Dispose();
            template.Dispose();

            if (!found)
            {
                Utils.LogDebug($"未找到与模板 '{templateName}' 匹配的图像", "GameInteractionHandler");
                return false;
            }

            // 获取游戏窗口位置以计算全局坐标
            Rectangle windowRect = _gameWindow.WindowRectangle;
            if (windowRect.IsEmpty)
            {
                Utils.LogError("获取游戏窗口位置失败", null, "GameInteractionHandler");
                return false;
            }

            // 计算全局点击坐标
            int clickX = windowRect.Left + matchPoint.X;
            int clickY = windowRect.Top + matchPoint.Y;

            // 执行点击操作
            if (!_mouseController.LeftClickAt(clickX, clickY))
            {
                Utils.LogError("点击按钮操作失败", null, "GameInteractionHandler");
                return false;
            }

            Utils.LogDebug($"成功找到并点击模板为 '{templateName}' 的按钮", "GameInteractionHandler");
            return true;
        }
        catch (Exception ex)
        {
            Utils.LogError("查找并点击按钮过程中出错", ex, "GameInteractionHandler");
            return false;
        }
    }

    /// <summary>
    /// 检查游戏交互是否激活
    /// </summary>
    /// <returns>是否激活</returns>
    public bool IsInteractionActive()
    {
        return _isInteractionActive;
    }

    /// <summary>
    /// 获取当前游戏窗口句柄
    /// </summary>
    /// <returns>窗口句柄</returns>
    public IntPtr GetGameWindowHandle()
    {
        return _gameWindowHandle;
    }
}