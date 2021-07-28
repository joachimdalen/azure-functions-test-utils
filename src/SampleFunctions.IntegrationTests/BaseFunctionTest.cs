using AzureFunctions.TestUtils;
using AzureFunctions.TestUtils.Settings;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SampleFunctions.IntegrationTests
{
    [TestClass]
    public class BaseFunctionTest : FunctionTestClass
    {
        [AssemblyInitialize]
        public static void AssemblyInit(TestContext context)
        {
            var settings = new TestUtilsSettings
            {
                FuncAppPath = "../../../../SampleFunctions/bin/Debug/netcoreapp3.1",
                RunAzurite = true,
            };
            AssemblyInitialize(context, settings);
        }

        [ClassInitialize(InheritanceBehavior.BeforeEachDerivedClass)]
        public static void TestFixtureSetup(TestContext context)
        {
            ClassInitialize(context);
        }

        [TestInitialize]
        public void Setup()
        {
            TestInitialize();
        }

        [AssemblyCleanup]
        public static void AssemblyTearDown()
        {
            AssemblyCleanup();
        }

        [ClassCleanup(InheritanceBehavior.BeforeEachDerivedClass)]
        public static void TestFixtureTearDown()
        {
            ClassCleanup();
        }

        [TestCleanup]
        public void TearDown()
        {
            TestCleanup();
        }
    }
}