using MediatR;

namespace ExampleAssistantAPI.Models;

/// <summary>
/// GenerateRecipeQuery.
/// </summary>
public class GenerateRecipeQuery : IRequest<GenerateRecipeQueryResult>
{
    public GenerateRecipeQuery(string prompt)
    {
        this.Prompt = prompt;
    }

    public string Prompt { get; set; }
}
