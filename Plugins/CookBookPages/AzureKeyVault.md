---
Title: The Azure Key Vault Client
Subtitle: Manage and Interact with Secrets Using the Azure Key Vault Plugin
Category: Cookbook
Author: Peter van de Pas
Keywords: [CookBook, Azure, KeyVault, Secrets]
Table-use-row-colors: true
Table-row-color: "D3D3D3"
Toc: true
Toc-title: Table of Contents
Toc-own-page: true
---

# Recipe: The Azure Key Vault Client

## Goal

Leverage the **Azure Key Vault Plugin** to interact dynamically with Azure Key Vault for secret management. Learn how to initialize the Key Vault client, retrieve and set secrets, and perform other operations like listing and deleting secrets.

---

## Steps

### Step 1: Initialize the Azure Key Vault

Connect to your Azure Key Vault by providing its URL:

```csharp
var keyVaultUrl = "https://your-key-vault-name.vault.azure.net";
var keyVault = new AzureKeyVault(new PluginLogger());

// Initialize the Key Vault client
await keyVault.InitializeKeyVaultAsync(keyVaultUrl);
Dump($"Connected to Key Vault: {keyVaultUrl}");
```

---

### Step 2: List All Secrets

Retrieve a list of all secret names stored in the Key Vault:

```csharp
Dump("Fetching secret names...");
var secretNames = await keyVault.ListSecretsAsync();

if (!secretNames.Any())
{
    Dump("No secrets found in the Key Vault.");
    return;
}

foreach (var secretName in secretNames)
{
    Dump($"Secret: {secretName}");
}
```

---

### Step 3: Retrieve a Secret

Fetch the value of a specific secret by its name:

```csharp
var secretName = "example-secret-name";
Dump($"Retrieving secret: {secretName}...");
var secretValue = await keyVault.GetSecretAsync(secretName);

Dump($"Value of secret '{secretName}': {secretValue}");
```

---

### Step 4: Set a New Secret

Add or update a secret in the Key Vault:

```csharp
var newSecretName = "new-example-secret";
var newSecretValue = "example-secret-value";

Dump($"Setting new secret '{newSecretName}'...");
await keyVault.SetSecretAsync(newSecretName, newSecretValue);
Dump($"Secret '{newSecretName}' set successfully.");
```

---

### Step 5: Delete a Secret

Soft-delete a secret from the Key Vault:

```csharp
var secretToDelete = "example-secret-name";

Dump($"Deleting secret: {secretToDelete}...");
await keyVault.DeleteSecretAsync(secretToDelete);
Dump($"Secret '{secretToDelete}' deleted successfully.");
```

---

### Step 6: Recover a Deleted Secret

Recover a soft-deleted secret, if supported by your Key Vault:

```csharp
var secretToRecover = "example-secret-name";

Dump($"Recovering secret: {secretToRecover}...");
await keyVault.RecoverDeletedSecretAsync(secretToRecover);
Dump($"Secret '{secretToRecover}' recovered successfully.");
```

---

### Step 7: Permanently Purge a Secret

Permanently delete a secret that has been soft-deleted:

```csharp
var secretToPurge = "example-secret-name";

Dump($"Purging secret: {secretToPurge}...");
await keyVault.PurgeSecretAsync(secretToPurge);
Dump($"Secret '{secretToPurge}' purged permanently.");
```

---

## Summary

The **Azure Key Vault Plugin** simplifies secret management for applications. With its rich API, you can seamlessly interact with secrets stored in Azure Key Vault, enabling dynamic and secure handling of sensitive information.
