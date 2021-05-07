using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;

namespace AppStoreServer
{
    public static class Config
    {
        private const string SyncLockKey = "1F04C5CC-1EC2-4E5E-B46F-8B46A362AA03";

        public const string Default_SpecialChar = "$";

        public static readonly string Default_DownloadableContentTag = $"{Default_SpecialChar}DOWNLOADABLECONTENT";

        private static readonly long Default_MaxViewableFileSize = 262144;                              // 256KB
        private static readonly string Default_AppDirectory = @"C:\AppStore\";
        private static readonly long Default_UpdateInterval = 3000000000;                               // 5 minutes
        private static readonly string Default_ApiURL = "http://localhost/AppStoreServer/Apps/";
        private static readonly string Default_IconsDirectoryName = $"{Default_SpecialChar}ICONS";
        private static readonly string Default_ExtraAppsDirectoryName = $"{Default_SpecialChar}EXTRA";
        private static readonly string Default_TempDirectoryName = $"{Default_SpecialChar}TEMP";
        private static readonly long Default_MaxDownloadableDirectorySize = 104857600;                  // 100MB

        public static void ReadConfigFile(bool isCallBack)
        {
            lock (SyncLockKey)
            {
                try
                {
                    if (_maxViewableFileSize != -1)
                    {
                        return;
                    }
                    string configFilePath = Path.Combine(Directory.GetCurrentDirectory(), "config.xml");
                    if (File.Exists(configFilePath))
                    {
                        XmlDocument document = new XmlDocument();
                        document.Load(configFilePath);

                        XmlNode settings = null;
                        foreach (XmlNode item in document.ChildNodes)
                        {
                            if (item.NodeType == XmlNodeType.Element)
                            {
                                settings = item;
                                break;
                            }
                        }
                        foreach (XmlNode item in settings?.ChildNodes)
                        {
                            if (item.ChildNodes.Count > 1)
                            {
                                string key = item.FirstChild.Name == "key" ? item.FirstChild.InnerText : item.LastChild.InnerText;
                                string value = item.FirstChild.Name == "value" ? item.FirstChild.InnerText : item.LastChild.InnerText;
                                switch (key.ToUpper())
                                {
                                    case "MAXVIEWABLEFILESIZE":
                                        _maxViewableFileSize = long.TryParse(value, out long maxViewableFileSize) 
                                            ? maxViewableFileSize 
                                            : Default_MaxViewableFileSize;
                                        break;
                                    case "UPDATEINTERVAL":
                                        _updateInterval = long.TryParse(value, out long updateInterval) 
                                            ? updateInterval 
                                            : Default_UpdateInterval;
                                        break;
                                    case "APIURL":
                                        _apiUrl = value;
                                        break;
                                    case "APPDIRECTORY":
                                        _appDirectory = value;
                                        break;
                                    case "EXTRAAPPSDIRECTORYNAME":
                                        _extraAppsDirectoryName = value;
                                        break;
                                    case "ICONSDIRECTORYNAME":
                                        _iconsDirectoryName = value;
                                        break;
                                    case "TEMPDIRECTORYNAME":
                                        _tempDirectoryName = value;
                                        break;
                                    case "MAXDOWNLOADABLEDIRECTORYSIZE":
                                        _maxDownloadableDirectorySize = long.TryParse(value, out long maxDownloadableDirectorySize)
                                            ? maxDownloadableDirectorySize
                                            : Default_MaxDownloadableDirectorySize;
                                        break;
                                }
                            }
                        }
                    }
                    else if (!isCallBack)
                    {
                        CreateNewConfigFile();
                        ReadConfigFile(true);
                    }
                }
                catch (Exception) { }

            }
        }

        public static void CreateNewConfigFile()
        {
            try
            {
                string configFilePath = Path.Combine(Directory.GetCurrentDirectory(), "config.xml");
                if (!File.Exists(configFilePath))
                {
                    using var xml = XmlWriter.Create(configFilePath);
                    void CreateSettingXmlNode(string key, string value)
                    {
                        xml.WriteStartElement("setting");
                        xml.WriteElementString("key", key);
                        xml.WriteElementString("value", value);
                        xml.WriteEndElement();
                    }
                    xml.WriteStartDocument();

                    xml.WriteStartElement("settings");

                    CreateSettingXmlNode("MaxViewableFileSize", Default_MaxViewableFileSize.ToString());
                    CreateSettingXmlNode("AppDirectory", Default_AppDirectory);
                    CreateSettingXmlNode("UpdateInterval", Default_UpdateInterval.ToString());
                    CreateSettingXmlNode("ApiURL", Default_ApiURL);
                    CreateSettingXmlNode("IconsDirectoryName", Default_IconsDirectoryName);
                    CreateSettingXmlNode("ExtraAppsDirectoryName", Default_ExtraAppsDirectoryName);
                    CreateSettingXmlNode("TempDirectoryName", Default_TempDirectoryName);
                    CreateSettingXmlNode("MaxDownloadableDirectorySize", Default_MaxDownloadableDirectorySize.ToString());

                    xml.WriteEndElement();

                    xml.WriteEndDocument();
                    xml.Flush();
                }
            }
            catch (Exception) { }
        }


        static long _maxViewableFileSize = -1;
        public static long MaxViewableFileSize
        {
            get
            {
                if (_maxViewableFileSize == -1)
                {
                    ReadConfigFile(false);
                }
                return _maxViewableFileSize;
            }
        }

        static long _updateInterval = -1;
        public static long UpdateInterval
        {
            get
            {
                if (_updateInterval == -1)
                {
                    ReadConfigFile(false);
                }
                return _updateInterval;
            }
        }

        static string _apiUrl = "";
        public static string ApiUrl
        {
            get
            {
                if (string.IsNullOrEmpty(_apiUrl))
                {
                    ReadConfigFile(false);
                }
                return _apiUrl;
            }
        }

        static string _appDirectory = "";
        public static string AppDirectory
        {
            get
            {
                if (string.IsNullOrEmpty(_appDirectory))
                {
                    ReadConfigFile(false);
                }
                return _appDirectory;
            }
        }

        static string _iconsDirectoryName = "";
        public static string IconsDirectoryName
        {
            get
            {
                if (string.IsNullOrEmpty(_iconsDirectoryName))
                {
                    ReadConfigFile(false);
                }
                return _iconsDirectoryName;
            }
        }

        static string _extraAppsDirectoryName = "";
        public static string ExtraAppsDirectoryName
        {
            get
            {
                if (string.IsNullOrEmpty(_extraAppsDirectoryName))
                {
                    ReadConfigFile(false);
                }
                return _extraAppsDirectoryName;
            }
        }

        static string _tempDirectoryName = "";
        public static string TempDirectoryName
        {
            get
            {
                if (string.IsNullOrEmpty(_tempDirectoryName))
                {
                    ReadConfigFile(false);
                }
                return _tempDirectoryName;
            }
        }

        static long _maxDownloadableDirectorySize = -1;
        public static long MaxDownloadableDirectorySize
        {
            get
            {
                if (_maxDownloadableDirectorySize == -1)
                {
                    ReadConfigFile(false);
                }
                return _maxDownloadableDirectorySize;
            }
        }
    }
}
