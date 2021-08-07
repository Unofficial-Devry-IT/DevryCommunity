using System;
using System.Collections.Generic;
using HtmlAgilityPack;

namespace ImageCreator
{
    public struct CachedResult
    {
        public List<HtmlNode> Nodes { get; set; }
        public DateTime DeleteAfter { get; set; }
    }
}