using Azure.Storage.Files.Shares;
using System;
using System.IO;
using System.Threading.Tasks;

namespace AzureStorageOptions
{
    /// <summary>
    /// Demonstrates uploading files to Azure File Storage.
    /// </summary>
    public class FileStorageDemo
    {
        private readonly string _connectionString;
        private readonly string _shareName;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileStorageDemo"/> class.
        /// </summary>
        /// <param name="connectionString">Azure Storage connection string.</param>
        /// <param name="shareName">File share name.</param>
        public FileStorageDemo(string connectionString, string shareName)
        {
            _connectionString = connectionString;
            _shareName = shareName;
        }

        /// <summary>
        /// Uploads a file to the specified Azure File share.
        /// </summary>
        /// <param name="filePath">Path to the file to upload.</param>
        public async Task UploadFileAsync(string filePath)
        {
            var shareClient = new ShareClient(_connectionString, _shareName);
            await shareClient.CreateIfNotExistsAsync();
            var rootDir = shareClient.GetRootDirectoryClient();
            var fileName = Path.GetFileName(filePath);
            var fileClient = rootDir.GetFileClient(fileName);
            try
            {
                using var fileStream = File.OpenRead(filePath);
                await fileClient.CreateAsync(fileStream.Length);
                await fileClient.UploadRangeAsync(new Azure.HttpRange(0, fileStream.Length), fileStream);
                Console.WriteLine($"Uploaded {fileName} to File Storage.");
            }
            catch (Exception ex)
            {
                await Console.Error.WriteLineAsync($"File upload failed: {ex.Message}");
            }
        }
    }
}
