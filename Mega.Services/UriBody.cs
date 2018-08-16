using System;

namespace Mega.Services
{
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
    }
}