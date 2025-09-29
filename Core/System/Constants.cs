namespace GoogleClashofClansLauncher.Core.System;

/// <summary>
/// 项目常量定义类
/// 集中管理项目中使用的所有常量
/// </summary>
public static class Constants
{
    // 进程和窗口相关常量
    public const string ProcessName = "crosvm";
    public const string WindowTitleKeyword = "部落冲突";
    public const string ApplicationTitle = "键盘与鼠标模拟器";
    public const string MainFormTitle = "键盘输入模拟器工具";
    
    // 配置相关常量
    public const string ConfigDirectory = "Config";
    public const string AppSettingsFileName = "appsettings.json";
    
    // 资源相关常量
    public const string ResourceDirectory = "Res";
    public const string IconDirectory = "2";
    public const string IconFileName = "002.ico";
    
    // 图像识别相关常量
    public const string ImageRecognitionDirectory = "1";
    public const string ImageTemplateDirectory = "1";
    public const string Template001 = "001.png";
    public const string Template003 = "003.png";
    public const string Template004 = "004.png";
    
    // 延迟相关常量
    public const int DefaultClickDelay = 1000 / 3; // 默认点击间隔（1秒3次）
    public const int DefaultKeyDelay = 10; // 默认按键延迟
    public const int DefaultTextInputDelay = 50; // 默认文本输入延迟
    public const int DefaultInitialDelay = 200; // 默认初始延迟
    public const int DefaultWindowWaitTimeout = 5000; // 默认窗口等待超时时间
    public const int DefaultDragDelay = 200; // 默认拖动延迟
    public const int DefaultGameInteractionDelay = 250; // 默认游戏交互延迟
    public const int DefaultDoubleClickDelay = 50; // 默认双击延迟

    // 测试相关常量
    public const int ClickTestDuration = 10; // 点击测试持续时间（秒）
    public const int ClicksPerSecond = 3; // 每秒点击次数
}