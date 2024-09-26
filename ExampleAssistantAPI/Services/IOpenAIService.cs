using ExampleAssistantAPI.Models;

namespace ExampleAssistantAPI.Services;

/// <summary>
/// OpenAI'a ait servisleri çağırırken kullanacağımız interface bilgisidir.
/// </summary>
public interface IOpenAIService
{
    /// <summary>
    /// OpenAI tarafında Assistant oluşturulmasını sağlar.
    /// </summary>
    /// <returns>CreateAssistantServiceResultDto model bilgisini döner.</returns>
    Task<CreateAssistantServiceResultDto> CreateAssistant();

    /// <summary>
    /// OpenAI tarafında thread oluşturulmasını sağlar.
    /// </summary>
    /// <returns>CreateThreadServiceResultDto model bilgisini döner.</returns>
    Task<CreateThreadServiceResultDto> CreateThread();

    /// <summary>
    /// Thread'e mesaj eklenmesini sağlar.
    /// </summary>
    /// <param name="content">Mesaj bilgisidir.</param>
    /// <param name="threadId">Thread bilgisidir.</param>
    /// <returns>işlemin başarılı olup/olmadığı bilgisini döner.</returns>
    Task<string> CreateMessageToThread(string content, string threadId);

    /// <summary>
    /// Thread'in assistant api ile çalışmasını tetikler.
    /// </summary>
    /// <param name="threadId">Thread bilgisidir.</param>
    /// <param name="assistantId">Oluşturulan Assistant'a ait id bilgisidir.</param>
    /// <returns>OpenAI'ın kuyruğa eklediği veriye ait run_id değerini döner.</returns>
    Task<string> RunThreadByAssistantId(string threadId, string assistantId);

    /// <summary>
    /// Thread'e eklenen mesajın durumunu sorgular ve servis yanıtını getirir.
    /// </summary>
    /// <param name="threadId">Thread bilgisidir.</param>
    /// <param name="runId">Thread'in assistant api ile çalıştırıldığı eşşiz id bilgisidir.</param>
    /// <returns>GetRunningThreadStatusServiceResultDto model bilgisini döner.</returns>
    Task<GetRunningThreadStatusServiceResultDto> GetRunningThreadStatus(string threadId, string runId);

    /// <summary>
    /// Thread'e eklenen mesaj listesini getirir.
    /// </summary>
    /// <param name="threadId">threadId</param>
    /// <returns>GetThreadMessagesResultDto model bilgisini döner.</returns>
    Task<GetThreadMessagesResultDto> GetMessagesByThreadId(string threadId);
}

