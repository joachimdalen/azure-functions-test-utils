using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace SampleFunctions.Functions
{
    public static class CreateOrder
    {
        [FunctionName(nameof(CreateOrder))]
        public static async Task<IActionResult> RunAsync(
            [HttpTrigger(AuthorizationLevel.System, "post", Route = "orders")]
            HttpRequest req,
            [Queue("orders")] ICollector<string> queue, ILogger log)
        {
            queue.Add(JsonConvert.SerializeObject(new {OrderId = 1}));
            return new OkResult();
        }
    }
}