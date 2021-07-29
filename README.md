# Azure Functions Test Utils

> MSTest wrapper for Azure Functions Core Tools for easy bootstrapping of integration tests.

Currently under development to make things work together. When all the intial functionality is in place, a refactor will be done to clean some things up.

## Getting started

Add the following base class [BaseFunctionTest](src/SampleFunctions.IntegrationTests/BaseFunctionTest.cs) to your test project, this allows us to hook into MSTest. All your test needs to inherit from this class.

In the `AssemblyInit` method, set your function app output path, e.g `src/SampleFunctions/bin/Debug/netcoreapp3.1`.

On startup, paths for `FuncHostPath` (azure-functions-core-tools) and `DotNetPath` will try to be resolved from the system if they are left unconfigured.

### Planned support

- Running function host (In progress)
- Running Azurite for storage account emulation (In progress)

**Future:**

- Running CosmosDB emulator
- Support for running side services as Docker containers

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

By default function authorization is not enabled when running locally. This can be enabled by adding the attribute `UseFunctionAuth`. Function and host keys can be pre generated using the attributes `UseFunctionKey`.

```csharp
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
```

**Scoped function key**

```csharp
[UseFunctionKey(FunctionAuthLevel.Function, "main", nameof(GetHello), "value1")]
```

**Global function key**

```csharp
[UseFunctionKey(FunctionAuthLevel.Function, "global-key", "value1")]
```

**Master key**

```csharp
[UseFunctionKey(FunctionAuthLevel.Admin, "ignored-value", "value1")]
```

**System key**

```csharp
[UseFunctionKey(FunctionAuthLevel.System, "system-key", "value1")]
```

### Storage

When running Azurite, you can also initialize the emulator with Blob Containers, Queues and Tables.

```csharp
[TestMethod]
[StartFunctions(nameof(CreateOrder), nameof(SendOrderConfirmation))]
[UseQueues("orders")]
[UseBlobContainers("order-confirmations")]
public async Task CreateOrder_Sends_Confirmation()
{
    var response = await Fixture.Client.PostAsync("/api/orders", new StringContent(""));
    await Task.Delay(5000); // Wait for message to be sent on queue

    Assert.IsTrue(response.IsSuccessStatusCode);
    Assert.That.BlobExists("UseDevelopmentStorage=true", "order-confirmations", "order-1.json");
}
```

### Custom Asserts

| Area  | Assert               | Description                                                                 |
| ----- | -------------------- | --------------------------------------------------------------------------- |
| Blob  | BlobExists           | Assert that a given blob item exists in the container                       |
| Blob  | ContainerExists      | Assert that a blob container exists                                         |
| Queue | QueueIsEmpty         | Assert that the given queue does not contain any messages                   |
| Queue | QueueHasMessageCount | Assert that the given queue contains the given amount of messages           |
| Queue | QueueHasMessage      | Assert that the queue contains at least one message matching the expression |

### Settings

| Setting                  | Environment                     | Description                                                                               |
| ------------------------ | ------------------------------- | ----------------------------------------------------------------------------------------- |
| `DotNetPath`             | `AFTU_DOT_NET_PATH`             | Path to dotnet executable                                                                 |
| `FuncHostPath`           | `AFTU_FUNC_HOST_PATH`           | Path to func.dll                                                                          |
| `FuncAppPath`            | `AFTU_FUNC_APP_PATH`            | Directory path to function app to run                                                     |
| `AzuritePath`            | `AFTU_AZURITE_PATH`             | Path to Azurite executable                                                                |
| `FuncHostPort`           | `AFTU_FUNC_HOST_PORT`           | Port to run function app on                                                               |
| `DataDirectory`          | `AFTU_DATA_DIRECTORY`           | Directory to store Azurite data in. Defaults to TestResults directory for the given test  |
| `UseAzuriteStorage`      | `AFTU_USE_AZURITE_STORAGE`      | Use azurite storage emulator. This should be true if using any storage related attributes |
| `RunAzurite`             | `AFTU_RUN_AZURITE`              | Start Azurite alongside function app host                                                 |
| `PersistAzureContainers` | `AFTU_PERSIST_AZURE_CONTAINERS` | Decides if containers created by the function app should be persisted after test run.     |
| `ClearStorageAfterRun`   | `AFTU_CLEAR_STORAGE_AFTER_RUN`  | Clears Azurite after each test run and provides a clean container for new runs            |
