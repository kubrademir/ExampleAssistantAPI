using System.Net.Http.Headers;
using System.Text;
using ExampleAssistantAPI.Models;

namespace ExampleAssistantAPI.Helpers;

/// <summary>
/// HttpRequestHelper.
/// </summary>
public class HttpRequestHelper
{
    private readonly IHttpClientFactory httpClientFactory;

    public HttpRequestHelper(IHttpClientFactory httpClientFactory)
    {
        this.httpClientFactory = httpClientFactory;
    }


    public async Task<HttpResponseMessage> SendWithAccessTokenAsync(HttpRequestModel httpRequestModel)
    {
        using var httpClient = this.httpClientFactory.CreateClient();

        httpClient.BaseAddress = new Uri(httpRequestModel.BaseAddress);

        using var request = new HttpRequestMessage(httpRequestModel.HttpMethod, httpRequestModel.RequestUrl);

        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        request.Headers.Add("authorization", httpRequestModel.AccessToken);
        request.Headers.Add("OpenAI-Beta", "assistants=v2");

        if (!string.IsNullOrEmpty(httpRequestModel.RequestContent))
        {
            request.Content = new StringContent(httpRequestModel.RequestContent, Encoding.UTF8, "application/json");
        }

        var response = await httpClient.SendAsync(request).ConfigureAwait(false);

        return response;
    }
}
