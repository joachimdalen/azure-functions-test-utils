using System;

namespace AzureFunctions.TestUtils.Attributes
{
    [AttributeUsage(AttributeTargets.Method)]
    public class UseFunctionKeyAttribute : Attribute
    {
        /// <summary>
        /// Create a new function key
        /// </summary>
        /// <param name="functionName">Name of the function the key belongs to</param>
        /// <param name="keyName">Name of the key</param>
        /// <param name="keyValue">Value of the key. If left empty, one will be auto generated</param>
        public UseFunctionKeyAttribute(string functionName, string keyName, string keyValue = null)
        {
            FunctionName = functionName;
            Name = keyName;
            Value = keyValue;
        }

        public string Name { get; }
        public string Value { get; }
        public string FunctionName { get; }
    }
}