# Azure Storage Options

This C# console project demonstrates how to use the main Azure Storage services: Blob, Table, Queue, and File. Each storage type is implemented in a separate class file with clear, real-world code samples.

## Structure

- `BlobStorageDemo.cs`: Uploads a file to Azure Blob Storage
- `TableStorageDemo.cs`: Inserts a record into Azure Table Storage
- `QueueStorageDemo.cs`: Sends a message to Azure Queue Storage
- `FileStorageDemo.cs`: Uploads a file to Azure File Storage
- `Program.cs`: Entry point to run each demo

## Prerequisites

- .NET 9.0 or later
- Azure Storage account and connection string
- NuGet packages:
  - Azure.Storage.Blobs
  - Azure.Data.Tables
  - Azure.Storage.Queues
  - Azure.Storage.Files.Shares


## How to Run

1. Set the following environment variables with your Azure Storage details:
  - `AZURE_STORAGE_CONNECTION_STRING`: Your Azure Storage account connection string
  - `AZURE_BLOB_CONTAINER`: Blob container name (default: `sample-container`)
  - `AZURE_TABLE_NAME`: Table name (default: `SampleTable`)
  - `AZURE_QUEUE_NAME`: Queue name (default: `sample-queue`)
  - `AZURE_FILE_SHARE`: File share name (default: `sample-share`)
  - `SAMPLE_FILE_PATH`: Path to a local file to upload

2. Build and run the project:

  ```pwsh
  dotnet run
  ```

## Notes

- All demos use async/await for best performance and reliability.
- Error handling is included for all storage operations; see code comments for details.
- Replace placeholder values with your actual Azure Storage credentials and resource names.

---

This project follows Azure best practices for security and reliability. See the code comments for details.

---
