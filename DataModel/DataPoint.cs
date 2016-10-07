#region Copyright WA Police

// 
// All rights are reserved.
// 

#endregion

using System;
using Microsoft.Azure.Documents.Spatial;

namespace DataModel
{
    public class DataPoint : IDocument
    {
        public int Id { get; set; }
        public DateTime? DateTime { get; set; }
        public double Speed { get; set; }
        public Point Location { get; set; }
    }
}