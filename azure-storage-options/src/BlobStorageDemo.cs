using Azure.Storage.Blobs;
using System;
using System.IO;
using System.Threading.Tasks;

namespace AzureStorageOptions
{
    /// <summary>
    /// Demonstrates uploading files to Azure Blob Storage.
    /// </summary>
    public class BlobStorageDemo
    {
        private readonly string _connectionString;
        private readonly string _containerName;

        /// <summary>
        /// Initializes a new instance of the <see cref="BlobStorageDemo"/> class.
        /// </summary>
        /// <param name="connectionString">Azure Storage connection string.</param>
        /// <param name="containerName">Blob container name.</param>
        public BlobStorageDemo(string connectionString, string containerName)
        {
            _connectionString = connectionString;
            _containerName = containerName;
        }

        /// <summary>
        /// Uploads a file to the specified Azure Blob container.
        /// </summary>
        /// <param name="filePath">Path to the file to upload.</param>
        public async Task UploadFileAsync(string filePath)
        {
            var blobServiceClient = new BlobServiceClient(_connectionString);
            var containerClient = blobServiceClient.GetBlobContainerClient(_containerName);
            await containerClient.CreateIfNotExistsAsync();
            var fileName = Path.GetFileName(filePath);
            var blobClient = containerClient.GetBlobClient(fileName);
            try
            {
                using var fileStream = File.OpenRead(filePath);
                await blobClient.UploadAsync(fileStream, overwrite: true);
                Console.WriteLine($"Uploaded {fileName} to Blob Storage.");
            }
            catch (Exception ex)
            {
                await Console.Error.WriteLineAsync($"Blob upload failed: {ex.Message}");
            }
        }
    }
}
