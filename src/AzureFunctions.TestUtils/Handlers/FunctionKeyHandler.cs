using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Azure.Storage.Blobs;
using AzureFunctions.TestUtils.Models;
using Newtonsoft.Json;

namespace AzureFunctions.TestUtils.Handlers
{
    internal class FunctionKeyHandler
    {
        private const string BlobContainerName = "azure-webjobs-secrets";
        private BlobContainerClient _blobContainerClient;

        public void Init()
        {
            var blobServiceClient = new BlobServiceClient(Context.Data.Settings.StorageConnectionString);
            _blobContainerClient = blobServiceClient.GetBlobContainerClient(BlobContainerName);
            _blobContainerClient.CreateIfNotExists();
        }

        #region Helpers

        private static string GenerateSecret()
        {
            using (var rng = RandomNumberGenerator.Create())
            {
                byte[] data = new byte[40];
                rng.GetBytes(data);
                string secret = Convert.ToBase64String(data);
                return secret.Replace('+', 'a');
            }
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

        #endregion

        private void CreateFunctionKeys(string function, FunctionKey[] keys)
        {
            var path = Path.Join(GetFunctionHostId(), $"{function.ToLower()}.json");

            _blobContainerClient.DeleteBlobIfExists(path);

            var secrets = keys.Select(x => new FunctionSecret
            {
                Name = x.Name,
                Value = x.Value ?? GenerateSecret(),
            }).ToArray();

            var model = new FunctionSecretRoot
            {
                Keys = secrets
            };

            var jsonContent = JsonConvert.SerializeObject(model, Formatting.Indented);
            using var ms = new MemoryStream(Encoding.UTF8.GetBytes(jsonContent));
            _blobContainerClient.UploadBlob(path, ms);
        }

        public void CreateFunctionKeys(FunctionKey[] functionKeys)
        {
            var functionNames = functionKeys.Select(x => x.FunctionName).Distinct();

            foreach (var functionName in functionNames)
            {
                var secrets = functionKeys.Where(x => x.FunctionName == functionName).ToArray();
                CreateFunctionKeys(functionName, secrets);
            }
        }

        public void CreateHostKeys(FunctionKey[] hostKeys)
        {
            var masterKey = hostKeys.FirstOrDefault(x => x.Scope == FunctionAuthLevel.Admin);
            var functionKeys = hostKeys.Where(x =>
                x.Scope == FunctionAuthLevel.Function && string.IsNullOrEmpty(x.FunctionName));
            var systemKeys = hostKeys.Where(x => x.Scope == FunctionAuthLevel.System);
            var path = Path.Join(GetFunctionHostId(), "host.json");
            var client = _blobContainerClient.GetBlobClient(path);
            FunctionSecretRoot root;
            if (client.Exists())
            {
                var blob = client.DownloadContent();
                var text = Encoding.UTF8.GetString(blob.Value.Content);
                root = JsonConvert.DeserializeObject<FunctionSecretRoot>(text);
            }
            else
            {
                root = new FunctionSecretRoot
                {
                    MasterKey = new FunctionSecret
                    {
                        Name = "_master",
                        Value = masterKey?.Value ?? GenerateSecret()
                    },
                    FunctionKeys = functionKeys.Select(x => new FunctionSecret
                    {
                        Name = x.Name,
                        Value = x.Value ?? GenerateSecret()
                    }).ToArray(),
                    SystemKeys = systemKeys.Select(x => new FunctionSecret
                    {
                        Name = x.Name,
                        Value = x.Value ?? GenerateSecret()
                    }).ToArray(),
                };
            }

            var jsonContent = JsonConvert.SerializeObject(root, Formatting.Indented);
            using var ms = new MemoryStream(Encoding.UTF8.GetBytes(jsonContent));
            _blobContainerClient.UploadBlob(path, ms);
        }
    }
}