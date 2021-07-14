using System;

namespace AzureFunctions.TestUtils.Attributes
{
    [AttributeUsage(AttributeTargets.Method)]
    public class UseQueuesAttribute : Attribute
    {
        public string[] QueueNames { get; }

        public UseQueuesAttribute(params string[] queueNames)
        {
            QueueNames = queueNames;
        }
    }
}