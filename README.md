# Azure Functions Test Utils

> MSTest wrapper for Azure Functions Core Tools for easy bootstrapping of integration tests.

## Getting started

Add the following base class [BaseFunctionTest](src/SampleFunctions.IntegrationTests/BaseFunctionTest.cs) to your test project, this allows us to hook into MSTest. All your test needs to inherit from this class.

In the `AssemblyInit` method, set your function app output path, e.g `src/SampleFunctions/bin/Debug/netcoreapp3.1`.

On startup, paths for `FuncHostPath` (azure-functions-core-tools) and `DotNetPath` will try to be resolved from the system if they are left unconfigured.

### Planned support

- Running function host
- Running Azurite for storage account emulation

**Future:**

- Running CosmosDB emulator

# Example

```csharp
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
        /**
        Starts a singe function instead of all. Omit this attribute to run all fuctions in the app.
        **/
        [StartFunctions(nameof(GetHello))]
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
```

## Usage

### Starting functions

The `StartFunctions` attribute allows you to select functions to run for the test.

**Run a single function**

```csharp
[TestMethod]
[StartFunctions(nameof(GetOrder))]
public async Task GerOrder_Returns_Ok()
{
    var response = await Fixture.Client.GetAsync("/api/order/2556");
    Assert.IsTrue(response.IsSuccessStatusCode);
}
```

**Run multiple functions**

```csharp
[TestMethod]
[StartFunctions(nameof(GetOrders), nameof(GetOrderItems))]
public async Task GetOrderItems_Returns_Ok()
{
    var response = await Fixture.Client.GetAsync("/api/orders");
    var responseText = await response.Content.ReadAsStringAsync();

    dynamic reponseObj = JsonConvert.DeserializeObject(responseText);
    var orderId = responseObj[0].orderId;
    var itemsResponse = await Fixture.Client.GetAsync($"/api/order/{orderId}/items");

    Assert.IsTrue(response.itemsResponse);
}
```

### Authorization

By default function authorization is not enabled when running locally. This can be enabled by adding the attribute `UseFunctionAuth`.
