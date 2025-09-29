using GoogleClashofClansLauncher.Input;
using GoogleClashofClansLauncher.Core;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace GoogleClashofClansLauncher
{
    public partial class Form1 : Form
    {
        private KeyboardSimulator keyboardSimulator;
        private WindowManager windowManager;

        // 进程名称和窗口标题关键字
        private const string ProcessName = "crosvm";
        private const string WindowTitleKeyword = "部落冲突";

        public Form1()
        {
            InitializeComponent();
            keyboardSimulator = new KeyboardSimulator();
            windowManager = new WindowManager();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // 检查是否已有crosvm.exe进程在运行
            if (IsProcessRunning(ProcessName))
            {
                // 不再显示消息框，避免干扰用户操作
                Debug.WriteLine("检测到Google Play Games模拟器已经在运行中。");
            }
        }

        private void simulateButton_Click(object sender, EventArgs e)
        {
            // 获取用户输入的文本
            string textToSimulate = inputTextBox.Text;
            
            if (!string.IsNullOrEmpty(textToSimulate))
            {
                // 尝试激活游戏窗口
                if (!ActivateGameWindow())
                {
                    // 简化提示信息
                    Debug.WriteLine("未能找到或激活部落冲突游戏窗口");
                }
                
                // 降低延迟时间，从1秒改为200毫秒
                Thread.Sleep(200);
                keyboardSimulator.TypeText(textToSimulate);
            }
        }

        private void fixed123Button_Click(object sender, EventArgs e)
        {
            // 尝试激活游戏窗口
            if (!ActivateGameWindow())
            {
                // 简化提示信息
                Debug.WriteLine("未能找到或激活部落冲突游戏窗口");
            }
            
            // 降低延迟时间，从1秒改为200毫秒
            Thread.Sleep(200);
            keyboardSimulator.TypeText("123");
        }

        private bool ActivateGameWindow()
        {
            // 尝试通过进程名和窗口标题关键字查找并激活窗口
            Process[] processes = Process.GetProcessesByName(ProcessName);
            
            foreach (Process process in processes)
            {
                try
                {
                    // 尝试获取窗口标题
                    if (!string.IsNullOrEmpty(process.MainWindowTitle) && 
                        process.MainWindowTitle.Contains(WindowTitleKeyword))
                    {
                        windowManager.ActivateWindow(process.MainWindowHandle);
                        return true;
                    }
                }
                catch { /* 忽略访问被拒绝的异常 */ }
            }
            
            // 如果通过进程名没找到，尝试通过窗口标题关键字枚举所有窗口
            IntPtr windowHandle = FindWindowByKeyword(WindowTitleKeyword);
            if (windowHandle != IntPtr.Zero)
            {
                windowManager.ActivateWindow(windowHandle);
                return true;
            }
            
            return false;
        }

        private bool IsProcessRunning(string processName)
        {
            return Process.GetProcessesByName(processName).Length > 0;
        }

        // 通过关键字模糊查找窗口
        private IntPtr FindWindowByKeyword(string keyword)
        {
            IntPtr foundWindow = IntPtr.Zero;
            EnumWindows((hWnd, lParam) =>
            {
                StringBuilder title = new StringBuilder(256);
                GetWindowText(hWnd, title, title.Capacity);
                
                if (title.ToString().Contains(keyword))
                {
                    foundWindow = hWnd;
                    return false; // 停止枚举
                }
                return true; // 继续枚举
            }, IntPtr.Zero);
            
            return foundWindow;
        }

        // Win32 API声明
        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        private static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

        [DllImport("user32.dll")]
        private static extern bool EnumWindows(EnumWindowsProc lpEnumFunc, IntPtr lParam);

        private delegate bool EnumWindowsProc(IntPtr hWnd, IntPtr lParam);
    }
}
