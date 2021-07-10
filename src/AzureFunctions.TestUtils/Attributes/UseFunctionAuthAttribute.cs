using System;
using AzureFunctions.TestUtils.Models;

namespace AzureFunctions.TestUtils.Attributes
{
    [AttributeUsage(AttributeTargets.Method)]
    public class UseFunctionAuthAttribute : Attribute
    {
        public UseFunctionAuthAttribute(FunctionKeyLocation location = FunctionKeyLocation.StorageAccount)
        {
            Location = location;
        }

        public FunctionKeyLocation Location { get; set; }
    }
}