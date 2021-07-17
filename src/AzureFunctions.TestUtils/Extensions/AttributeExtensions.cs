using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using AzureFunctions.TestUtils.Attributes;

namespace AzureFunctions.TestUtils.Extensions
{
    public static class AttributeExtensions
    {
        public static IEnumerable<UseFunctionKeyAttribute> GetFunctionKeys(this MethodInfo methodInfo)
        {
            return GetMethodOrClassAttributes<UseFunctionKeyAttribute>(methodInfo);
        }

        public static IEnumerable<UseFunctionAuthAttribute> GetUseFunctionAuth(this MethodInfo methodInfo)
        {
            return GetMethodOrClassAttributes<UseFunctionAuthAttribute>(methodInfo);
        }

        public static IEnumerable<UseAzuriteAttribute> GetUseAzurite(this MethodInfo methodInfo)
        {
            return GetMethodOrClassAttributes<UseAzuriteAttribute>(methodInfo);
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

        private static IEnumerable<T> GetMethodOrClassAttributes<T>(this MethodInfo methodInfo) where T : Attribute
        {
            var methodAttributes = methodInfo.GetCustomAttributes<T>()?.ToArray();

            if (methodAttributes.Any()) return methodAttributes;

            if (methodInfo.DeclaringType != null)
            {
                return methodInfo.DeclaringType.GetCustomAttributes<T>();
            }

            return Array.Empty<T>();
        }
    }
}