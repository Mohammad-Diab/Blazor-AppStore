using System;
using System.Collections.Generic;
using System.Text;

namespace SharedLibraries
{
    public static class Settings
    {
        private static long _maxViewableFileSize;
        public static long MaxViewableFileSize
        {
            get
            {
                if (_maxViewableFileSize == 0)
                {
                    _maxViewableFileSize = 262144;
                }
                return _maxViewableFileSize;
            }
        }
    }
}
