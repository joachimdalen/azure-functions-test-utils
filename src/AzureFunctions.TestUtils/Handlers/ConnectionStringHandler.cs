using AzureFunctions.TestUtils.Settings;

namespace AzureFunctions.TestUtils.Handlers
{
    public class ConnectionStringHandler
    {
        private string AccountFormat = "AccountName={0};AccountKey={1};DefaultEndpointsProtocol=http;";

        public string GetConnectionString(TestUtilsSettings storageSettings)
        {
            var accountName = storageSettings.AccountName;
            var conString = string.Format(AccountFormat, accountName, storageSettings.AccountKey);
            conString += GetPart("BlobEndpoint", storageSettings.BlobPort, accountName);
            conString += GetPart("QueueEndpoint", storageSettings.QueuePort, accountName);
            conString += GetPart("TableEndpoint", storageSettings.TablePort, accountName);

            return conString;
        }

        private string GetPart(string endpoint, int port, string account)
        {
            return $"{endpoint}=http://127.0.0.1:{port}/{account};";
        }
    }
}