---
Title: The Azure Resource Client
Subtitle: Manage and Query Azure Resources Using the Azure Resource Client Plugin
Category: Cookbook
Author: Peter van de Pas
keywords: [CookBook, Azure, ResourceClient]
table-use-row-colors: true
table-row-color: "D3D3D3"
toc: true
toc-title: Table of Contents
toc-own-page: true
---

# Recipe: The Azure Resource Client

## Goal

Leverage the **Azure Resource Client Plugin** to interact with Azure resources in a streamlined and automated manner. Learn how to fetch subscriptions, query resource groups, count resources by location, and more.

---

## Steps

### Step 1: Fetch Azure Subscriptions

To start, retrieve all available subscriptions for your Azure account:

```csharp
var azureResourceClient = new AzureResourceClient();

// List all subscriptions
Dump("Fetching subscriptions...");
var subscriptions = await azureResourceClient.ListSubscriptionsAsync();

if (!subscriptions.Any())
{
    Dump("No subscriptions found.");
    return "No subscriptions to display.";
}

foreach (var subscription in subscriptions)
{
    Dump($"Subscription: {subscription.Data.DisplayName} ({subscription.Data.SubscriptionId})");
}
```

---

### Step 2: Set the Subscription Context

Before managing resources, set the context to the desired subscription:

```csharp
var subscriptionId = "your-subscription-id-here";
await azureResourceClient.SetSubscriptionContextAsync(subscriptionId);
Dump($"Subscription context set to: {subscriptionId}");
```

---

### Step 3: Fetch Resource Groups by Tag

Filter resource groups using a specific tag key and value:

```csharp
var tagName = "Environment";
var tagValue = "Production";

Dump($"Fetching resource groups with tag: {tagName}={tagValue}...");
var resourceGroups = await azureResourceClient.GetResourceGroupsByTagAsync(tagName, tagValue);

if (!resourceGroups.Any())
{
    Dump("No resource groups found with the specified tag.");
    return;
}

foreach (var rg in resourceGroups)
{
    Dump($"Resource Group: {rg.Data.Name}");
}
```

---

### Step 4: List Resources in a Resource Group

Retrieve resources within a specific resource group:

```csharp
var resourceGroupName = "example-resource-group";

Dump($"Fetching resources in resource group: {resourceGroupName}...");
var resources = await azureResourceClient.GetResourcesInResourceGroupAsync(resourceGroupName);

if (!resources.Any())
{
    Dump("No resources found in the specified resource group.");
    return;
}

foreach (var resource in resources)
{
    Dump($"Resource: {resource.Name} - Type: {resource.ResourceType} - Location: {resource.Location}");
}
```

---

### Step 5: Count Resources by Location

Count resources across your subscription, grouped by their location:

```csharp
Dump("Counting resources by location...");
var locationCounts = await azureResourceClient.CountResourcesByLocationAsync();

foreach (var location in locationCounts)
{
    Dump($"Location: {location.Key}, Count: {location.Value}");
}
```

---

### Step 6: Save Resource Details to a JSON File

Export all resources from a resource group into a JSON file:

```csharp
var resourceGroupName = "example-resource-group";
var filePath = "resources.json";

Dump($"Saving resources in resource group '{resourceGroupName}' to {filePath}...");
await azureResourceClient.SaveResourcesToJsonFileAsync(resourceGroupName, filePath);

Dump("Resources saved successfully.");
```

---

### Step 7: Query Resources by Type

Fetch resources of a specific type, such as virtual machines:

```csharp
var resourceType = "Microsoft.Compute/virtualMachines";

Dump($"Fetching resources of type: {resourceType}...");
var resourcesByType = await azureResourceClient.GetResourcesByTypeAsync(resourceType);

if (!resourcesByType.Any())
{
    Dump("No resources found for the specified type.");
    return;
}

foreach (var resource in resourcesByType)
{
    Dump($"Resource: {resource.Name} - ResourceGroup: {resource.ResourceGroupName} - Location: {resource.Location}");
}
```

---

## Summary

With the **Azure Resource Client Plugin**, managing Azure resources becomes an efficient and straightforward process. Whether listing subscriptions, filtering resources, or exporting details, this plugin equips you with powerful tools to automate resource management.
