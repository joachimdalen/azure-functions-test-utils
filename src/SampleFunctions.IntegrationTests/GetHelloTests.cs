using System.Net;
using System.Threading.Tasks;
using AzureFunctions.TestUtils.Attributes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SampleFunctions.Functions;

namespace SampleFunctions.IntegrationTests
{
    [TestClass]
    public class GetHelloTests : BaseFunctionTest
    {
        [TestMethod]
        [StartFunctions(nameof(GetHello))]
        [UseFunctionKey(nameof(GetHello), "main", "helloValue")]
        public async Task GetHello_NoQueryParams_ReturnsBadRequest()
        {
            var response = await Fixture.Client.GetAsync("/api/hello");

            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [TestMethod]
        [StartFunctions(nameof(GetHello))]
        public async Task GetHello_QueryParams_ReturnsOk()
        {
            var response = await Fixture.Client.GetAsync("/api/hello?name=James");

            Assert.IsTrue(response.IsSuccessStatusCode);
            var responseText = await response.Content.ReadAsStringAsync();
            Assert.AreEqual("Hello, James", responseText);
        }
    }
}