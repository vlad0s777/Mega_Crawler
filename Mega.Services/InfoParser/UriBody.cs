namespace Mega.Services.InfoParser
{
    using System;

    using Newtonsoft.Json;

    public class UriBody
    {
        public readonly Uri Uri;

        public readonly string Body;

        public readonly int Depth;

        public UriBody(Uri uri, string body, int depth = 1)
        {
            this.Uri = uri;
            this.Body = body;
            this.Depth = depth;
        }

        [JsonConstructor]
        public UriBody(string uri, string body, int depth = 1)
            : this(new Uri(uri), body, depth)
        {
        }
    }
}