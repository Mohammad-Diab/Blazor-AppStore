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
        private AppItem currentItem;
        List<AppItem> breadcrumbList;
        List<AppItem> contentList;

        async protected override Task OnInitializedAsync()
        {
            breadcrumbList = new List<AppItem>();
            AppItem app = new AppItem("root", "root", true, SharedLibraries.ItemType.Folder, "", "", "");
            await GetDataFromServer(app);
            base.OnInitialized();
        }

        public async Task Navigate(AppItem item)
        {
            if (item.IsFolder)
            {
                await GetDataFromServer(item);
            }
            else
            {
                
            }
            StateHasChanged();
        }

        public async Task NavigateBack(AppItem item)
        {
            if (item.IsFolder)
            {
                int index = breadcrumbList.IndexOf(item);
                if (index > -1)
                {
                    for (; index < breadcrumbList.Count;)
                    {
                        breadcrumbList.RemoveAt(index);
                    }
                }

               await  GetDataFromServer(item);
            }
            else
            {

            }
            StateHasChanged();
        }

        private async Task GetDataFromServer(AppItem item)
        {
            currentItem = item;
            breadcrumbList.Add(item);
            contentList = null;
            StateHasChanged();
            using (HttpClient request = new HttpClient())
            {
                var requestResult = await request.GetFromJsonAsync<List<AppItem>>($"{Config.ApiUrl}GetApps?appId={item.Id}");
                contentList = new List<AppItem>();
                contentList.AddRange(requestResult);
            }
        }

    }
}
