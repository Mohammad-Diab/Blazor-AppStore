using System;

namespace SharedLibraries
{
    public interface IAppItem
    {
        string Id { get; set; }

        string Name { get; set; }

        bool IsFolder { get; }

        ItemType Type { get; set; }

        string Content { get; }

        string DateModified { get; }

        string ImageName { get; set; }
    }
}
