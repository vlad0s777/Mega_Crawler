using System;

namespace Mega.Services
{
    public class UriAttempt
    {
        public Uri Uri { get; }
        public int Attempt { get; set; }

        public UriAttempt(Uri uri)
        {
            Uri = uri;
            Attempt = 0;
        }
    }
}