using System;
using System.Collections.Generic;
using System.Net.Http;

namespace DbSeeder.Model.Models
{
    /// <summary>
    /// Class to store the values/parameters to be used in the HttpRequest
    /// </summary>
    public struct SeedItem
    {
        // To store the updated Uri
        public string Url { get; set; }
        // To store the key-value pairs for the URL parameters
        public IDictionary<string, string> UriParameters { get; set; }
        // To store the key-value pairs for the JSON
        public IDictionary<string, object> JsonParameters { get; set; }
        // To store the response message
        public HttpResponseMessage ResponseMessage { get; set; }

        public bool CheckUrl()
        {
            return Uri.TryCreate(Url, UriKind.Absolute, out Uri uriResult)
                && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
        }
    }
}
