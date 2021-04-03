using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AppStore
{
    public static class Config
    {
        public static void SetConfig(string apiUrl, long maxViewableFileSize)
        {
            ApiUrl = apiUrl;
            MaxViewableFileSize = maxViewableFileSize;
        }

        public static string ApiUrl { get; private set; }
        public static long MaxViewableFileSize { get; private set; }


    }
}
