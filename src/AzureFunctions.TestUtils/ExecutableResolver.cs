using System;
using System.Diagnostics;
using System.IO;

namespace AzureFunctions.TestUtils
{
    public class ExecutableResolver
    {
        private string GetProcessOutput(string shell, string arguments, string workingDirectory = null)
        {
            var process = new Process
            {
                StartInfo =
                {
                    FileName = shell,
                    Arguments = arguments,
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    CreateNoWindow = true,
                },
            };

            if (!string.IsNullOrEmpty(workingDirectory))
                process.StartInfo.WorkingDirectory = workingDirectory;

            process.Start();
            process.WaitForExit();

            if (process.ExitCode != 0)
            {
                throw new Exception($"Failed to process output");
            }

            return process.StandardOutput.ReadToEnd()?.Replace(Environment.NewLine, "");
        }


        private string GetToolFromLookup(string executable)
        {
            return GetProcessOutput(GetShell(), GetArguments(executable));
        }

        private string GetSymlinkPath(string tool)
        {
            var command = $"-c \"readlink $(which {tool})\"";
            return GetProcessOutput(GetShell(), command);
        }


        private string GetArguments(string executable)
        {
            if (OperatingSystem.IsLinux() || OperatingSystem.IsMacOS())
                return $"-c \"which {executable}\"";
            return null;
        }

        private string GetShell()
        {
            if (OperatingSystem.IsLinux() || OperatingSystem.IsMacOS())
                return "/bin/bash";
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
            var tool = GetToolFromLookup("func");
            var toolDir = tool.Replace("/func", "");

            if (OperatingSystem.IsLinux())
            {
                var resolvedPath = tool.Replace("bin/func", "lib/node_modules/azure-functions-core-tools/bin/func.dll");
                return resolvedPath;
            }

            if (OperatingSystem.IsMacOS())
            {
                var symlinkedPath = GetSymlinkPath("func");
                var installDir = Path.GetFullPath(symlinkedPath, toolDir);
                return installDir.Replace("bin/func", "func.dll");
            }

            var tools = GetToolFromLookup("func");
            return tools;
        }

        public string GetAzuritePath() => GetToolFromLookup("azurite");
    }
}