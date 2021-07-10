using System;
using System.Linq;
using System.Text;

namespace AzureFunctions.TestUtils.Handlers
{
    internal class FunctionKeyHandler
    {
        private const string BlobContainerName = "azure-webjobs-secrets";

        public FunctionKeyHandler()
        {
            var blobPath = $"{BlobContainerName}";
            var host = GetFunctionHostId();
        }

        private static int GetStableHash(string value)
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            unchecked
            {
                int hash = 23;
                foreach (char c in value)
                {
                    hash = (hash * 31) + c;
                }

                return hash;
            }
        }

        private static string GetFunctionHostId()
        {
            var sanitizedMachineName = Environment.MachineName
                .Where(char.IsLetterOrDigit)
                .Aggregate(new StringBuilder(), (b, c) => b.Append(c)).ToString();
            var hostId = $"{sanitizedMachineName}-{Math.Abs(GetStableHash(Context.Data.Settings.FuncAppPath))}";
            if (string.IsNullOrEmpty(hostId)) return hostId?.ToLowerInvariant().TrimEnd('-');
            if (hostId.Length > 32)
            {
                hostId = hostId.Substring(0, 32);
            }

            return hostId?.ToLowerInvariant().TrimEnd('-');
        }
    }
}