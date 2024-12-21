# Azure Resource Client Plugin

![License](https://img.shields.io/badge/license-MIT-green)  
![Version](https://img.shields.io/badge/version-1.0.0-blue)

The **Azure Resource Client Plugin** is a powerful extension for [ScriptRunner](https://github.com/petervdpas/ScriptRunner) that allows you to interact seamlessly with Azure resources. Using the Azure Resource Manager (ARM) SDK, this plugin provides a variety of features to list, filter, and manage resources in your Azure subscriptions.

---

## Features

- **Subscription Management**: List all Azure subscriptions and set a subscription context.
- **Resource Group Management**: Fetch resource groups by tags or other attributes.
- **Resource Queries**: Query resources by type, tag, or resource group.
- **Export to JSON**: Save resource details to a JSON file for analysis or backup.
- **Location Analysis**: Count resources by location.
- **Resource Providers**: List all available resource providers in a subscription.
- **Validation**: Check if a resource exists in a resource group.

---

## Installation

1. Clone this repository or download the latest release.
2. Place the plugin assembly into the `Plugins` folder of your ScriptRunner installation.
3. Restart ScriptRunner to activate the plugin.

---

## Usage

Here’s an example of how to use the Azure Resource Client Plugin to fetch and display Azure resources:

### Fetch Subscriptions and List Resources in a Resource Group

```csharp
/*
{
    "TaskCategory": "Azure",
    "TaskName": "Azure Resource Client Test",
    "TaskDetail": "A script to test AzureResourceClient functionality."
}
*/

var azureResourceClient = PluginLoader.GetPlugin<ScriptRunner.Plugins.AzureResourceClient.IAzureResourceClient>();

// List all subscriptions
Dump("Fetching subscriptions...");
var subscriptions = await azureResourceClient.ListSubscriptionsAsync();

if (!subscriptions.Any())
{
    Dump("No subscriptions found.");
    return "No subscriptions to display.";
}

// Set the context to the first subscription
var firstSubscriptionId = subscriptions.First().Data.SubscriptionId;
await azureResourceClient.SetSubscriptionContextAsync(firstSubscriptionId);

// Fetch resources in a specific resource group
var resourceGroupName = "example-resource-group";
Dump($"Fetching resources in resource group: {resourceGroupName}...");
var resources = await azureResourceClient.GetResourcesInResourceGroupAsync(resourceGroupName);

if (!resources.Any())
{
    Dump("No resources found.");
}
else
{
    foreach (var resource in resources)
    {
        Dump($"Resource: {resource.Name} - Type: {resource.ResourceType}");
    }
}

return "Azure Resource Client Test completed.";
```

---

## Documentation

### CookBook

For detailed usage and examples, see the [CookBook Page](Manual/CookBook/TheAzureResourceClient.md).

### API Overview

#### Core Methods

| Method                                      | Description                                                                                 |
|---------------------------------------------|---------------------------------------------------------------------------------------------|
| `ListSubscriptionsAsync()`                  | Retrieves all Azure subscriptions for the authenticated account.                           |
| `SetSubscriptionContextAsync(string)`       | Sets the current subscription context.                                                     |
| `GetResourceGroupsByTagAsync(string, string)` | Fetches resource groups filtered by a specific tag key and value.                          |
| `GetResourcesInResourceGroupAsync(string)`  | Lists all resources in a specified resource group.                                         |
| `SaveResourcesToJsonFileAsync(string, string)` | Exports resources from a resource group to a JSON file.                                    |
| `CountResourcesByLocationAsync()`           | Counts resources grouped by location.                                                      |

---

## Contributing

Contributions are welcome! If you find a bug or want to enhance the plugin, feel free to:

1. Fork this repository.
2. Create a feature branch: `git checkout -b feature/my-feature`.
3. Commit your changes: `git commit -m "Add my feature"`.
4. Push to the branch: `git push origin feature/my-feature`.
5. Open a pull request.

---

## Author

Developed with **🧡 passion** by **Peter van de Pas**.

For any questions or feedback, feel free to open an issue or contact me directly!

---

## 🔗 Links

- [ScriptRunner Plugins Repository](https://github.com/petervdpas/ScriptRunner.Plugins)

---

## License

This project is licensed under the [MIT License](./LICENSE).
