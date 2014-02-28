using Odin;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Odin.TableStoreProvider
{
    public class OdinTableStore : IOdin
    {
        class Entity : TableEntity
        {
            public string Value { get; set; }
        }

        CloudTable cloudTable;
        string partitionKey;

        public OdinTableStore(string table, string partitionKey)
        {
            var account = CloudStorageAccount.DevelopmentStorageAccount;
            var tableClient = account.CreateCloudTableClient();
            cloudTable = tableClient.GetTableReference(table);
            this.partitionKey = partitionKey;
        }

        public async Task CreateTable()
        {
            await cloudTable.CreateIfNotExistsAsync();
        }

        public async Task Put(string key, string value)
        {
            var operation = TableOperation.InsertOrReplace(new Entity { PartitionKey = partitionKey, RowKey = key, Value = value });
            await cloudTable.ExecuteAsync(operation);
        }

        public async Task<string> Get(string key)
        {
            var operation = TableOperation.Retrieve<Entity>(partitionKey, key);
            var result = await cloudTable.ExecuteAsync(operation);
            if (result.Result == null) return null;
            return (result.Result as Entity).Value;
        }

        public async Task Delete(string key)
        {
            try
            {
                var operation = TableOperation.Delete(new Entity { PartitionKey = partitionKey, RowKey = key, ETag = "*" });
                await cloudTable.ExecuteAsync(operation);
            }
            catch (StorageException)
            { 
                // TODO: check for 404
            
            }
        }

        public async Task<IEnumerable<KeyValue>> Search(string start = null, string end = null)
        {
            // TODO: observe continuation token

            var query = new TableQuery<Entity>();
            query.FilterString = string.Format("PartitionKey eq '{0}'", this.partitionKey);
            if (!string.IsNullOrWhiteSpace(start)) query.FilterString += string.Format(" and RowKey ge '{0}'", start);
            if (!string.IsNullOrWhiteSpace(end)) query.FilterString += string.Format(" and RowKey le '{0}'", end);
            var result = await cloudTable.ExecuteQuerySegmentedAsync<Entity>(query, null);
            return result.Results.Select(x => new KeyValue { Key = x.RowKey, Value = x.Value });
        }


    }
}
