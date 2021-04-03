using AppStore.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Timers;

namespace AppStore.Pages
{
    public partial class Browser
    {
        private AppItem currentItem;
        List<AppItem> breadcrumbList;
        List<AppItem> contentList;
        List<AppItem> fullList;
        Modal ModalRef;

        string _filterText;
        string FilterText
        {
            get
            {
                return _filterText;
            }
            set
            {
                TextChanged(value);
            }
        }

        Timer filterTimer;

        void InitializeTimer()
        {
            filterTimer = new Timer
            {
                Interval = 300,
                AutoReset = false
            };
            filterTimer.Elapsed += (object sender, ElapsedEventArgs args) =>
            {
                FilterTheList();
            };
        }

        void TextChanged(string value)
        {
            _filterText = value?.ToLower() ?? "";
            filterTimer?.Stop();
            filterTimer?.Start();
        }

        async protected override Task OnInitializedAsync()
        {
            breadcrumbList = new List<AppItem>();
            fullList = new List<AppItem>();
            InitializeTimer();
            AppItem app = new AppItem("root", "root", "", "root", true, SharedLibraries.ItemType.Folder, 0, "", "", "");
            await GetDataFromServer(app);
            await base.OnInitializedAsync();
        }

        public async Task Navigate(AppItem item)
        {
            if (item.IsFolder)
            {
                await GetDataFromServer(item);
            }
            else
            {
                ShowModal(item);
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

                await GetDataFromServer(item);
            }
            StateHasChanged();
        }

        private async Task GetDataFromServer(AppItem item)
        {
            currentItem = item;
            breadcrumbList.Add(item);
            contentList = null;
            FilterText = "";
            filterTimer?.Stop();
            StateHasChanged();
            using HttpClient request = new HttpClient();
            var requestResult = await request.GetFromJsonAsync<List<AppItem>>($"{Config.ApiUrl}GetApps?appId={item.Id}");
            contentList = new List<AppItem>();
            contentList.AddRange(requestResult);
            fullList.Clear();
            fullList.AddRange(requestResult);
        }

        void FilterTheList()
        {
            if (!(contentList is null && fullList is null))
            {
                contentList = null;
                StateHasChanged();

                var result = fullList.Where(x => x.Name.ToLower().Contains(FilterText) || (!x.IsFolder && x.Extension.ToLower().Equals(FilterText)));
                contentList = new List<AppItem>();
                contentList.AddRange(result);
                StateHasChanged();
            }
        }

        public void ShowModal(AppItem item)
        {
            ModalRef.ShowModal(item);
        }

        void Dispose()
        {
            filterTimer?.Dispose();
        }

    }
}
