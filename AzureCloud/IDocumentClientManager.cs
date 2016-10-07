#region Copyright WA Police

// 
// All rights are reserved.
// 

#endregion

using System.Threading.Tasks;
using Microsoft.Azure.Documents.Client;

namespace AzureCloud
{
    public interface IDocumentClientManager
    {
        DocumentClient GetDocumentClient();
        Task CreateDatabaseIfNotExistsAsync(DocumentClient client, string databaseName);
        Task CreateDocumentCollectionIfNotExists(DocumentClient client, string databaseName, string collectionName);
        Task CreateDocument(DocumentClient client, string databaseName, string collectionName, object document);
    }
}