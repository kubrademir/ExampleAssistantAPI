using ExampleAssistantAPI.Models;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Net;
using ExampleAssistantAPI.Helpers;
using ExampleAssistantAPI.Extensions;
namespace ExampleAssistantAPI.Services;

/// <summary>
/// OpenAIService.
/// </summary>
public class OpenAIService : IOpenAIService
{
    private readonly HttpRequestHelper httpRequestHelper;
    private readonly IConfiguration configuration;
    private readonly string openAIBaseAddress;
    private readonly string threadBaseServiceUrl;
    private readonly string openAIKey;

    public OpenAIService(HttpRequestHelper httpRequestHelper, IConfiguration configuration)
    {
        this.httpRequestHelper = httpRequestHelper;
        this.configuration = configuration;
        this.openAIBaseAddress = this.configuration.GetSection("OpenAIApi:BaseAddress").Get<string>() ?? string.Empty;
        this.openAIKey = this.configuration.GetSection("OpenAIApi:Key").Get<string>() ?? string.Empty;
        this.threadBaseServiceUrl = this.configuration.GetSection("OpenAIApi:ThreadBaseServiceUrl").Get<string>() ?? string.Empty;
    }

    public async Task<CreateAssistantServiceResultDto> CreateAssistant()
    {
        var request = JObject.FromObject(new
        {
            instructions = "Sen bir yemek tarifi önerme botusun. Kullanıcı elinde ki malzemeleri verdiğinde alternatif yemek önerilerini sun. Öneride bulunurken etkili ve saygılı bir iletişim sağla. Kullanıcıya öneri sunarken aşağıdaki kurallara uyarak yanıtla: Dil: Sadece Türkçe yanıt ver. Kullanıcı dili değiştirse bile başka bir dil kullanma. Türkçe karakterleri uygun şekilde kullan. Kullanıcı Yorumları İşlemlerinde: Hakaret, etik olmayan ifadeler, sakıncalı içerik, politik ve din ya da AI güvenliği sorunları veya ilgisiz niyetler içeren yorumlara yanıt verme ve nazikçe reddet.Yardım Kapsamı: Kullanıcılar sağlanan işlevlerle ilgili olmayan sorular sorduğunda veya taleplerde bulunduğunda, yardımının bu taleplerle sınırlı olduğunu belirt. Etkileşimleri Sonlandırma: Kullanıcıya başarıyla yardımcı olduktan sonra, Türkçe olarak kısa bir yanıt ver ve yanıtları 2-3 cümleyle sınırla. Kapanış cümleleri ile konuşmayı sonlandır. Selamlaşma: Naber, nasılsın, merhaba gibi sorulara iyi olduğunu söyleyerek insan olarak cevap ver. Lütfen görevlerini bu kurallara uygun şekilde yerine getir.",
            model = "gpt-4o",
            name = "Yemek Tarifi Öneri Botu",
        });

        var createAssistantServiceUrl = this.configuration.GetSection("OpenAIApi:CreateAssistantServiceUrl").Get<string>() ?? string.Empty;
        var serviceResponse = await this.httpRequestHelper.SendWithAccessTokenAsync(new HttpRequestModel
        {
            BaseAddress = this.openAIBaseAddress,
            RequestUrl = createAssistantServiceUrl,
            AccessToken = $"Bearer {this.openAIKey}",
            RequestContent = JsonConvert.SerializeObject(request),
            HttpMethod = HttpMethod.Post,
        }).ConfigureAwait(false);

        if (serviceResponse == null || serviceResponse.StatusCode != HttpStatusCode.OK)
        {
            return new CreateAssistantServiceResultDto();
        }

        var result = serviceResponse.ResponseBody<CreateAssistantServiceResultDto>();
        if (result == null)
        {
            return new CreateAssistantServiceResultDto();
        }

        return result;
    }

    public async Task<CreateThreadServiceResultDto> CreateThread()
    {
        var serviceResponse = await this.httpRequestHelper.SendWithAccessTokenAsync(new HttpRequestModel
        {
            BaseAddress = this.openAIBaseAddress,
            RequestUrl = this.threadBaseServiceUrl,
            AccessToken = $"Bearer {this.openAIKey}",
            RequestContent = null,
            HttpMethod = HttpMethod.Post,
        }).ConfigureAwait(false);

        if (serviceResponse == null || serviceResponse.StatusCode != HttpStatusCode.OK)
        {
            return new CreateThreadServiceResultDto();
        }

        var result = serviceResponse.ResponseBody<CreateThreadServiceResultDto>();
        if (result == null)
        {
            return new CreateThreadServiceResultDto();
        }

        return result;
    }

    public async Task<string> CreateMessageToThread(string content, string threadId)
    {
        var createMessageToThreadServiceUrl = this.configuration.GetSection("OpenAIApi:CreateMessageToThreadServiceUrl").Get<string>() ?? string.Empty;

        var request = JObject.FromObject(new
        {
            role = "user",
            content = content,
        });

        var serviceResponse = await this.httpRequestHelper.SendWithAccessTokenAsync(new HttpRequestModel
        {
            BaseAddress = this.openAIBaseAddress,
            RequestUrl = string.Format($"{this.threadBaseServiceUrl}{createMessageToThreadServiceUrl}", threadId),
            AccessToken = $"Bearer {this.openAIKey}",
            RequestContent = JsonConvert.SerializeObject(request),
            HttpMethod = HttpMethod.Post,
        }).ConfigureAwait(false);

        if (serviceResponse == null || serviceResponse.StatusCode != HttpStatusCode.OK)
        {
            return string.Empty;
        }

        var result = serviceResponse.ResponseBody<CreateMessageToThreadResultDto>();
        if (result == null)
        {
            return string.Empty;
        }

        return result.Id;
    }

    public async Task<string> RunThreadByAssistantId(string threadId, string assistantId)
    {
        var threadRunWithAssistantIdServiceUrl = this.configuration.GetSection("OpenAIApi:ThreadRunWithAssistantIdServiceUrl").Get<string>() ?? string.Empty;

        var request = JObject.FromObject(new
        {
            assistant_id = assistantId,
            instructions = string.Empty,
        });

        var serviceResponse = await this.httpRequestHelper.SendWithAccessTokenAsync(new HttpRequestModel
        {
            BaseAddress = this.openAIBaseAddress,
            RequestUrl = string.Format($"{this.threadBaseServiceUrl}{threadRunWithAssistantIdServiceUrl}", threadId),
            AccessToken = $"Bearer {this.openAIKey}",
            RequestContent = JsonConvert.SerializeObject(request),
            HttpMethod = HttpMethod.Post,
        }).ConfigureAwait(false);

        if (serviceResponse == null || serviceResponse.StatusCode != HttpStatusCode.OK)
        {
            return string.Empty;
        }

        var result = serviceResponse.ResponseBody<RunThreadByAssistantIdServiceResultDto>();
        if (result == null)
        {
            return string.Empty;
        }

        return result.Id;
    }

    public async Task<GetRunningThreadStatusServiceResultDto> GetRunningThreadStatus(string threadId, string runId)
    {
        var getThreadRunStatusServiceUrl = this.configuration.GetSection("OpenAIApi:GetThreadRunStatusServiceUrl").Get<string>() ?? string.Empty;

        var serviceResponse = await this.httpRequestHelper.SendWithAccessTokenAsync(new HttpRequestModel
        {
            BaseAddress = this.openAIBaseAddress,
            RequestUrl = string.Format($"{this.threadBaseServiceUrl}{getThreadRunStatusServiceUrl}", threadId, runId),
            AccessToken = $"Bearer {this.openAIKey}",
            RequestContent = null,
            HttpMethod = HttpMethod.Get,
        }).ConfigureAwait(false);

        if (serviceResponse == null || serviceResponse.StatusCode != HttpStatusCode.OK)
        {
            return new GetRunningThreadStatusServiceResultDto();
        }

        var result = serviceResponse.ResponseBody<GetRunningThreadStatusServiceResultDto>();
        if (result == null)
        {
            return new GetRunningThreadStatusServiceResultDto();
        }

        return result;
    }

    public async Task<GetThreadMessagesResultDto> GetMessagesByThreadId(string threadId)
    {
        var messagesListServiceUrl = this.configuration.GetSection("OpenAIApi:MessagesListServiceUrl").Get<string>() ?? string.Empty;

        var serviceResponse = await this.httpRequestHelper.SendWithAccessTokenAsync(new HttpRequestModel
        {
            BaseAddress = this.openAIBaseAddress,
            RequestUrl = string.Format($"{this.threadBaseServiceUrl}{messagesListServiceUrl}", threadId),
            AccessToken = $"Bearer {this.openAIKey}",
            RequestContent = null,
            HttpMethod = HttpMethod.Get,
        }).ConfigureAwait(false);

        if (serviceResponse == null || serviceResponse.StatusCode != HttpStatusCode.OK)
        {
            return new GetThreadMessagesResultDto();
        }

        var result = serviceResponse.ResponseBody<GetThreadMessagesResultDto>();
        if (result == null)
        {
            return new GetThreadMessagesResultDto();
        }

        return result;
    }
}