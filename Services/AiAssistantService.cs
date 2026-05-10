using System.Net.Http.Json;
using System.Text.Json;

namespace AVSSecurityAuditor.Services
{
    public class AiAssistantService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<AiAssistantService> _logger;

        private const string API_KEY = "sk-5a25b193a6154b8baf3ae2e45da100c7";
        private const string API_URL = "https://api.deepseek.com/v1/chat/completions";

        public AiAssistantService(HttpClient httpClient, ILogger<AiAssistantService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<string> ExplainRequirementAsync(string requirementId, string description, string userLanguage = "en")
        {
            try
            {
                var prompt = userLanguage == "fr"
                    ? $"Explique clairement l'exigence ASVS {requirementId}: \"{description}\". Donne un exemple pratique et pourquoi c'est important pour la sécurité des applications web. Réponds en français."
                    : $"Clearly explain the ASVS requirement {requirementId}: \"{description}\". Provide a practical example and why it matters for web application security. Answer in English.";

                _httpClient.DefaultRequestHeaders.Clear();
                _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {API_KEY}");

                var body = new
                {
                    model = "deepseek-chat",
                    messages = new[]
                    {
                        new { role = "system", content = "You are a cybersecurity expert specializing in OWASP ASVS." },
                        new { role = "user", content = prompt }
                    },
                    max_tokens = 600,
                    temperature = 0.7
                };

                var resp = await _httpClient.PostAsJsonAsync(API_URL, body);

                if (!resp.IsSuccessStatusCode)
                {
                    var error = await resp.Content.ReadAsStringAsync();
                    _logger.LogError("DeepSeek error: {error}", error);
                    return userLanguage == "fr"
                        ? $"❌ Erreur API ({resp.StatusCode}). Vérifiez votre clé DeepSeek."
                        : $"❌ API error ({resp.StatusCode}). Check your DeepSeek key.";
                }

                var json = await resp.Content.ReadAsStringAsync();
                var doc = JsonDocument.Parse(json);
                return doc.RootElement
                    .GetProperty("choices")[0]
                    .GetProperty("message")
                    .GetProperty("content")
                    .GetString() ?? "No response";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "AI call failed: {msg}", ex.Message);
                return userLanguage == "fr"
                    ? "❌ Erreur de connexion au service IA."
                    : "❌ AI connection error.";
            }
        }
    }
}
