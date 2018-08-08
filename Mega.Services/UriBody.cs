using System;

namespace Mega.Services
{
    public class UriBody
    {
        public UriBody(Uri uri, string body)
        {
            Uri = uri;
            Body = body;
        }

        public Uri Uri { get; }
        public string Body { get; }
    }
}