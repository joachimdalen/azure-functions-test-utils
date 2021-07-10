using AzureFunctions.TestUtils.Settings;

namespace AzureFunctions.TestUtils.Handlers
{
    public static class StorageHandler
    {
        private static string AccountFormat = "AccountName={0};AccountKey={1};DefaultEndpointsProtocol=http;";

        public static string GetConnectionString(StorageSettings storageSettings)
        {
            var accountName = storageSettings.AccountName;

            var conString = string.Format(AccountFormat, accountName, storageSettings.AccountKey);
            conString += GetPart("BlobEndpoint", storageSettings.BlobPort, accountName);
            conString += GetPart("QueueEndpoint", storageSettings.QueuePort, accountName);
            conString += GetPart("TableEndpoint", storageSettings.TablePort, accountName);

            return conString;
        }

        private static string GetPart(string endpoint, int port, string account)
        {
            return $"{endpoint}=http://127.0.0.1:{port}/{account};";
        }
    }
}