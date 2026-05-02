using System.Text;
using System.Text.Json;
using WebApplicationAsp.Repository;
using WebApplicationAsp.ViewModels;

namespace WebApplicationAsp.Services
{
    public class AIAssistantService : IAIAssistantService
    {
        private readonly HttpClient _http;
        private readonly IUnitOfWork _uow;
        private readonly IConfiguration _config;

        private const string HfRouterBaseUrl = "https://router.huggingface.co/v1/chat/completions";

        public AIAssistantService(HttpClient http, IUnitOfWork uow, IConfiguration config)
        {
            _http = http;
            _uow = uow;
            _config = config;
        }

        public async Task<AIExplanationResult> ExplainRequirementAsync(int itemId, string technology)
        {
            var apiKey = _config["HuggingFace:ApiKey"];
            var model = _config["HuggingFace:Model"] ?? "deepseek-ai/DeepSeek-V4-Pro";

            if (string.IsNullOrWhiteSpace(apiKey))
                return Fallback();

            var items = await _uow.Items.GetAllAsync(
                filter: i => i.Id == itemId,
                includeProperties: "SubCategory.Category");

            var req = items.FirstOrDefault();
            if (req is null)
                return new AIExplanationResult { Success = false, Error = "Exigence introuvable." };

            var tech = string.IsNullOrWhiteSpace(technology) ? "générique" : technology;

            var prompt = BuildPrompt(
                req.Code,
                req.Description,
                req.SubCategory.Category.Name,
                req.SubCategory.Name,
                req.Level,
                tech
            );

            try
            {
                var payload = new
                {
                    model = model,
                    messages = new[]
                    {
                        new { role = "user", content = prompt }
                    },
                    max_tokens = 1500,
                    temperature = 0.1
                };

                var json = JsonSerializer.Serialize(payload);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                _http.DefaultRequestHeaders.Clear();
                _http.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", apiKey);

                var response = await _http.PostAsync(HfRouterBaseUrl, content);
                var body = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    return new AIExplanationResult
                    {
                        Success = false,
                        Error = $"DeepSeek HF Error {(int)response.StatusCode}: {body[..Math.Min(300, body.Length)]}"
                    };
                }

                using var doc = JsonDocument.Parse(body);
                var text = doc.RootElement
                    .GetProperty("choices")[0]
                    .GetProperty("message")
                    .GetProperty("content")
                    .GetString();

                if (string.IsNullOrWhiteSpace(text))
                    return Fallback();

                // Sécurité : strip code fences si jamais
                text = text.Trim();
                if (text.StartsWith("```")) text = text[(text.IndexOf('\n') + 1)..];
                if (text.EndsWith("```")) text = text[..text.LastIndexOf("```")].Trim();

                var result = JsonSerializer.Deserialize<AIExplanationResult>(
                    text,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
                );

                if (result is null)
                    return Fallback();

                result.Success = true;
                return result;
            }
            catch (Exception ex)
            {
                return new AIExplanationResult
                {
                    Success = false,
                    Error = ex.Message
                };
            }
        }

        private static string BuildPrompt(
            string code,
            string description,
            string chapter,
            string section,
            string level,
            string technology) => $$"""
Tu es un expert en sécurité applicative OWASP ASVS.

Exigence : {{code}} — {{description}}
Chapitre  : {{chapter}}
Section   : {{section}}
Niveau    : L{{level}}
Technologie cible : {{technology}}

Fournis une analyse adaptée à la technologie "{{technology}}".
Réponds UNIQUEMENT en JSON valide (sans markdown, sans commentaires) :
{
  "explanation": "explication simple en 2-3 phrases, adaptée à {{technology}}",
  "vulnerability": "exemple concret de vulnérabilité si cette exigence n'est PAS respectée en {{technology}}",
  "bestPractices": [
    "pratique 1 spécifique à {{technology}}",
    "pratique 2 spécifique à {{technology}}",
    "pratique 3 spécifique à {{technology}}"
  ]
}
""";

        private static AIExplanationResult Fallback() => new()
        {
            Success = true,
            Explanation = "Configurez votre clé HuggingFace (HF_TOKEN) pour activer DeepSeek.",
            Vulnerability = "Sans IA, les explications automatiques ne sont pas disponibles.",
            BestPractices = new()
            {
                "Consulter OWASP ASVS",
                "Appliquer le principe du moindre privilège",
                "Mettre en place des contrôles automatisés"
            }
        };
    }
}