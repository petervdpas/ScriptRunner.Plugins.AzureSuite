using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Azure.Messaging.ServiceBus;

namespace ScriptRunner.Plugins.AzureSuite.Interfaces;

public interface IAzureServiceBus
{
    void Setup(
        string? connectionString,
        string? queueName,
        string contentType = "application/json");

    void SwitchQueue(string? queueName);

    void AddOrUpdateProperty(
        string key, object value);

    bool RemoveProperty(string key);

    void ClearProperties();

    Task SendMessageAsync(
        string messageBody,
        string? sessionId = null);

    Task SendBatchMessagesAsync(
        List<string> messages,
        string? sessionId = null);

    Task SendScheduledMessageAsync(
        string messageBody,
        DateTimeOffset scheduleTimeUtc,
        string? sessionId = null);

    Task<List<ServiceBusReceivedMessage>> ReceiveMessagesAsync(
        int maxMessages = 10,
        string? sessionId = null);

    Task CompleteMessageAsync(ServiceBusReceivedMessage message);
}