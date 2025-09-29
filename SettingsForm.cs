using GoogleClashofClansLauncher.Input;
using GoogleClashofClansLauncher.Core;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using System.Threading.Tasks;
using System.Threading;
using System.Text;
using System.Runtime.InteropServices;
using System.Configuration;
using System.IO;

namespace GoogleClashofClansLauncher
{
    public partial class SettingsForm : Form
    {
        private ImageRecognition imageRecognition;
        private WindowManager windowManager;
        
        // 进程名称和窗口标题关键字
        private const string ProcessName = "crosvm";
        private const string WindowTitleKeyword = "部落冲突";
        
        // 配置文件路径
        private string configFilePath;

        public SettingsForm()
        {
            InitializeComponent();
            imageRecognition = new ImageRecognition();
            windowManager = new WindowManager();
            
            // 初始化配置文件路径
            configFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Config", "appsettings.json");
            
            // 加载保存的设置
            LoadSavedSettings();
        }
        
        private void LoadSavedSettings()
        {
            try
            {
                if (File.Exists(configFilePath))
                {
                    string jsonContent = File.ReadAllText(configFilePath);
                    // 简单解析JSON配置
                    if (jsonContent.Contains("\"ApiEndpoint\":\""))
                    {
                        int startIndex = jsonContent.IndexOf("\"ApiEndpoint\":\"") + 14;
                        int endIndex = jsonContent.IndexOf("\"", startIndex);
                        if (startIndex > 13 && endIndex > startIndex)
                        {
                            apiEndpointTextBox.Text = jsonContent.Substring(startIndex, endIndex - startIndex);
                        }
                    }
                    if (jsonContent.Contains("\"ApiKey\":\""))
                    {
                        int startIndex = jsonContent.IndexOf("\"ApiKey\":\"") + 10;
                        int endIndex = jsonContent.IndexOf("\"", startIndex);
                        if (startIndex > 9 && endIndex > startIndex)
                        {
                            apiKeyTextBox.Text = jsonContent.Substring(startIndex, endIndex - startIndex);
                        }
                    }
                }
                else
                {
                    // 确保Config目录存在
                        string configDir = Path.GetDirectoryName(configFilePath);
                        if (!string.IsNullOrEmpty(configDir))
                        {
                            Directory.CreateDirectory(configDir);
                        }
                    // 设置默认值
                    apiEndpointTextBox.Text = "https://api.example.com/v1/chat/completions";
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("加载设置失败: " + ex.Message);
                statusLabel.Text = "加载设置失败: " + ex.Message;
            }
        }
        
        private void saveSettingsButton_Click(object sender, EventArgs e)
        {
            try
            {
                // 验证API地址格式
                if (!string.IsNullOrEmpty(apiEndpointTextBox.Text))
                {
                    Uri uriResult;
                    if (!Uri.TryCreate(apiEndpointTextBox.Text, UriKind.Absolute, out uriResult) || 
                        (uriResult.Scheme != Uri.UriSchemeHttp && uriResult.Scheme != Uri.UriSchemeHttps))
                    {
                        statusLabel.Text = "API地址格式无效";
                        return;
                    }
                }
                
                // 构建配置JSON
                string apiEndpoint = apiEndpointTextBox.Text ?? string.Empty;
                string apiKey = apiKeyTextBox.Text ?? string.Empty;
                
                // 构建配置JSON
                string jsonConfig = "{\n" +
                                   "  \"ApiEndpoint\": \"" + apiEndpoint.Replace("\"", "\\\"") + "\",\n" +
                                   "  \"ApiKey\": \"" + apiKey.Replace("\"", "\\\"") + "\"\n" +
                                   "}";
                
                // 保存到文件
                File.WriteAllText(configFilePath, jsonConfig);
                
                statusLabel.Text = "设置已保存";
                Debug.WriteLine("API设置已保存");
                
                // 显示保存成功消息
                MessageBox.Show("设置已成功保存", "保存成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("保存设置失败: " + ex.Message);
                statusLabel.Text = "保存设置失败: " + ex.Message;
                MessageBox.Show("保存设置失败: " + ex.Message, "保存失败", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void settingsRecognitionButton_Click(object sender, EventArgs e)
        {
            try
            {
                // 尝试激活游戏窗口
                if (!ActivateGameWindow())
                {
                    Debug.WriteLine("未能找到或激活部落冲突游戏窗口");
                    statusLabel.Text = "未能找到或激活游戏窗口";
                    return;
                }

                // 显示正在识别的状态
                settingsRecognitionButton.Text = "正在识别图像...";
                settingsRecognitionButton.Enabled = false;
                statusLabel.Text = "正在识别图像...";
                Debug.WriteLine("开始识别设置图像");

                // 使用Task在后台线程中执行图像识别，避免UI冻结
                Task.Run(() =>
                {
                    try
                    {
                        // 等待窗口激活
                        Thread.Sleep(200);

                        // 调用图像识别并点击功能，使用res/2/002.png
                        bool success = imageRecognition.RecognizeAndClickResImage("002", "2");

                        // 更新UI状态
                        this.Invoke((MethodInvoker)delegate
                        {
                            if (success)
                            {
                                settingsRecognitionButton.Text = "识别并点击成功! (res/2/002.png)";
                                statusLabel.Text = "识别并点击成功";
                                Debug.WriteLine("设置图像识别并点击成功");
                            }
                            else
                            {
                                settingsRecognitionButton.Text = "未找到图像，请重试 (res/2/002.png)";
                                statusLabel.Text = "未找到图像或点击失败";
                                Debug.WriteLine("未找到设置图像或点击失败");
                            }

                            // 恢复按钮状态
                            Thread.Sleep(1000);
                            settingsRecognitionButton.Text = "识别设置图像并点击 (res/2/002.png)";
                            settingsRecognitionButton.Enabled = true;
                            statusLabel.Text = "就绪";
                        });
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine("设置图像识别过程中发生错误: " + ex.Message);
                        // 发生异常时恢复按钮状态
                        this.Invoke((MethodInvoker)delegate
                        {
                            settingsRecognitionButton.Text = "识别设置图像并点击 (res/2/002.png)";
                            settingsRecognitionButton.Enabled = true;
                            statusLabel.Text = "发生错误: " + ex.Message;
                        });
                    }
                });
            }
            catch (Exception ex)
            {
                Debug.WriteLine("设置按钮点击处理异常: " + ex.Message);
                settingsRecognitionButton.Text = "识别设置图像并点击 (res/2/002.png)";
                settingsRecognitionButton.Enabled = true;
                statusLabel.Text = "发生错误: " + ex.Message;
            }
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

        // 通过关键字模糊查找窗口
        private IntPtr FindWindowByKeyword(string keyword)
        {
            IntPtr foundWindow = IntPtr.Zero;
            NativeMethods.EnumWindows((hWnd, lParam) =>
            {
                StringBuilder title = new StringBuilder(256);
                NativeMethods.GetWindowText(hWnd, title, title.Capacity);
                
                if (title.ToString().Contains(keyword))
                {
                    foundWindow = hWnd;
                    return false; // 停止枚举
                }
                return true; // 继续枚举
            }, IntPtr.Zero);
            
            return foundWindow;
        }

        // 嵌套类用于存放Win32 API声明
        private static class NativeMethods
        {
            [DllImport("user32.dll", CharSet = CharSet.Unicode)]
            public static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

            [DllImport("user32.dll")]
            public static extern bool EnumWindows(EnumWindowsProc lpEnumFunc, IntPtr lParam);

            public delegate bool EnumWindowsProc(IntPtr hWnd, IntPtr lParam);
        }
    }
}