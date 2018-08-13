using System;
using System.Collections.Generic;

namespace Mega.Services
{
    public class UriBody
    {
        public UriBody(Uri uri, string body, int depth = 1)
        {
            this.Uri = uri;
            this.Body = body;
            this.Depth = depth;
        }

        public readonly Uri Uri;
        public readonly string Body;
        public readonly int Depth;
    }

    public class ArticleInfo
    {
        public readonly DateTime DateCreate;
        public readonly List<string> Tags;
        public readonly string Text;

        public ArticleInfo(DateTime dateCreate, List<string> tags, string text)
        {
            this.DateCreate = dateCreate;
            this.Tags = tags;
            this.Text = text;
        }

        public ArticleInfo(DateTime dateCreate, string tag, string text) : this(dateCreate, new List<string> {tag},
            text) { }
    }
}