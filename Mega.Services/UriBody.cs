using System;

namespace Mega.Services
{
    public class UriBody : object
    {
        public UriBody(Uri uri, string body)
        {
            Uri = uri;
            Body = body;
        }

        public Uri Uri { get; }
        public string Body { get; }

        public override bool Equals(object obj)
        {
            // Check for null values and compare run-time types.
            if (obj == null || GetType() != obj.GetType())
                return false;

            var ub = (UriBody) obj;
            return Uri == ub.Uri && Body == ub.Body;
        }

        public override int GetHashCode()
        {
            return Uri.GetHashCode() ^ Body.GetHashCode();
        }
    }
}