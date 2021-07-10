using System;
using AzureFunctions.TestUtils.Models;

namespace AzureFunctions.TestUtils.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class FunctionKeyAttribute : Attribute
    {
        public FunctionKeyAttribute(FunctionAuthLevel level, string key, string functionName = null)
        {
            if (level == FunctionAuthLevel.Function && string.IsNullOrEmpty(functionName))
            {
                throw new ArgumentException("Function name must be specified when setting function auth level to Function");
            }

            Level = level;
            Key = key;
        }

        public string Key { get; set; }
        public FunctionAuthLevel Level { get; set; }
    }
}