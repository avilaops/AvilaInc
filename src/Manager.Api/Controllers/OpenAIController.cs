using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Manager.Infrastructure.Services;
using OpenAI.ObjectModels.RequestModels;

namespace Manager.Api.Controllers;

[ApiController]
[Route("api/openai")]
[Authorize]
public class OpenAIController : ControllerBase
{
    private readonly IOpenAIService _openAiService;
    private readonly ILogger<OpenAIController> _logger;

    public OpenAIController(
        IOpenAIService openAiService,
        ILogger<OpenAIController> logger)
    {
        _openAiService = openAiService;
        _logger = logger;
    }

    #region Chat Completions

    /// <summary>
    /// Chat simples com GPT-4
    /// </summary>
    [HttpPost("chat")]
    public async Task<IActionResult> Chat([FromBody] SimpleChatRequest request)
    {
        try
        {
            var response = await _openAiService.ChatAsync(
                request.Message,
                request.SystemMessage,
                request.Model
            );

            var message = response.Choices?.FirstOrDefault()?.Message?.Content ?? "";

            return Ok(new
            {
                success = true,
                message,
                model = response.Model,
                usage = new
                {
                    promptTokens = response.Usage?.PromptTokens,
                    completionTokens = response.Usage?.CompletionTokens,
                    totalTokens = response.Usage?.TotalTokens
                },
                finishReason = response.Choices?.FirstOrDefault()?.FinishReason
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro no chat");
            return StatusCode(500, new { success = false, message = ex.Message });
        }
    }

    /// <summary>
    /// Chat avançado com histórico de mensagens
    /// </summary>
    [HttpPost("chat/completions")]
    public async Task<IActionResult> ChatCompletion([FromBody] ChatCompletionRequest request)
    {
        try
        {
            var messages = request.Messages.Select(m => new ChatMessage(m.Role, m.Content)).ToList();

            var response = await _openAiService.CreateChatCompletionAsync(
                messages,
                request.Model,
                request.Temperature,
                request.MaxTokens
            );

            return Ok(new
            {
                success = true,
                data = new
                {
                    id = response.Id,
                    model = response.Model,
                    choices = response.Choices?.Select(c => new
                    {
                        index = c.Index,
                        message = new
                        {
                            role = c.Message?.Role,
                            content = c.Message?.Content
                        },
                        finishReason = c.FinishReason
                    }),
                    usage = new
                    {
                        promptTokens = response.Usage?.PromptTokens,
                        completionTokens = response.Usage?.CompletionTokens,
                        totalTokens = response.Usage?.TotalTokens
                    },
                    created = response.CreatedAt
                }
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro no chat completion");
            return StatusCode(500, new { success = false, message = ex.Message });
        }
    }

    #endregion

    #region Images (DALL-E)

    /// <summary>
    /// Gerar imagem com DALL-E 3
    /// </summary>
    [HttpPost("images/generate")]
    public async Task<IActionResult> GenerateImage([FromBody] GenerateImageRequest request)
    {
        try
        {
            var response = await _openAiService.GenerateImageAsync(
                request.Prompt,
                request.Size,
                request.NumberOfImages,
                request.Model
            );

            return Ok(new
            {
                success = true,
                count = response.Results?.Count ?? 0,
                images = response.Results?.Select(r => new
                {
                    url = r.Url,
                    revisedPrompt = r.RevisedPrompt
                })
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao gerar imagem");
            return StatusCode(500, new { success = false, message = ex.Message });
        }
    }

    /// <summary>
    /// Editar imagem existente
    /// </summary>
    [HttpPost("images/edit")]
    public async Task<IActionResult> EditImage([FromForm] EditImageRequest request)
    {
        try
        {
            if (request.Image == null || request.Image.Length == 0)
                return BadRequest(new { success = false, message = "Imagem obrigatória" });

            byte[] imageBytes;
            using (var ms = new MemoryStream())
            {
                await request.Image.CopyToAsync(ms);
                imageBytes = ms.ToArray();
            }

            byte[]? maskBytes = null;
            if (request.Mask != null && request.Mask.Length > 0)
            {
                using var maskMs = new MemoryStream();
                await request.Mask.CopyToAsync(maskMs);
                maskBytes = maskMs.ToArray();
            }

            var response = await _openAiService.EditImageAsync(imageBytes, request.Prompt, maskBytes);

            return Ok(new
            {
                success = true,
                count = response.Results?.Count ?? 0,
                images = response.Results?.Select(r => new { url = r.Url })
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao editar imagem");
            return StatusCode(500, new { success = false, message = ex.Message });
        }
    }

    /// <summary>
    /// Criar variações de uma imagem
    /// </summary>
    [HttpPost("images/variations")]
    public async Task<IActionResult> CreateImageVariation([FromForm] ImageVariationRequest request)
    {
        try
        {
            if (request.Image == null || request.Image.Length == 0)
                return BadRequest(new { success = false, message = "Imagem obrigatória" });

            byte[] imageBytes;
            using (var ms = new MemoryStream())
            {
                await request.Image.CopyToAsync(ms);
                imageBytes = ms.ToArray();
            }

            var response = await _openAiService.CreateImageVariationAsync(imageBytes, request.NumberOfVariations);

            return Ok(new
            {
                success = true,
                count = response.Results?.Count ?? 0,
                images = response.Results?.Select(r => new { url = r.Url })
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao criar variação");
            return StatusCode(500, new { success = false, message = ex.Message });
        }
    }

    #endregion

    #region Embeddings

    /// <summary>
    /// Criar embedding de um texto (para busca semântica, clustering, etc)
    /// </summary>
    [HttpPost("embeddings")]
    public async Task<IActionResult> CreateEmbedding([FromBody] EmbeddingRequest request)
    {
        try
        {
            var response = await _openAiService.CreateEmbeddingAsync(request.Text, request.Model);

            var embedding = response.Data?.FirstOrDefault()?.Embedding ?? new List<double>();

            return Ok(new
            {
                success = true,
                model = response.Model,
                dimensions = embedding.Count,
                embedding,
                usage = new
                {
                    promptTokens = response.Usage?.PromptTokens,
                    totalTokens = response.Usage?.TotalTokens
                }
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao criar embedding");
            return StatusCode(500, new { success = false, message = ex.Message });
        }
    }

    /// <summary>
    /// Criar embeddings de múltiplos textos
    /// </summary>
    [HttpPost("embeddings/batch")]
    public async Task<IActionResult> CreateEmbeddings([FromBody] BatchEmbeddingRequest request)
    {
        try
        {
            var response = await _openAiService.CreateEmbeddingsAsync(request.Texts, request.Model);

            return Ok(new
            {
                success = true,
                model = response.Model,
                count = response.Data?.Count ?? 0,
                embeddings = response.Data?.Select(d => new
                {
                    index = d.Index,
                    embedding = d.Embedding,
                    dimensions = d.Embedding?.Count ?? 0
                }),
                usage = new
                {
                    promptTokens = response.Usage?.PromptTokens,
                    totalTokens = response.Usage?.TotalTokens
                }
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao criar embeddings");
            return StatusCode(500, new { success = false, message = ex.Message });
        }
    }

    #endregion

    #region Moderation

    /// <summary>
    /// Moderar texto (detectar conteúdo inapropriado)
    /// </summary>
    [HttpPost("moderation")]
    public async Task<IActionResult> ModerateText([FromBody] ModerationRequest request)
    {
        try
        {
            var response = await _openAiService.CreateModerationAsync(request.Text);

            var result = response.Results?.FirstOrDefault();

            return Ok(new
            {
                success = true,
                flagged = result?.Flagged ?? false,
                categories = new
                {
                    hate = result?.Categories?.Hate,
                    hateThreatening = result?.Categories?.HateThreatening,
                    harassment = result?.Categories?.Harassment,
                    harassmentThreatening = result?.Categories?.HarassmentThreatening,
                    selfHarm = result?.Categories?.SelfHarm,
                    selfHarmIntent = result?.Categories?.SelfHarmIntent,
                    selfHarmInstructions = result?.Categories?.SelfHarmInstructions,
                    sexual = result?.Categories?.Sexual,
                    sexualMinors = result?.Categories?.SexualMinors,
                    violence = result?.Categories?.Violence,
                    violenceGraphic = result?.Categories?.ViolenceGraphic
                },
                categoryScores = new
                {
                    hate = result?.CategoryScores?.Hate,
                    hateThreatening = result?.CategoryScores?.HateThreatening,
                    harassment = result?.CategoryScores?.Harassment,
                    harassmentThreatening = result?.CategoryScores?.HarassmentThreatening,
                    selfHarm = result?.CategoryScores?.SelfHarm,
                    selfHarmIntent = result?.CategoryScores?.SelfHarmIntent,
                    selfHarmInstructions = result?.CategoryScores?.SelfHarmInstructions,
                    sexual = result?.CategoryScores?.Sexual,
                    sexualMinors = result?.CategoryScores?.SexualMinors,
                    violence = result?.CategoryScores?.Violence,
                    violenceGraphic = result?.CategoryScores?.ViolenceGraphic
                }
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro na moderação");
            return StatusCode(500, new { success = false, message = ex.Message });
        }
    }

    #endregion

    #region Models

    /// <summary>
    /// Listar modelos disponíveis
    /// </summary>
    [HttpGet("models")]
    public async Task<IActionResult> ListModels()
    {
        try
        {
            var models = await _openAiService.ListAvailableModelsAsync();

            // Filtrar apenas modelos relevantes
            var gptModels = models.Where(m => m.Contains("gpt")).ToList();
            var dalleModels = models.Where(m => m.Contains("dall-e")).ToList();
            var embeddingModels = models.Where(m => m.Contains("embedding")).ToList();

            return Ok(new
            {
                success = true,
                total = models.Count,
                models = new
                {
                    gpt = gptModels,
                    dalle = dalleModels,
                    embeddings = embeddingModels,
                    all = models
                }
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao listar modelos");
            return StatusCode(500, new { success = false, message = ex.Message });
        }
    }

    #endregion

    #region Use Cases

    /// <summary>
    /// Resumir texto longo
    /// </summary>
    [HttpPost("summarize")]
    public async Task<IActionResult> SummarizeText([FromBody] SummarizeRequest request)
    {
        try
        {
            var systemMessage = "Você é um assistente especializado em resumir textos. " +
                               $"Resuma o texto a seguir em {request.MaxWords ?? 100} palavras ou menos.";

            var response = await _openAiService.ChatAsync(request.Text, systemMessage);

            return Ok(new
            {
                success = true,
                summary = response.Choices?.FirstOrDefault()?.Message?.Content ?? "",
                originalLength = request.Text.Length,
                usage = response.Usage
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao resumir texto");
            return StatusCode(500, new { success = false, message = ex.Message });
        }
    }

    /// <summary>
    /// Traduzir texto
    /// </summary>
    [HttpPost("translate")]
    public async Task<IActionResult> TranslateText([FromBody] TranslateRequest request)
    {
        try
        {
            var systemMessage = $"Traduza o texto a seguir de {request.FromLanguage ?? "auto-detectar"} " +
                               $"para {request.ToLanguage}. Retorne apenas a tradução, sem explicações.";

            var response = await _openAiService.ChatAsync(request.Text, systemMessage);

            return Ok(new
            {
                success = true,
                translation = response.Choices?.FirstOrDefault()?.Message?.Content ?? "",
                fromLanguage = request.FromLanguage,
                toLanguage = request.ToLanguage,
                usage = response.Usage
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao traduzir texto");
            return StatusCode(500, new { success = false, message = ex.Message });
        }
    }

    #endregion
}

#region DTOs

public record SimpleChatRequest(
    string Message,
    string? SystemMessage = null,
    string? Model = null
);

public record ChatCompletionRequest(
    List<MessageDto> Messages,
    string? Model = null,
    float? Temperature = null,
    int? MaxTokens = null
);

public record MessageDto(string Role, string Content);

public record GenerateImageRequest(
    string Prompt,
    string? Size = null,
    int? NumberOfImages = 1,
    string? Model = null
);

public class EditImageRequest
{
    public IFormFile Image { get; set; } = null!;
    public string Prompt { get; set; } = string.Empty;
    public IFormFile? Mask { get; set; }
}

public class ImageVariationRequest
{
    public IFormFile Image { get; set; } = null!;
    public int? NumberOfVariations { get; set; } = 1;
}

public record EmbeddingRequest(string Text, string? Model = null);

public record BatchEmbeddingRequest(List<string> Texts, string? Model = null);

public record ModerationRequest(string Text);

public record SummarizeRequest(string Text, int? MaxWords = 100);

public record TranslateRequest(
    string Text,
    string ToLanguage,
    string? FromLanguage = null
);

#endregion
