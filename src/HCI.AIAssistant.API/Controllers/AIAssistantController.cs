using Microsoft.AspNetCore.Mvc;

using HCI.AIAssistant.API.Models.DTOs.AIAssistantController;
using HCI.AIAssistant.API.Services;
using HCI.AIAssistant.API.Models.DTOs;
using Newtonsoft.Json;
using Microsoft.Azure.Devices;
using System.Text;

namespace HCI.AIAssistant.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AIAssistantController : ControllerBase
{
    private readonly ISecretsService _secretsService;
    private readonly IAppConfigurationsService _appConfigurationsService;
    private readonly IAIAssistantService _aIAssistantService;

    public AIAssistantController(
        ISecretsService secretsService,
        IAppConfigurationsService appConfigurationsService,
        IAIAssistantService aIAssistantService
    )
    {
        _secretsService = secretsService;
        _appConfigurationsService = appConfigurationsService;
        _aIAssistantService = aIAssistantService;
    }

    [HttpPost("message")]
    [ProducesResponseType(typeof(AIAssistantControllerPostMessageResponseDTO), 200)]
    [ProducesResponseType(typeof(ErrorResponseDTO), 400)]
    public async Task<ActionResult> PostMessage([FromBody] AIAssistantControllerPostMessageRequestDTO request)
    {
        if (request?.TextMessage == null)
        {
            return BadRequest(
                new ErrorResponseDTO()
                {
                    TextErrorTitle = "AtLeastOneNullParameter",
                    TextErrorMessage = "Some parameters are null/missing.",
                    TextErrorTrace = "AIAssistantController.PostMessage"
                }
            );
        }

        string textMessageResponse = await _aIAssistantService.GetResponse(request.TextMessage);

        AIAssistantControllerPostMessageResponseDTO response = new()
        {
            TextMessage = textMessageResponse
        };

        string? ioTHubConnectionString = _secretsService?.IoTHubSecrets?.ConnectionString;
        if (ioTHubConnectionString != null)
        {
            var serviceClientForIoTHub = ServiceClient.CreateFromConnectionString(ioTHubConnectionString);
            var seralizedMessage = JsonConvert.SerializeObject(textMessageResponse);

            var ioTMessage = new Message(Encoding.UTF8.GetBytes(seralizedMessage));
            await serviceClientForIoTHub.SendAsync(_appConfigurationsService.IoTDeviceName, ioTMessage);
        }

        return Ok(response);
    }
}