using System;

namespace AzureFunctions.TestUtils.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class UseAzuriteAttribute : Attribute
    {
        public bool Blob { get; }
        public bool Table { get; }

        public UseAzuriteAttribute(bool blob = true, bool table = true)
        {
            Blob = blob;
            Table = table;
        }
    }
}