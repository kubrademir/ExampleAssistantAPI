using ExampleAssistantAPI.Constants;
using ExampleAssistantAPI.Models;
using ExampleAssistantAPI.Services;
using MediatR;

namespace ExampleAssistantAPI.QueryHandlers;

/// <summary>
/// Servise iletilen prompt'u OpenAI servisleri ile işleyerek geriye yemek tarifi bilgisi ile yanıt döner.
/// </summary>
public class GenerateRecipeQueryHandler : IRequestHandler<GenerateRecipeQuery, GenerateRecipeQueryResult>
{
    private readonly IOpenAIService openAIService;
    private readonly IConfiguration configuration;

    public GenerateRecipeQueryHandler(IOpenAIService openAIService, IConfiguration configuration)
    {
        this.openAIService = openAIService;
        this.configuration = configuration;
    }

    public async Task<GenerateRecipeQueryResult> Handle(GenerateRecipeQuery request, CancellationToken cancellationToken)
    {
        try
        {
            // assistant oluşturuluyor.
            // burası bir kereye mahsus yapılmalıdır. Bir assistant yaratıldıktan sonra silinene kadar kullanılabilir durumdadır.
            // örnek olması açısından bu adım eklenmiştir.
            var createAssistantResult = await this.openAIService.CreateAssistant().ConfigureAwait(false);
            var assistantId = createAssistantResult.Id;
            if (string.IsNullOrEmpty(assistantId))
            {
                return new GenerateRecipeQueryResult();
            }

            // thread oluşturuluyor.
            var createThreadResult = await this.openAIService.CreateThread().ConfigureAwait(false);
            var threadId = createThreadResult?.Id;

            if (string.IsNullOrEmpty(threadId))
            {
                return new GenerateRecipeQueryResult();
            }

            // Thread'e mesaj ekleniyor.
            var createMessageToThreadResult = await this.openAIService.CreateMessageToThread(request.Prompt, threadId).ConfigureAwait(false);
            if (string.IsNullOrEmpty(createMessageToThreadResult))
            {
                return new GenerateRecipeQueryResult();
            }

            // Eklenen mesaj, yemek tarifi üretmesi için oluşturulan assistant'a gönderiliyor.
            var runThreadResultId = await this.openAIService.RunThreadByAssistantId(threadId, assistantId).ConfigureAwait(false);
            if (string.IsNullOrEmpty(runThreadResultId))
            {
                return new GenerateRecipeQueryResult();
            }

            // Gönderilen mesajın durumu kontrol ediliyor ve cevap okunuyor.
            // Bu kısım real-time/stream ile olacak şekilde de düzenlenebilir.
            // Örnek olması açısından bir kez çağrılacak şekilde süreç işletilmiştir.
            var serviceCallCount = this.configuration.GetSection("OpenAIApi:GetThreadRunStatusServiceCallCount").Get<int>();
            GetRunningThreadStatusServiceResultDto? runningThreadStatusResult = null;
            for (int i = 0; i <= serviceCallCount; i++)
            {
                runningThreadStatusResult = await this.openAIService.GetRunningThreadStatus(threadId, runThreadResultId).ConfigureAwait(false);
                if (runningThreadStatusResult?.Status == OpenAIThreadRunStatuConstants.Completed)
                {
                    break;
                }
            }

            // işlem tamamlanmamış ise default response set ediliyor.
            if (runningThreadStatusResult == null || runningThreadStatusResult.Status != OpenAIThreadRunStatuConstants.Completed)
            {
                return new GenerateRecipeQueryResult();
            }

            // thread'e eklenen mesaj listesi getiriliyor.
            var messages = await this.openAIService.GetMessagesByThreadId(threadId).ConfigureAwait(false);

            // örnek olması açısından oluşturulan son mesaj bilgisi alınıyor ve response içerisine set ediliyor.
            var lastItem = messages?.Data?.OrderByDescending(x => x.CreatedAt)?.FirstOrDefault();
            return new GenerateRecipeQueryResult
            {
                Content = lastItem?.Content?.FirstOrDefault()?.Text?.Value ?? string.Empty,
            };
        }
        catch (Exception ex)
        {
            // hata loglanabilir.
            return new GenerateRecipeQueryResult();
        }
    }
}
