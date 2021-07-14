﻿using System;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Threading;
using AzureFunctions.TestUtils.Handlers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AzureFunctions.TestUtils
{
    public sealed class FunctionTestFixture : IDisposable
    {
        private readonly object _functionLock = new object();
        private readonly AzuriteHandler AzuriteHandler;
        private FunctionKeyHandler KeyHandler = new FunctionKeyHandler();
        private Process _funcHostProcess;
        public readonly HttpClient Client = new HttpClient();

        public FunctionTestFixture()
        {
            Client.BaseAddress = new Uri($"http://localhost:{Context.Data.Settings.FuncHostPort}");
            AzuriteHandler = new AzuriteHandler();
        }

        public void Dispose()
        {
            StopFunctionHost();
        }


        private string GetRunFunctionsArgument()
        {
            if (Context.Data.FunctionsToRun == null || !Context.Data.FunctionsToRun.Any())
            {
                return null;
            }

            return $"--functions {string.Join(" ", Context.Data.FunctionsToRun.Distinct())}";
        }

        private string GetEnableAuthArgument()
        {
            return Context.Data.EnableAuth ? null : "--enableAuth";
        }

        public void InitStorage(TestContext testContext)
        {
            AzuriteHandler.InitAzuriteHost();
        }

        public void InitFunctionHost(TestContext context)
        {
            var dotnetExePath = Context.Data.Settings.DotNetPath;
            var functionHostPath = Context.Data.Settings.FuncHostPath;
            var functionAppFolder = Context.Data.Settings.FuncAppPath;
            var functionHostPort = Context.Data.Settings.FuncHostPort;

            var arguments = new[]
            {
                $"\"{functionHostPath}\" start",
                $"-p {functionHostPort}",
                "--timeout 5",
                GetRunFunctionsArgument(),
                GetEnableAuthArgument()
            }.Where(x => !string.IsNullOrEmpty(x)).ToArray();


            context.WriteLine(string.Join(" ", arguments));

            lock (_functionLock)
            {
                _funcHostProcess = new Process
                {
                    StartInfo =
                    {
                        FileName = dotnetExePath,
                        Arguments = string.Join(" ", arguments),
                        WorkingDirectory = functionAppFolder
                    },
                };
                _funcHostProcess.StartInfo.EnvironmentVariables["AzureWebJobsStorage"] =
                    Context.Data.Settings.StorageConnectionString;
                var success = _funcHostProcess.Start();
                if (!success)
                {
                    throw new InvalidOperationException("Could not start Azure Functions host.");
                }
            }

            WaitForHostStart();
        }


        private void WaitForHostStart()
        {
            Thread.Sleep(5000);
            // Task.Run(async () => await Retry.Try(async () =>
            // {
            //     var response = await Client.GetAsync("/admin/host/status");
            //     if (!response.IsSuccessStatusCode) throw new Exception();
            //     return;
            // }));
        }

        public void StopFunctionHost()
        {
            lock (_functionLock)
            {
                if (_funcHostProcess == null) return;
                if (!_funcHostProcess.HasExited)
                {
                    _funcHostProcess.Kill();
                }

                _funcHostProcess.Dispose();
                _funcHostProcess = null;
            }
        }

        public void ClearStorage()
        {
            AzuriteHandler.ClearQueues();
            AzuriteHandler.ClearBlobContainers();
        }

        public void StopAzurite()
        {
            AzuriteHandler.Stop();
        }
    }
}