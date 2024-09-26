using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace ExampleAssistantAPI.Extensions;

/// <summary>
/// HttpResponseExtension.
/// </summary>
public static class HttpResponseExtension
{
    public static T ResponseBody<T>(this HttpResponseMessage response)
    {
        var responseContent = JObject.Parse(response.Content.ReadAsStringAsync().Result);

        var responseBody = JsonConvert.DeserializeObject<T>(responseContent?.ToString());

        return responseBody;
    }
}
