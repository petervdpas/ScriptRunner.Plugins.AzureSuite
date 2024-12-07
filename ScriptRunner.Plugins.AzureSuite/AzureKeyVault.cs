using System;
using System.Threading.Tasks;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using ScriptRunner.Plugins.AzureSuite.Interfaces;

namespace ScriptRunner.Plugins.AzureSuite;

/// <summary>
///     A helper class to connect to Azure Key Vault and retrieve secrets.
///     Implements <see cref="IAzureKeyVault" />.
/// </summary>
public class AzureKeyVault : IAzureKeyVault
{
    private static SecretClient? _secretClient;
    private readonly IPluginLogger _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="AzureKeyVault"/> class with the specified logger.
    /// </summary>
    /// <param name="logger">
    /// The <see cref="IPluginLogger"/> instance used for logging messages within the Azure Key Vault operations.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// Thrown when the <paramref name="logger"/> parameter is <c>null</c>.
    /// </exception>
    public AzureKeyVault(IPluginLogger logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }
    
    /// <summary>
    ///     Initializes the Key Vault connection.
    /// </summary>
    /// <param name="vaultUrl">The URL of the Azure Key Vault.</param>
    public Task InitializeKeyVaultAsync(string vaultUrl)
    {
        try
        {
            var credential = new DefaultAzureCredential();
            _secretClient = new SecretClient(new Uri(vaultUrl), credential);
            _logger.Debug($"Key Vault client initialized for {vaultUrl}");
        }
        catch (Exception ex)
        {
            _logger.Error($"Error initializing Key Vault client: {ex.Message}");
            throw;
        }

        return Task.CompletedTask;
    }

    /// <summary>
    ///     Retrieves a secret from Azure Key Vault by its name.
    /// </summary>
    /// <param name="secretName">The name of the secret in the Key Vault.</param>
    /// <returns>The secret value.</returns>
    public async Task<string> GetSecretAsync(string secretName)
    {
        if (_secretClient == null)
            throw new InvalidOperationException(
                "Key Vault client is not initialized. Call InitializeKeyVaultAsync first.");

        try
        {
            KeyVaultSecret secret = await _secretClient.GetSecretAsync(secretName);
            return secret.Value;
        }
        catch (Exception ex)
        {
            _logger.Error($"Error retrieving secret {secretName}: {ex.Message}");
            throw;
        }
    }

    /// <summary>
    ///     Sets or updates a secret in Azure Key Vault.
    /// </summary>
    /// <param name="secretName">The name of the secret to set or update.</param>
    /// <param name="secretValue">The value of the secret.</param>
    public async Task SetSecretAsync(string secretName, string secretValue)
    {
        if (_secretClient == null)
            throw new InvalidOperationException(
                "Key Vault client is not initialized. Call InitializeKeyVaultAsync first.");

        try
        {
            await _secretClient.SetSecretAsync(secretName, secretValue);
            _logger.Debug($"Secret {secretName} set successfully.");
        }
        catch (Exception ex)
        {
            _logger.Error($"Error setting secret {secretName}: {ex.Message}");
            throw;
        }
    }

    /// <summary>
    ///     Deletes a secret from Azure Key Vault (soft delete).
    /// </summary>
    /// <param name="secretName">The name of the secret to delete.</param>
    public async Task DeleteSecretAsync(string secretName)
    {
        if (_secretClient == null)
            throw new InvalidOperationException(
                "Key Vault client is not initialized. Call InitializeKeyVaultAsync first.");

        try
        {
            var operation = await _secretClient.StartDeleteSecretAsync(secretName);
            await operation.WaitForCompletionAsync();
            _logger.Debug($"Secret {secretName} deleted and in soft-delete state.");
        }
        catch (Exception ex)
        {
            _logger.Error($"Error deleting secret {secretName}: {ex.Message}");
            throw;
        }
    }

    /// <summary>
    ///     Purges a deleted secret from Azure Key Vault (permanently deletes).
    /// </summary>
    /// <param name="secretName">The name of the secret to purge.</param>
    public async Task PurgeSecretAsync(string secretName)
    {
        if (_secretClient == null)
            throw new InvalidOperationException(
                "Key Vault client is not initialized. Call InitializeKeyVaultAsync first.");

        try
        {
            await _secretClient.PurgeDeletedSecretAsync(secretName);
            _logger.Debug($"Secret {secretName} purged and permanently deleted.");
        }
        catch (Exception ex)
        {
            _logger.Error($"Error purging secret {secretName}: {ex.Message}");
            throw;
        }
    }

    /// <summary>
    ///     Recovers a deleted secret from Azure Key Vault.
    /// </summary>
    /// <param name="secretName">The name of the secret to recover.</param>
    public async Task RecoverDeletedSecretAsync(string secretName)
    {
        if (_secretClient == null)
            throw new InvalidOperationException(
                "Key Vault client is not initialized. Call InitializeKeyVaultAsync first.");

        try
        {
            await _secretClient.StartRecoverDeletedSecretAsync(secretName);
            _logger.Debug($"Secret {secretName} has been recovered.");
        }
        catch (Exception ex)
        {
            _logger.Error($"Error recovering secret {secretName}: {ex.Message}");
            throw;
        }
    }
}