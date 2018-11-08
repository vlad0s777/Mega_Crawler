// ReSharper disable InconsistentNaming
namespace Mega.Domain
{
    using System;

    public class Articles
    {
        public int Article_Id { get; set; }

        public DateTime Date_Create { get; set; }

        public string Text { get; set; }

        public string Head { get; set; }

        public int Outer_Article_Id { get; set; }
    }
}
