using System.Collections.Generic;
using System.Reflection;
using AzureFunctions.TestUtils.Attributes;

namespace AzureFunctions.TestUtils.Extensions
{
    public static class AttributeExtensions
    {
        public static IEnumerable<UseFunctionKeyAttribute> GetFunctionKeys(this MethodInfo methodInfo)
        {
            return methodInfo.GetCustomAttributes<UseFunctionKeyAttribute>();
        }

        public static IEnumerable<UseFunctionAuthAttribute> GetUseFunctionAuth(this MethodInfo methodInfo)
        {
            return methodInfo.GetCustomAttributes<UseFunctionAuthAttribute>();
        }

        public static IEnumerable<StartFunctionsAttribute> GetStartFunctions(this MethodInfo methodInfo)
        {
            return methodInfo.GetCustomAttributes<StartFunctionsAttribute>();
        }

        public static IEnumerable<UseQueuesAttribute> GetQueues(this MethodInfo methodInfo)
        {
            return methodInfo.GetCustomAttributes<UseQueuesAttribute>();
        }

        public static IEnumerable<UseBlobContainersAttribute> GetBlobContainers(this MethodInfo methodInfo)
        {
            return methodInfo.GetCustomAttributes<UseBlobContainersAttribute>();
        }
        public static IEnumerable<UseTablesAttribute> GetTables(this MethodInfo methodInfo)
        {
            return methodInfo.GetCustomAttributes<UseTablesAttribute>();
        }
    }
}