namespace Mega.Services.BrokerHandler
{
    using System;

    using Newtonsoft.Json;

    public class UriRequest
    {
        public readonly int Attempt;

        public readonly int Depth;

        public readonly Uri Uri;

        public UriRequest(Uri uri, int attempt = 0, int depth = 1)
        {
            this.Uri = uri;
            this.Attempt = attempt;
            this.Depth = depth;
        }

        [JsonConstructor]
        public UriRequest(string uri, int attempt = 0, int depth = 1)
            : this(new Uri(uri), attempt, depth)
        {
        }
    }
}