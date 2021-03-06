namespace JoachimDalen.AzureFunctions.TestUtils.Settings
{
    public class TestUtilsSettings
    {
        /// <summary>
        /// Path to dotnet executable
        /// </summary>
        public string DotNetPath { get; set; }

        /// <summary>
        /// Path to func.dll 
        /// </summary>
        public string FuncHostPath { get; set; }

        /// <summary>
        /// Directory path to function app to run
        /// </summary>
        public string FuncAppPath { get; set; }

        /// <summary>
        /// Path to Azurite executable
        /// </summary>
        public string AzuritePath { get; set; }

        public string StorageConnectionString { get; set; }

        /// <summary>
        /// Port to run function app on
        /// </summary>
        public int FuncHostPort { get; set; } = 7071;

        /// <summary>
        /// Storage account name
        /// </summary>
        public string AccountName { get; set; }

        /// <summary>
        /// Storage account key
        /// </summary>
        public string AccountKey { get; set; }

        /// <summary>
        /// Port to run blob host on
        /// </summary>
        public int BlobPort { get; set; } = 10000;

        /// <summary>
        /// Port to run queue host on
        /// </summary>
        public int QueuePort { get; set; } = 10001;

        /// <summary>
        /// Port to run table host on
        /// </summary>
        public int TablePort { get; set; } = 10002;

        /// <summary>
        /// Directory to store Azurite data in. Defaults to TestResults directory for the given test
        /// </summary>
        public string DataDirectory { get; set; }

        /// <summary>
        /// Use azurite storage emulator. This should be true if using any storage related attributes
        /// </summary>
        public bool UseAzuriteStorage { get; set; }

        /// <summary>
        /// Start Azurite alongside function app host
        /// </summary>
        public bool RunAzurite { get; set; }
        
        /// <summary>
        /// Run Azurite in silent mode (no logs)
        /// </summary>
        public bool RunAzuriteSilent { get; set; }

        /// <summary>
        /// Decides if containers created by the function app should be persisted after test run.
        /// </summary>
        public bool PersistAzureContainers { get; set; }

        /// <summary>
        /// Clears Azurite after each test run and provides a clean container for new runs
        /// </summary>
        public bool ClearStorageAfterRun { get; set; }
        
        
        /// <summary>
        /// Write to logfiles for Azurite and Function host
        /// </summary>
        public bool WriteLog { get; set; }
    }
}