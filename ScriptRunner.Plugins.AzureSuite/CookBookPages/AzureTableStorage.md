---
Title: The Azure Table Storage Client  
Subtitle: Manage and Query Azure Tables Using the Azure Table Storage Plugin  
Category: Cookbook  
Author: Peter van de Pas  
Keywords: [CookBook, Azure, TableStorage, Entities]  
Table-use-row-colors: true  
Table-row-color: "D3D3D3"  
Toc: true  
Toc-title: Table of Contents  
Toc-own-page: true  
---

# Recipe: The Azure Table Storage Client

## Goal

Leverage the **Azure Table Storage Plugin** to interact with Azure Table Storage for managing tables and entities. Learn how to list tables, query entities, upsert data, and more.

---

## Steps

### Step 1: Initialize the Azure Table Storage

Connect to your Azure Table Storage by providing its connection string:

```csharp
var connectionString = "your-storage-account-connection-string";
var tableStorage = new AzureTableStorage();

// Initialize the client
tableStorage.Initialize(connectionString, null);
Dump("Connected to Azure Table Storage.");
```

---

### Step 2: List All Tables

Retrieve a list of all tables in your Azure Table Storage account:

```csharp
Dump("Fetching table names...");
var tableNames = await tableStorage.ListTablesAsync();

if (!tableNames.Any())
{
    Dump("No tables found in the storage account.");
    return;
}

foreach (var tableName in tableNames)
{
    Dump($"Table: {tableName}");
}
```

---

### Step 3: Set a Table to Work With

Set the table to interact with for further operations:

```csharp
var tableName = "example-table";
tableStorage.SetTable(tableName);
Dump($"Using table: {tableName}");
```

---

### Step 4: Add or Update an Entity

Upsert an entity into the selected table:

```csharp
public class ExampleEntity : ITableEntity
{
    public string PartitionKey { get; set; } = "ExamplePartition";
    public string RowKey { get; set; } = Guid.NewGuid().ToString();
    public string Data { get; set; } = "Sample Data";
    public DateTimeOffset? Timestamp { get; set; }
    public ETag ETag { get; set; }
}

var entity = new ExampleEntity
{
    Data = "Updated Data"
};

await tableStorage.UpsertEntityAsync(entity);
Dump($"Entity with RowKey '{entity.RowKey}' upserted successfully.");
```

---

### Step 5: Query Entities by Filter

Query entities using an OData filter:

```csharp
var filter = "PartitionKey eq 'ExamplePartition'";
Dump($"Querying entities with filter: {filter}...");

var entities = await tableStorage.QueryEntitiesAsync<ExampleEntity>(filter);

if (!entities.Any())
{
    Dump("No entities found matching the filter.");
    return;
}

foreach (var entity in entities)
{
    Dump($"Entity: PartitionKey = {entity.PartitionKey}, RowKey = {entity.RowKey}, Data = {entity.Data}");
}
```

---

### Step 6: Retrieve a Specific Entity

Retrieve an entity using its partition and row keys:

```csharp
var partitionKey = "ExamplePartition";
var rowKey = "example-row-key";

var entity = await tableStorage.GetEntityAsync<ExampleEntity>(partitionKey, rowKey);

if (entity != null)
{
    Dump($"Entity found: PartitionKey = {entity.PartitionKey}, RowKey = {entity.RowKey}, Data = {entity.Data}");
}
else
{
    Dump("Entity not found.");
}
```

---

### Step 7: Delete an Entity

Remove an entity from the table:

```csharp
var partitionKey = "ExamplePartition";
var rowKey = "example-row-key";

await tableStorage.DeleteEntityAsync(partitionKey, rowKey);
Dump($"Entity with PartitionKey '{partitionKey}' and RowKey '{rowKey}' deleted successfully.");
```

---

### Step 8: Check for Entity Existence

Verify if entities with specific row keys exist in the table:

```csharp
var rowKeys = new List<string> { "example-row-key-1", "example-row-key-2" };

Dump("Checking for entity existence...");
var existingRowKeys = await tableStorage.CheckEntitiesExistAsync<ExampleEntity>(rowKeys);

if (!existingRowKeys.Any())
{
    Dump("No entities found with the specified RowKeys.");
}
else
{
    Dump($"Entities found with RowKeys: {string.Join(", ", existingRowKeys)}");
}
```

---

## Summary

With the **Azure Table Storage Plugin**, you can manage tables and entities in Azure Table Storage. Whether listing tables, querying data, or performing CRUD operations, this plugin provides a seamless interface for interacting with Azure Table Storage.
