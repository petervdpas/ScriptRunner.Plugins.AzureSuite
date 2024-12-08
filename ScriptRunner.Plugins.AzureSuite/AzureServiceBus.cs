using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Azure.Messaging.ServiceBus;
using ScriptRunner.Plugins.AzureSuite.Interfaces;

namespace ScriptRunner.Plugins.AzureSuite;

/// <summary>
///     A helper class for interacting with Azure Service Bus.
///     Provides methods for sending and receiving messages, managing message properties (headers), and scheduling
///     messages.
///     Implements <see cref="IAzureServiceBus" />.
/// </summary>
public class AzureServiceBus : IAzureServiceBus
{
    private readonly ServiceBusClient? _client;
    private readonly Dictionary<string, object> _properties = new();
    private string? _connectionString;
    private string? _queueName;

    /// <summary>
    ///     Initializes a new instance of the <see cref="AzureServiceBus" /> class with default settings.
    /// </summary>
    public AzureServiceBus()
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="AzureServiceBus" /> class with a specified
    ///     <see cref="ServiceBusClient" />.
    ///     This constructor is useful for dependency injection and testing, allowing a custom or mocked
    ///     <see cref="ServiceBusClient" /> to be provided.
    /// </summary>
    /// <param name="client">
    ///     The <see cref="ServiceBusClient" /> to use for sending and receiving messages. Can be null if not
    ///     using dependency injection.
    /// </param>
    public AzureServiceBus(ServiceBusClient? client)
    {
        _client = client;
    }

    private string ContentType { get; set; } = "application/json"; // Default content type

    /// <summary>
    ///     Sets up the connection string, queue name, and optional content type for the Azure Service Bus.
    /// </summary>
    /// <param name="connectionString">The connection string for the Service Bus namespace.</param>
    /// <param name="queueName">The name of the queue to send and receive messages from.</param>
    /// <param name="contentType">Optional. The content type for the messages, defaults to "application/json".</param>
    public void Setup(
        string? connectionString,
        string? queueName,
        string contentType = "application/json")
    {
        _connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
        _queueName = queueName ?? throw new ArgumentNullException(nameof(queueName));
        ContentType = string.IsNullOrEmpty(contentType) ? "application/json" : contentType;
    }

    /// <summary>
    ///     Switches the active queue for sending/receiving messages without changing the connection string.
    /// </summary>
    /// <param name="queueName">The new queue name to switch to.</param>
    public void SwitchQueue(
        string? queueName)
    {
        if (string.IsNullOrEmpty(_connectionString))
            throw new InvalidOperationException("Connection string must be set via Setup() before switching queues.");

        if (string.IsNullOrEmpty(queueName))
            throw new ArgumentNullException(nameof(queueName), "Queue name cannot be null or empty.");

        _queueName = queueName;
    }

    /// <summary>
    ///     Adds or updates a property (header) for messages.
    /// </summary>
    /// <param name="key">The property key.</param>
    /// <param name="value">The property value.</param>
    public void AddOrUpdateProperty(
        string key,
        object value)
    {
        _properties[key] = value;
    }

    /// <summary>
    ///     Removes a property (header) by its key.
    /// </summary>
    /// <param name="key">The property key to remove.</param>
    /// <returns>True if the property was removed; otherwise, false.</returns>
    public bool RemoveProperty(
        string key)
    {
        return _properties.Remove(key);
    }

    /// <summary>
    ///     Clears all properties (headers).
    /// </summary>
    public void ClearProperties()
    {
        _properties.Clear();
    }

    /// <summary>
    ///     Sends a message to the specified Azure Service Bus queue.
    /// </summary>
    /// <param name="messageBody">The message body to send to the queue.</param>
    /// <param name="sessionId">Optional. The session ID for session-based messaging.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task SendMessageAsync(
        string messageBody,
        string? sessionId = null)
    {
        EnsureConfigured();

        var client = CreateServiceBusClient();
        var sender = client.CreateSender(_queueName);

        var message = new ServiceBusMessage(messageBody)
        {
            ContentType = ContentType
        };

        if (!string.IsNullOrEmpty(sessionId))
            message.SessionId = sessionId;

        foreach (var property in _properties)
            message.ApplicationProperties[property.Key] = property.Value;

        await sender.SendMessageAsync(message);
    }

    /// <summary>
    ///     Sends a batch of messages to the specified Azure Service Bus queue.
    /// </summary>
    /// <param name="messages">A list of messages to send.</param>
    /// <param name="sessionId">Optional session ID for session-based messaging.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public async Task SendBatchMessagesAsync(
        List<string> messages,
        string? sessionId = null)
    {
        EnsureConfigured();

        var client = CreateServiceBusClient();
        var sender = client.CreateSender(_queueName);
        var index = 0;

        while (index < messages.Count)
        {
            using var messageBatch = await sender.CreateMessageBatchAsync();
            for (; index < messages.Count; index++)
            {
                var messageBody = messages[index];
                var message = new ServiceBusMessage(Encoding.UTF8.GetBytes(messageBody))
                {
                    ContentType = ContentType
                };

                if (!string.IsNullOrEmpty(sessionId))
                    message.SessionId = sessionId;

                foreach (var property in _properties)
                    message.ApplicationProperties[property.Key] = property.Value;

                if (!messageBatch.TryAddMessage(message))
                    break;
            }

            await sender.SendMessagesAsync(messageBatch);
        }
    }

    /// <summary>
    ///     Sends a message to the queue with scheduled delivery at a later time.
    /// </summary>
    /// <param name="messageBody">The message body to send to the queue.</param>
    /// <param name="scheduleTimeUtc">The time at which to schedule the message for delivery.</param>
    /// <param name="sessionId">Optional session ID for session-based messaging.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public async Task SendScheduledMessageAsync(
        string messageBody,
        DateTimeOffset scheduleTimeUtc,
        string? sessionId = null)
    {
        EnsureConfigured();

        var client = CreateServiceBusClient();
        var sender = client.CreateSender(_queueName);

        var message = new ServiceBusMessage(Encoding.UTF8.GetBytes(messageBody))
        {
            ContentType = ContentType,
            ScheduledEnqueueTime = scheduleTimeUtc.UtcDateTime
        };

        if (!string.IsNullOrEmpty(sessionId))
            message.SessionId = sessionId;

        foreach (var property in _properties)
            message.ApplicationProperties[property.Key] = property.Value;

        await sender.ScheduleMessageAsync(message, scheduleTimeUtc.UtcDateTime);
    }

    /// <summary>
    ///     Receives messages from the queue.
    /// </summary>
    /// <param name="maxMessages">The maximum number of messages to receive.</param>
    /// <param name="sessionId">Optional session ID for session-based messaging.</param>
    /// <returns>A list of messages received from the queue.</returns>
    public async Task<List<ServiceBusReceivedMessage>> ReceiveMessagesAsync(
        int maxMessages = 10,
        string? sessionId = null)
    {
        EnsureConfigured();

        var client = CreateServiceBusClient();
        ServiceBusReceiver receiver;

        if (sessionId == null)
            receiver = client.CreateReceiver(_queueName);
        else
            receiver = await client.AcceptSessionAsync(_queueName, sessionId);

        // Receive messages
        var messages = await receiver.ReceiveMessagesAsync(maxMessages);

        // Handle null by returning an empty list
        return messages?.ToList() ?? [];
    }

    /// <summary>
    ///     Completes a received message, marking it as processed.
    /// </summary>
    /// <param name="message">The received message to complete.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public async Task CompleteMessageAsync(ServiceBusReceivedMessage message)
    {
        EnsureConfigured();

        var client = CreateServiceBusClient();
        var receiver = client.CreateReceiver(_queueName);
        await receiver.CompleteMessageAsync(message);
    }

    /// <summary>
    ///     Ensures that the Azure Service Bus is properly configured before any operations are performed.
    /// </summary>
    /// <remarks>
    ///     This method checks if the connection string and queue name have been set by the <see cref="Setup" /> method.
    ///     If either the connection string or queue name is missing, an <see cref="InvalidOperationException" /> is thrown.
    ///     This safeguard prevents the use of Azure Service Bus operations without proper configuration.
    /// </remarks>
    /// <exception cref="InvalidOperationException">
    ///     Thrown if the connection string or queue name has not been set by the <see cref="Setup" /> method.
    /// </exception>
    private void EnsureConfigured()
    {
        if (string.IsNullOrEmpty(_connectionString) || string.IsNullOrEmpty(_queueName))
            throw new InvalidOperationException(
                "AzureServiceBus is not configured. Call Setup() before performing any operations.");
    }

    /// <summary>
    ///     Creates and returns a new instance of the ServiceBusClient with configured options.
    /// </summary>
    /// <returns>A new instance of <see cref="ServiceBusClient" />.</returns>
    private ServiceBusClient CreateServiceBusClient()
    {
        if (_client != null)
            return _client; // Use the injected client for testing

        var clientOptions = new ServiceBusClientOptions
        {
            TransportType = ServiceBusTransportType.AmqpWebSockets
        };

        return new ServiceBusClient(_connectionString, clientOptions);
    }
}