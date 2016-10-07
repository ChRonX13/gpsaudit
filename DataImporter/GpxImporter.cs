#region Copyright WA Police

// 
// All rights are reserved.
// 

#endregion

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Xml.Linq;
using DataModel;
using Microsoft.Azure.Documents.Spatial;
using NLog;

namespace DataImporter
{
    public class GpxImporter : IGpxImporter
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public IEnumerable<DataPoint> ReadGpxFile(string file)
        {
            Logger.Info("Importing GPX data...");
            Stopwatch watch = Stopwatch.StartNew();

            XDocument document = XDocument.Load(file);
            XNamespace gpx = XNamespace.Get("http://www.topografix.com/GPX/1/1");

            int count = 0;

            List<DataPoint> dataPoints = (from segment in document.Descendants(gpx + "trk")
                from trackPoint in segment.Descendants(gpx + "trkpt")
                let longitude = double.Parse(trackPoint.Attribute("lon").Value)
                let latitude = double.Parse(trackPoint.Attribute("lat").Value)
                let elevation = trackPoint.Element(gpx + "ele") != null
                    // ReSharper disable once PossibleNullReferenceException
                    ? Double.Parse(trackPoint.Element(gpx + "ele").Value)
                    : (double?) null
                let id = count++ 
                select new DataPoint
                {
                    Id = id,
                    // ReSharper disable once PossibleNullReferenceException
                    DateTime =
                        trackPoint.Element(gpx + "time") != null
                            ? DateTime.Parse(trackPoint.Element(gpx + "time").Value)
                            : (DateTime?) null,
                    Location = new Point(new Position(longitude, latitude, elevation))
                }).ToList();

            Logger.Info("Imported {0} records, {1} per second, in {2} seconds", dataPoints.Count,
                string.Format("{0:N2}", dataPoints.Count/watch.Elapsed.TotalSeconds),
                string.Format("{0:N2}", watch.Elapsed.TotalSeconds));

            return dataPoints.OrderBy(x => x.DateTime);
        }
    }
}