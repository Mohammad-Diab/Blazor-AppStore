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
        public async Task<IActionResult> GetImageAsync(string ImageName)
        {
            Random Rand = new Random();
            await Task.Delay(Rand.Next(10, 100));
            return File(Logic.ReadImage(ImageName), "image/png");
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
            string path = AppItem.GetFdmPath("","");
            if (!string.IsNullOrEmpty(path))
            {
                return new FileStreamResult(new FileStream(path, FileMode.Open), "application/force-download")
                {
                    FileDownloadName = Path.GetFileName(path)
                };
            }
            return null;
        }
    }
}
