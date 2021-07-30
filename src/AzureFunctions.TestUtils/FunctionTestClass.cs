using System;
using System.IO;
using System.Linq;
using System.Reflection;
using AzureFunctions.TestUtils.Extensions;
using AzureFunctions.TestUtils.Handlers;
using AzureFunctions.TestUtils.Models;
using AzureFunctions.TestUtils.Settings;
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
            Logger.ClearLogs();
            settings.DataDirectory = GetFirstNotNullOrEmpty(EnvironmentHelper.DataDirectory, settings.DataDirectory);
            settings.RunAzurite = GetFirstBool(EnvironmentHelper.RunAzurite, settings.RunAzurite);
            settings.ClearStorageAfterRun =
                GetFirstBool(EnvironmentHelper.ClearStorageAfterRun, settings.ClearStorageAfterRun);
            settings.UseAzuriteStorage = GetFirstBool(EnvironmentHelper.UseAzuriteStorage, settings.UseAzuriteStorage);
            settings.PersistAzureContainers =
                GetFirstBool(EnvironmentHelper.PersistAzureContainers, settings.PersistAzureContainers);

            if (string.IsNullOrEmpty(settings.DotNetPath))
            {
                settings.DotNetPath =
                    GetFirstNotNullOrEmpty(EnvironmentHelper.DotNetPath, ExecutableResolver.GetDotNetPath());
            }

            if (string.IsNullOrEmpty(settings.AzuritePath))
            {
                settings.AzuritePath =
                    GetFirstNotNullOrEmpty(EnvironmentHelper.AzuritePath, ExecutableResolver.GetAzuritePath());
            }

            if (string.IsNullOrEmpty(settings.FuncHostPath))
            {
                settings.FuncHostPath = GetFirstNotNullOrEmpty(EnvironmentHelper.FuncHostPath,
                    ExecutableResolver.GetFunctionHostPath());
                if (!File.Exists(settings.FuncHostPath))
                {
                    throw new Exception(
                        $"FuncHostPath must be the full path, including func.dll. Currently {settings.FuncHostPath}");
                }
            }

            settings.FuncAppPath = GetFirstNotNullOrEmpty(EnvironmentHelper.FuncAppPath, settings.FuncAppPath);
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
            if (!Context.Data.IsInitialized) return;

            var currentTest = GetCurrentTestMethod();
            var functionKeys = currentTest.GetFunctionKeys()?.Select(x => new FunctionKey
            {
                FunctionName = x.FunctionName,
                Name = x.Name,
                Value = x.Value,
                Scope = x.AuthLevel
            }).ToArray() ?? Array.Empty<FunctionKey>();
            var functionsToRun = currentTest.GetStartFunctions()?.FirstOrDefault()?.FunctionNames;
            var enableAuth = currentTest.GetUseFunctionAuth().Any();
            var queues = currentTest.GetQueues().SelectMany(x => x.QueueNames).Distinct().ToArray();
            var containers = currentTest.GetBlobContainers().SelectMany(x => x.ContainerNames).Distinct().ToArray();
            var tables = currentTest.GetTables().SelectMany(x => x.TableNames).Distinct().ToArray();
            Context.Data.FunctionKeys = functionKeys;
            Context.Data.FunctionsToRun = functionsToRun;
            Context.Data.EnableAuth = enableAuth;
            Context.Data.Queues = queues;
            Context.Data.BlobContainers = containers;
            Context.Data.Tables = tables;

            if (Context.Data.Settings.UseAzuriteStorage)
            {
                Fixture.InitStorage();
                Fixture.InitFunctionKeys();
            }

            Fixture.InitFunctionHost(TestContext);
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

            if (Context.Data.Settings.ClearStorageAfterRun)
            {
                Fixture.ClearStorage();
            }

            if (Context.Data.Settings.RunAzurite)
            {
                Fixture.StopAzurite();
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

        private static string GetFirstNotNullOrEmpty(string first, string second)
        {
            if (!string.IsNullOrEmpty(first)) return first;
            return !string.IsNullOrEmpty(second) ? second : null;
        }

        private static bool GetFirstBool(bool? first, bool defaultValue)
        {
            return first ?? defaultValue;
        }

        #endregion
    }
}