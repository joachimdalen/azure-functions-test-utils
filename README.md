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
