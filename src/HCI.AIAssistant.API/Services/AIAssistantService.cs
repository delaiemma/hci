using Azure;
using Azure.AI.OpenAI.Assistants;

namespace HCI.AIAssistant.API.Services;

public class AIAssistantService : IAIAssistantService
{
    private readonly AssistantsClient _client;
    private readonly string _assistantId;

    public AIAssistantService(ISecretsService secretsService, IParametricFunctions parametricFunctions)
    {
        var endpoint = parametricFunctions.GetOrThrow(secretsService.AIAssistantSecrets?.EndPoint, "EndPoint");
        var key = parametricFunctions.GetOrThrow(secretsService.AIAssistantSecrets?.Key, "Key");
        _assistantId = parametricFunctions.GetOrThrow(secretsService.AIAssistantSecrets?.Id, "Id");

        _client = new AssistantsClient(new Uri(endpoint), new AzureKeyCredential(key));
    }

    public async Task<string> GetResponse(string userMessage)
    {
        var thread = await _client.CreateThreadAsync();
        await _client.CreateMessageAsync(thread.Value.Id, MessageRole.User, userMessage);
        var run = await _client.CreateRunAsync(thread.Value.Id, new CreateRunOptions(_assistantId));

        while (run.Value.Status == RunStatus.Queued || run.Value.Status == RunStatus.InProgress)
        {
            await Task.Delay(TimeSpan.FromMilliseconds(500));
            run = await _client.GetRunAsync(thread.Value.Id, run.Value.Id);
        }

        var messagesResponse = await _client.GetMessagesAsync(thread.Value.Id);
        var messages = messagesResponse.Value.Data;

        var lastMessage = messages.FirstOrDefault(m => m.Role == MessageRole.Assistant);
        if (lastMessage != null)
        {
            var content = lastMessage.ContentItems.FirstOrDefault();
            if (content is MessageTextContent textContent)
            {
                return textContent.Text;
            }
        }

        return "No response from assistant.";
    }
}
