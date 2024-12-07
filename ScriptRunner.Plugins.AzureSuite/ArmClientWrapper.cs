
using Azure.Core;
using Azure.ResourceManager;
using Azure.ResourceManager.Resources;
using ScriptRunner.Plugins.AzureSuite.Interfaces;
namespace ScriptRunner.Plugins.AzureSuite;

/// <summary>
/// A wrapper for the Azure Resource Manager (ARM) client to facilitate interaction with Azure resources.
/// </summary>
public class ArmClientWrapper : IArmClientWrapper
{
    private readonly ArmClient _armClient;

    /// <summary>
    /// Initializes a new instance of the <see cref="ArmClientWrapper"/> class.
    /// </summary>
    /// <param name="credential">
    /// The <see cref="TokenCredential"/> used for authenticating with Azure services.
    /// </param>
    public ArmClientWrapper(TokenCredential credential)
    {
        _armClient = new ArmClient(credential);
    }

    /// <summary>
    /// Retrieves the underlying ARM client instance.
    /// </summary>
    /// <returns>The <see cref="ArmClient"/> instance.</returns>
    public ArmClient GetArmClient()
    {
        return _armClient;
    }

    /// <summary>
    /// Retrieves a collection of subscriptions available to the authenticated user.
    /// </summary>
    /// <returns>A <see cref="SubscriptionCollection"/> containing the available subscriptions.</returns>
    public SubscriptionCollection GetSubscriptions()
    {
        return _armClient.GetSubscriptions();
    }

    /// <summary>
    /// Retrieves a specific subscription resource by its ID.
    /// </summary>
    /// <param name="subscriptionId">The ID of the subscription to retrieve.</param>
    /// <returns>
    /// A <see cref="SubscriptionResource"/> representing the specified subscription.
    /// </returns>
    public SubscriptionResource GetSubscriptionResource(string subscriptionId)
    {
        var resourceIdentifier = new ResourceIdentifier($"/subscriptions/{subscriptionId}");
        return _armClient.GetSubscriptionResource(resourceIdentifier);
    }
}