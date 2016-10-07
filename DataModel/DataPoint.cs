#region Copyright WA Police

// 
// All rights are reserved.
// 

#endregion

using System;
using Microsoft.Azure.Documents.Spatial;

namespace DataModel
{
    public class DataPoint
    {
        public DateTime? DateTime { get; set; }
        public Point Location { get; set; }
    }
}