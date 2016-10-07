#region Copyright WA Police

// 
// All rights are reserved.
// 

#endregion

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AzureCloud;
using DataImporter;
using DataModel;
using Microsoft.Azure.Documents.Client;
using NLog;

namespace Gps
{
    public class Program
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private static DocumentClient _documentClient;
        private static IDocumentClientManager _cloudManager;
        private static IGpxImporter _dataImporter;

        public static void Main(string[] args)
        {
            Logger.Info("Starting");
            try
            {
                new Program().Execute().Wait();
            }
            catch (Exception e)
            {
                Logger.Error(e);
            }
            finally
            {
                Logger.Info("Finished");
                Console.ReadKey();
            }
        }

        private async Task Execute()
        {
            const string databaseName = "Gps";
            const string collectionName = "GpsCollection";

            _dataImporter = new GpxImporter();

            IEnumerable<DataPoint> dataPoints = _dataImporter.ReadGpxFile(@"C:\temp\20130616_063718.gpx");

            _cloudManager = new CloudManager(new AppConfiguration());

            _documentClient = _cloudManager.GetDocumentClient();

            await _cloudManager.CreateDatabaseIfNotExistsAsync(_documentClient, databaseName);

            await _cloudManager.CreateDocumentCollectionIfNotExists(_documentClient, databaseName, collectionName);

            foreach (var dataPoint in dataPoints)
            {
                await _cloudManager.CreateDocument(_documentClient, databaseName, collectionName, dataPoint);
            }
        }
    }
}