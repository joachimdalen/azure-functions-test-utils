namespace AzureFunctions.TestUtils.Settings
{
    public class TestUtilsSettings
    {
        public StorageSettings Storage { get; set; }
        public string DotNetPath { get; set; }
        public string FuncHostPath { get; set; }
        public string FuncAppPath { get; set; }

        public string StorageConnectionString { get; set; }

        public int FuncHostPort { get; set; } = 7071;
    }
}