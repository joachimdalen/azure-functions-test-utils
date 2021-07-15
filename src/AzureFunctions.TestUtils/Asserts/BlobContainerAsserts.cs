using Azure.Storage.Blobs;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AzureFunctions.TestUtils.Asserts
{
    public static class BlobContainerAsserts
    {
        private static BlobClient GetClient(string conStr, string container, string blob) =>
            new BlobClient(conStr, container, blob);

        private static BlobContainerClient GetContainerClient(string conStr, string container) =>
            new BlobContainerClient(conStr, container);


        /// <summary>
        /// Assert that a given blob item exists in the container
        /// </summary>
        /// <param name="container">Container name</param>
        /// <param name="blobPath">Path to blob item inside container</param>
        public static void BlobExists(this Assert assert, string container, string blobPath)
            => BlobExists(assert, AzureFunctionConstants.DevelopmentConnectionString, container, blobPath);

        /// <summary>
        /// Assert that a given blob item exists in the container
        /// </summary>
        /// <param name="connectionString">Connection string to storage account</param>
        /// <param name="container">Container name</param>
        /// <param name="blobPath">Path to blob item inside container</param>
        public static void BlobExists(this Assert assert, string connectionString, string container, string blobPath)
        {
            var client = GetClient(connectionString, container, blobPath);
            var result = client.Exists();
            if (!result) throw new AssertFailedException($"Blob {blobPath} does not exist in container {container}");
        }

        /// <summary>
        /// Assert that a blob container exists
        /// </summary>
        /// <param name="container">Container name</param>
        public static void ContainerExists(this Assert assert, string container)
            => ContainerExists(assert, AzureFunctionConstants.DevelopmentConnectionString, container);

        /// <summary>
        /// Assert that a blob container exists
        /// </summary>
        /// <param name="connectionString">Connection string to storage account</param>
        /// <param name="container">Container name</param>
        public static void ContainerExists(this Assert assert, string connectionString, string container)
        {
            var client = GetContainerClient(connectionString, container);
            var result = client.Exists();
            if (!result) throw new AssertFailedException($"Blob container {container} does not exist");
        }
    }
}