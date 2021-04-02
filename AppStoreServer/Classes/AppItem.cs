using SharedLibraries;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AppStoreServer
{
    public class AppItem : IAppItem
    {
        private const string AppDirectory = @"C:\AppStore\";
        private const long UpdateInterval = 3000000000; /* 5 minutes */
        private const string SyncLockKey = "C9CAEFBE-5754-4753-989C-C05620F536E1";

        private static readonly Regex HexReg = new Regex(@"^[0-9A-F\r\n]+$");

        private static long LastUpdatedTime = 0;

        public static Dictionary<string, AppItem> _appsList;
        public static Dictionary<string, AppItem> AppsList
        {
            get
            {
                if (_appsList is null)
                {
                    _appsList = LoadAllApps();
                    LastUpdatedTime = DateTime.Now.Ticks;
                }

                if (DateTime.Now.Ticks - LastUpdatedTime > UpdateInterval)
                {
                    _ = UpdateAppsListTask();
                }
                return _appsList;
            }
        }

        private async static Task UpdateAppsListTask()
        {
            await Task.Run(UpdateAppsList);
            LastUpdatedTime = DateTime.Now.Ticks;
        }

        private static void UpdateAppsList()
        {
            lock (SyncLockKey)
            {
                if (DateTime.Now.Ticks - LastUpdatedTime < UpdateInterval)
                {
                    return;
                }
                foreach (var item in AppsList.Values)
                {
                    string fullItemPath = item.Location;
                    try
                    {
                        if (item.IsFolder)
                        {
                            item.FileDateModified = Directory.GetLastWriteTimeUtc(fullItemPath);
                            item.FileDateAccessed = Directory.GetLastAccessTimeUtc(fullItemPath).Ticks;
                        }
                        else
                        {
                            item.FileDateModified = File.GetLastWriteTimeUtc(fullItemPath);
                            item.FileDateAccessed = File.GetLastAccessTimeUtc(fullItemPath).Ticks;
                        }
                    }
                    catch (Exception) { }
                }
            }
        }

        public static void ResetAppList()
        {
            _appsList = null;
        }

        internal static string GetFullImagePath(string imageName)
        {
            if (!(HexReg.Match(imageName).Success && File.Exists(Path.Combine(AppDirectory, Shared.IconsDirectoryName, imageName + ".png"))))
            {
                return FileIconPath;
            }
            else
            {
                return Path.Combine(AppDirectory, Shared.IconsDirectoryName, imageName + ".png");
            }
        }

        internal string GetFullPath()
        {
            return Path.Combine(AppDirectory, Location);
        }

        #region Constructors

        public AppItem()
        {
            childernCount = -1;
        }

        public AppItem(string id, bool isFolder, string parentId, FileSystemInfo item)
        {
            Id = id;
            Name = Path.GetFileNameWithoutExtension(item.Name);
            Extension = isFolder ? "" : Path.GetExtension(item.Name);
            Location = item.FullName.Replace(AppDirectory, "");
            if (isFolder)
            {
                Type = ItemType.Folder;
            }
            else
            {
                Type = item.Extension.ToLower() switch
                {
                    ".iso" => ItemType.ISO,
                    ".txt" => ItemType.Text,
                    _ => ItemType.File
                };

                FileSize = (item as FileInfo).Length;
            }
            FileDateModified = item.LastWriteTimeUtc;
            FileDateAccessed = item.LastAccessTimeUtc.Ticks;
            ImageName = Shared.HashText(item.FullName);
            if (!File.Exists(Path.Combine(AppDirectory, Shared.IconsDirectoryName, $"{ImageName}.png")))
            {
                ImageName = Shared.HashText(Type switch
                {
                    ItemType.Folder => "FOLDER",
                    ItemType.ISO => "ISO",
                    ItemType.Text => "TEXT",
                    _ => "APPLICATION"
                });
            }
            ParentId = parentId;
            childernCount = -1;
        }

        #endregion

        #region Properties
        public string Id { get; set; }

        public string Name { get; set; }

        public string Extension { get; set; }

        public string Location { get; set; }

        public ItemType Type { get; set; }

        internal DateTime FileDateModified { get; set; }

        internal long FileDateAccessed { get; set; }

        public string ImageName { get; set; }

        internal long FileSize { get; set; }

        internal string ParentId { get; set; }

        #endregion

        #region Read Only Properties

        private static string FileIconPath
        {
            get
            {
                return Path.Combine(AppDirectory, Shared.IconsDirectoryName, $"{Shared.HashText("APPLICATION")}.png");
            }
        }
        public bool IsFolder
        {
            get
            {
                return Type == ItemType.Folder;
            }
        }

        private int childernCount;

        private int ChildernCount
        {
            get
            {
                if (!IsFolder)
                {
                    childernCount = 0;
                }

                if (childernCount == -1 && IsFolder)
                {
                    childernCount = AppsList.Values.Count(x => x.ParentId == Id);
                }
                return childernCount;
            }
        }

        public string Content
        {
            get
            {
                return IsFolder ? $"{ChildernCount} item{(ChildernCount == 1 ? "" : "s")}" : Shared.FriendlyFileSize(FileSize, 2);
            }
        }

        public long Size
        {
            get
            {
                return FileSize;
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
}
