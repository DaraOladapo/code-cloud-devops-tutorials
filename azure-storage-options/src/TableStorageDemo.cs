using Azure.Data.Tables;
using System;
using System.Threading.Tasks;

namespace AzureStorageOptions
{
    /// <summary>
    /// Demonstrates inserting entities into Azure Table Storage.
    /// </summary>
    public class TableStorageDemo
    {
        private readonly string _connectionString;
        private readonly string _tableName;

        /// <summary>
        /// Initializes a new instance of the <see cref="TableStorageDemo"/> class.
        /// </summary>
        /// <param name="connectionString">Azure Storage connection string.</param>
        /// <param name="tableName">Table name.</param>
        public TableStorageDemo(string connectionString, string tableName)
        {
            _connectionString = connectionString;
            _tableName = tableName;
        }

        /// <summary>
        /// Inserts an entity into the specified Azure Table.
        /// </summary>
        /// <param name="entity">TableEntity to insert.</param>
        public async Task InsertEntityAsync(TableEntity entity)
        {
            var serviceClient = new TableServiceClient(_connectionString);
            var tableClient = serviceClient.GetTableClient(_tableName);
            await tableClient.CreateIfNotExistsAsync();
            try
            {
                await tableClient.AddEntityAsync(entity);
                Console.WriteLine($"Inserted entity into Table Storage.");
            }
            catch (Exception ex)
            {
                await Console.Error.WriteLineAsync($"Table entity insert failed: {ex.Message}");
            }
        }
    }
}
