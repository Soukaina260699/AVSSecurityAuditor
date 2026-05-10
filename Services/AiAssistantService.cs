using System.Net.Http.Json;
using System.Text.Json;

namespace AVSSecurityAuditor.Services
{
    public class AiAssistantService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<AiAssistantService> _logger;

        private const string API_KEY = "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";
        private const string API_URL = "https://api.anthropic.com/v1/messages";

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
                    ? $"Explique l'exigence ASVS {requirementId}: \"{description}\". Donne un exemple pratique et concret en francais."
                    : $"Explain ASVS requirement {requirementId}: \"{description}\". Give a practical concrete example in English.";

                _httpClient.DefaultRequestHeaders.Clear();
                _httpClient.DefaultRequestHeaders.Add("x-api-key", API_KEY);
                _httpClient.DefaultRequestHeaders.Add("anthropic-version", "2023-06-01");

                var body = new
                {
                    model = "claude-haiku-4-5-20251001",
                    max_tokens = 500,
                    messages = new[]
                    {
                        new { role = "user", content = prompt }
                    }
                };

                var resp = await _httpClient.PostAsJsonAsync(API_URL, body);

                if (!resp.IsSuccessStatusCode)
                {
                    var err = await resp.Content.ReadAsStringAsync();
                    _logger.LogError("Anthropic error {status}: {err}", resp.StatusCode, err);
                    return $"Erreur Anthropic ({resp.StatusCode}): {err}";
                }

                var json = await resp.Content.ReadAsStringAsync();
                var doc = JsonDocument.Parse(json);
                return doc.RootElement
                    .GetProperty("content")[0]
                    .GetProperty("text")
                    .GetString() ?? "No response";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "AI failed: {msg}", ex.Message);
                return $"Erreur: {ex.Message}";
            }
        }
    }
}
