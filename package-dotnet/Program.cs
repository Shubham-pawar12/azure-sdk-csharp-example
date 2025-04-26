using System;
using System.Threading.Tasks;
using Azure;
using Azure.Identity;
using Azure.ResourceManager;
using Azure.ResourceManager.Resources;
using Azure.ResourceManager.Storage;
using Azure.ResourceManager.Storage.Models;

class Program
{
    static async Task Main(string[] args)
    {
        // Authenticate using DefaultAzureCredential
        var credential = new DefaultAzureCredential();
        var armClient = new ArmClient(credential);

        // Define names and location
        string subscriptionId = "70396b64-2fce-496a-b285-9482f935fe9c";
        string resourceGroupName = "my-sdk-shubh-ResourceGroup";
        string storageAccountName = "helloshubh12345"; // must be globally unique and lowercase
        string location = "centralindia";

        // Get the subscription
        SubscriptionResource subscription = await armClient.GetDefaultSubscriptionAsync();

        // Create the Resource Group
        ResourceGroupCollection resourceGroups = subscription.GetResourceGroups();
        ResourceGroupData rgData = new ResourceGroupData(location);
        ArmOperation<ResourceGroupResource> rgLro = await resourceGroups.CreateOrUpdateAsync(WaitUntil.Completed, resourceGroupName, rgData);
        ResourceGroupResource resourceGroup = rgLro.Value;

        // Define Storage Account properties
        StorageAccountCreateOrUpdateContent parameters = new StorageAccountCreateOrUpdateContent(
            new StorageSku(StorageSkuName.StandardLrs),
            StorageKind.StorageV2,
            location)
        {
            AccessTier = StorageAccountAccessTier.Hot
        };

        // Create the Storage Account
        StorageAccountCollection storageAccounts = resourceGroup.GetStorageAccounts();
        ArmOperation<StorageAccountResource> storageLro = await storageAccounts.CreateOrUpdateAsync(
            WaitUntil.Completed,
            storageAccountName,
            parameters);

        StorageAccountResource storageAccount = storageLro.Value;

        Console.WriteLine($"✅ Storage Account '{storageAccount.Data.Name}' successfully created in resource group '{resourceGroupName}'.");
    }
}
