using System;
using System.IO;
using System.Linq;
using System.Reflection;
using AzureFunctions.TestUtils.Extensions;
using AzureFunctions.TestUtils.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AzureFunctions.TestUtils
{
    public abstract class FunctionTestClass
    {
        private static readonly ExecutableResolver ExecutableResolver = new ExecutableResolver();
        protected static FunctionTestFixture Fixture;
        public TestContext TestContext { get; set; }

        #region MsTest Events

        protected static void AssemblyInitialize(TestContext context)
        {
            AssemblyInitialize(context, new TestUtilsSettings());
        }

        protected static void AssemblyInitialize(TestContext context, TestUtilsSettings settings)
        {
            if (string.IsNullOrEmpty(settings.DotNetPath))
            {
                settings.DotNetPath = ExecutableResolver.GetDotNetPath();
            }

            if (string.IsNullOrEmpty(settings.FuncHostPath))
            {
                settings.FuncHostPath = ExecutableResolver.GetFunctionHostPath();
            }

            if (string.IsNullOrEmpty(settings.FuncAppPath))
            {
                throw new ArgumentException("Function app path must be given");
            }

            if (settings.FuncAppPath.StartsWith("."))
            {
                settings.FuncAppPath = Path.GetFullPath(settings.FuncAppPath);
            }

            Context.Data.Settings = settings;
        }


        protected static void ClassInitialize(TestContext context)
        {
            lock (Context.Data)
            {
                if (!Context.Data.IsInitialized)
                {
                    Fixture = new FunctionTestFixture();
                    Context.Data.IsInitialized = true;
                }
            }
        }


        protected void TestInitialize()
        {
            if (Context.Data.IsInitialized)
            {
                var currentTest = GetCurrentTestMethod();
                var functionKeys = currentTest.GetFunctionKeys()?.Select(x => new FunctionKey
                {
                    Key = x.Key,
                    Level = x.Level
                }).ToArray();
                var functionsToRun = currentTest.GetStartFunctions()?.FirstOrDefault()?.FunctionNames;
                var enableAuth = currentTest.GetUseFunctionAuth();

                Context.Data.FunctionKeys = functionKeys;
                Context.Data.FunctionsToRun = functionsToRun;
                Context.Data.EnableAuth = enableAuth != null;

                Fixture.InitHost(TestContext);
            }
        }

        protected static void AssemblyCleanup()
        {
            // Executes once after the test run. (Optional)
        }

        protected static void ClassCleanup()
        {
            if (!Context.Data.IsInitialized) return;

            Context.Reset();
            Context.Data.IsInitialized = false;
        }

        protected void TestCleanup()
        {
            Fixture.StopHost();
            Context.Reset();
        }

        private MethodInfo GetCurrentTestMethod()
        {
            var currentlyRunningClassType = GetType().Assembly.GetTypes()
                .FirstOrDefault(f => f.FullName == TestContext.FullyQualifiedTestClassName);
            return currentlyRunningClassType?.GetMethod(TestContext.TestName);
        }

        #endregion
    }
}