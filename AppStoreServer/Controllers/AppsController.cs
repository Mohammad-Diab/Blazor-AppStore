using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SharedLibraries;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace AppStoreServer.Controllers
{
    [Route("[controller]/[action]")]
    [ApiController]
    public class AppsController : ControllerBase
    {
        [HttpGet]
        public IEnumerable<IAppItem> GetApps(string appId = "root")
        {
            return Logic.GetApps(appId: appId);
        }

        [HttpGet]
        public IEnumerable<IAppItem> SearchInApps(string filterText = "")
        {
            return Logic.GetApps(filterText: filterText);
        }

        [HttpGet]
        public IEnumerable<IAppItem> GetLastAccessedApps()
        {
            return Logic.LastAccessedApps();
        }

        [HttpGet]
        public async Task<IActionResult> GetImage(string ImageName, int imageWidth)
        {
            Random Rand = new Random();
            await Task.Delay(Rand.Next(10, 100));
            return File(Logic.ReadImage(ImageName, imageWidth), "image/png");
        }

        [HttpGet]
        public JsonResult GetFileContent(string FileId)
        {
            return new JsonResult(Logic.ReadFile(FileId));
        }

        [HttpGet]
        public FileStreamResult DownloadFile(string FileId)
        {
            AppItem item = Logic.GetApp(FileId);
            if (!(item is null))
            {
                var path = item.GetFullPath();
                return new FileStreamResult(new FileStream(path, FileMode.Open), "application/force-download")
                {
                    FileDownloadName = Path.GetFileName(path)
                };
            }
            return null;
        }

        [HttpGet]
        public FileStreamResult DownloadFDM()
        {
            string os = Shared.GetClientOS(Request.Headers["User-Agent"].ToString());
            string architecture = Shared.Is64bitClient(Request.Headers["User-Agent"].ToString()) ? "x64" : "x86";
            string path = AppItem.GetFdmPath(os, architecture);
            if (!string.IsNullOrEmpty(path))
            {
                return new FileStreamResult(new FileStream(path, FileMode.Open), "application/force-download")
                {
                    FileDownloadName = Path.GetFileName(path)
                };
            }
            return null;
        }


        [HttpGet]
        public FileStreamResult DownloadDirectory(string DirectoryId)
        {
            (string zipFilePath, string name) = Logic.GetDirctoryPath(DirectoryId);
            if (!string.IsNullOrEmpty(zipFilePath))
            {
                return new FileStreamResult(new FileStream(zipFilePath, FileMode.Open), "application/force-download")
                {
                    FileDownloadName = $"{name}.zip"
                };
            }
            return null;
        }

    }
}
