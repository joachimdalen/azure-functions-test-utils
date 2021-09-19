using System.Net.Http;
using System.Threading.Tasks;
using JoachimDalen.AzureFunctions.TestUtils.Asserts;
using JoachimDalen.AzureFunctions.TestUtils.Attributes;
using JoachimDalen.AzureFunctions.TestUtils.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SampleFunctions.Functions;

namespace SampleFunctions.IntegrationTests
{
    [TestClass]
    public class OrdersTests : BaseFunctionTest
    {
        [TestMethod]
        [UseFunctionAuth]
        [StartFunctions(nameof(CreateOrder), nameof(SendOrderConfirmation))]
        [UseQueues("orders")]
        [UseBlobContainers("order-confirmations")]
        [UseFunctionKey(FunctionAuthLevel.Function, "main", nameof(GetHello), "value1")]
        [UseFunctionKey(FunctionAuthLevel.Admin, "hhh", "master-key-value")]
        [UseFunctionKey(FunctionAuthLevel.System, "global-system-key", "global-system-key-value")]
        public async Task CreateOrder_Sends_Confirmation()
        {
            var response =
                await Fixture.Client.PostAsync("/api/orders?code=global-system-key-value", new StringContent(""));
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
        public async Task CreateOrder_Input_AddsSingleMessageToQueue()
        {
            var response = await Fixture.Client.PostAsync("/api/orders", new StringContent(""));
            Assert.IsTrue(response.IsSuccessStatusCode);

            await Task.Delay(5000);

            Assert.That.QueueHasMessageCount("UseDevelopmentStorage=true", "orders", 1);
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

            Assert.That.QueueHasMessage<Order>("orders", x => x.OrderId == 1);
        }

        private class Order
        {
            public int OrderId { get; set; }
        }
    }
}