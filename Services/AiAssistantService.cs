using System.Net.Http.Json;
using System.Text.Json;

namespace AVSSecurityAuditor.Services
{
    public class AiAssistantService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<AiAssistantService> _logger;

        public AiAssistantService(HttpClient httpClient, ILogger<AiAssistantService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        private string GetKey() =>
            Environment.GetEnvironmentVariable("OPENAI_API_KEY")
            ?? Environment.GetEnvironmentVariable("AI__OpenAiApiKey")
            ?? string.Empty;

        public async Task<string> ExplainRequirementAsync(string requirementId, string description, string userLanguage = "en")
        {
            try
            {
                var apiKey = GetKey();

                if (string.IsNullOrEmpty(apiKey))
                {
                    _logger.LogWarning("No OpenAI API key found in environment variables");
                    return userLanguage == "fr"
                        ? "❌ Clé API manquante. Ajoutez OPENAI_API_KEY dans les variables Railway."
                        : "❌ API key missing. Add OPENAI_API_KEY in Railway variables.";
                }

                var prompt = userLanguage == "fr"
                    ? $"Tu es un expert OWASP. Explique l'exigence ASVS {requirementId}: \"{description}\". Donne un exemple concret et pratique. Réponds en français."
                    : $"You are an OWASP expert. Explain ASVS requirement {requirementId}: \"{description}\". Give a concrete practical example. Answer in English.";

                _httpClient.DefaultRequestHeaders.Clear();
                _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");

                var body = new
                {
                    model = "gpt-3.5-turbo",
                    messages = new[]
                    {
                        new { role = "system", content = "You are a cybersecurity expert specializing in OWASP ASVS v4.0." },
                        new { role = "user", content = prompt }
                    },
                    max_tokens = 500,
                    temperature = 0.7
                };

                var resp = await _httpClient.PostAsJsonAsync("https://api.openai.com/v1/chat/completions", body);

                if (!resp.IsSuccessStatusCode)
                {
                    var error = await resp.Content.ReadAsStringAsync();
                    _logger.LogError("OpenAI error {status}: {error}", resp.StatusCode, error);
                    return userLanguage == "fr"
                        ? $"❌ Erreur OpenAI ({resp.StatusCode}). Vérifiez votre clé et votre solde."
                        : $"❌ OpenAI error ({resp.StatusCode}). Check your key and credits.";
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
