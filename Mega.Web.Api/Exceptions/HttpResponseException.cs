namespace Mega.Web.Api.Exceptions
{
    using System;

    using Newtonsoft.Json.Linq;

    public class HttpResponseException : Exception
    {
        public int StatusCode { get; set; }

        public string ContentType { get; set; } = @"text/plain";

        public HttpResponseException(int statusCode) => this.StatusCode = statusCode;

        public HttpResponseException(int statusCode, string message)
            : base(message) 
            => this.StatusCode = statusCode;

        public HttpResponseException(int statusCode, Exception inner)
            : this(statusCode, inner.ToString())
        {
        }

        public HttpResponseException(int statusCode, JObject errorObject)
            : this(statusCode, errorObject.ToString())
        {
            this.ContentType = @"application/json";
        }
    }
}
