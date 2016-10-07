#region Copyright WA Police

// 
// All rights are reserved.
// 

#endregion

using System;
using System.Net;
using System.Runtime.ExceptionServices;
using System.Threading.Tasks;
using DataModel;
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
            ExceptionDispatchInfo capturedException = null;

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
                    capturedException = ExceptionDispatchInfo.Capture(ex);
                }
                else
                {
                    throw;
                }
            }

            if (capturedException != null)
            {
                Logger.Info("Database does not exist, creating...");

                await CreateDatabase(client, databaseName);
            }
        }

        public async Task CreateDocumentCollectionIfNotExists(DocumentClient client, string databaseName, string collectionName)
        {
            ExceptionDispatchInfo capturedException = null;

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
                    capturedException = ExceptionDispatchInfo.Capture(ex);
                }
                else
                {
                    throw;
                }
            }

            if (capturedException != null)
            {
                Logger.Info("Collection does not exist, creating...");

                await CreateCollection(client, databaseName, collectionName);
            }
        }

        public async Task CreateDocumentIfNotExists(DocumentClient client, string databaseName, string collectionName, IDocument document)
        {
            ExceptionDispatchInfo capturedException = null;

            try
            {
                await client.ReadDocumentAsync(UriFactory.CreateDocumentUri(databaseName, collectionName, document.Id));
            }
            catch (DocumentClientException ex)
            {
                if (ex.StatusCode == HttpStatusCode.NotFound)
                {
                    capturedException = ExceptionDispatchInfo.Capture(ex);
                }
                else
                {
                    throw;
                }
            }

            if (capturedException != null)
            {
                await CreateDocument(client, databaseName, collectionName, document);
            }
        }

        public async Task ReplaceDocument(DocumentClient client, string databaseName, string collectionName,
            IDocument document)
        {
            await client.ReplaceDocumentAsync(UriFactory.CreateDocumentUri(databaseName, collectionName, document.Id), document);
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

        private async Task CreateDocument(DocumentClient client, string databaseName, string collectionName,
            object document)
        {
            await client.CreateDocumentAsync(UriFactory.CreateDocumentCollectionUri(databaseName, collectionName), document);
        }
    }
}