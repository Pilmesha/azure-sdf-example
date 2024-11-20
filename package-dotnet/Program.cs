using System;
using System.Threading.Tasks;
using Azure;
using Azure.Core;
using Azure.Identity;
using Azure.ResourceManager;
using Azure.ResourceManager.Resources;
using Azure.ResourceManager.Storage;
using Azure.ResourceManager.Storage.Models;

class Program
{
    static async Task Main(string[] args)
    {
        string subscriptionId = "10d02ecf-b895-411f-b5ba-a08f5e6db497";

        // Authenticate with Azure
        ArmClient armClient = new ArmClient(new DefaultAzureCredential());

        // Get subscription
        ResourceIdentifier subscriptionResourceId = new ResourceIdentifier($"/subscriptions/{subscriptionId}");
        SubscriptionResource subscription = armClient.GetSubscriptionResource(subscriptionResourceId);

        // Resource group details
        string resourceGroupName = "NewResourceGroup";
        string location = "EastUS";

        Console.WriteLine($"Creating resource group: {resourceGroupName}");
        var resourceGroupOperation = await subscription.GetResourceGroups()
            .CreateOrUpdateAsync(WaitUntil.Completed, resourceGroupName, new ResourceGroupData(location));
        ResourceGroupResource resourceGroup = resourceGroupOperation.Value;

        Console.WriteLine($"Resource group {resourceGroupName} created.");

        // Storage account details
        string storageAccountName = "stor" + Guid.NewGuid().ToString("n").Substring(0, 20);

        Console.WriteLine($"Creating storage account: {storageAccountName}");
        var storageAccountData = new StorageAccountCreateOrUpdateContent(
            new StorageSku(StorageSkuName.StandardLrs),
            StorageKind.StorageV2,
            location);

        var storageAccountOperation = await resourceGroup.GetStorageAccounts()
            .CreateOrUpdateAsync(WaitUntil.Completed, storageAccountName, storageAccountData);
        StorageAccountResource storageAccount = storageAccountOperation.Value;

        Console.WriteLine($"Storage account {storageAccountName} created.");
    }
}
