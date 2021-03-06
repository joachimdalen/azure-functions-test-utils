using System;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using JoachimDalen.AzureFunctions.TestUtils.Handlers;
using JoachimDalen.AzureFunctions.TestUtils.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace JoachimDalen.AzureFunctions.TestUtils
{
    public sealed class FunctionTestFixture : IDisposable
    {
        private readonly object _functionLock = new object();
        private readonly AzuriteHandler _azuriteHandler;
        private readonly FunctionKeyHandler _keyHandler;
        private Process _funcHostProcess;
        public readonly HttpClient Client = new HttpClient();

        public FunctionTestFixture()
        {
            Client.BaseAddress = new Uri($"http://localhost:{Context.Data.Settings.FuncHostPort}");
            _azuriteHandler = new AzuriteHandler();
            _keyHandler = new FunctionKeyHandler();
        }

        public void Dispose()
        {
            StopFunctionHost();
            StopAzurite();
        }

        #region Function Host

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
            return Context.Data.EnableAuth ? "--enableAuth" : null;
        }

        public void InitFunctionKeys()
        {
            var functionKeys = Context.Data.FunctionKeys?.Where(functionKey =>
                    functionKey.Scope == FunctionAuthLevel.Function && !string.IsNullOrEmpty(functionKey.FunctionName))
                .ToArray();
            var hostKeys =
                Context.Data.FunctionKeys?.Where(functionKey => string.IsNullOrEmpty(functionKey.FunctionName))
                    .ToArray();
            _keyHandler.CreateFunctionKeys(functionKeys);
            _keyHandler.CreateHostKeys(hostKeys);
        }

        public void InitFunctionHost(TestContext context)
        {
            var dotnetExePath = Context.Data.Settings.DotNetPath;
            var functionHostPath = Context.Data.Settings.FuncHostPath;
            var functionAppFolder = Context.Data.Settings.FuncAppPath;
            var functionHostPort = Context.Data.Settings.FuncHostPort;
            var writeLog = EnvironmentHelper.WriteLog.HasValue && EnvironmentHelper.WriteLog.Value;

            var arguments = new[]
            {
                $"\"{functionHostPath}\" start",
                $"-p {functionHostPort}",
                "--timeout 5",
                GetRunFunctionsArgument(),
                GetEnableAuthArgument()
            }.Where(x => !string.IsNullOrEmpty(x)).ToArray();

            lock (_functionLock)
            {
                _funcHostProcess = new Process
                {
                    StartInfo =
                    {
                        FileName = dotnetExePath,
                        Arguments = string.Join(" ", arguments),
                        WorkingDirectory = functionAppFolder,
                        RedirectStandardError = writeLog,
                        RedirectStandardOutput = writeLog,
                        UseShellExecute = false
                    },
                };
                _funcHostProcess.StartInfo.EnvironmentVariables["AzureWebJobsStorage"] =
                    Context.Data.Settings.StorageConnectionString;

                if (writeLog)
                {
                    _funcHostProcess.ErrorDataReceived += (sender, args) => Logger.Log("func-host-error", args.Data);
                    _funcHostProcess.OutputDataReceived += (sender, args) => Logger.Log("func-host", args.Data);
                }

                if (!_funcHostProcess.Start())
                {
                    throw new InvalidOperationException("Could not start Azure Functions host.");
                }

                if (writeLog)
                {
                    _funcHostProcess.BeginErrorReadLine();
                    _funcHostProcess.BeginOutputReadLine();
                }
            }

            WaitForHostStart();
        }


        private void WaitForHostStart()
        {
            Task.Run(async () => await Retry.Try(async () =>
            {
                var response = await Client.GetAsync("/admin/host/ping");
                if (!response.IsSuccessStatusCode && response.StatusCode != HttpStatusCode.Unauthorized)
                    throw new Exception();
                return true;
            })).ConfigureAwait(false).GetAwaiter().GetResult();
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

        #endregion


        #region Azurite

        public void InitStorage()
        {
            _azuriteHandler.InitAzuriteHost();
            _keyHandler.Init();
        }

        public void ClearStorage()
        {
            _azuriteHandler.ClearQueues();
            _azuriteHandler.ClearBlobContainers();
            _azuriteHandler.ClearTables();
            Thread.Sleep(3000); // Allow Azurite to save state
        }

        public void StopAzurite() => _azuriteHandler.Stop();

        #endregion
    }
}