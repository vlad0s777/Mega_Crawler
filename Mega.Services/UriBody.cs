using System;

namespace Mega.Services
{
    public class UriBody
    {
        public UriBody(Uri uri, string body)
        {
            this.Uri = uri;
            this.Body = body;
        }

        public Uri Uri { get; set; }
        public string Body { get; set; }
    }
}