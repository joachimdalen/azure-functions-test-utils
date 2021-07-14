namespace AzureFunctions.TestUtils.Settings
{
    public class TestUtilsSettings
    {
        public string DotNetPath { get; set; }
        public string FuncHostPath { get; set; }
        public string FuncAppPath { get; set; }
        public string AzuritePath { get; set; }
        public string StorageConnectionString { get; set; }
        public int FuncHostPort { get; set; } = 7071;
        public string AccountName { get; set; }
        public string AccountKey { get; set; }
        public int BlobPort { get; set; } = 10000;
        public int QueuePort { get; set; } = 10001;
        public int TablePort { get; set; } = 10002;
        public string DataDirectory { get; set; }
    }
}