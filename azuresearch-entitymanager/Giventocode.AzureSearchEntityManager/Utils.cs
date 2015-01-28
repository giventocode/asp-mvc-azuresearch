using Microsoft.WindowsAzure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Giventocode.AzureSearchEntityManager
{
    public class Utils
    {
         internal static string GetRequiredConfigurationValue(string key)
        {
            var value = CloudConfigurationManager.GetSetting(key);

            if (string.IsNullOrEmpty(value))
            {
                throw new InvalidOperationException(string.Format("Configuration value is missing or empty. Name: {0}.", key));
            }

            return value;
        }
    }
}
