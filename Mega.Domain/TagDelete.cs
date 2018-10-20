namespace Mega.Domain
{
    using System;

    public class TagDelete
    {
        public int TagDeleteId { get; set; }

        public Tag Tag { get; set; }

        public int TagId { get; set; }

        public DateTime DateDelete { get; set; }
    }
}
