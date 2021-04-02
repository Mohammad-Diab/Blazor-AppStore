using SharedLibraries;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
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
                if (string.IsNullOrEmpty(appId) || appId.ToLower() == "root")
                    appId = "";
                result = AppItem.AppsList.Values.Where(x => x.ParentId == appId).OrderBy(x => !x.IsFolder).ThenBy(x => x.Name);
            }
            else
            {
                result = AppItem.AppsList.Values.Where(x => !x.IsFolder && x.Name.ToLower().Contains(filterText.ToLower()));
                result = result.Union(AppItem.AppsList.Values.Where(x => !x.IsFolder && x.Location.ToLower().Contains(filterText.ToLower()))).Distinct().Take(5);
            }
            return result.ToList();
        }

        internal static IEnumerable<IAppItem> LastAccessedApps()
        {
            IEnumerable<AppItem> result = AppItem.AppsList.Values.Where(x => !x.IsFolder).OrderByDescending(x => x.FileDateAccessed).ThenBy(x => x.Name).Take(5);
            return result.ToList();
        }

        internal static string ReadFile(string fileId)
        {
            string result = "";
            string fullFilePath = AppItem.AppsList.Values.Where(x => x.Id == fileId).FirstOrDefault()?.GetFullPath();
            if (File.Exists(fullFilePath))
            {
                if (!(new FileInfo(fullFilePath).Length > Settings.MaxViewableFileSize))
                {
                    using (StreamReader stream = new StreamReader(File.OpenRead(fullFilePath)))
                    {
                        result = stream.ReadToEnd();
                    }
                }
            }
            return result;
        }

        internal static byte[] ReadImage(string imageName)
        {
            string fullIconPath = AppItem.GetFullImagePath(imageName);

            byte[] result = { };
            if (File.Exists(fullIconPath))
            {
                FileInfo imageFile = new FileInfo(fullIconPath);
                if (imageFile.Length > Settings.MaxViewableFileSize)
                {
                    fullIconPath = AppItem.GetFullImagePath("A");
                    imageFile = new FileInfo(fullIconPath);
                }
                using (BinaryReader stream = new BinaryReader(imageFile.OpenRead()))
                {
                    result = stream.ReadBytes((int)imageFile.Length);
                }   
            }
            return result;
        }
    }
}
