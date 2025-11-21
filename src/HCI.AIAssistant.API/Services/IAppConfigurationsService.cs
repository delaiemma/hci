namespace HCI.AIAssistant.API.Services;

public interface IAppConfigurationsService
{
    string? KeyVaultName { get; set; }
    string? SecretsPrefix { get; set; }
    string? IoTDeviceName { get; set; }
}
