using SharedLibraries;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.IO.Compression;

namespace AppStoreServer
{

    public class Logic
    {
        internal static IEnumerable<IAppItem> GetApps(string appId = "", string filterText = "")
        {
            IEnumerable<AppItem> result;
            if (string.IsNullOrEmpty(filterText))
            {
                if (string.IsNullOrEmpty(appId) || appId.ToLower() == "root")
                    appId = "";
                result = AppItem.AppsList.Values.Where(x => x.ParentId == appId).OrderBy(x => !x.IsFolder).ThenBy(x => x.Name);
            }
            else
            {
                result = AppItem.AppsList.Values.Where(x => (!x.IsFolder || x.IsDownloadableContent) && x.Name.ToLower().Contains(filterText.ToLower()));
                result = result.Union(AppItem.AppsList.Values.Where(x => (!x.IsFolder || x.IsDownloadableContent) && x.Location.ToLower().Contains(filterText.ToLower()))).Distinct().Take(5);
            }
            return result.ToList();
        }

        internal static IEnumerable<IAppItem> LastAccessedApps()
        {
            IEnumerable<AppItem> result = AppItem.AppsList.Values.Where(x => !x.IsFolder).OrderByDescending(x => x.FileDateAccessed).ThenBy(x => x.Name).Take(5);
            return result.ToList();
        }

        internal static AppItem GetApp(string fileId)
        {
            return AppItem.AppsList.Values.Where(x => x.Id == fileId).FirstOrDefault();
        }

        internal static string ReadFile(string fileId)
        {
            string result = "";
            string fullFilePath = AppItem.AppsList.Values.Where(x => x.Id == fileId).FirstOrDefault()?.GetFullPath();
            if (File.Exists(fullFilePath))
            {
                if (!(new FileInfo(fullFilePath).Length > Config.MaxViewableFileSize))
                {
                    using (StreamReader stream = new StreamReader(File.OpenRead(fullFilePath)))
                    {
                        result = stream.ReadToEnd();
                    }
                }
            }
            return result;
        }

        internal static byte[] ReadImage(string imageName, int imageWidth)
        {
            string fullIconPath = AppItem.GetFullImagePath(imageName, imageWidth);

            byte[] result = { };
            if (File.Exists(fullIconPath))
            {
                FileInfo imageFile = new FileInfo(fullIconPath);
                if (imageFile.Length > Config.MaxViewableFileSize)
                {
                    fullIconPath = AppItem.GetFullImagePath("A", imageWidth);
                    imageFile = new FileInfo(fullIconPath);
                }
                using (BinaryReader stream = new BinaryReader(imageFile.OpenRead()))
                {
                    result = stream.ReadBytes((int)imageFile.Length);
                }
            }
            return result;
        }

        internal static (string filePath, string directoryName) GetDirctoryPath(string DirectoryId)
        {
            AppItem item = GetApp(DirectoryId);
            string directoryPath = item?.GetFullPath();
            string directoryName = item?.Name;
            if (!string.IsNullOrEmpty(directoryPath) && File.Exists(Path.Combine(directoryPath, Config.Default_DownloadableContentTag)))
            {
                long DirectorySize = item.GetSize();
                string zipFileName = Shared.HashText($"{item.GetFullPath().Replace(Config.AppDirectory, "")}/{item.ChildernCount}");
                string zipFileDirectory = Path.Combine(Config.AppDirectory, Config.TempDirectoryName);
                string zipFileFullName = Path.Combine(zipFileDirectory, $"{zipFileName}.zip");
                if (!File.Exists(zipFileFullName))
                {
                    long currentDriveFreeSpace = Shared.GetDriveFreeSize(zipFileDirectory);
                    if (DirectorySize < currentDriveFreeSpace && DirectorySize < Config.MaxDownloadableDirectorySize)
                    {
                        try
                        {
                            if (!Directory.Exists(zipFileDirectory))
                            {
                                Directory.CreateDirectory(zipFileDirectory);
                            }
                            ZipFile.CreateFromDirectory(directoryPath, zipFileFullName, CompressionLevel.Fastest, false);
                        }
                        catch (Exception) { }
                        return (zipFileFullName, directoryName);
                    }
                }
                else
                {
                    return (zipFileFullName, directoryName);
                }
            }
            return (null, null);
        }
    }
}
