using System;

namespace AzureFunctions.TestUtils.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class UseAzuriteAttribute : Attribute
    {
        public UseAzuriteAttribute()
        {
        }
    }
}