using System;
using System.Collections.Generic;

namespace Mega.Services
{
    public class ArticleInfo
    {
        public readonly DateTime DateCreate;
        public readonly Dictionary<string,string> Tags;
        public readonly string Text;
        public readonly string Head;

        public ArticleInfo(DateTime dateCreate, Dictionary<string, string> tags, string text, string head)
        {
            this.DateCreate = dateCreate;
            this.Tags = tags;
            this.Text = text;
            this.Head = head;
        }
    }
}