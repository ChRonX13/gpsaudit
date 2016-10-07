#region Copyright WA Police

// 
// All rights are reserved.
// 

#endregion

using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using NLog;

namespace AzureCloud
{
    public class CloudManager : IDocumentClientManager
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private readonly IDocumentConfiguration _documentConfiguration;

        public CloudManager(IDocumentConfiguration documentConfiguration)
        {
            _documentConfiguration = documentConfiguration;
        }

        public DocumentClient GetDocumentClient()
        {
            Logger.Info("Creating Document Client...");

            return new DocumentClient(new Uri(_documentConfiguration.GetEndpointUri),
                _documentConfiguration.GetPrimaryKey);
        }

        public async Task CreateDatabaseIfNotExistsAsync(DocumentClient client, string databaseName)
        {
            try
            {
                Logger.Info("Checking if database exists...");

                await client.ReadDatabaseAsync(UriFactory.CreateDatabaseUri(databaseName));

                Logger.Info("Database exists...");
            }
            catch (DocumentClientException ex)
            {
                if (ex.StatusCode == HttpStatusCode.NotFound)
                {
                    Logger.Info("Database does not exist, creating...");

                    CreateDatabase(client, databaseName).Wait();
                }
                else
                {
                    throw;
                }
            }
        }

        public async Task CreateDocumentCollectionIfNotExists(DocumentClient client, string databaseName, string collectionName)
        {
            try
            {
                Logger.Info("Checking if collection exists...");

                await client.ReadDocumentCollectionAsync(UriFactory.CreateDocumentCollectionUri(databaseName, collectionName));

                Logger.Info("Collection exists...");
            }
            catch (DocumentClientException ex)
            {
                if (ex.StatusCode == HttpStatusCode.NotFound)
                {
                    Logger.Info("Collection does not exist, creating...");

                    CreateCollection(client, databaseName, collectionName).Wait();
                }
                else
                {
                    throw;
                }
            }
        }

        public async Task CreateDocument(DocumentClient client, string databaseName, string collectionName, object document)
        {
            await client.CreateDocumentAsync(UriFactory.CreateDocumentCollectionUri(databaseName, collectionName), document);
        }

        private async Task CreateCollection(DocumentClient client, string databaseName, string collectionName)
        {
            var collectionInfo = new DocumentCollection
            {
                Id = collectionName,
                IndexingPolicy = new IndexingPolicy(new RangeIndex(DataType.String) {Precision = -1})
            };

            await client.CreateDocumentCollectionAsync(UriFactory.CreateDatabaseUri(databaseName), collectionInfo, new RequestOptions {OfferThroughput = 400});
        }

        private async Task CreateDatabase(DocumentClient client, string databaseName)
        {
            await client.CreateDatabaseAsync(new Database {Id = databaseName});
        }
    }
}