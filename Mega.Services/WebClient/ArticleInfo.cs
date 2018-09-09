namespace Mega.Services.WebClient
{
    using System;
    using System.Collections.Generic;

    public class ArticleInfo
    {
        public readonly DateTime DateCreate;

        public readonly Dictionary<string, string> Tags;

        public readonly string Text;

        public readonly string Head;

        public readonly string Url;

        public ArticleInfo(DateTime dateCreate, Dictionary<string, string> tags, string text, string head, string url)
        {
            this.DateCreate = dateCreate;
            this.Tags = tags;
            this.Text = text;
            this.Head = head;
            this.Url = url;
        }
    }
}
