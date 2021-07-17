using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using AzureFunctions.TestUtils;
using AzureFunctions.TestUtils.Attributes;
using AzureFunctions.TestUtils.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SampleFunctions.Functions;

namespace SampleFunctions.IntegrationTests
{
    [TestClass]
    public class GetHelloTests : BaseFunctionTest
    {
        [TestMethod]
        [StartFunctions(nameof(GetHello))]
        public async Task GetHello_NoQueryParams_ReturnsBadRequest()
        {
            var response = await Fixture.Client.GetAsync("/api/hello");
            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [TestMethod]
        [UseFunctionAuth]
        [StartFunctions(nameof(GetHello))]
        public async Task GetHello_NoKey_ReturnsUnauthorized()
        {
            var response = await Fixture.Client.GetAsync("/api/hello?name=James");
            Assert.AreEqual(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [TestMethod]
        [UseFunctionAuth]
        [StartFunctions(nameof(GetHello))]
        [UseFunctionKey(FunctionAuthLevel.Function, "main", nameof(GetHello), "helloValue")]
        public async Task GetHello_Key_ReturnsOk()
        {
            var httpRequest = new HttpRequestMessage(HttpMethod.Get, "/api/hello?name=James");
            httpRequest.Headers.Add(AzureFunctionConstants.FunctionKeyHeaderName, "helloValue");
            var response = await Fixture.Client.SendAsync(httpRequest);

            Assert.IsTrue(response.IsSuccessStatusCode);
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