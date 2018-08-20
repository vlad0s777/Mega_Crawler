namespace Mega.Services.ContentCollector
{
    using System;
    using System.Collections.Generic;

    public class WrapperUries
    {
        public readonly HashSet<Uri> Uries;

        public WrapperUries()
        {
            this.Uries = new HashSet<Uri>();
        }
    }
}