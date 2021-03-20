using System;

namespace Shared
{
    public interface IAppItem
    {
        string Id { get; set; }

        string Name { get; set; }

        bool IsFolder { get; }

        ItemType Type { get; set; }

        string Size { get; }

        string DateModified { get; }

        string ImagePath { get; set; }
    }

    public enum ItemType
    {
        Folder = 1,
        File = 2,
        Text = 3,
        ISO = 4
    }
}
