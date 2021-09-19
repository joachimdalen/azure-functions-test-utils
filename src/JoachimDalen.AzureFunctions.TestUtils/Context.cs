using JoachimDalen.AzureFunctions.TestUtils.Models;
using JoachimDalen.AzureFunctions.TestUtils.Settings;

namespace JoachimDalen.AzureFunctions.TestUtils
{
    public static class Context
    {
        public static void Reset()
        {
            _context.EnableAuth = false;
            _context.FunctionKeys = null;
            _context.FunctionsToRun = null;
            _context.Queues = null;
            _context.BlobContainers = null;
            _context.Tables = null;
        }

        private static ContextData _context { get; set; }

        public static ContextData Data => _context ??= new ContextData();

        public sealed class ContextData
        {
            public TestUtilsSettings Settings { get; set; }
            public bool IsInitialized { get; set; }
            public FunctionKey[] FunctionKeys { get; set; }
            public string[] FunctionsToRun { get; set; }
            public bool EnableAuth { get; set; }
            public string[] Queues { get; set; }
            public string[] BlobContainers { get; set; }
            public string[] Tables { get; set; }
        }
    }
}