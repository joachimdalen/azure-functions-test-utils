using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Azure.Storage.Blobs;
using AzureFunctions.TestUtils.Models;
using Newtonsoft.Json;

namespace AzureFunctions.TestUtils.Handlers
{
    internal class FunctionKeyHandler
    {
        private const string BlobContainerName = "azure-webjobs-secrets";
        private readonly BlobContainerClient _blobContainerClient;

        public FunctionKeyHandler()
        {
            var blobPath = $"{BlobContainerName}";
            var host = GetFunctionHostId();
            var blobServiceClient = new BlobServiceClient(Context.Data.Settings.StorageConnectionString);
            _blobContainerClient = blobServiceClient.GetBlobContainerClient(BlobContainerName);
            _blobContainerClient.CreateIfNotExists();
        }

        public void CreateFunctionKey(string function, string keyName, string value)
        {
            var path = Path.Join(GetFunctionHostId(), $"{function.ToLower()}.json");

            _blobContainerClient.DeleteBlobIfExists(path);

            var model = new FunctionSecretRoot
            {
                Keys = new[]
                {
                    new FunctionSecret
                    {
                        Encrypted = false,
                        Name = keyName,
                        Value = value,
                    }
                },
                HostName = "localhost:7071",
                InstanceId = "some-instance",
                Source = "runtime",
                DecryptionKeyId = ""
            };

            var jsonContent = JsonConvert.SerializeObject(model);

            using var ms = new MemoryStream(Encoding.UTF8.GetBytes(jsonContent));
            _blobContainerClient.UploadBlob(path, ms);
        }

        public string CreateHostKey(string keyName, string value)
        {
            return "";
        }

        #region Helpers

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

        #endregion
    }
}