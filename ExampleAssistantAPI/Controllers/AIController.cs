using System.Diagnostics.Contracts;
using ExampleAssistantAPI.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ExampleAssistantAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class AIController : ControllerBase
{
    private readonly IMediator _mediator;

    public AIController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Girilen prompta ait yemek tarifi önerisi üretme süreci işletilir. 
    /// </summary>
    /// <param name="request">GenerateRecipeQueryDto model bilgisini alır.</param>
    /// <param name="cancellationToken">cancellationToken</param>
    /// <returns>GenerateRecipeQueryResult model bilgisini döner.</returns>
    [HttpPost("generate-recipe")]
    public async Task<GenerateRecipeQueryResult> GenerateRecipe([FromBody] GenerateRecipeQueryDto request, CancellationToken cancellationToken)
    {
        return await _mediator.Send(new GenerateRecipeQuery(request.Prompt), cancellationToken).ConfigureAwait(false);
    }
}
