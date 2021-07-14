using System;

namespace AzureFunctions.TestUtils.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class UseAzuriteAttribute : Attribute
    {
        public UseAzuriteAttribute()
        {
        }
    }
}