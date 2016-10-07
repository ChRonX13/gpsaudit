#region Copyright WA Police

// 
// All rights are reserved.
// 

#endregion

using System.Configuration;

namespace AzureCloud
{
    public class AppConfiguration : IDocumentConfiguration
    {
        public string GetPrimaryKey
        {
            get { return GetSetting("PrimaryKey"); }
        }

        public string GetEndpointUri
        {
            get { return GetSetting("EndpointUri"); }
        }

        private string GetSetting(string key)
        {
            string setting = ConfigurationManager.AppSettings[key];

            if (string.IsNullOrWhiteSpace(setting))
            {
                throw new ConfigurationErrorsException(
                    string.Format(
                        "Setting [{0}] was not found, please ensure it has been set in AppSettings in app.config or the environment!",
                        key));
            }

            return setting;
        }
    }
}