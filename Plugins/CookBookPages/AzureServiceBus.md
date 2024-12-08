---
Title: Azure Service Bus Interaction
Subtitle: Manage Azure Service Bus Queues with Ease
Category: Cookbook  
Author: Peter van de Pas  
Keywords: [CookBook, Azure, ServiceBus, Messaging]
Table-use-row-colors: true  
Table-row-color: "D3D3D3"  
Toc: true  
Toc-title: Table of Contents  
Toc-own-page: true
---

# Recipe: Using Azure Service Bus with the AzureSuite Plugin

## **Goal**

Leverage the **Azure Service Bus Plugin** to perform common operations such as sending messages, receiving messages, and scheduling messages. This guide walks through dynamically interacting with Azure Service Bus for real-world use cases.

---

## **Steps**

---

### **Step 1: Connect to Azure Service Bus**

To begin, set up the connection to your Azure Service Bus namespace and queue.

```csharp
// Initialize connection to Azure Service Bus
var serviceBus = new AzureServiceBus();

var connectionString = "your-service-bus-connection-string";
var queueName = "your-queue-name";

serviceBus.Setup(connectionString, queueName);

Dump($"Connected to Azure Service Bus queue: {queueName}");
```

---

### **Step 2: Send a Message**

To send a single message to the queue:

```csharp
var messageBody = "Hello, Azure Service Bus!";
await serviceBus.SendMessageAsync(messageBody);

Dump($"Message sent: {messageBody}");
```

---

### **Step 3: Receive Messages**

Retrieve a batch of messages from the queue:

```csharp
var maxMessages = 5;
var receivedMessages = await serviceBus.ReceiveMessagesAsync(maxMessages);

if (receivedMessages.Any())
{
    Dump($"Received {receivedMessages.Count} messages:");

    foreach (var message in receivedMessages)
    {
        Dump($"Message ID: {message.MessageId}, Body: {Encoding.UTF8.GetString(message.Body)}");

        // Mark message as complete
        await serviceBus.CompleteMessageAsync(message);
        Dump($"Message ID '{message.MessageId}' marked as complete.");
    }
}
else
{
    Dump("No messages available in the queue.");
}
```

---

### **Step 4: Schedule a Message**

To schedule a message for delivery at a later time:

```csharp
var scheduledMessageBody = "This message will be delivered later!";
var scheduleTimeUtc = DateTimeOffset.UtcNow.AddMinutes(10);

await serviceBus.SendScheduledMessageAsync(scheduledMessageBody, scheduleTimeUtc);

Dump($"Message scheduled for delivery at {scheduleTimeUtc}: {scheduledMessageBody}");
```

---

### **Step 5: Add Custom Properties**

To include custom properties in the message:

```csharp
serviceBus.AddOrUpdateProperty("Priority", "High");
serviceBus.AddOrUpdateProperty("Category", "Notification");

await serviceBus.SendMessageAsync("This is a message with custom properties.");

Dump("Message with custom properties sent.");
```

---

### **Step 6: Switch Queues**

Switch to a different queue without resetting the connection string:

```csharp
var newQueueName = "another-queue";
serviceBus.SwitchQueue(newQueueName);

Dump($"Switched to queue: {newQueueName}");
```

---

## **Advanced Use Cases**

---

### **Batch Sending Messages**

Send a batch of messages to the queue:

```csharp
var messages = new List<string>
{
    "Message 1",
    "Message 2",
    "Message 3"
};

await serviceBus.SendBatchMessagesAsync(messages);

Dump("Batch of messages sent successfully.");
```

---

### **Session-Based Messaging**

Use session-based messaging for advanced workflows:

```csharp
var sessionId = "user-session-123";

// Send a session-bound message
await serviceBus.SendMessageAsync("This is a session-bound message.", sessionId);

// Receive messages for the specific session
var sessionMessages = await serviceBus.ReceiveMessagesAsync(sessionId: sessionId);

Dump($"Received {sessionMessages.Count} messages for session {sessionId}");
```

---

### **Handle Dead-Letter Messages**

Retrieve messages from the dead-letter queue (DLQ):

```csharp
var deadLetterQueueName = $"{queueName}/$DeadLetterQueue";
serviceBus.SwitchQueue(deadLetterQueueName);

var deadLetterMessages = await serviceBus.ReceiveMessagesAsync();

if (deadLetterMessages.Any())
{
    Dump($"Found {deadLetterMessages.Count} dead-letter messages:");

    foreach (var message in deadLetterMessages)
    {
        Dump($"Dead-letter Message ID: {message.MessageId}, Body: {Encoding.UTF8.GetString(message.Body)}");

        // Process or complete the message as needed
        await serviceBus.CompleteMessageAsync(message);
    }
}
else
{
    Dump("No messages in the dead-letter queue.");
}
```

---

## **Summary**

With the **Azure Service Bus Plugin**, you can manage queues dynamically and efficiently. This guide covered the basics of connecting to Azure Service Bus, sending and receiving messages, scheduling messages, and handling advanced scenarios like session-based messaging and dead-letter queues.

Explore these features to unlock the full potential of Azure Service Bus for your applications!