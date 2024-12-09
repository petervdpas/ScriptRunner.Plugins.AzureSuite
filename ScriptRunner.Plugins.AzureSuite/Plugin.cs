﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using ScriptRunner.Plugins.Attributes;
using ScriptRunner.Plugins.AzureSuite.Interfaces;

namespace ScriptRunner.Plugins.AzureSuite;

/// <summary>
///     A plugin that registers and provides a restful client.
/// </summary>
/// <remarks>
///     This plugin demonstrates how to register a service with the host application's DI container.
/// </remarks>
[PluginMetadata(
    name: "AzureResource Client Plugin",
    description: "Provides ScriptRunner with a Azure Resource Client",
    author: "Peter van de Pas",
    version: "1.0.2",
    pluginSystemVersion: "1.0.26",
    frameworkVersion: ".NET 8.0",
    services: ["IAzureResourceClient", "IAzureKeyVault", "IAzureTableStorage", "IAzureServiceBus"])]
public class Plugin : BaseAsyncServicePlugin
{
    /// <summary>
    ///     Gets the name of the plugin.
    /// </summary>
    public override string Name => "Azure Suite Plugin";

    /// <summary>
    /// Asynchronously initializes the plugin using the provided configuration.
    /// </summary>
    /// <param name="configuration">A dictionary containing configuration key-value pairs for the plugin.</param>
    public override async Task InitializeAsync(IDictionary<string, object> configuration)
    {
        // Simulate async initialization (e.g., loading settings or validating configurations)
        await Task.Delay(100);
        Console.WriteLine(configuration.TryGetValue("AzureResourceClientKey", out var azureResourceClientValue)
            ? $"AzureResourceClientKey value: {azureResourceClientValue}"
            : "AzureResourceClientKey not found in configuration.");
    }

    /// <summary>
    /// Asynchronously executes the plugin's main functionality.
    /// </summary>
    public override async Task ExecuteAsync()
    {
        // Example execution logic
        await Task.Delay(50);
        Console.WriteLine("AzureResourceClient Plugin executed.");
    }
    
    /// <summary>
    /// Asynchronously registers the plugin's services into the application's dependency injection container.
    /// </summary>
    /// <param name="services">The service collection to register services into.</param>
    public override async Task RegisterServicesAsync(IServiceCollection services)
    {
        // Simulate async service registration (e.g., initializing an external resource)
        await Task.Delay(50);
        services.AddSingleton<IAzureResourceClient, AzureResourceClient>();
        services.AddSingleton<IAzureKeyVault, AzureKeyVault>();
        services.AddSingleton<IAzureTableStorage, AzureTableStorage>();
        services.AddSingleton<IAzureServiceBus, AzureServiceBus>();
    }
}