#region Copyright WA Police

// 
// All rights are reserved.
// 

#endregion

using System.Collections.Generic;
using DataModel;

namespace DataImporter
{
    public interface IGpxImporter
    {
        IEnumerable<DataPoint> ReadGpxFile(string filename);
    }
}