using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Shared;
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
        public IEnumerable<IAppItem> GetApps(string appId = "")
        {
            return Logic.GetApps(appId: appId);
        }

        [HttpGet]
        public IEnumerable<IAppItem> SearchInApps(string filterText = "")
        {
            return Logic.GetApps(filterText: filterText);
        }

    }
}
