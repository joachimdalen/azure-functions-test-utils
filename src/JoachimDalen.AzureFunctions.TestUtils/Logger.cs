using System;
using System.IO;

namespace JoachimDalen.AzureFunctions.TestUtils
{
    public static class Logger
    {
        private const string BasePath = "/tmp/aftu";

        public static void Log(string filename, string message)
        {
            var path = Path.Join(BasePath, Path.ChangeExtension(filename, "txt"));
            if (!Directory.Exists(BasePath)) Directory.CreateDirectory(BasePath);
            File.AppendAllLines(path, message?.Split(Environment.NewLine));
        }

        public static void ClearLogs()
        {
            if (Directory.Exists(BasePath))
                Directory.Delete(BasePath,true);
        }
    }
}