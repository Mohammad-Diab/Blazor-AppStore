using System;

namespace SharedLibraries
{
    public interface IAppItem
    {
        string Id { get; set; }

        string Name { get; set; }

        string Extension { get; set; }

        string Location { get; set; }

        bool IsFolder { get; }

        ItemType Type { get; set; }

        string Content { get; }

        long Size { get; }

        string DateModified { get; }

        string ImageName { get; set; }
    }
}
