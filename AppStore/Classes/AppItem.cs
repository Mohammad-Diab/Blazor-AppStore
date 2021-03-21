using SharedLibraries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AppStore
{
    public class AppItem : IAppItem
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public bool IsFolder { get; }
        public ItemType Type { get; set; }
        public string Size { get; }
        public string DateModified { get; }
        public string ImagePath { get; set; }

        public AppItem()
        {

        }

        public AppItem(string id, string name, bool isFolder, ItemType type, string size, string dateModified, string imagePath)
        {
            Id = id;
            Name = name;
            IsFolder = isFolder;
            Type = type;
            Size = size;
            DateModified = dateModified;
            ImagePath = imagePath;
        }
    }
}
