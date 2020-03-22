using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;

namespace DbSeeder.Model.Models
{
    /// <summary>
    /// This is the class to store the processed content of request. 
    /// I.e.: the default URL, the Method to be used, etc. Also it stores each SeedItems - which represent the values to be used in the request
    /// </summary>
    public struct SeedDetails
    {
        public HttpMethod Method { get; set; }
        public string DefaultUrl { get; set; }
        public string Separator { get; set; }
        public IList<string> UriKeys { get; set; }

        // To store key - type of keys
        public IDictionary<string, string> JsonKeys { get; set; }

        // Store the values of each line to be used for each HttpRequest
        public IList<SeedItem> SeedItems { get; set; }

        /// <summary>
        /// Checks if all mandatory parameters are provided and correct
        /// </summary>
        /// <returns></returns>
        public bool CheckMe()
        {
            return (
                CheckMethod() &&
                !string.IsNullOrEmpty(Separator) &&
                SeedItems.Count > 0
                );
        }

        private bool CheckMethod()
        {
            var acceptedMethods = new string[] { "PATCH", "PUT", "DELETE", "POST" };
            if (Array.IndexOf(acceptedMethods, Method) != acceptedMethods.GetLowerBound(0) -1 ) return true;

            return false;
        }
    }
}
