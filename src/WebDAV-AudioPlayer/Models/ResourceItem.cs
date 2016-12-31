using System;
using System.Collections.Generic;
using System.IO;

namespace WebDav.AudioPlayer.Models
{
    public class ResourceItem
    {
        public string DisplayName { get; set; }

        public Uri FullPath { get; set; }

        public bool IsCollection { get; set; }

        public long? ContentLength { get; set; }

        public Stream Stream { get; set; }

        public IList<ResourceItem> Items { get; set; }
    }
}