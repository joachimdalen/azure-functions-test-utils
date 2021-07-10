using System;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using AzureFunctions.TestUtils.Handlers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AzureFunctions.TestUtils
{
    public sealed class FunctionTestFixture : IDisposable
    {
        private readonly object _functionLock = new object();
        private Process _funcHostProcess;
        private FunctionKeyHandler KeyHandler = new FunctionKeyHandler();
        public readonly HttpClient Client = new HttpClient();

        public FunctionTestFixture()
        {
            Client.BaseAddress = new Uri($"http://localhost:{Context.Data.Settings.FuncHostPort}");
        }

        public void Dispose()
        {
            StopHost();
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

        public void InitHost(TestContext context)
        {
            StopHost();

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

        public void StopHost()
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
    }
}