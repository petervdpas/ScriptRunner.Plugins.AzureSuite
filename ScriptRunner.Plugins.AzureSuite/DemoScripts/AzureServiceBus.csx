/*
{
    "TaskCategory": "Azure",
    "TaskName": "Azure Service Bus Dynamic Interaction",
    "TaskDetail": "Interact with Azure Service Bus dynamically for sending, receiving, and scheduling messages."
}
*/

var realm = "local";

// Step 1: Prompt for Service Bus connection string and queue name
var serviceBusConnectionString = await GetSettingPickerAsync(realm, 460, 130);
if (string.IsNullOrWhiteSpace(serviceBusConnectionString))
{
    Dump("Azure Service Bus connection string is not valid!");
    return -1;
}

var queueName = await GetUserInputAsync("Enter the name of the Service Bus queue:", "Queue Name");
if (string.IsNullOrWhiteSpace(queueName))
{
    Dump("Queue name cannot be empty!");
    return -1;
}

// Step 2: Initialize the AzureServiceBus instance
var serviceBus = new AzureServiceBus();
serviceBus.Setup(serviceBusConnectionString, queueName);
Dump($"Connected to Azure Service Bus queue: {queueName}");

// Step 3: Select operation to perform
var options = new List<string> { "Send Message", "Receive Messages", "Schedule Message" };
var selectedOption = await GetComboSelectionAsync("Select Operation", "Choose an operation to perform:", options, 500, 130);

if (string.IsNullOrWhiteSpace(selectedOption))
{
    Dump("No operation selected. Exiting...");
    return -1;
}

switch (selectedOption)
{
    case "Send Message":
        // Step 4A: Send a single message
        var messageBody = await GetUserInputAsync("Enter the message body to send:", "Message Body");
        if (!string.IsNullOrWhiteSpace(messageBody))
        {
            await serviceBus.SendMessageAsync(messageBody);
            Dump($"Message sent successfully to queue '{queueName}': {messageBody}");
        }
        else
        {
            Dump("Message body cannot be empty.");
        }
        break;

    case "Receive Messages":
        // Step 4B: Receive messages
        var maxMessagesInput = await GetUserInputAsync("Enter the maximum number of messages to receive (default: 5):", "Max Messages", "5");
        var maxMessages = int.TryParse(maxMessagesInput, out var limit) ? limit : 5;

        var receivedMessages = await serviceBus.ReceiveMessagesAsync(maxMessages);

        if (receivedMessages.Any())
        {
            Dump($"Received {receivedMessages.Count} messages:");
            foreach (var msg in receivedMessages)
            {
                Dump($"Message ID: {msg.MessageId}, Body: {Encoding.UTF8.GetString(msg.Body)}");
                await serviceBus.CompleteMessageAsync(msg); // Mark the message as completed
                Dump($"Message ID '{msg.MessageId}' marked as complete.");
            }
        }
        else
        {
            Dump("No messages available in the queue.");
        }
        break;

    case "Schedule Message":
        // Step 4C: Schedule a message
        var scheduledMessageBody = await GetUserInputAsync("Enter the message body to schedule:", "Scheduled Message Body");
        var scheduleTimeInput = await GetUserInputAsync("Enter the schedule time (UTC) in 'yyyy-MM-dd HH:mm:ss' format:", "Schedule Time");

        if (DateTimeOffset.TryParse(scheduleTimeInput, out var scheduleTimeUtc) && !string.IsNullOrWhiteSpace(scheduledMessageBody))
        {
            await serviceBus.SendScheduledMessageAsync(scheduledMessageBody, scheduleTimeUtc);
            Dump($"Message scheduled successfully for delivery at {scheduleTimeUtc.UtcDateTime}: {scheduledMessageBody}");
        }
        else
        {
            Dump("Invalid schedule time or message body.");
        }
        break;

    default:
        Dump("Invalid option selected.");
        break;
}

return "Azure Service Bus interaction completed successfully.";