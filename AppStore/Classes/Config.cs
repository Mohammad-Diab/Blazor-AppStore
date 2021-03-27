using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AppStore
{
    public static class Config
    {
        private static string _apiUrl;
        public static string ApiUrl
        {
            get
            {
                if (string.IsNullOrEmpty(_apiUrl))
                {
                    _apiUrl = "http://localhost/AppStoreServer/Apps/";
                }
                return _apiUrl;
            }
        }
    }
}
