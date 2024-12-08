/*
{
    "TaskCategory": "Azure",
    "TaskName": "Azure TableStorage Dynamic Reader",
    "TaskDetail": "Dynamically select and read content from Azure TableStorage"
}
*/

// Step 1: Get the storage connection string
var tableStorage = new AzureTableStorage();

var realm = "local";
var storageConnectionString = await GetSettingPickerAsync(realm, 460, 130);

if (string.IsNullOrWhiteSpace(storageConnectionString))
{
    Dump("AzureTableStorage ConnectionString is not valid!");
    return -1;
}

try
{
    // Step 2: Initialize table storage without specifying a table
    tableStorage.Initialize(storageConnectionString, null);

    // Step 3: List available tables and let the user select one
    var tableNames = await tableStorage.ListTablesAsync();
    if (!tableNames.Any())
    {
        Dump("No tables found in the storage account.");
        return -1;
    }

    var selectedTable = await GetComboSelectionAsync("Select a table to read:", tableNames);

    if (string.IsNullOrWhiteSpace(selectedTable))
    {
        Dump("No table selected.");
        return -1;
    }
    
    // Step 4: Set the selected table
    tableStorage.SetTable(selectedTable);
    
    // Step 5: Get user input for the filter and row limit
    var filter = await GetUserInputAsync("Enter a filter expression (e.g., RowKey eq 'sample'):", "Filter", string.Empty);
    var rowLimitInput = await GetUserInputAsync("Enter number of rows to read (default: 10):", "Read Limit", "10");
    var rowLimit = int.TryParse(rowLimitInput, out var limit) ? limit : 10;

    // Step 6: Query and display data from the table
    var queryResults = await tableStorage.QueryEntitiesAsync<TableEntity>(filter);
    var rowsToDisplay = queryResults.Take(rowLimit).ToList();

    if (rowsToDisplay.Any())
    {
        Dump($"Displaying the first {rowsToDisplay.Count} rows from table '{selectedTable}':");
        foreach (var row in rowsToDisplay)
        {
            var rowDetails = row
                .Where(property => property.Key != "PartitionKey" && property.Key != "RowKey")
                .Select(property => $"{property.Key}: {property.Value}")
                .Aggregate((prop1, prop2) => $"{prop1}, {prop2}");
            Dump($"PartitionKey: {row.PartitionKey}, RowKey: {row.RowKey}, {rowDetails}");
        }
    }
    else
    {
        Dump($"No data found in table '{selectedTable}' with the applied filter.");
    }
}
catch (Exception ex)
{
    Dump($"An error occurred: {ex.Message}");
}

return 0;