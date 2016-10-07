#region Copyright WA Police

// 
// All rights are reserved.
// 

#endregion

using System;
using Microsoft.Azure.Documents.Spatial;
using Newtonsoft.Json;

namespace DataModel
{
    public class DataPoint : IDocument
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("datetime")]
        public DateTime? DateTime { get; set; }

        [JsonProperty("speed")]
        public double Speed { get; set; }

        [JsonProperty("location")]
        public Point Location { get; set; }
    }
}