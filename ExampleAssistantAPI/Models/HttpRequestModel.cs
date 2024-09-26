namespace ExampleAssistantAPI.Models;


/// <summary>
/// Helper'a istek atarken göndereceğimiz modele ait sınıf bilgisidir.
/// </summary>
public class HttpRequestModel
{
    public string BaseAddress { get; set; } = string.Empty;
    public string RequestUrl { get; set; } = string.Empty;
    public string AccessToken { get; set; } = string.Empty;
    public HttpMethod HttpMethod { get; set; }
    public string RequestContent { get; set; }
}
