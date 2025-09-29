using GoogleClashofClansLauncher.Input;
using GoogleClashofClansLauncher.Core;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Drawing;

namespace GoogleClashofClansLauncher
{
    public partial class Form1 : Form
    {
        private KeyboardSimulator keyboardSimulator;
        private WindowManager windowManager;
        private MouseSimulator mouseSimulator;
        private CancellationTokenSource? cancellationTokenSource;
        private ImageRecognition imageRecognition;

        // 进程名称和窗口标题关键字
        private const string ProcessName = "crosvm";
        private const string WindowTitleKeyword = "部落冲突";

        public Form1()
        {
            InitializeComponent();
            keyboardSimulator = new KeyboardSimulator();
            windowManager = new WindowManager();
            mouseSimulator = new MouseSimulator();
            imageRecognition = new ImageRecognition();
            cancellationTokenSource = null;
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

        /// <summary>
        /// 鼠标点击模拟按钮点击事件
        /// 实现1秒点击3次，持续10秒的功能
        /// 使用后台线程执行以避免UI冻结
        /// </summary>
        private void mouseClickButton_Click(object sender, EventArgs e)
        {
            // 如果已经有模拟在运行，先取消
            if (cancellationTokenSource != null && !cancellationTokenSource.IsCancellationRequested)
            {
                cancellationTokenSource.Cancel();
                mouseClickButton.Text = "开始鼠标点击模拟 (1秒3次，持续10秒)";
                Debug.WriteLine("鼠标点击模拟已取消");
                return;
            }

            // 尝试激活游戏窗口
            if (!ActivateGameWindow())
            {
                Debug.WriteLine("未能找到或激活部落冲突游戏窗口");
                return;
            }

            // 更新按钮文本
            mouseClickButton.Text = "停止鼠标点击模拟";
            Debug.WriteLine("开始鼠标点击模拟");

            // 创建新的取消令牌
            cancellationTokenSource = new CancellationTokenSource();
            CancellationToken token = cancellationTokenSource.Token;

            // 在后台线程中执行鼠标点击模拟
            Task.Run(() =>
            {
                try
                {
                    // 初始延迟
                    Thread.Sleep(200);

                    // 记录开始时间
                    DateTime startTime = DateTime.Now;
                    // 持续10秒或直到取消
                    while ((DateTime.Now - startTime).TotalSeconds < 10 && !token.IsCancellationRequested)
                    {
                        // 1秒内点击3次（间隔约333毫秒）
                        for (int i = 0; i < 3; i++)
                        {
                            if (token.IsCancellationRequested) break;
                            mouseSimulator.LeftClick();
                            // 约333毫秒的间隔
                            Thread.Sleep(333);
                        }
                    }

                    // 确保UI线程中更新按钮文本
                    this.Invoke((MethodInvoker)delegate
                    {
                        mouseClickButton.Text = "鼠标点击模拟 (1秒3次，持续10秒)";
                    });

                    if (!token.IsCancellationRequested)
                    {
                        Debug.WriteLine("鼠标点击模拟完成");
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("鼠标点击模拟发生错误: " + ex.Message);
                }
            }, token);
        }

        /// <summary>
        /// 图像识别并点击按钮点击事件
        /// 识别res/1/001.png图像并点击对应位置
        /// </summary>
        private void imageRecognitionButton_Click(object sender, EventArgs e)
        {
            try
            {
                // 尝试激活游戏窗口
                if (!ActivateGameWindow())
                {
                    Debug.WriteLine("未能找到或激活部落冲突游戏窗口");
                    return;
                }

                // 显示正在识别的状态
                imageRecognitionButton.Text = "正在识别图像...";
                imageRecognitionButton.Enabled = false;
                Debug.WriteLine("开始识别图像");

                // 使用Task在后台线程中执行图像识别，避免UI冻结
                Task.Run(() =>
                {
                    try
                    {
                        // 等待窗口激活
                        Thread.Sleep(200);

                        // 调用图像识别并点击功能
                        bool success = imageRecognition.RecognizeAndClickResImage("001", "1");

                        // 更新UI状态
                        this.Invoke((MethodInvoker)delegate
                        {
                            if (success)
                            {
                                imageRecognitionButton.Text = "识别并点击成功! (res/1/001.png)";
                                Debug.WriteLine("图像识别并点击成功");
                            }
                            else
                            {
                                imageRecognitionButton.Text = "未找到图像，请重试 (res/1/001.png)";
                                Debug.WriteLine("未找到图像或点击失败");
                            }

                            // 恢复按钮状态
                            Thread.Sleep(1000);
                            imageRecognitionButton.Text = "识别图像并点击 (res/1/001.png)";
                            imageRecognitionButton.Enabled = true;
                        });
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine("图像识别过程中发生错误: " + ex.Message);
                        // 发生异常时恢复按钮状态
                        this.Invoke((MethodInvoker)delegate
                        {
                            imageRecognitionButton.Text = "识别图像并点击 (res/1/001.png)";
                            imageRecognitionButton.Enabled = true;
                        });
                    }
                });
            }
            catch (Exception ex)
            {
                Debug.WriteLine("按钮点击处理异常: " + ex.Message);
                imageRecognitionButton.Text = "识别图像并点击 (res/1/001.png)";
                imageRecognitionButton.Enabled = true;
            }
        }

        // Win32 API声明
        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        private static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

        [DllImport("user32.dll")]
        private static extern bool EnumWindows(EnumWindowsProc lpEnumFunc, IntPtr lParam);

        private delegate bool EnumWindowsProc(IntPtr hWnd, IntPtr lParam);
    }
}
