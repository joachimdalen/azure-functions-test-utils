using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using AzureFunctions.TestUtils.Asserts;
using AzureFunctions.TestUtils.Attributes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SampleFunctions.Functions;

namespace SampleFunctions.IntegrationTests
{
    [TestClass]
    public class OrdersTests : BaseFunctionTest
    {
        [TestMethod]
        [StartFunctions(nameof(CreateOrder), nameof(SendOrderConfirmation))]
        [UseQueues("orders")]
        [UseBlobContainers("order-confirmations")]
        public async Task CreateOrder_Sends_Confirmation()
        {
            var response = await Fixture.Client.PostAsync("/api/orders", new StringContent(""));
            Assert.IsTrue(response.IsSuccessStatusCode);

            await Task.Delay(5000);

            Assert.That.ContainerExists("UseDevelopmentStorage=true", "order-confirmations");
            //Assert.That.ContainerExists("order-confirmations");
            Assert.That.BlobExists("UseDevelopmentStorage=true", "order-confirmations", "order-1.json");
            //Assert.That.BlobExists("order-confirmations", "order-1.json");
        }

        [TestMethod]
        [StartFunctions(nameof(CreateOrder))]
        [UseQueues("orders")]
        [UseBlobContainers("order-confirmations")]
        public async Task CreateOrder_Input_AddsMessageToQueue()
        {
            var response = await Fixture.Client.PostAsync("/api/orders", new StringContent(""));
            Assert.IsTrue(response.IsSuccessStatusCode);

            await Task.Delay(5000);

            Assert.That.QueueHasMessageCount("UseDevelopmentStorage=true", "orders", 1);
        }
    }
}