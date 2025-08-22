using Azure.Storage.Queues;
using System;
using System.Threading.Tasks;

namespace AzureStorageOptions
{
    /// <summary>
    /// Demonstrates sending messages to Azure Queue Storage.
    /// </summary>
    public class QueueStorageDemo
    {
        private readonly string _connectionString;
        private readonly string _queueName;

        /// <summary>
        /// Initializes a new instance of the <see cref="QueueStorageDemo"/> class.
        /// </summary>
        /// <param name="connectionString">Azure Storage connection string.</param>
        /// <param name="queueName">Queue name.</param>
        public QueueStorageDemo(string connectionString, string queueName)
        {
            _connectionString = connectionString;
            _queueName = queueName;
        }

        /// <summary>
        /// Sends a message to the specified Azure Queue.
        /// </summary>
        /// <param name="message">Message to send.</param>
        public async Task SendMessageAsync(string message)
        {
            var queueClient = new QueueClient(_connectionString, _queueName);
            await queueClient.CreateIfNotExistsAsync();
            try
            {
                await queueClient.SendMessageAsync(message);
                Console.WriteLine($"Sent message to Queue Storage.");
            }
            catch (Exception ex)
            {
                await Console.Error.WriteLineAsync($"Queue message send failed: {ex.Message}");
            }
        }
    }
}
