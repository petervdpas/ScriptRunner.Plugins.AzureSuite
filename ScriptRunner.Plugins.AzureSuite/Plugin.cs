using System;
using System.Collections.Generic;
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
    "AzureResource Client Plugin",
    "Provides ScriptRunner with a Azure Resource Client",
    "Peter van de Pas",
    "1.0.2",
    "net8.0",
    ["IAzureResourceClient", "IAzureKeyVault", "IAzureTableStorage"])]
public class Plugin : IServicePlugin
{
    /// <summary>
    ///     Gets the name of the plugin.
    /// </summary>
    public string Name => "Azure Suite Plugin";

    /// <summary>
    ///     Initializes the plugin with the provided configuration.
    /// </summary>
    /// <param name="configuration">A dictionary containing key-value pairs for the plugin's configuration.</param>
    public void Initialize(IDictionary<string, object> configuration)
    {
        Console.WriteLine(configuration.TryGetValue("AzureResourceClientKey", out var azureResourceClientValue)
            ? $"AzureResourceClientKey value: {azureResourceClientValue}"
            : "AzureResourceClientKey not found in configuration.");
    }

    /// <summary>
    ///     Executes the plugin's main functionality.
    /// </summary>
    public void Execute()
    {
        Console.WriteLine("AzureResourceClient Plugin executed.");
    }

    /// <summary>
    ///     Registers the plugin's services into the application's dependency injection container.
    /// </summary>
    /// <param name="services">The service collection to register the plugin's services into.</param>
    public void RegisterServices(IServiceCollection services)
    {
        services.AddSingleton<IAzureResourceClient, AzureResourceClient>();
        services.AddSingleton<IAzureKeyVault, AzureKeyVault>();
        services.AddSingleton<IAzureTableStorage, AzureTableStorage>();
    }
}