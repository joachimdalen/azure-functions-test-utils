using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
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
            await Task.Delay(5000);
            Assert.IsTrue(response.IsSuccessStatusCode);
        }
    }
}