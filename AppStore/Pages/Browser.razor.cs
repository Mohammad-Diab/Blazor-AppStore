using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace AppStore.Pages
{
    public partial class Browser
    {
        string currentLocation = "root";
        List<string> breadcrumbList = new List<string>();
        List<string> contentListClassList;
        List<AppItem> contentList;

        async protected override Task OnInitializedAsync()
        {
            using (HttpClient request = new HttpClient())
            {
                await Task.Delay(1000);
                contentList = new List<AppItem>();
                contentList.AddRange(await request.GetFromJsonAsync<List<AppItem>>("http://localhost/AppStoreServer/Apps/GetApps"));
                contentListClassList = new List<string>(contentList.Select(x => "d-none"));
            }
            base.OnInitialized();
        }

        async protected override Task OnAfterRenderAsync(bool firstRender)
        {
            for (int i = 0; i < contentListClassList?.Count; i++)
            {
                contentListClassList[i] = "";
                await Task.Delay(1000);
            }
            await base.OnAfterRenderAsync(firstRender);
        }
    }
}
