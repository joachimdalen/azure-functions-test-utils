namespace AzureFunctions.TestUtils.Settings
{
    public class StorageSettings
    {
        public string AccountName { get; set; } = "aftu";
        public string AccountKey { get; set; } = "key123";
        public int BlobPort { get; set; } = 10000;
        public int QueuePort { get; set; } = 10001;
        public int TablePort { get; set; } = 10002;
    }
}