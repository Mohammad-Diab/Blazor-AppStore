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
        public string Extension { get; set; }
        public string Location { get; set; }
        public bool IsFolder { get; set; }
        public ItemType Type { get; set; }
        public string Content { get; set; }
        public long Size { get; set; }
        public string DateModified { get; set; }
        public string ImageName { get; set; }

        public AppItem()
        {

        }

        public AppItem(string id, string name, string extension, string location, bool isFolder, ItemType type, long size, string content, string dateModified, string imagePath)
        {
            Id = id;
            Name = name;
            Extension = extension;
            Location = location;
            IsFolder = isFolder;
            Type = type;
            Content = content;
            Size = size;
            DateModified = dateModified;
            ImageName = imagePath;
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj is AppItem)
            {
                return Id.Equals((obj as AppItem)?.Id);
            }
            return false;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
