using System.IO;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Blob;

namespace SampleFunctions.Functions
{
    public static class SendOrderConfirmation
    {
        [FunctionName(nameof(SendOrderConfirmation))]
        public static async Task RunAsync(
            [QueueTrigger("orders")] string myQueueItem,
            IBinder binder,
            ILogger log)
        {
            var blob = await binder.BindAsync<CloudBlockBlob>(new BlobAttribute("order-confirmations/order-1.json",
                FileAccess.Write));
            await blob.UploadTextAsync(myQueueItem);
        }
    }
}