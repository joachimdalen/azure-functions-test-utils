using System;

namespace JoachimDalen.AzureFunctions.TestUtils.Attributes
{
    [AttributeUsage(AttributeTargets.Method)]
    public class StartFunctionsAttribute : Attribute
    {
        public string[] FunctionNames { get; }

        public StartFunctionsAttribute(params string[] funcNames)
        {
            FunctionNames = funcNames;
        }
    }
}