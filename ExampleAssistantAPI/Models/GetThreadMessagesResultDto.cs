using Newtonsoft.Json;

namespace ExampleAssistantAPI.Models;

/// <summary>
/// Thread'e eklenen mesaj listesini getirme servisine ait result dto model bilgisidir.
/// </summary>
public class GetThreadMessagesResultDto
{
    public List<MessageData> Data { get; set; }
}

/// <summary>
/// MessageData.
/// </summary>
public class MessageData
{
    [JsonProperty("created_at")]
    public int CreatedAt { get; set; }

    public List<MessageDataContent> Content { get; set; }
}

/// <summary>
/// MessageDataContent.
/// </summary>
public class MessageDataContent
{
    public string Type { get; set; }
    public MessageDataContentText Text { get; set; }
}

/// <summary>
/// MessageDataContentText.
/// </summary>
public class MessageDataContentText
{
    public string Value { get; set; }
}
