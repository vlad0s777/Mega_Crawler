namespace Mega.Domain
{
    using System;

    public class RemovedTag
    {
        public int RemovedTagId { get; set; }

        public Tag Tag { get; set; }

        public int TagId { get; set; }

        public DateTime DeletionDate { get; set; }
    }
}
