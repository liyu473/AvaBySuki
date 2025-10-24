namespace AvaAIChat.Models;

/// <summary>
/// OpenRouter API 配置
/// </summary>
public class OpenRouterConfig
{
    /// <summary>
    /// API Key
    /// </summary>
    public string ApiKey { get; set; } = string.Empty;

    /// <summary>
    /// API 基础地址
    /// </summary>
    public string BaseUrl { get; set; } = "https://openrouter.ai/api/v1";

    /// <summary>
    /// 使用的模型
    /// </summary>
    public string Model { get; set; } = "google/gemini-2.0-flash-exp:free";

    /// <summary>
    /// 最大生成 token 数
    /// </summary>
    public int MaxTokens { get; set; } = 4096;

    /// <summary>
    /// 温度参数 (0-2)
    /// </summary>
    public double Temperature { get; set; } = 0.7;
    
    /// <summary>
    /// Top-p 采样参数 (0-1)，控制生成文本的多样性
    /// </summary>
    public double TopP { get; set; } = 0.95;
}
