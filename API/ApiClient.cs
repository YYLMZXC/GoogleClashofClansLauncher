using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using GoogleClashofClansLauncher.Config;
using GoogleClashofClansLauncher.Core;
using GoogleClashofClansLauncher.Core.System;

namespace GoogleClashofClansLauncher.API;

/// <summary>
/// API客户端类
/// 负责与外部API的通信
/// </summary>
public class ApiClient : IDisposable
{
    private HttpClient _httpClient;
    private readonly APIConfig _apiConfig;
    private bool _disposed = false;

    /// <summary>
    /// 构造函数
    /// </summary>
    public ApiClient()
    {
        _apiConfig = ConfigManager.GetConfig().API;
        _httpClient = CreateHttpClient();
    }

    /// <summary>
    /// 构造函数（带自定义配置）
    /// </summary>
    /// <param name="apiConfig">API配置</param>
    public ApiClient(APIConfig apiConfig)
    {
        _apiConfig = apiConfig ?? throw new ArgumentNullException(nameof(apiConfig));
        _httpClient = CreateHttpClient();
    }

    /// <summary>
    /// 创建HTTP客户端
    /// </summary>
    /// <returns>配置好的HttpClient实例</returns>
    private HttpClient CreateHttpClient()
    {
        var client = new HttpClient();
        
        // 设置基础URL
        if (!string.IsNullOrEmpty(_apiConfig.ApiUrl))
        {
            client.BaseAddress = new Uri(_apiConfig.ApiUrl);
        }
        
        // 设置超时时间
        client.Timeout = TimeSpan.FromMilliseconds(_apiConfig.Timeout);
        
        // 添加默认请求头
        client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
        
        // 如果有API密钥，添加认证头部
        if (!string.IsNullOrEmpty(_apiConfig.ApiKey))
        {
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _apiConfig.ApiKey);
        }
        
        return client;
    }

    /// <summary>
    /// 发送GET请求
    /// </summary>
    /// <typeparam name="T">返回数据类型</typeparam>
    /// <param name="endpoint">API端点</param>
    /// <returns>API响应结果</returns>
    public async Task<ApiResponse<T>> GetAsync<T>(string endpoint)
    {
        try
        {
            LogRequest("GET", endpoint);
            
            var response = await _httpClient.GetAsync(endpoint);
            
            return await ProcessResponse<T>(response, endpoint);
        }
        catch (Exception ex)
        {
            return HandleException<T>(ex, "GET", endpoint);
        }
    }

    /// <summary>
    /// 发送POST请求
    /// </summary>
    /// <typeparam name="TRequest">请求数据类型</typeparam>
    /// <typeparam name="TResponse">返回数据类型</typeparam>
    /// <param name="endpoint">API端点</param>
    /// <param name="data">请求数据</param>
    /// <returns>API响应结果</returns>
    public async Task<ApiResponse<TResponse>> PostAsync<TRequest, TResponse>(string endpoint, TRequest data)
    {
        try
        {
            var jsonData = JsonSerializer.Serialize(data);
            var content = new StringContent(jsonData, Encoding.UTF8, "application/json");
            
            LogRequest("POST", endpoint, jsonData);
            
            var response = await _httpClient.PostAsync(endpoint, content);
            
            return await ProcessResponse<TResponse>(response, endpoint);
        }
        catch (Exception ex)
        {
            return HandleException<TResponse>(ex, "POST", endpoint);
        }
    }

    /// <summary>
    /// 发送PUT请求
    /// </summary>
    /// <typeparam name="TRequest">请求数据类型</typeparam>
    /// <typeparam name="TResponse">返回数据类型</typeparam>
    /// <param name="endpoint">API端点</param>
    /// <param name="data">请求数据</param>
    /// <returns>API响应结果</returns>
    public async Task<ApiResponse<TResponse>> PutAsync<TRequest, TResponse>(string endpoint, TRequest data)
    {
        try
        {
            var jsonData = JsonSerializer.Serialize(data);
            var content = new StringContent(jsonData, Encoding.UTF8, "application/json");
            
            LogRequest("PUT", endpoint, jsonData);
            
            var response = await _httpClient.PutAsync(endpoint, content);
            
            return await ProcessResponse<TResponse>(response, endpoint);
        }
        catch (Exception ex)
        {
            return HandleException<TResponse>(ex, "PUT", endpoint);
        }
    }

    /// <summary>
    /// 发送DELETE请求
    /// </summary>
    /// <typeparam name="T">返回数据类型</typeparam>
    /// <param name="endpoint">API端点</param>
    /// <returns>API响应结果</returns>
    public async Task<ApiResponse<T>> DeleteAsync<T>(string endpoint)
    {
        try
        {
            LogRequest("DELETE", endpoint);
            
            var response = await _httpClient.DeleteAsync(endpoint);
            
            return await ProcessResponse<T>(response, endpoint);
        }
        catch (Exception ex)
        {
            return HandleException<T>(ex, "DELETE", endpoint);
        }
    }

    /// <summary>
    /// 处理API响应
    /// </summary>
    /// <typeparam name="T">返回数据类型</typeparam>
    /// <param name="response">HTTP响应</param>
    /// <param name="endpoint">API端点</param>
    /// <returns>处理后的API响应结果</returns>
    private async Task<ApiResponse<T>> ProcessResponse<T>(HttpResponseMessage response, string endpoint)
    {
        var responseContent = await response.Content.ReadAsStringAsync();
        LogResponse(response.StatusCode, responseContent, endpoint);

        if (response.IsSuccessStatusCode)
         {
             try
             {
                T? data = JsonSerializer.Deserialize<T>(responseContent);
                 return new ApiResponse<T>
                 {
                     Success = true,
                    Data = data,
                    StatusCode = (int)response.StatusCode,
                    Message = "请求成功"
                };
            }
            catch (JsonException ex)
            {
                Utils.LogError("响应数据反序列化失败", ex, "ApiClient");
                return new ApiResponse<T>
                {
                    Success = false,
                    StatusCode = (int)response.StatusCode,
                    Message = "响应数据格式无效"
                };
            }
        }
        else
        {
            Utils.LogWarning($"API请求失败: {response.StatusCode} - {responseContent}", "ApiClient");
            return new ApiResponse<T>
            {
                Success = false,
                StatusCode = (int)response.StatusCode,
                Message = responseContent
            };
        }
    }

    /// <summary>
    /// 处理异常情况
    /// </summary>
    /// <typeparam name="T">返回数据类型</typeparam>
    /// <param name="ex">异常对象</param>
    /// <param name="method">HTTP方法</param>
    /// <param name="endpoint">API端点</param>
    /// <returns>错误响应结果</returns>
    private ApiResponse<T> HandleException<T>(Exception ex, string method, string endpoint)
    {
        string errorMessage = ex.Message;
        int statusCode = 500;

        if (ex is TaskCanceledException) // 超时异常
        {
            errorMessage = "API请求超时";
            statusCode = 408;
        }
        else if (ex is HttpRequestException httpEx)
        {
            errorMessage = httpEx.Message;
            if (httpEx.StatusCode.HasValue)
            {
                statusCode = (int)httpEx.StatusCode.Value;
            }
        }

        Utils.LogError($"API {method}请求失败: {endpoint}", ex, "ApiClient");
         
         return new ApiResponse<T>
         {
            Success = false,
            StatusCode = statusCode,
            Message = errorMessage,
            Data = default
         };
     }
 
     /// <summary>
    /// 记录API请求日志
    /// </summary>
    /// <param name="method">HTTP方法</param>
    /// <param name="endpoint">API端点</param>
    /// <param name="requestBody">请求体（可选）</param>
    private void LogRequest(string method, string endpoint, string requestBody = null)
    {
        if (_apiConfig.EnableLogging)
        {
            if (string.IsNullOrEmpty(requestBody))
            {
                Utils.LogDebug($"API请求: {method} {endpoint}", "ApiClient");
            }
            else
            {
                // 避免记录敏感信息
                string safeRequestBody = MaskSensitiveData(requestBody);
                Utils.LogDebug($"API请求: {method} {endpoint}\n请求数据: {safeRequestBody}", "ApiClient");
            }
        }
    }

    /// <summary>
    /// 记录API响应日志
    /// </summary>
    /// <param name="statusCode">HTTP状态码</param>
    /// <param name="responseBody">响应体</param>
    /// <param name="endpoint">API端点</param>
    private void LogResponse(System.Net.HttpStatusCode statusCode, string responseBody, string endpoint)
    {
        if (_apiConfig.EnableLogging)
        {
            // 避免记录敏感信息
            string safeResponseBody = MaskSensitiveData(responseBody);
            Utils.LogDebug($"API响应: {statusCode} {endpoint}\n响应数据: {safeResponseBody}", "ApiClient");
        }
    }

    /// <summary>
    /// 屏蔽敏感数据
    /// </summary>
    /// <param name="jsonData">JSON数据</param>
    /// <returns>屏蔽后的JSON数据</returns>
    private string MaskSensitiveData(string jsonData)
    {
        if (string.IsNullOrEmpty(jsonData))
        {
            return jsonData;
        }

        // 使用正则表达式替换敏感数据（使用逐字字符串避免转义问题）
        string maskedData = jsonData;
        maskedData = System.Text.RegularExpressions.Regex.Replace(maskedData, @"""apiKey"":""[\w\d]*""", @"""apiKey"":""***""");
        maskedData = System.Text.RegularExpressions.Regex.Replace(maskedData, @"""password"":""[\w\d]*""", @"""password"":""***""");
        maskedData = System.Text.RegularExpressions.Regex.Replace(maskedData, @"""token"":""[\w\d]*""", @"""token"":""***""");
        maskedData = System.Text.RegularExpressions.Regex.Replace(maskedData, @"""secret"":""[\w\d]*""", @"""secret"":""***""");

        return maskedData;
    }

    /// <summary>
    /// 更新API配置
    /// </summary>
    /// <param name="apiConfig">新的API配置</param>
    public void UpdateConfig(APIConfig apiConfig)
    {
        if (apiConfig == null)
        {
            throw new ArgumentNullException(nameof(apiConfig));
        }

        // 释放旧的HttpClient
        _httpClient.Dispose();
        
        // 更新配置并创建新的HttpClient
        _apiConfig.ApiUrl = apiConfig.ApiUrl;
        _apiConfig.Timeout = apiConfig.Timeout;
        _apiConfig.EnableLogging = apiConfig.EnableLogging;
        
        // 只在有新密钥时更新
         if (!string.IsNullOrEmpty(apiConfig.ApiKey))
         {
            _apiConfig.ApiKey = apiConfig.ApiKey;
         }
         
         // 创建新的HttpClient
        _httpClient = CreateHttpClient();
        
        Utils.LogDebug("API客户端配置已更新", "ApiClient");
    }

    /// <summary>
    /// 资源释放
    /// </summary>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// 资源释放
    /// </summary>
    /// <param name="disposing">是否由Dispose调用</param>
    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                _httpClient.Dispose();
            }
            
            _disposed = true;
        }
    }

    /// <summary>
    /// 析构函数
    /// </summary>
    ~ApiClient()
    {
        Dispose(false);
    }
}

/// <summary>
/// API响应类
/// 统一的API响应格式
/// </summary>
/// <typeparam name="T">响应数据类型</typeparam>
public class ApiResponse<T>
{
    /// <summary>
    /// 请求是否成功
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// 响应数据
    /// </summary>
    public T? Data { get; set; }
 
     /// <summary>
     /// HTTP状态码
    /// </summary>
    public int StatusCode { get; set; }

    /// <summary>
    /// 消息描述
    /// </summary>
    public string? Message { get; set; }
 
     /// <summary>
     /// 错误详情
     /// </summary>
    public string? ErrorDetails { get; set; }
 
     /// <summary>
     /// 响应时间戳
    /// </summary>
    public DateTime Timestamp { get; set; } = DateTime.Now;
}