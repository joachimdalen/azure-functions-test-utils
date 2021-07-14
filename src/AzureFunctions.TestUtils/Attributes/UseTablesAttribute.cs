using System;

namespace AzureFunctions.TestUtils.Attributes
{
    [AttributeUsage(AttributeTargets.Method)]
    public class UseTablesAttribute : Attribute
    {
        public string[] TableNames { get; }

        public UseTablesAttribute(params string[] tableNames)
        {
            TableNames = tableNames;
        }
    }
}