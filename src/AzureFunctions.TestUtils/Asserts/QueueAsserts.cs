using System;
using System.Linq;
using System.Text;
using Azure.Storage.Queues;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;

namespace AzureFunctions.TestUtils.Asserts
{
    public static class QueueAsserts
    {
        private static QueueClient GetClient(string connectionString, string queue) =>
            new QueueClient(connectionString, queue);

        /// <summary>
        /// Assert that the given queue does not contain any messages
        /// </summary>
        /// <param name="queue">Name of queue</param>
        public static void QueueIsEmpty(this Assert assert, string queue) =>
            QueueHasMessageCount(assert, AzureFunctionConstants.DevelopmentConnectionString, queue, 0);

        /// <summary>
        /// Assert that the given queue does not contain any messages
        /// </summary>
        /// <param name="connectionString">Connection string to storage account</param>
        /// <param name="queue">Name of queue</param>
        public static void QueueIsEmpty(this Assert assert, string connectionString, string queue) =>
            QueueHasMessageCount(assert, connectionString, queue, 0);

        /// <summary>
        /// Assert that the given queue contains the given amount of messages
        /// </summary>
        /// <param name="queue">Name of queue</param>
        /// <param name="expectedCount">Expected amount of messages to exist on queue</param>
        public static void QueueHasMessageCount(this Assert assert, string queue, int expectedCount) =>
            QueueHasMessageCount(assert, AzureFunctionConstants.DevelopmentConnectionString, queue, expectedCount);

        /// <summary>
        /// Assert that the given queue contains the given amount of messages
        /// </summary>
        /// <param name="connectionString">Connection string to storage account</param>
        /// <param name="queue">Name of queue</param>
        /// <param name="expectedCount">Expected amount of messages to exist on queue</param>
        public static void QueueHasMessageCount(this Assert assert, string connectionString, string queue,
            int expectedCount)
        {
            var client = GetClient(connectionString, queue);
            var count = client.GetProperties().Value.ApproximateMessagesCount;
            if (expectedCount != count)
                throw new AssertFailedException(
                    $"Expected {expectedCount} messages to be on the queue, Actual {count}");
        }

        /// <summary>
        /// Assert that the queue contains at least one message matching the expression
        /// </summary>
        /// <param name="connectionString">Connection string to storage account</param>
        /// <param name="queue">Name of queue</param>
        /// <param name="expression">Message match expression</param>
        /// <typeparam name="T"></typeparam>
        /// <exception cref="AssertFailedException"></exception>
        public static void QueueHasMessage<T>(this Assert assert, string connectionString, string queue,
            Func<T, bool> expression)
        {
            var client = GetClient(connectionString, queue);
            var messages = client.PeekMessages();
            var mm = messages.Value.Select(x =>
            {
                var decoded = Convert.FromBase64String(x.MessageText);
                var text = Encoding.UTF8.GetString(decoded);
                return JsonConvert.DeserializeObject<T>(text);
            });
            var result = mm.FirstOrDefault(expression) != null;

            if (!result)
            {
                throw new AssertFailedException("Failed to find a message on queue matching the expression");
            }
        }

        /// <summary>
        /// Assert that the queue contains at least one message matching the expression
        /// </summary>
        /// <param name="queue">Name of queue</param>
        /// <param name="expression">Message match expression</param>
        /// <typeparam name="T"></typeparam>
        /// <exception cref="AssertFailedException"></exception>
        public static void QueueHasMessage<T>(this Assert assert, string queue, Func<T, bool> expression) =>
            QueueHasMessage<T>(assert, AzureFunctionConstants.DevelopmentConnectionString, queue, expression);
    }
}