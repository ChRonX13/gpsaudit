#region Copyright WA Police

// 
// All rights are reserved.
// 

#endregion

namespace AzureCloud
{
    public interface IDocumentConfiguration
    {
        string GetPrimaryKey { get; }
        string GetEndpointUri { get; }
    }
}