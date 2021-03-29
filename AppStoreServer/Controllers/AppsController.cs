using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SharedLibraries;
using System;
using System.Collections.Generic;
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

    }
}
