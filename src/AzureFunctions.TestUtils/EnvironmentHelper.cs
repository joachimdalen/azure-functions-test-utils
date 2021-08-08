using System;

namespace AzureFunctions.TestUtils
{
    public static class EnvironmentHelper
    {
        public static string DotNetPath => Environment.GetEnvironmentVariable("AFTU_DOT_NET_PATH");
        public static string FuncHostPath => Environment.GetEnvironmentVariable("AFTU_FUNC_HOST_PATH");
        public static string FuncAppPath => Environment.GetEnvironmentVariable("AFTU_FUNC_APP_PATH");
        public static string AzuritePath => Environment.GetEnvironmentVariable("AFTU_AZURITE_PATH");
        public static string FuncHostPort => Environment.GetEnvironmentVariable("AFTU_FUNC_HOST_PORT");
        public static string DataDirectory => Environment.GetEnvironmentVariable("AFTU_DATA_DIRECTORY");
        public static bool? UseAzuriteStorage => GetBoolEnvironmentVariable("AFTU_USE_AZURITE_STORAGE");
        public static bool? RunAzurite => GetBoolEnvironmentVariable("AFTU_RUN_AZURITE");
        public static bool? RunAzuriteSilent => GetBoolEnvironmentVariable("AFTU_AZURITE_SILENT");
        
        public static bool? PersistAzureContainers =>
            GetBoolEnvironmentVariable("AFTU_PERSIST_AZURE_CONTAINERS");

        public static bool? ClearStorageAfterRun => GetBoolEnvironmentVariable("AFTU_CLEAR_STORAGE_AFTER_RUN");
        public static bool? WriteLog => GetBoolEnvironmentVariable("AFTU_WRITE_LOG");

        private static bool? GetBoolEnvironmentVariable(string variable)
        {
            var value = Environment.GetEnvironmentVariable(variable);
            if (string.IsNullOrEmpty(value)) return null;
            return bool.TryParse(value, out var result) && result;
        }
    }
}