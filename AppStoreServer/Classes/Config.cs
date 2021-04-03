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
                                switch (key)
                                {
                                    case "MaxViewableFileSize":
                                        long maxViewableFileSize = 0;
                                        if (!long.TryParse(value, out maxViewableFileSize))
                                        {
                                            maxViewableFileSize = 262144;
                                        }
                                        _maxViewableFileSize = maxViewableFileSize;
                                        break;
                                    case "UpdateInterval":
                                        long updateInterval = 0;
                                        if (!long.TryParse(value, out updateInterval))
                                        {
                                            updateInterval = 3000000000;
                                        }
                                        _updateInterval = updateInterval;
                                        break;
                                    case "ApiUrl":
                                        _apiUrl = value;
                                        break;
                                    case "AppDirectory":
                                        _appDirectory = value;
                                        break;
                                    case "ExtraAppsDirectoryName":
                                        _extraAppsDirectoryName = value;
                                        break;
                                    case "IconsDirectoryName":
                                        _iconsDirectoryName = value;
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

                    CreateSettingXmlNode("MaxViewableFileSize", "262144");
                    CreateSettingXmlNode("AppDirectory", @"C:\AppStore\");
                    CreateSettingXmlNode("UpdateInterval", "3000000000");
                    CreateSettingXmlNode("ApiUrl", "http://localhost/AppStoreServer/Apps/");
                    CreateSettingXmlNode("IconsDirectoryName", "$ICONS");
                    CreateSettingXmlNode("ExtraAppsDirectoryName", "$Extra");

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
    }
}
