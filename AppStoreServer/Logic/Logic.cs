using Shared;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace AppStoreServer
{
    public class Logic
    {
        internal static IEnumerable<IAppItem> GetApps(string appId = "", string filterText = "")
        {
            IEnumerable<AppItem> result;
            if (string.IsNullOrEmpty(filterText))
            {
                appId ??= "";
                result = AppItem.AppsList.Values.Where(x => x.ParentId == appId).OrderBy(x => x.IsFolder).ThenBy(x => x.Name);
            }
            else
            {
                result = AppItem.AppsList.Values.Where(x => !x.IsFolder && x.Name.ToLower().Contains(filterText.ToLower()));
                result.Union(AppItem.AppsList.Values.Where(x => !x.IsFolder && x.Path.ToLower().Contains(filterText.ToLower()))).Distinct();
            }
            return result.ToList();
        }
    }

    public class AppItem : IAppItem
    {
        private const string AppDirectory = @"C:\AppStore\";

        public static Dictionary<string, AppItem> _appsList;
        public static Dictionary<string, AppItem> AppsList
        {
            get
            {
                if (_appsList is null)
                {
                    _appsList = LoadAllApps();
                }
                return _appsList;
            }
        }

        public static void ResetAppList()
        {
            _appsList = null;
        }        

        #region Constructors

        public AppItem()
        {

        }

        public AppItem(string id, bool isFolder, string parentId, FileSystemInfo item)
        {
            Id = id;
            Name = item.Name;
            Path = item.FullName.Replace(AppDirectory, "");
            if (isFolder)
            {
                Type = ItemType.Folder;
            }
            else
            {
                Type = item.Extension.ToLower() switch
                {
                    "iso" => ItemType.ISO,
                    "txt" => ItemType.Text,
                    _ => ItemType.File
                };
                
                FileSize = (item as FileInfo).Length;
            }
            FileDateModified = item.LastWriteTimeUtc;
            ImagePath = System.IO.Path.Combine("$ICONS", Shared.HashText(item.FullName) + ".png");
            FullImagePath = System.IO.Path.Combine(AppDirectory, ImagePath);
            ParentId = parentId;

        }

        #endregion

        #region Properties
        public string Id { get; set; }

        public string Name { get; set; }

        public string Path { get; set; }

        public ItemType Type { get; set; }

        internal DateTime FileDateModified { get; set; }

        public string ImagePath { get; set; }

        internal string FullImagePath { get; set; }

        internal long FileSize { get; set; }

        internal string ParentId { get; set; }

        #endregion

        #region Read Only Properties

        public bool IsFolder
        {
            get
            {
                return Type == ItemType.Folder;
            }
        }

        public string Size
        {
            get
            {
                return Shared.FriendlyFileSize(FileSize, 2);
            }
        }

        public string DateModified
        {
            get
            {
                return FileDateModified.ToString("yyyy/MM/dd HH:mm");
            }
        }

        #endregion

        #region Private Method

        private static Dictionary<string, AppItem> LoadAllApps()
        {
            DirectoryInfo root = new DirectoryInfo(AppDirectory);

            List<AppItem> list = new List<AppItem>();
            LoadChildren(root, "", ref list, out int k);
            return list.ToDictionary(x => x.Id, x => x);
        }

        private static void LoadChildren(DirectoryInfo directory, string parentId, ref List<AppItem> result, out int addedFilesCount)
        {
            addedFilesCount = 0;
            bool isRoot = AppDirectory == directory.FullName;
            if (directory.Exists)
            {
                string Id = isRoot ? "" : Guid.NewGuid().ToString();
                try
                {
                    var subDirectory = directory.GetDirectories();
                    foreach (var item in subDirectory)
                    {
                        if (item.Name.StartsWith("$"))
                            continue;
                        LoadChildren(item, Id, ref result, out int subAddedFilesCount);
                        addedFilesCount += subAddedFilesCount;
                    }
                }
                catch (Exception) { }

                try
                {
                    var files = directory.GetFiles();
                    foreach (var item in files)
                    {
                        if (item.Name.StartsWith("$"))
                            continue;
                        string fileId = Guid.NewGuid().ToString();
                        result.Add(new AppItem(fileId, false, Id, item));
                        addedFilesCount++;
                    }
                }
                catch (Exception) { }


                if (!isRoot && addedFilesCount > 0)
                {
                    result.Add(new AppItem(Id, true, parentId, directory));
                }
            }
        }
        
        #endregion

        #region Override Functions

        public override bool Equals(object obj)
        {
            if (obj is AppItem)
            {
                return Id.Equals((obj as AppItem)?.Id);
            }
            return false;
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        public override string ToString()
        {
            return Name;
        }

        #endregion

    }

    public static class Shared
    {
        internal static string FriendlyFileSize(double SizeInByte, int round)
        {
            if (SizeInByte < 1)
                return "0 byte";
            byte i = 0;
            while (SizeInByte / 1024 > 1)
            {
                SizeInByte /= 1024;
                i++;
            }
            return Math.Round(SizeInByte, round) + " " + FileSizeUnit(i);
        }

        internal static string HashText(string Text)
        {
            return Text.GetHashCode().ToString("X");
        }

        private static string FileSizeUnit(byte num) => num switch
        {
            0 => "byte",
            1 => "KiB",
            2 => "MiB",
            3 => "GiB",
            4 => "TiB",
            5 => "PiB",
            6 => "EiB",
            7 => "ZiB",
            8 => "YiB",
            _ => "byte",
        };
    }

}
