using System;
using System.Diagnostics;
using System.Linq;
using Azure.Data.Tables;
using Azure.Storage.Blobs;
using Azure.Storage.Queues;

namespace JoachimDalen.AzureFunctions.TestUtils.Handlers
{
    public class AzuriteHandler : IDisposable
    {
        private Process _azuriteProcess;
        private readonly object _azuriteLock = new object();
        private readonly BlobServiceClient _blobServiceClient;
        private readonly QueueServiceClient _queueServiceClient;
        private readonly TableServiceClient _tableServiceClient;

        private readonly string[] _azureContainers =
        {
            "azure-webjobs-secrets",
            "azure-webjobs-hosts"
        };

        public AzuriteHandler()
        {
            _blobServiceClient = new BlobServiceClient(Context.Data.Settings.StorageConnectionString);
            _queueServiceClient = new QueueServiceClient(Context.Data.Settings.StorageConnectionString);
            _tableServiceClient = new TableServiceClient(Context.Data.Settings.StorageConnectionString);
        }

        private string GetBlobArguments() =>
            $"--blobHost 127.0.0.1 --blobPort {Context.Data.Settings.BlobPort}";

        private string GetQueueArguments() =>
            $"--queueHost 127.0.0.1 --queuePort {Context.Data.Settings.QueuePort}";

        private string GetTableArguments() =>
            $"--tableHost 127.0.0.1 --tablePort {Context.Data.Settings.TablePort}";

        private string GetLocation() => $"--location \"{Context.Data.Settings.DataDirectory}\"";
        private string GetSilentArguments() => Context.Data.Settings.RunAzuriteSilent ? "--silent" : null;

        public void InitAzuriteHost()
        {
            if (Context.Data.Settings.RunAzurite)
            {
                var azuriteHost = Context.Data.Settings.AzuritePath;
                var writeLog = EnvironmentHelper.WriteLog.HasValue && EnvironmentHelper.WriteLog.Value;

                var arguments = new[]
                {
                    GetBlobArguments(),
                    GetQueueArguments(),
                    GetTableArguments(),
                    GetSilentArguments(),
                    GetLocation()
                }.Where(x => !string.IsNullOrEmpty(x)).ToArray();


                _azuriteProcess = new Process
                {
                    StartInfo =
                    {
                        FileName = azuriteHost,
                        Arguments = string.Join(" ", arguments),
                        RedirectStandardError = writeLog,
                        RedirectStandardOutput = writeLog,
                        UseShellExecute = false
                    },
                };

                if (writeLog)
                {
                    _azuriteProcess.ErrorDataReceived += (sender, args) => Logger.Log("azurite-error", args.Data);
                    _azuriteProcess.OutputDataReceived += (sender, args) => Logger.Log("azurite", args.Data);
                }

                if (!_azuriteProcess.Start())
                {
                    throw new InvalidOperationException("Could not start Azurite host.");
                }

                if (writeLog)
                {
                    _azuriteProcess.BeginErrorReadLine();
                    _azuriteProcess.BeginOutputReadLine();
                }

                if (_azuriteProcess.HasExited)
                {
                    var error = _azuriteProcess.StandardError.ReadToEnd();
                    var output = _azuriteProcess.StandardOutput.ReadToEnd();
                    throw new Exception($"Failed to start Azurite. Out: {output} ; Error: {error}");
                }
            }


            //WaitForHostStart();

            CreateQueues(Context.Data.Queues);
            CreateBlobContainers(Context.Data.BlobContainers);
            CreateTables(Context.Data.Tables);
        }

        #region Blob

        public void CreateBlobContainers(string[] containerNames)
        {
            if (containerNames == null || !containerNames.Any()) return;
            foreach (var container in containerNames)
            {
                _blobServiceClient.GetBlobContainerClient(container).CreateIfNotExists();
            }
        }

        public void ClearBlobContainers()
        {
            var pages = _blobServiceClient.GetBlobContainers().AsPages();
            foreach (var page in pages)
            {
                foreach (var containerItem in page.Values)
                {
                    if (Context.Data.Settings.PersistAzureContainers && _azureContainers.Contains(containerItem.Name))
                    {
                        continue;
                    }

                    _blobServiceClient.DeleteBlobContainer(containerItem.Name);
                }
            }
        }

        #endregion

        #region Queues

        public void ClearQueues()
        {
            var pages = _queueServiceClient.GetQueues().AsPages();
            foreach (var page in pages)
            {
                foreach (var queueItem in page.Values)
                {
                    _queueServiceClient.DeleteQueue(queueItem.Name);
                }
            }
        }

        public void CreateQueues(string[] queueNames)
        {
            if (queueNames == null || !queueNames.Any()) return;

            foreach (var queueName in queueNames)
            {
                _queueServiceClient.GetQueueClient(queueName).CreateIfNotExists();
            }
        }

        #endregion

        #region Tables

        public void CreateTables(string[] tableNames)
        {
            if (tableNames == null || !tableNames.Any()) return;
            foreach (var tableName in tableNames)
            {
                _tableServiceClient.CreateTableIfNotExists(tableName);
            }
        }

        public void ClearTables()
        {
            var pages = _tableServiceClient.Query().AsPages();
            foreach (var page in pages)
            {
                foreach (var tableItem in page.Values)
                {
                    _tableServiceClient.DeleteTable(tableItem.Name);
                }
            }
        }

        #endregion

        public void Stop()
        {
            lock (_azuriteLock)
            {
                if (_azuriteProcess == null) return;
                if (!_azuriteProcess.HasExited)
                {
                    _azuriteProcess.Kill();
                }

                _azuriteProcess.Dispose();
                _azuriteProcess = null;
            }
        }

        public void Dispose()
        {
            Stop();
        }
    }
}