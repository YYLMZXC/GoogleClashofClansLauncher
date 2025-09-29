using GoogleClashofClansLauncher.Input;
using GoogleClashofClansLauncher.Core;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Drawing;
using System.IO;
using System.Drawing.Imaging;

namespace GoogleClashofClansLauncher
{
    public partial class Form1 : Form
    {
        private KeyboardSimulator keyboardSimulator;
        private WindowManager windowManager;
        private MouseSimulator mouseSimulator;
        private CancellationTokenSource? cancellationTokenSource;
        // 【注意】：图像识别功能暂时禁用
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
            // 【注意】：图像识别功能暂时禁用，但仍初始化实例以避免空引用异常
            imageRecognition = new ImageRecognition();
            cancellationTokenSource = null;
        }

        /// <summary>
        /// 识别003图像并点击3次按钮点击事件
        /// 识别res/1/003.png图像并点击对应位置3次
        /// </summary>
        private void recognize003Button_Click(object sender, EventArgs e)
        {
            try
            {
                // 尝试激活游戏窗口
                IntPtr windowHandle = ActivateGameWindow();
                if (windowHandle == IntPtr.Zero)
                {
                    Debug.WriteLine("未能找到或激活部落冲突游戏窗口");
                    return;
                }

                // 显示正在识别的状态
                recognize003Button.Text = "正在识别003图像...";
                recognize003Button.Enabled = false;
                Debug.WriteLine("开始识别003图像");

                // 使用Task在后台线程中执行图像识别，避免UI冻结
                Task.Run(() =>
                {
                    try
                    {
                        // 等待窗口激活
                        Thread.Sleep(200);

                        // 激活图像识别功能
                        // 注意：实际项目中应该考虑是否需要修改FEATURE_ENABLED常量
                        // 这里我们直接使用现有的实现，但会额外点击一次以满足3次点击的要求

                        // 调用图像识别并点击功能，如果识别失败则使用固定位置作为备选方案
                        bool success = imageRecognition.RecognizeAndClickWithFallback("003", "1", 100, 100);

                        if (success)
                        {
                            // 额外点击一次，以完成3次点击的要求
                            Thread.Sleep(100);
                            mouseSimulator.LeftClick();
                            Debug.WriteLine("已完成第3次点击");
                        }

                        // 更新UI状态
                        this.Invoke((MethodInvoker)delegate
                        {
                            if (success)
                            {
                                recognize003Button.Text = "识别并点击003成功! (已点击3次)";
                                Debug.WriteLine("003图像识别并点击3次成功");
                            }
                            else
                            {
                                recognize003Button.Text = "未找到003图像，请重试";
                                Debug.WriteLine("未找到003图像或点击失败");
                            }

                            // 恢复按钮状态
                            Thread.Sleep(1000);
                            recognize003Button.Text = "识别003图像并点击3次";
                            recognize003Button.Enabled = true;
                        });
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine("003图像识别过程中发生错误: " + ex.Message);
                        // 发生异常时恢复按钮状态
                        this.Invoke((MethodInvoker)delegate
                        {
                            recognize003Button.Text = "识别003图像并点击3次";
                            recognize003Button.Enabled = true;
                        });
                    }
                });
            }
            catch (Exception ex)
            {
                Debug.WriteLine("003按钮点击处理异常: " + ex.Message);
                recognize003Button.Text = "识别003图像并点击3次";
                recognize003Button.Enabled = true;
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // 检查是否已有crosvm.exe进程在运行
            if (IsProcessRunning(ProcessName))
            {
                // 不再显示消息框，避免干扰用户操作
                Debug.WriteLine("检测到Google Play Games模拟器已经在运行中。");
            }
            
            // 加载设置按钮图标
            try
            {
                // 使用相对路径加载Res/2文件夹下的002.ico图标
                string appDir = Path.GetDirectoryName(Application.ExecutablePath);
                string projectDir = appDir;
                
                // 尝试向上查找项目根目录，直到找到Res文件夹
                for (int i = 0; i < 3; i++)
                {
                    string possibleResDir = Path.Combine(projectDir, "Res");
                    if (Directory.Exists(possibleResDir))
                    {
                        break;
                    }
                    projectDir = Path.GetDirectoryName(projectDir);
                    if (string.IsNullOrEmpty(projectDir)) break;
                }
                
                string iconPath = Path.Combine(projectDir ?? appDir, "Res", "2", "002.ico");
                
                if (File.Exists(iconPath))
                {
                    using (Icon icon = new Icon(iconPath))
                    {
                        settingsButton.Image = icon.ToBitmap();
                        settingsButton.Text = "";
                        settingsButton.ImageAlign = ContentAlignment.MiddleCenter;
                        Debug.WriteLine("成功加载图标: " + iconPath);
                    }
                }
                else
                {
                    Debug.WriteLine("图标文件不存在: " + iconPath);
                    
                    // 尝试备用路径
                    string altIconPath = Path.Combine(Application.StartupPath, "Res", "2", "002.ico");
                    if (File.Exists(altIconPath))
                    {
                        using (Icon icon = new Icon(altIconPath))
                        {
                            settingsButton.Image = icon.ToBitmap();
                            settingsButton.Text = "";
                            settingsButton.ImageAlign = ContentAlignment.MiddleCenter;
                            Debug.WriteLine("使用备用路径成功加载图标: " + altIconPath);
                        }
                    }
                    else
                    {
                        Debug.WriteLine("备用图标路径也不存在: " + altIconPath);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("加载图标时发生错误: " + ex.Message);
                Debug.WriteLine("异常详细信息: " + ex.StackTrace);
            }
        }

        private void simulateButton_Click(object sender, EventArgs e)
        {
            // 获取用户输入的文本
            string textToSimulate = inputTextBox.Text;
            
            if (!string.IsNullOrEmpty(textToSimulate))
            {
                // 仅获取窗口句柄而不激活窗口，避免强制游戏窗口化
                IntPtr windowHandle = GetGameWindowHandle();
                if (windowHandle == IntPtr.Zero)
                {
                    // 简化提示信息
                    Debug.WriteLine("未能找到部落冲突游戏窗口");
                    return;
                }
                
                // 确保窗口可见但不激活
                windowManager.RestoreWindowOnly(windowHandle);
                
                // 检查鼠标是否在目标窗口内（可选，根据需要启用）
                // 注意：如果需要在后台发送输入，可能需要移除这个检查
                if (!mouseSimulator.IsMouseInTargetWindow())
                {
                    Debug.WriteLine("注意：鼠标不在游戏窗口内，但仍尝试发送输入");
                    // 如果不需要鼠标位置检查，可以注释掉下面的return语句
                    // return;
                }
                
                // 降低延迟时间，从1秒改为200毫秒
                Thread.Sleep(200);
                keyboardSimulator.TypeText(textToSimulate);
            }
        }

        private bool WaitForMouseInTargetWindow(int timeoutMs)
        {
            DateTime startTime = DateTime.Now;
            while ((DateTime.Now - startTime).TotalMilliseconds < timeoutMs)
            {
                if (mouseSimulator.IsMouseInTargetWindow())
                {
                    return true;
                }
                Thread.Sleep(100); // 每100毫秒检查一次
            }
            return false; // 超时返回false
        }

        private void fixed123Button_Click(object sender, EventArgs e)
        {
            // 仅获取窗口句柄而不激活窗口，避免强制游戏窗口化
            IntPtr windowHandle = GetGameWindowHandle();
            if (windowHandle == IntPtr.Zero)
            {
                // 简化提示信息
                Debug.WriteLine("未能找到部落冲突游戏窗口");
                return;
            }
            
            // 确保窗口可见但不激活
            windowManager.RestoreWindowOnly(windowHandle);
            
            // 检查鼠标是否在目标窗口内（可选）
            if (!mouseSimulator.IsMouseInTargetWindow())
            {
                Debug.WriteLine("注意：鼠标不在游戏窗口内，但仍尝试发送输入");
                // 如果不需要鼠标位置检查，可以注释掉下面的return语句
                // return;
            }
            
            // 降低延迟时间，从1秒改为200毫秒
            Thread.Sleep(200);
            keyboardSimulator.TypeText("123");
        }

        /// <summary>
        /// 仅获取游戏窗口句柄而不激活窗口
        /// </summary>
        /// <returns>窗口句柄，未找到返回IntPtr.Zero</returns>
        private IntPtr GetGameWindowHandle()
        {
            Debug.WriteLine("开始查找部落冲突游戏窗口...");
            
            // 策略1: 尝试通过进程名查找
            Debug.WriteLine("策略1: 尝试通过进程名 '" + ProcessName + "' 查找");
            Process[] processes = Process.GetProcessesByName(ProcessName);
            
            foreach (Process process in processes)
            {
                try
                {
                    string windowTitle = process.MainWindowTitle;
                    Debug.WriteLine("找到进程: '" + process.ProcessName + "', ID: " + process.Id + ", 窗口标题: '" + windowTitle + "'");
                    
                    // 尝试获取窗口标题，使用不区分大小写的比较
                    if (!string.IsNullOrEmpty(windowTitle) && 
                        windowTitle.IndexOf(WindowTitleKeyword, StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        Debug.WriteLine("找到匹配的窗口");
                        // 锁定键盘和鼠标模拟器到找到的窗口，但不激活
                        keyboardSimulator.SetTargetWindow(process.MainWindowHandle);
                        mouseSimulator.SetTargetWindow(process.MainWindowHandle);
                        return process.MainWindowHandle;
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("访问进程时出错: " + ex.Message);
                }
            }
            
            // 策略2: 通过窗口标题关键字枚举所有窗口
            Debug.WriteLine("策略2: 尝试通过窗口标题关键字 '" + WindowTitleKeyword + "' 枚举所有窗口");
            IntPtr windowHandle = FindWindowByKeyword(WindowTitleKeyword);
            if (windowHandle != IntPtr.Zero)
            {
                Debug.WriteLine("通过关键字找到窗口");
                // 锁定键盘和鼠标模拟器到找到的窗口，但不激活
                keyboardSimulator.SetTargetWindow(windowHandle);
                mouseSimulator.SetTargetWindow(windowHandle);
                return windowHandle;
            }
            
            // 策略3: 尝试查找可能的子窗口或其他相关窗口
            Debug.WriteLine("策略3: 尝试查找可能的子窗口或其他相关窗口");
            string[] alternativeKeywords = new string[]
            {
                "Clash of Clans", // 英文标题
                "部落冲突 COC", // 可能的变体
                "COC", // 缩写
                "Google Play Games"
            };
            
            foreach (string altKeyword in alternativeKeywords)
            {
                IntPtr altWindowHandle = FindWindowByKeyword(altKeyword);
                if (altWindowHandle != IntPtr.Zero)
                {
                    Debug.WriteLine("通过备用关键字 '" + altKeyword + "' 找到窗口");
                    keyboardSimulator.SetTargetWindow(altWindowHandle);
                    mouseSimulator.SetTargetWindow(altWindowHandle);
                    return altWindowHandle;
                }
            }
            
            Debug.WriteLine("未找到部落冲突游戏窗口");
            return IntPtr.Zero;
        }
        
        /// <summary>
        /// 激活游戏窗口（将其置于前台）
        /// </summary>
        /// <returns>窗口句柄，未找到返回IntPtr.Zero</returns>
        private IntPtr ActivateGameWindow()
        {
            IntPtr windowHandle = GetGameWindowHandle();
            
            if (windowHandle != IntPtr.Zero)
            {
                // 只在需要时激活窗口（例如点击操作）
                Debug.WriteLine("激活游戏窗口");
                windowManager.ActivateWindow(windowHandle);
            }
            
            return windowHandle;
        }

        private bool IsProcessRunning(string processName)
        {
            return Process.GetProcessesByName(processName).Length > 0;
        }

        // 通过关键字模糊查找窗口
        private IntPtr FindWindowByKeyword(string keyword)
        {
            // 先尝试完整匹配，针对部落冲突(Clash of Clans)xxx这种格式
            IntPtr fullMatchWindow = FindWindowByFullKeyword(keyword);
            if (fullMatchWindow != IntPtr.Zero)
            {
                Debug.WriteLine("找到完整匹配的窗口");
                return fullMatchWindow;
            }

            // 如果完整匹配失败，尝试使用模糊匹配
            Debug.WriteLine("完整匹配失败，尝试模糊匹配");
            IntPtr foundWindow = IntPtr.Zero;
            EnumWindows((hWnd, lParam) =>
            {
                StringBuilder title = new StringBuilder(512); // 增加缓冲区大小以处理更长的标题
                GetWindowText(hWnd, title, title.Capacity);
                string windowTitle = title.ToString();
                
                // 检查窗口标题是否包含关键字，不区分大小写
                if (!string.IsNullOrEmpty(windowTitle) && windowTitle.IndexOf(keyword, StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    Debug.WriteLine("找到匹配窗口: " + windowTitle);
                    foundWindow = hWnd;
                    return false; // 停止枚举
                }
                return true; // 继续枚举
            }, IntPtr.Zero);
            
            return foundWindow;
        }
        
        private IntPtr FindWindowByFullKeyword(string keyword)
        {
            IntPtr foundWindow = IntPtr.Zero;
            // 尝试匹配包含完整部落冲突标题格式的窗口
            string[] patterns = new string[]
            {
                keyword, // 直接匹配
                $"{keyword}(Clash of Clans)", // 常见格式
                $"{keyword}(Clash of Clans)*" // 带后缀的格式
            };
            
            EnumWindows((hWnd, lParam) =>
            {
                StringBuilder title = new StringBuilder(512);
                GetWindowText(hWnd, title, title.Capacity);
                string windowTitle = title.ToString();
                
                foreach (string pattern in patterns)
                {
                    if (!string.IsNullOrEmpty(windowTitle) && 
                        (windowTitle.Equals(pattern, StringComparison.OrdinalIgnoreCase) ||
                         (pattern.EndsWith("*") && windowTitle.StartsWith(pattern.Substring(0, pattern.Length - 1), StringComparison.OrdinalIgnoreCase))))
                    {
                        Debug.WriteLine("找到符合模式的窗口: " + windowTitle);
                        foundWindow = hWnd;
                        return false; // 停止枚举
                    }
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
            IntPtr windowHandle = ActivateGameWindow();
            if (windowHandle == IntPtr.Zero)
            {
                Debug.WriteLine("未能找到或激活部落冲突游戏窗口");
                return;
            }

            // 等待鼠标移动到目标窗口内
            if (!WaitForMouseInTargetWindow(5000)) // 等待5秒
            {
                Debug.WriteLine("鼠标未在游戏窗口内，操作已取消");
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
                            
                            // 每次点击前检查鼠标是否在目标窗口内
                            if (!mouseSimulator.IsMouseInTargetWindow())
                            {
                                // 鼠标不在窗口内时等待，但不超过总时间
                                if (!WaitForMouseInTargetWindow(2000)) // 每次等待2秒
                                {
                                    Debug.WriteLine("鼠标离开游戏窗口，停止点击");
                                    break;
                                }
                            }
                            
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
                IntPtr windowHandle = ActivateGameWindow();
                if (windowHandle == IntPtr.Zero)
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

                        // 调用图像识别并点击功能，如果识别失败则使用固定位置作为备选方案
                        // 参数说明：图像名称，子文件夹，X轴偏移量，Y轴偏移量
                        bool success = imageRecognition.RecognizeAndClickWithFallback("001", "1", 100, 100);

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

        /// <summary>
        /// 设置按钮点击事件
        /// 打开设置界面
        /// </summary>
        private void settingsButton_Click(object sender, EventArgs e)
        {
            try
            {
                // 创建并显示设置界面
                SettingsForm settingsForm = new SettingsForm();
                settingsForm.ShowDialog();
            }
            catch (Exception ex)
            {
                Debug.WriteLine("打开设置界面时发生错误: " + ex.Message);
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
