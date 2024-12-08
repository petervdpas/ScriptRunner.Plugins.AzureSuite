/*
{
    "TaskCategory": "Azure",
    "TaskName": "KeyVault Dynamic Interaction",
    "TaskDetail": "Dynamically interact with Azure Key Vault for secret retrieval and updates"
}
*/

// Step 1: Initialize Key Vault dynamically
var realm = "local";
var keyVaultUrl = await GetSettingPickerAsync(realm, 460, 130);

if (string.IsNullOrWhiteSpace(keyVaultUrl))
{
    Dump("Key Vault URL is not valid!");
    return -1;
}

// Initialize the AzureKeyVault instance
var keyVault = new AzureKeyVault(new PluginLogger());
await keyVault.InitializeKeyVaultAsync(keyVaultUrl);
Dump($"Connected to Key Vault: {keyVaultUrl}");

// Step 2: List available secrets
var secretNames = await keyVault.ListSecretsAsync();
if (!secretNames.Any())
{
    Dump("No secrets found in the Key Vault.");
    return -1;
}

// Step 3: Allow the user to select a secret to retrieve
var selectedSecret = await GetComboSelectionAsync(
   "Select secret",
   "Select a secret to retrieve:",
   secretNames,
   500,
   130
);

if (string.IsNullOrWhiteSpace(selectedSecret))
{
    Dump("No secret selected.");
    return -1;
}

// Step 4: Retrieve the selected secret's value
var retrievedValue = await keyVault.GetSecretAsync(selectedSecret);
Dump($"Retrieved secret '{selectedSecret}': {retrievedValue}");

bool? makeNew = await GetUserDecisionAsync(
        "Make a new secret...", "Do you want create a new secret?", 500, 150);

if (makeNew == true)
{
    // Optionally allow setting a new secret
    var newSecretName = await GetUserInputAsync(
        "Enter a name for a new secret (or press Enter to skip):", 
        "New Secret Name",
        500, 
        150);
    
    if (!string.IsNullOrWhiteSpace(newSecretName))
    {
        var newSecretValue = await GetMaskedUserInputAsync(
            "Enter the value for the new secret:", 
            "New Secret Value",
            500,
            150);
        await keyVault.SetSecretAsync(newSecretName, newSecretValue);
        Dump($"New secret '{newSecretName}' set with value: {newSecretValue}");
    }
}
else
{
    Dump("Exiting...");
}

return "Key Vault interaction completed successfully.";