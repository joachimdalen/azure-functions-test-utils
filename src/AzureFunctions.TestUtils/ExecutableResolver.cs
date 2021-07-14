using System;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;

namespace AzureFunctions.TestUtils
{
    public class ExecutableResolver
    {
        private string GetToolFromLookup(string executable)
        {
            var process = new Process
            {
                StartInfo =
                {
                    FileName = GetShell(),
                    Arguments = GetArguments(executable),
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    CreateNoWindow = true
                },
            };

            process.Start();
            process.WaitForExit();

            if (process.ExitCode != 0)
            {
                throw new Exception($"Failed to lookup tool {executable}");
            }

            return process.StandardOutput.ReadToEnd();
        }


        private string GetArguments(string executable)
        {
            if (OperatingSystem.IsLinux())
                return $"-c \"which {executable}\"";
            return null;
        }

        private string GetShell()
        {
            if (OperatingSystem.IsLinux()) return "/bin/bash";
            return null;
        }


        public string GetDotNetPath()
        {
            var path = GetToolFromLookup("dotnet");

            if (path.EndsWith(Environment.NewLine))
            {
                path = path.Replace(Environment.NewLine, "");
            }

            return path;
        }

        public string GetFunctionHostPath()
        {
            var tools = GetToolFromLookup("func");
            return tools;
        }

        public string GetAzuritePath() => GetToolFromLookup("azurite");
    }
}