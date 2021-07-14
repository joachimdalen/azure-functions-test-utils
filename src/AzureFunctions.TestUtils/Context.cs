using AzureFunctions.TestUtils.Models;
using AzureFunctions.TestUtils.Settings;

namespace AzureFunctions.TestUtils
{
    public static class Context
    {
        public static void Reset()
        {
            _context.EnableAuth = false;
            _context.FunctionKeys = null;
            _context.FunctionsToRun = null;
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