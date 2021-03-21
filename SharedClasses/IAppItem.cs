using System;

namespace SharedClasses
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
}
