using HCI.AIAssistant.API.Models.CustomTypes;

namespace HCI.AIAssistant.API.Services;

public interface ISecretsService
{
    AIAssistantSecrets? AIAssistantSecrets { get; set; }
    IoTHubSecrets? IoTHubSecrets { get; set; }
}
