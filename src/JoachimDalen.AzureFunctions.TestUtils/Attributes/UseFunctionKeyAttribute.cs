using System;
using JoachimDalen.AzureFunctions.TestUtils.Models;

namespace JoachimDalen.AzureFunctions.TestUtils.Attributes
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class UseFunctionKeyAttribute : Attribute
    {
        /// <summary>
        /// Create a new function key
        /// </summary>
        /// <param name="functionName">Name of the function the key belongs to</param>
        /// <param name="authLevel"></param>
        /// <param name="keyName">Name of the key</param>
        /// <param name="keyValue">Value of the key. If left empty, one will be auto generated</param>
        public UseFunctionKeyAttribute(FunctionAuthLevel authLevel, string keyName, string functionName,
            string keyValue)
        {
            AuthLevel = authLevel;
            FunctionName = functionName;
            Name = keyName;
            Value = keyValue;
        }

        public UseFunctionKeyAttribute(FunctionAuthLevel authLevel, string keyName, string keyValue) : this(authLevel,
            keyName, null, keyValue)
        {
        }

        public string Name { get; }
        public string Value { get; }
        public string FunctionName { get; }
        public FunctionAuthLevel AuthLevel { get; }
    }
}