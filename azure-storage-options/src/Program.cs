using System;
using System.IO;
using System.Threading.Tasks;

namespace AzureStorageOptions
{
    /// <summary>
    /// Entry point for Azure Storage Options demo application.
    /// Demonstrates usage of Blob, Table, Queue, and File storage in Azure.
    /// </summary>
    static class Program
    {
        /// <summary>
        /// Main method to run all Azure Storage demos.
        /// </summary>
        /// <param name="args">Command-line arguments (not used).</param>
        public static async Task Main(string[] args)
        {
            // Get configuration from environment variables
            string connectionString = Environment.GetEnvironmentVariable("AZURE_STORAGE_CONNECTION_STRING")
                ?? throw new InvalidOperationException("AZURE_STORAGE_CONNECTION_STRING environment variable is required.");
            string blobContainer = Environment.GetEnvironmentVariable("AZURE_BLOB_CONTAINER") ?? "sample-container";
            string tableName = Environment.GetEnvironmentVariable("AZURE_TABLE_NAME") ?? "SampleTable";
            string queueName = Environment.GetEnvironmentVariable("AZURE_QUEUE_NAME") ?? "sample-queue";
            string fileShare = Environment.GetEnvironmentVariable("AZURE_FILE_SHARE") ?? "sample-share";
            string sampleFilePath = Environment.GetEnvironmentVariable("SAMPLE_FILE_PATH")
                ?? throw new InvalidOperationException("SAMPLE_FILE_PATH environment variable is required.");

            // Validate that sample file exists
            if (!File.Exists(sampleFilePath))
            {
                Console.Error.WriteLine($"Sample file not found: {sampleFilePath}");
                return;
            }

            try
            {
                // Blob Storage Demo
                var blobDemo = new BlobStorageDemo(connectionString, blobContainer);
                await blobDemo.UploadFileAsync(sampleFilePath);

                // Table Storage Demo
                var tableDemo = new TableStorageDemo(connectionString, tableName);
                var entity = new Azure.Data.Tables.TableEntity("partitionKey", Guid.NewGuid().ToString())
                {
                    { "SampleProperty", "SampleValue" }
                };
                await tableDemo.InsertEntityAsync(entity);

                // Queue Storage Demo
                var queueDemo = new QueueStorageDemo(connectionString, queueName);
                await queueDemo.SendMessageAsync("Hello from Azure Queue!");

                // File Storage Demo
                var fileDemo = new FileStorageDemo(connectionString, fileShare);
                await fileDemo.UploadFileAsync(sampleFilePath);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error running Azure Storage demos: {ex.Message}");
            }
        }
    }
}
