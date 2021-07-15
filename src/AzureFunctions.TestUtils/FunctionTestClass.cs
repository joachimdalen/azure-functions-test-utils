using System;
using System.IO;
using System.Linq;
using System.Reflection;
using AzureFunctions.TestUtils.Extensions;
using AzureFunctions.TestUtils.Handlers;
using AzureFunctions.TestUtils.Models;
using AzureFunctions.TestUtils.Settings;
using Microsoft.Extensions.Azure;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AzureFunctions.TestUtils
{
    public abstract class FunctionTestClass
    {
        private static readonly ExecutableResolver ExecutableResolver = new ExecutableResolver();
        private static readonly ConnectionStringHandler ConnectionStringHandler = new ConnectionStringHandler();
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

            if (string.IsNullOrEmpty(settings.AzuritePath))
            {
                settings.AzuritePath = ExecutableResolver.GetAzuritePath();
            }

            if (string.IsNullOrEmpty(settings.FuncHostPath))
            {
                settings.FuncHostPath = ExecutableResolver.GetFunctionHostPath() + ".dll";

                if (!settings.FuncHostPath.EndsWith("func.dll"))
                {
                    if (settings.FuncHostPath.EndsWith("/"))
                        settings.FuncHostPath += "func.dll";
                    else
                        settings.FuncHostPath += "/func.dll";
                }

                if (!File.Exists(settings.FuncHostPath))
                {
                    throw new Exception("FuncAppPath must be the full path, including func.dll");
                }
            }

            if (string.IsNullOrEmpty(settings.FuncAppPath))
            {
                throw new ArgumentException("Function app path must be given");
            }

            if (settings.FuncAppPath.StartsWith("."))
            {
                settings.FuncAppPath = Path.GetFullPath(settings.FuncAppPath);
            }

            if (string.IsNullOrEmpty(settings.StorageConnectionString) && string.IsNullOrEmpty(settings.AccountKey) &&
                string.IsNullOrEmpty(settings.AccountName))
            {
                settings.StorageConnectionString = "UseDevelopmentStorage=true";
            }
            else
            {
                if (string.IsNullOrEmpty(settings.AccountName) || string.IsNullOrEmpty(settings.AccountKey))
                {
                    throw new ArgumentException("Account name and account key must be set");
                }

                settings.StorageConnectionString = ConnectionStringHandler.GetConnectionString(settings);
            }

            var path = Path.Combine(context.TestRunDirectory, "azurite-data");
            Directory.CreateDirectory(path);
            settings.DataDirectory = path;
            Context.Data.Settings = settings;
        }

        protected static void ClassInitialize(TestContext context)
        {
            lock (Context.Data)
            {
                if (Context.Data.IsInitialized) return;

                Fixture = new FunctionTestFixture();
                Context.Data.IsInitialized = true;
            }
        }

        protected void TestInitialize()
        {
            if (Context.Data.IsInitialized)
            {
                var currentTest = GetCurrentTestMethod();
                var functionKeys = currentTest.GetFunctionKeys()?.Select(x => new FunctionKey
                {
                    FunctionName = x.FunctionName,
                    Name = x.Name,
                    Value = x.Value
                }).ToArray();
                var functionsToRun = currentTest.GetStartFunctions()?.FirstOrDefault()?.FunctionNames;
                var enableAuth = currentTest.GetUseFunctionAuth().Any();
                var queues = currentTest.GetQueues().FirstOrDefault()?.QueueNames;
                var containers = currentTest.GetBlobContainers().FirstOrDefault()?.ContainerNames;
                var tables = currentTest.GetTables().FirstOrDefault()?.TableNames;
                Context.Data.FunctionKeys = functionKeys;
                Context.Data.FunctionsToRun = functionsToRun;
                Context.Data.EnableAuth = enableAuth;
                Context.Data.Queues = queues;
                Context.Data.BlobContainers = containers;
                Context.Data.Tables = tables;

                if (Context.Data.Settings.RunAzurite)
                {
                    Fixture.InitStorage(TestContext);
                    Fixture.InitFunctionKeys();
                }

                Fixture.InitFunctionHost(TestContext);
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
            Fixture.StopFunctionHost();

            if (Context.Data.Settings.RunAzurite)
            {
                Fixture.ClearStorage();
                //     Fixture.StopAzurite();    
            }

            Context.Reset();
        }

        private MethodInfo GetCurrentTestMethod()
        {
            var currentlyRunningClassType = GetCurrentTestClass();
            return currentlyRunningClassType?.GetMethod(TestContext.TestName);
        }

        private Type GetCurrentTestClass()
        {
            var currentlyRunningClassType = GetType().Assembly.GetTypes()
                .FirstOrDefault(f => f.FullName == TestContext.FullyQualifiedTestClassName);
            return currentlyRunningClassType;
        }

        #endregion
    }
}