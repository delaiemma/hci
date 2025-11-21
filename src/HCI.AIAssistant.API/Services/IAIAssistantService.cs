using Azure.AI.OpenAI.Assistants;

namespace HCI.AIAssistant.API.Services;

public interface IAIAssistantService
{
    Task<string> GetResponse(string userMessage);
}
