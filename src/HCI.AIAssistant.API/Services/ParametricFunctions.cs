namespace HCI.AIAssistant.API.Services;

public class ParametricFunctions : IParametricFunctions
{
    public string GetOrThrow(string? value, string parameterName)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentNullException(parameterName, $"{parameterName} is missing.");
        }

        return value;
    }
}
