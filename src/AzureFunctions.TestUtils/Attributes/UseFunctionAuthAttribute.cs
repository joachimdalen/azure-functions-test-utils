using System;

namespace AzureFunctions.TestUtils.Attributes
{
    [AttributeUsage(AttributeTargets.Method)]
    public class UseFunctionAuthAttribute : Attribute
    {
    }
}