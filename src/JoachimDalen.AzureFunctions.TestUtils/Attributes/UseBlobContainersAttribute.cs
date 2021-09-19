using System;

namespace JoachimDalen.AzureFunctions.TestUtils.Attributes
{
    [AttributeUsage(AttributeTargets.Method)]
    public class UseBlobContainersAttribute : Attribute
    {
        public string[] ContainerNames { get; }

        public UseBlobContainersAttribute(params string[] containerNames)
        {
            ContainerNames = containerNames;
        }
    }
}