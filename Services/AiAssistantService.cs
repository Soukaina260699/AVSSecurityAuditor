using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

namespace AVSSecurityAuditor.Services
{
    public class AiAssistantService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _config;
        private readonly ILogger<AiAssistantService> _logger;

        public AiAssistantService(HttpClient httpClient, IConfiguration config, ILogger<AiAssistantService> logger)
        {
            _httpClient = httpClient;
            _config = config;
            _logger = logger;
        }

        public async Task<string> ExplainRequirementAsync(string requirementId, string description, string userLanguage = "en")
        {
            try
            {
                var provider = _config["AI:Provider"] ?? "openai";

                if (provider == "deepseek")
                    return await CallDeepSeekAsync(requirementId, description, userLanguage);
                else if (provider == "anthropic")
                    return await CallAnthropicAsync(requirementId, description, userLanguage);
                else
                    return await CallOpenAiAsync(requirementId, description, userLanguage);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "AI call failed");
                return userLanguage == "fr"
                    ? "❌ Le service IA est temporairement indisponible. Veuillez vérifier votre clé API dans appsettings.json."
                    : "❌ The AI service is temporarily unavailable. Please check your API key in appsettings.json.";
            }
        }

        private async Task<string> CallDeepSeekAsync(string reqId, string desc, string lang)
        {
            var apiKey = _config["AI:DeepSeekApiKey"];
            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");

            var prompt = lang == "fr"
                ? $"Explique clairement l'exigence ASVS {reqId}: \"{desc}\". Donne un exemple pratique et pourquoi c'est important pour la sécurité."
                : $"Clearly explain the ASVS requirement {reqId}: \"{desc}\". Provide a practical example and why it matters for security.";

            var body = new
            {
                model = "deepseek-chat",
                messages = new[] { new { role = "user", content = prompt } },
                max_tokens = 500
            };

            var resp = await _httpClient.PostAsJsonAsync("https://api.deepseek.com/v1/chat/completions", body);
            var json = await resp.Content.ReadAsStringAsync();
            var doc = JsonDocument.Parse(json);
            return doc.RootElement.GetProperty("choices")[0].GetProperty("message").GetProperty("content").GetString() ?? "No response";
        }

        private async Task<string> CallOpenAiAsync(string reqId, string desc, string lang)
        {
            var apiKey = _config["AI:OpenAiApiKey"];
            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");

            var prompt = lang == "fr"
                ? $"Explique clairement l'exigence ASVS {reqId}: \"{desc}\". Donne un exemple pratique."
                : $"Clearly explain ASVS requirement {reqId}: \"{desc}\". Give a practical example.";

            var body = new
            {
                model = "gpt-3.5-turbo",
                messages = new[] { new { role = "user", content = prompt } },
                max_tokens = 500
            };

            var resp = await _httpClient.PostAsJsonAsync("https://api.openai.com/v1/chat/completions", body);
            var json = await resp.Content.ReadAsStringAsync();
            var doc = JsonDocument.Parse(json);
            return doc.RootElement.GetProperty("choices")[0].GetProperty("message").GetProperty("content").GetString() ?? "No response";
        }

        private async Task<string> CallAnthropicAsync(string reqId, string desc, string lang)
        {
            var apiKey = _config["AI:AnthropicApiKey"];
            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("x-api-key", apiKey!);
            _httpClient.DefaultRequestHeaders.Add("anthropic-version", "2023-06-01");

            var prompt = lang == "fr"
                ? $"Explique l'exigence ASVS {reqId}: \"{desc}\" avec un exemple pratique."
                : $"Explain ASVS requirement {reqId}: \"{desc}\" with a practical example.";

            var body = new
            {
                model = "claude-3-haiku-20240307",
                max_tokens = 500,
                messages = new[] { new { role = "user", content = prompt } }
            };

            var resp = await _httpClient.PostAsJsonAsync("https://api.anthropic.com/v1/messages", body);
            var json = await resp.Content.ReadAsStringAsync();
            var doc = JsonDocument.Parse(json);
            return doc.RootElement.GetProperty("content")[0].GetProperty("text").GetString() ?? "No response";
        }
    }
}
