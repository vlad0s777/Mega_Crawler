﻿using System;

namespace Mega.Services
{
    public class UriLimits
    {
        public readonly Uri Uri;
        public readonly int Attempt;
        public readonly int Depth;
        public UriLimits(Uri uri, int attempt = 0, int depth = 1)
        {
            this.Uri = uri;
            this.Attempt = attempt;
            this.Depth = depth;
        }
    }
}