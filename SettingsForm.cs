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
using System.Collections.Generic;
using Newtonsoft.Json;

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
        private Dictionary<string, ApiInfo> apiConfigurations = new Dictionary<string, ApiInfo>();
        private List<string> customApis = new List<string>();

        // API信息类
        private class ApiInfo
        {
            public string Endpoint { get; set; }
            public string Key { get; set; }
        }

        public SettingsForm()
        {
            InitializeComponent();
            imageRecognition = new ImageRecognition();
            windowManager = new WindowManager();
            
            // 初始化配置文件路径
            configFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Config", "appsettings.json");
            
            // 初始化按钮状态
            editApiButton.Enabled = false;
            deleteApiButton.Enabled = false;
            customApiNameTextBox.Enabled = false;
            apiTypeLabel.Text = "";
            
            // 添加预定义的AI API接口
            AddPredefinedApis();
            // 加载保存的设置
            LoadSavedSettings();
        }

        private void AddPredefinedApis()
        {
            // 添加常见的AI API接口
            apiConfigurations.Add("OpenAI GPT-3.5", new ApiInfo { 
                Endpoint = "https://api.openai.com/v1/chat/completions", 
                Key = "sk-" 
            });
            apiConfigurations.Add("OpenAI GPT-4", new ApiInfo { 
                Endpoint = "https://api.openai.com/v1/chat/completions", 
                Key = "sk-" 
            });
            apiConfigurations.Add("Anthropic Claude", new ApiInfo { 
                Endpoint = "https://api.anthropic.com/v1/messages", 
                Key = "sk-ant-" 
            });
            apiConfigurations.Add("Google Gemini", new ApiInfo { 
                Endpoint = "https://generativelanguage.googleapis.com/v1beta/models", 
                Key = "AIzaSy" 
            });
            apiConfigurations.Add("百度文心一言", new ApiInfo { 
                Endpoint = "https://aip.baidubce.com/rpc/2.0/ai_custom/v1/wenxinworkshop", 
                Key = "" 
            });
            apiConfigurations.Add("阿里通义千问", new ApiInfo { 
                Endpoint = "https://dashscope.aliyuncs.com/api/v1/services", 
                Key = "sk-" 
            });
            apiConfigurations.Add("腾讯混元大模型", new ApiInfo { 
                Endpoint = "https://api.tencentcloudapi.com", 
                Key = "" 
            });
            apiConfigurations.Add("讯飞星火认知大模型", new ApiInfo { 
                Endpoint = "https://spark-api.xf-yun.com/v3.1/chat", 
                Key = "" 
            });
            apiConfigurations.Add("豆包", new ApiInfo { 
                Endpoint = "https://api.doubao.com/chat/completions", 
                Key = "db-" 
            });

            // 加载ComboBox
            LoadApiComboBox();
        }

        private void LoadApiComboBox()
        {
            apiComboBox.Items.Clear();
            
            // 添加预定义API
            foreach (var apiName in apiConfigurations.Keys)
            {
                apiComboBox.Items.Add(apiName);
            }
            
            // 添加自定义API
            foreach (var customApi in customApis)
            {
                apiComboBox.Items.Add(customApi);
            }
        }
        
        private void LoadSavedSettings()
        {
            try
            {
                if (File.Exists(configFilePath))
                {
                    string jsonContent = File.ReadAllText(configFilePath);
                    var config = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, dynamic>>(jsonContent);
                    
                    // 加载API配置
                    if (config.ContainsKey("ApiEndpoint"))
                    {
                        apiEndpointTextBox.Text = config["ApiEndpoint"]?.ToString() ?? "";
                    }
                    
                    if (config.ContainsKey("ApiKey"))
                    {
                        apiKeyTextBox.Text = config["ApiKey"]?.ToString() ?? "";
                    }

                    // 加载自定义API
                    if (config.ContainsKey("CustomApis"))
                    {
                        try
                        {
                            var customApisArray = Newtonsoft.Json.JsonConvert.DeserializeObject<List<string>>(config["CustomApis"]?.ToString() ?? "[]");
                            if (customApisArray != null)
                            {
                                customApis = customApisArray;
                                LoadApiComboBox();
                            }
                        }
                        catch { }
                    }

                    // 加载选定的API
                    if (config.ContainsKey("SelectedApi"))
                    {
                        string selectedApi = config["SelectedApi"]?.ToString();
                        if (!string.IsNullOrEmpty(selectedApi))
                        {
                            int index = apiComboBox.FindStringExact(selectedApi);
                            if (index >= 0)
                            {
                                apiComboBox.SelectedIndex = index;
                            }
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

                // 创建配置字典
                var config = new Dictionary<string, dynamic>
                {
                    { "ApiEndpoint", apiEndpointTextBox.Text },
                    { "ApiKey", apiKeyTextBox.Text }
                };

                // 保存选定的API
                if (apiComboBox.SelectedIndex >= 0)
                {
                    config["SelectedApi"] = apiComboBox.SelectedItem.ToString();
                }

                // 保存自定义API列表
                if (customApis.Count > 0)
                {
                    config["CustomApis"] = Newtonsoft.Json.JsonConvert.SerializeObject(customApis);
                }

                // 确保配置目录存在
                string configDir = Path.GetDirectoryName(configFilePath);
                if (!Directory.Exists(configDir))
                {
                    Directory.CreateDirectory(configDir);
                }

                // 保存配置文件
                string jsonContent = Newtonsoft.Json.JsonConvert.SerializeObject(config, Newtonsoft.Json.Formatting.Indented);
                File.WriteAllText(configFilePath, jsonContent);

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

        private void apiComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (apiComboBox.SelectedIndex >= 0)
            {
                string selectedApi = apiComboBox.SelectedItem.ToString();
                
                // 检查是否是预定义API
                if (apiConfigurations.ContainsKey(selectedApi))
                {
                    ApiInfo apiInfo = apiConfigurations[selectedApi];
                    apiEndpointTextBox.Text = apiInfo.Endpoint;
                    apiKeyTextBox.Text = apiInfo.Key;
                    apiTypeLabel.Text = "官方API";
                    apiTypeLabel.ForeColor = Color.Green;
                    // 预定义API不能编辑和删除
                    editApiButton.Enabled = false;
                    deleteApiButton.Enabled = false;
                    customApiNameTextBox.Text = selectedApi;
                    customApiNameTextBox.Enabled = false;
                }
                else if (customApis.Contains(selectedApi))
                {
                    // 对于自定义API，我们只保存了名称，用户需要重新输入地址和密钥
                    apiEndpointTextBox.Text = "";
                    apiKeyTextBox.Text = "";
                    apiTypeLabel.Text = "自定义API";
                    apiTypeLabel.ForeColor = Color.Blue;
                    // 自定义API可以编辑和删除
                    editApiButton.Enabled = true;
                    deleteApiButton.Enabled = true;
                    customApiNameTextBox.Text = selectedApi;
                    customApiNameTextBox.Enabled = true;
                }
            }
        }

        private void addCustomApiButton_Click(object sender, EventArgs e)
        {
            // 如果输入框是禁用的，先启用它
            if (!customApiNameTextBox.Enabled)
            {
                customApiNameTextBox.Enabled = true;
                customApiNameTextBox.Focus();
                statusLabel.Text = "请输入自定义API名称";
                return;
            }
            
            string customApiName = customApiNameTextBox.Text.Trim();
            
            if (string.IsNullOrEmpty(customApiName))
            {
                statusLabel.Text = "请输入自定义API名称";
                return;
            }

            if (apiConfigurations.ContainsKey(customApiName) || customApis.Contains(customApiName))
            {
                statusLabel.Text = "该API名称已存在";
                return;
            }

            // 添加自定义API
            customApis.Add(customApiName);
            
            // 刷新ComboBox
            LoadApiComboBox();
            
            // 选择新添加的API
            int index = apiComboBox.FindStringExact(customApiName);
            if (index >= 0)
            {
                apiComboBox.SelectedIndex = index;
            }

            // 清空输入框
            customApiNameTextBox.Text = "";
            
            statusLabel.Text = "自定义API已添加";
        }

        private void editApiButton_Click(object sender, EventArgs e)
        {
            if (apiComboBox.SelectedIndex >= 0)
            {
                string selectedApi = apiComboBox.SelectedItem.ToString();
                
                // 只允许编辑自定义API
                if (customApis.Contains(selectedApi))
                {
                    string newApiName = customApiNameTextBox.Text.Trim();
                    
                    if (string.IsNullOrEmpty(newApiName))
                    {
                        statusLabel.Text = "请输入新的API名称";
                        return;
                    }

                    // 检查新名称是否已存在
                    if ((newApiName != selectedApi) && 
                        (apiConfigurations.ContainsKey(newApiName) || customApis.Contains(newApiName)))
                    {
                        statusLabel.Text = "该API名称已存在";
                        return;
                    }

                    // 更新自定义API名称
                    int index = customApis.IndexOf(selectedApi);
                    if (index >= 0)
                    {
                        customApis[index] = newApiName;
                    }
                    
                    // 刷新ComboBox
                    LoadApiComboBox();
                    
                    // 选择重命名后的API
                    int newIndex = apiComboBox.FindStringExact(newApiName);
                    if (newIndex >= 0)
                    {
                        apiComboBox.SelectedIndex = newIndex;
                    }
                    
                    statusLabel.Text = "自定义API已重命名";
                }
            }
        }

        private void deleteApiButton_Click(object sender, EventArgs e)
        {
            if (apiComboBox.SelectedIndex >= 0)
            {
                string selectedApi = apiComboBox.SelectedItem.ToString();
                
                // 只允许删除自定义API
                if (customApis.Contains(selectedApi))
                {
                    // 显示确认对话框
                    DialogResult result = MessageBox.Show(
                        $"确定要删除自定义API '{selectedApi}' 吗？", 
                        "确认删除", 
                        MessageBoxButtons.YesNo, 
                        MessageBoxIcon.Warning);
                    
                    if (result == DialogResult.Yes)
                    {
                        // 从列表中移除自定义API
                        customApis.Remove(selectedApi);
                        
                        // 刷新ComboBox
                        LoadApiComboBox();
                        
                        // 如果ComboBox还有项，选择第一项
                        if (apiComboBox.Items.Count > 0)
                        {
                            apiComboBox.SelectedIndex = 0;
                        }
                        else
                        {
                            // 清空输入框
                            apiEndpointTextBox.Text = "";
                            apiKeyTextBox.Text = "";
                            customApiNameTextBox.Text = "";
                            apiTypeLabel.Text = "";
                            editApiButton.Enabled = false;
                            deleteApiButton.Enabled = false;
                        }
                        
                        statusLabel.Text = "自定义API已删除";
                    }
                }
            }
        }

        // 已移除识别设置图像的功能和相关窗口操作代码
    }
}