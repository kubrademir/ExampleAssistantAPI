namespace ExampleAssistantAPI.Models;

/// <summary>
/// /{0}/runs/{1} servisine ait result dto model bilgisidir.
/// </summary>
public class GetRunningThreadStatusServiceResultDto
{
    public string Id { get; set; }

    public string Status { get; set; }
}
