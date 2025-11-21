namespace HCI.AIAssistant.API.Services;

public interface IParametricFunctions
{
    string GetOrThrow(string? value, string parameterName);
}
