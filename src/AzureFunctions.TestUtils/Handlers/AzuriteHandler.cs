using System;
using System.Diagnostics;
using System.Linq;
using Azure.Storage.Blobs;
using Azure.Storage.Queues;

namespace AzureFunctions.TestUtils.Handlers
{
    public class AzuriteHandler : IDisposable
    {
        private Process _azuriteProcess;
        private readonly object _azuriteLock = new object();
        private readonly BlobServiceClient _blobServiceClient;
        private readonly QueueServiceClient _queueServiceClient;

        public AzuriteHandler()
        {
            _blobServiceClient = new BlobServiceClient(Context.Data.Settings.StorageConnectionString);
            _queueServiceClient = new QueueServiceClient(Context.Data.Settings.StorageConnectionString);
        }

        private string GetBlobArguments() =>
            $"--blobHost 127.0.0.1 --blobPort {Context.Data.Settings.BlobPort}";

        private string GetQueueArguments() =>
            $"--queueHost 127.0.0.1 --queuePort {Context.Data.Settings.QueuePort}";

        private string GetTableArguments() =>
            $"--tableHost 127.0.0.1 --tablePort {Context.Data.Settings.TablePort}";

        private string GetLocation() => $"--location \"{Context.Data.Settings.DataDirectory}\"";

        public void InitAzuriteHost()
        {
            var azuriteHost = Context.Data.Settings.AzuritePath;

            var arguments = new[]
            {
                GetBlobArguments(),
                GetQueueArguments(),
                GetTableArguments(),
                GetLocation()
            }.Where(x => !string.IsNullOrEmpty(x)).ToArray();


            _azuriteProcess = new Process
            {
                StartInfo =
                {
                    FileName = azuriteHost,
                    Arguments = string.Join(" ", arguments),
                    RedirectStandardError = true,
                    RedirectStandardOutput = true
                },
            };

            /*var success = _azuriteProcess.Start();
            if (!success)
            {
                throw new InvalidOperationException("Could not start Azurite host.");
            }

            var error = _azuriteProcess.StandardError.ReadToEnd();
            var output = _azuriteProcess.StandardOutput.ReadToEnd();

            if (_azuriteProcess.HasExited)
            {
                throw new Exception("Failed to start azurite");
            }*/


            //WaitForHostStart();

            CreateQueues(Context.Data.Queues);
            CreateBlobContainers(Context.Data.BlobContainers);
        }

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

        public void ClearBlobContainers()
        {
            var pages = _blobServiceClient.GetBlobContainers().AsPages();
            foreach (var page in pages)
            {
                foreach (var queueItem in page.Values)
                {
                    _blobServiceClient.DeleteBlobContainer(queueItem.Name);
                }
            }
        }

        public void CreateQueues(string[] queueNames)
        {
            foreach (var queueName in queueNames)
            {
                _queueServiceClient.CreateQueue(queueName);
            }
        }

        public void CreateBlobContainers(string[] containerNames)
        {
            foreach (var container in containerNames)
            {
                _blobServiceClient.CreateBlobContainer(container);
            }
        }


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