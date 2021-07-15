using Azure.Storage.Queues;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AzureFunctions.TestUtils.Asserts
{
    public static class QueueAsserts
    {
        private static QueueClient GetClient(string conStr, string queue) =>
            new QueueClient(conStr, queue);

        public static void QueueHasMessageCount(this Assert assert, string queue, int expectedCount) =>
            QueueHasMessageCount(assert, AzureFunctionConstants.DevelopmentConnectionString, queue, expectedCount);

        public static void QueueHasMessageCount(this Assert assert, string conStr, string queue, int expectedCount)
        {
            var client = GetClient(conStr, queue);
            var count = client.GetProperties().Value.ApproximateMessagesCount;
            if (expectedCount != count)
                throw new AssertFailedException(
                    $"Expected {expectedCount} messages to be on the queue, Actual {count}");
        }
    }
}