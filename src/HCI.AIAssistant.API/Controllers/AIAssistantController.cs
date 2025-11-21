using HCI.AIAssistant.API.Models.DTOs;
using HCI.AIAssistant.API.Services;
using Microsoft.AspNetCore.Mvc;
using Unitbv.Assistant.Api.Models.DTOs.AIAssistantController;

namespace Unitbv.Assistant.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AIAssistantController : ControllerBase
{
    private readonly IAIAssistantService _aiAssistantService;

    public AIAssistantController(IAIAssistantService aiAssistantService)
    {
        _aiAssistantService = aiAssistantService;
    }

    [HttpPost("/message")]
    public async Task<ActionResult<AIAssistantControllerPostMessageResponseDTO>> PostMessage([FromBody] AIAssistantControllerPostMessageRequestDTO request)
    {
        try
        {
            var responseText = await _aiAssistantService.GetResponse(request.TextMessage);

            AIAssistantControllerPostMessageResponseDTO response = new()
            {
                TextMessage = responseText
            };

            return Ok(response);
        }
        catch (Exception ex)
        {
            var errorResponse = new ErrorResponseDTO
            {
                TextErrorTitle = "Error",
                TextErrorMessage = ex.Message,
                TextErrorTrace = ex.StackTrace ?? string.Empty
            };

            return StatusCode(500, errorResponse);
        }
    }
}