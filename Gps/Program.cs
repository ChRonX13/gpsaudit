#region Copyright WA Police

// 
// All rights are reserved.
// 

#endregion

using System;
using System.Linq;
using System.Threading.Tasks;
using AzureCloud;
using DataImporter;
using DataModel;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Spatial;
using NLog;

namespace Gps
{
    public class Program
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private static DocumentClient _client;
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

            var dataPoints = _dataImporter.ReadGpxFile(@"C:\temp\20130616_030908.gpx").ToList();

            _cloudManager = new CloudManager(new AppConfiguration());

            _client = _cloudManager.GetDocumentClient();

            await _cloudManager.CreateDatabaseIfNotExistsAsync(_client, databaseName);

            await _cloudManager.CreateDocumentCollectionIfNotExists(_client, databaseName, collectionName);

            foreach (var dataPoint in dataPoints)
            {
                await _cloudManager.CreateDocumentIfNotExists(_client, databaseName, collectionName, dataPoint);
            }

            var documents =
                _client.CreateDocumentQuery<DataPoint>(UriFactory.CreateDocumentCollectionUri(databaseName,
                    collectionName)).ToList();

            for (int i = 1; i < documents.Count; i++)
            {
                var previous = documents[i - 1];
                var current = documents[i];

                var distanceBetweenPointsInMeters = previous.Location.Distance(current.Location);
                // ReSharper disable PossibleInvalidOperationException
                var timeBetweenPointsInSeconds = (current.DateTime.Value - previous.DateTime.Value).TotalSeconds;

                var speedInMetersPerSecond = distanceBetweenPointsInMeters / timeBetweenPointsInSeconds;
                var speedInKmPerHour = speedInMetersPerSecond * 3.6;

                current.Speed = speedInKmPerHour;

                await _cloudManager.ReplaceDocument(_client, databaseName, collectionName, current);
            }
        }
    }
}