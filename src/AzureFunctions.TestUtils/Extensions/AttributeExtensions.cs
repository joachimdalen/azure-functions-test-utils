using System.Collections.Generic;
using System.Reflection;
using AzureFunctions.TestUtils.Attributes;

namespace AzureFunctions.TestUtils.Extensions
{
    public static class AttributeExtensions
    {
        public static IEnumerable<FunctionKeyAttribute> GetFunctionKeys(this MethodInfo methodInfo)
        {
            return methodInfo.GetCustomAttributes<FunctionKeyAttribute>();
        }

        public static IEnumerable<UseFunctionAuthAttribute> GetUseFunctionAuth(this MethodInfo methodInfo)
        {
            return methodInfo.GetCustomAttributes<UseFunctionAuthAttribute>();
        }

        public static IEnumerable<StartFunctionsAttribute> GetStartFunctions(this MethodInfo methodInfo)
        {
            return methodInfo.GetCustomAttributes<StartFunctionsAttribute>();
        }
    }
}