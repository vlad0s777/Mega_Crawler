namespace Mega.Services.WebClient
{
    using System;
    using System.Collections.Generic;

    public class ArticleInfo
    {
        public readonly DateTime DateCreate;

        public readonly List<TagInfo> Tags;

        public readonly string Text;

        public readonly string Head;

        public readonly int Id;

        public ArticleInfo(DateTime dateCreate, List<TagInfo> tags, string text, string head, int id)
        {
            this.DateCreate = dateCreate;
            this.Tags = tags;
            this.Text = text;
            this.Head = head;
            this.Id = id;
        }
    }
}
