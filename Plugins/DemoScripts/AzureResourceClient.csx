/*
{
    "TaskCategory": "Azure",
    "TaskName": "Azure Resource Client Test",
    "TaskDetail": "A script to test AzureResourceClient functionality."
}
*/

var azureResourceClient = new AzureResourceClient();

// List all subscriptions
Dump("Fetching subscriptions...");
var subscriptions = await azureResourceClient.ListSubscriptionsAsync();
if (!subscriptions.Any())
{
    Dump("No subscriptions found.");
    return "No subscriptions to display.";
}

// Prepare options for the Combo Dialog
var subscriptionOptions = subscriptions
    .OrderBy(subscription => subscription.Data.DisplayName)
    .ToDictionary(
        subscription => subscription.Data.SubscriptionId,
        subscription => subscription.Data.DisplayName
    );

// Show the Combo Dialog to select a subscription
var selectedSubscriptionId = await GetComboSelectionAsync(
    "Select Subscription",
    "Please select a subscription from the list:",
    subscriptionOptions,
    500,
    130
);

if (string.IsNullOrEmpty(selectedSubscriptionId))
{
    Dump("Subscription selection was cancelled!");
    return "Subscription selection cancelled.";
}

// Set the selected subscription context
await azureResourceClient.SetSubscriptionContextAsync(selectedSubscriptionId);
Dump($"Subscription set to: {subscriptionOptions[selectedSubscriptionId]} - {selectedSubscriptionId}");

string? tagName = null;
string? tagValue = null;

tagName = await GetUserInputAsync(
        "Give Tag Name", "Please enter a Tag Name:", 500, 150);

if (string.IsNullOrEmpty(tagName))
{
    return "No input provided in input-dialog... script stopped running.";
}

tagValue = await GetUserInputAsync(
        "Give Tag Value", $"Please enter the Tag Value for '{tagName}':", 500, 150);

if (string.IsNullOrEmpty(tagValue))
{
    return "No input provided in input-dialog... script stopped running.";
}

// Fetch resource groups by tag
Dump("Fetching resource groups by tag...");
var resourceGroups = await azureResourceClient.GetResourceGroupsByTagAsync(tagName, tagValue);
if (!resourceGroups.Any())
{
    Dump("No resource groups found with the specified tag.");
    return "No resource groups to display.";
}

// Prepare options for the Combo Dialog
var resourceGroupOptions = resourceGroups
    .OrderBy(rg => rg.Data.Name) // Sort alphabetically by Name
    .ToDictionary(
        rg => rg.Data.Name,
        rg => rg.Data.Name
    );

// Show the Combo Dialog to select a resource group
var selectedResourceGroupName = await GetComboSelectionAsync(
    "Select Resource Group",
    "Please select a resource group from the list:",
    resourceGroupOptions,
    500,
    150
);

if (string.IsNullOrEmpty(selectedResourceGroupName))
{
    Dump("Resource group selection was cancelled!");
    return "Resource group selection cancelled.";
}

Dump($"Listing resources in the resource group: {selectedResourceGroupName}");
var resources = await azureResourceClient.GetResourcesInResourceGroupAsync(selectedResourceGroupName);
if (resources.Any())
{
    foreach (var resource in resources)
    {
        Dump($"Resource: {resource.Name} - {resource.ResourceType}");
    }
}
else
{
    Dump("No resources found in the specified resource group.");
}

// Count resources by location
Dump("Counting resources by location...");
var resourceCounts = await azureResourceClient.CountResourcesByLocationAsync();
foreach (var location in resourceCounts)
{
    Dump($"Location: {location.Key}, Count: {location.Value}");
}

return "Azure Resource Client Test completed successfully.";
