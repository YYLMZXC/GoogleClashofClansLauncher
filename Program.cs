using GoogleClashofClansLauncher.Core;
using GoogleClashofClansLauncher.Input;

namespace GoogleClashofClansLauncher;

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("Google Clash of Clans Launcher - .NET 8.0");
        Console.WriteLine("----------------------------------------");

        // 配置进程路径和窗口标题
        const string processPath = @"C:\Program Files\Google\Play Games\current\emulator\crosvm.exe"; 
        const string windowTitle = "部落冲突(Clash of Clans)"; // 窗口标题

        // 初始化管理器
        var processManager = new ProcessManager();
        var windowManager = new WindowManager();
        var mouse = new MouseSimulator();
        var keyboard = new KeyboardSimulator();

        // 启动进程
        var cocProcess = processManager.StartProcess(processPath);
        if (cocProcess == null)
        {
            Console.WriteLine("启动游戏失败！");
            return;
        }

        try
        {
            // 等待窗口加载
            Console.WriteLine("等待游戏窗口启动...");
            System.Threading.Thread.Sleep(5000);

            // 查找并激活窗口
            var windowHandle = windowManager.FindWindowByTitle(windowTitle);
            if (windowHandle == IntPtr.Zero)
            {
                Console.WriteLine("未找到游戏窗口！");
                return;
            }

            windowManager.ActivateWindow(windowHandle);
            System.Threading.Thread.Sleep(1000);

            // 模拟输入示例
            Console.WriteLine("开始模拟操作...");

            // 移动鼠标到(500, 300)并点击
            mouse.Move(500, 300);
            System.Threading.Thread.Sleep(1000);
            mouse.LeftClick();

            // 输入文本并按回车
            System.Threading.Thread.Sleep(1000);
            keyboard.TypeText("Hello Clash!");
            System.Threading.Thread.Sleep(500);
            keyboard.PressKey(VirtualKeyCode.RETURN);

            Console.WriteLine("操作完成！");
        }
        finally
        {
            // 等待用户确认后关闭进程
            Console.WriteLine("按任意键关闭游戏...");
            Console.ReadKey();
            processManager.CloseProcess(cocProcess);
        }
    }
}