using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using OpenAI.Managers;
using OpenAI.ObjectModels;
using OpenAI.ObjectModels.RequestModels;
using OpenAI.ObjectModels.ResponseModels;
using OpenAI.ObjectModels.ResponseModels.ImageResponseModel;

namespace Manager.Infrastructure.Services;

public interface IOpenAIService
{
    // Chat Completions
    Task<ChatCompletionCreateResponse> CreateChatCompletionAsync(
        List<ChatMessage> messages, 
        string? model = null, 
        float? temperature = null,
        int? maxTokens = null);
    
    Task<ChatCompletionCreateResponse> ChatAsync(
        string userMessage, 
        string? systemMessage = null,
        string? model = null);

    // Images (DALL-E)
    Task<ImageCreateResponse> GenerateImageAsync(
        string prompt, 
        string? size = null, 
        int? numberOfImages = null,
        string? model = null);
    
    Task<ImageCreateResponse> EditImageAsync(
        byte[] imageBytes, 
        string prompt, 
        byte[]? maskBytes = null);
    
    Task<ImageCreateResponse> CreateImageVariationAsync(byte[] imageBytes, int? numberOfVariations = null);

    // Embeddings
    Task<EmbeddingCreateResponse> CreateEmbeddingAsync(string text, string? model = null);
    Task<EmbeddingCreateResponse> CreateEmbeddingsAsync(List<string> texts, string? model = null);

    // Moderation
    Task<CreateModerationResponse> CreateModerationAsync(string text);

    // Models
    Task<List<string>> ListAvailableModelsAsync();
}

public class OpenAIService : IOpenAIService
{
    private readonly ILogger<OpenAIService> _logger;
    private readonly OpenAI.Interfaces.IOpenAIService _client;
    private readonly string _defaultChatModel;
    private readonly string _defaultImageModel;

    public OpenAIService(IConfiguration configuration, ILogger<OpenAIService> logger)
    {
        _logger = logger;

        var apiKey = configuration["Integrations:OpenAI:ApiKey"] 
                    ?? throw new ArgumentException("OpenAI ApiKey não configurada");

        _client = new OpenAI.Managers.OpenAIService(new OpenAI.OpenAiOptions
        {
            ApiKey = apiKey
        });

        _defaultChatModel = configuration["Integrations:OpenAI:DefaultChatModel"] ?? Models.Gpt_4_turbo;
        _defaultImageModel = configuration["Integrations:OpenAI:DefaultImageModel"] ?? Models.Dall_e_3;

        _logger.LogInformation("OpenAIService inicializado com modelo padrão: {ChatModel}", _defaultChatModel);
    }

    #region Chat Completions

    public async Task<ChatCompletionCreateResponse> CreateChatCompletionAsync(
        List<ChatMessage> messages,
        string? model = null,
        float? temperature = null,
        int? maxTokens = null)
    {
        try
        {
            var request = new ChatCompletionCreateRequest
            {
                Model = model ?? _defaultChatModel,
                Messages = messages,
                Temperature = temperature,
                MaxTokens = maxTokens
            };

            var response = await _client.ChatCompletion.CreateCompletion(request);

            if (!response.Successful)
            {
                _logger.LogError("Erro na API OpenAI: {Error}", response.Error?.Message);
                throw new Exception($"OpenAI Error: {response.Error?.Message}");
            }

            _logger.LogInformation("Chat completion criado. Tokens: {Tokens}", 
                response.Usage?.TotalTokens ?? 0);

            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao criar chat completion");
            throw;
        }
    }

    public async Task<ChatCompletionCreateResponse> ChatAsync(
        string userMessage,
        string? systemMessage = null,
        string? model = null)
    {
        try
        {
            var messages = new List<ChatMessage>();

            if (!string.IsNullOrEmpty(systemMessage))
            {
                messages.Add(ChatMessage.FromSystem(systemMessage));
            }

            messages.Add(ChatMessage.FromUser(userMessage));

            return await CreateChatCompletionAsync(messages, model);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro no chat simples");
            throw;
        }
    }

    #endregion

    #region Images (DALL-E)

    public async Task<ImageCreateResponse> GenerateImageAsync(
        string prompt,
        string? size = null,
        int? numberOfImages = null,
        string? model = null)
    {
        try
        {
            var request = new ImageCreateRequest
            {
                Prompt = prompt,
                Model = model ?? _defaultImageModel,
                N = numberOfImages ?? 1,
                Size = size ?? StaticValues.ImageStatics.Size.Size1024,
                ResponseFormat = StaticValues.ImageStatics.ResponseFormat.Url
            };

            var response = await _client.Image.CreateImage(request);

            if (!response.Successful)
            {
                _logger.LogError("Erro ao gerar imagem: {Error}", response.Error?.Message);
                throw new Exception($"OpenAI Error: {response.Error?.Message}");
            }

            _logger.LogInformation("Imagem gerada: {Count} imagem(ns)", response.Results?.Count ?? 0);

            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao gerar imagem");
            throw;
        }
    }

    public async Task<ImageCreateResponse> EditImageAsync(
        byte[] imageBytes,
        string prompt,
        byte[]? maskBytes = null)
    {
        try
        {
            var request = new ImageEditCreateRequest
            {
                Image = imageBytes,
                Prompt = prompt,
                Mask = maskBytes,
                N = 1,
                Size = StaticValues.ImageStatics.Size.Size1024,
                ResponseFormat = StaticValues.ImageStatics.ResponseFormat.Url
            };

            var response = await _client.Image.CreateImageEdit(request);

            if (!response.Successful)
            {
                _logger.LogError("Erro ao editar imagem: {Error}", response.Error?.Message);
                throw new Exception($"OpenAI Error: {response.Error?.Message}");
            }

            _logger.LogInformation("Imagem editada com sucesso");

            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao editar imagem");
            throw;
        }
    }

    public async Task<ImageCreateResponse> CreateImageVariationAsync(
        byte[] imageBytes,
        int? numberOfVariations = null)
    {
        try
        {
            var request = new ImageVariationCreateRequest
            {
                Image = imageBytes,
                N = numberOfVariations ?? 1,
                Size = StaticValues.ImageStatics.Size.Size1024,
                ResponseFormat = StaticValues.ImageStatics.ResponseFormat.Url
            };

            var response = await _client.Image.CreateImageVariation(request);

            if (!response.Successful)
            {
                _logger.LogError("Erro ao criar variação: {Error}", response.Error?.Message);
                throw new Exception($"OpenAI Error: {response.Error?.Message}");
            }

            _logger.LogInformation("Variações criadas: {Count}", response.Results?.Count ?? 0);

            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao criar variação de imagem");
            throw;
        }
    }

    #endregion

    #region Embeddings

    public async Task<EmbeddingCreateResponse> CreateEmbeddingAsync(string text, string? model = null)
    {
        try
        {
            var request = new EmbeddingCreateRequest
            {
                Model = model ?? Models.TextEmbeddingV3Small,
                InputAsList = new List<string> { text }
            };

            var response = await _client.Embeddings.CreateEmbedding(request);

            if (!response.Successful)
            {
                _logger.LogError("Erro ao criar embedding: {Error}", response.Error?.Message);
                throw new Exception($"OpenAI Error: {response.Error?.Message}");
            }

            _logger.LogInformation("Embedding criado. Dimensões: {Dimensions}", 
                response.Data?.FirstOrDefault()?.Embedding?.Count ?? 0);

            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao criar embedding");
            throw;
        }
    }

    public async Task<EmbeddingCreateResponse> CreateEmbeddingsAsync(List<string> texts, string? model = null)
    {
        try
        {
            var request = new EmbeddingCreateRequest
            {
                Model = model ?? Models.TextEmbeddingV3Small,
                InputAsList = texts
            };

            var response = await _client.Embeddings.CreateEmbedding(request);

            if (!response.Successful)
            {
                _logger.LogError("Erro ao criar embeddings: {Error}", response.Error?.Message);
                throw new Exception($"OpenAI Error: {response.Error?.Message}");
            }

            _logger.LogInformation("Embeddings criados: {Count}", response.Data?.Count ?? 0);

            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao criar embeddings");
            throw;
        }
    }

    #endregion

    #region Moderation

    public async Task<CreateModerationResponse> CreateModerationAsync(string text)
    {
        try
        {
            var request = new CreateModerationRequest
            {
                Input = text
            };

            var response = await _client.Moderation.CreateModeration(request);

            if (!response.Successful)
            {
                _logger.LogError("Erro na moderação: {Error}", response.Error?.Message);
                throw new Exception($"OpenAI Error: {response.Error?.Message}");
            }

            var flagged = response.Results?.FirstOrDefault()?.Flagged ?? false;
            _logger.LogInformation("Moderação concluída. Flagged: {Flagged}", flagged);

            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao moderar texto");
            throw;
        }
    }

    #endregion

    #region Models

    public async Task<List<string>> ListAvailableModelsAsync()
    {
        try
        {
            var response = await _client.Models.ListModel();

            if (!response.Successful)
            {
                _logger.LogError("Erro ao listar modelos: {Error}", response.Error?.Message);
                throw new Exception($"OpenAI Error: {response.Error?.Message}");
            }

            var modelIds = response.Models?.Select(m => m.Id).ToList() ?? new List<string>();
            _logger.LogInformation("Modelos listados: {Count}", modelIds.Count);

            return modelIds;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao listar modelos");
            throw;
        }
    }

    #endregion
}
