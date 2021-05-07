using System;
using System.IO;
using System.Security.Cryptography;

namespace AppStoreServer
{
    public static class Shared
    {
        private static readonly MD5 md5Hash = MD5.Create();

        internal static string FriendlyFileSize(double SizeInByte, int round)
        {
            if (SizeInByte < 1)
                return "0 bytes";

            if (SizeInByte == 1)
                return "1 byte";

            byte i = 0;
            while (SizeInByte / 1024 > 1)
            {
                SizeInByte /= 1024;
                i++;
            }
            return Math.Round(SizeInByte, round) + " " + FileSizeUnit(i);
        }

        internal static string HashText(string Text)
        {
            byte[] hashResult = md5Hash.ComputeHash(System.Text.Encoding.UTF8.GetBytes(Text));
            var result = BitConverter.ToString(hashResult).Replace("-", "");
            return result;
        }

        private static string FileSizeUnit(byte num) => num switch
        {
            0 => "byte",
            1 => "KiB",
            2 => "MiB",
            3 => "GiB",
            4 => "TiB",
            5 => "PiB",
            6 => "EiB",
            7 => "ZiB",
            8 => "YiB",
            _ => "bytes",
        };

        internal static string GetClientOS(string userAgent)
        {
            if (string.IsNullOrEmpty(userAgent))
                return "windows";

            if (userAgent.Contains("Android"))
                return "android";

            if (userAgent.Contains("iPad"))
                return "ipad os";

            if (userAgent.Contains("iPhone"))
                return "iphone os";

            if (userAgent.Contains("Linux") && userAgent.Contains("KFAPWI"))
                return "kindle fire";

            if (userAgent.Contains("RIM Tablet") || (userAgent.Contains("BB") && userAgent.Contains("Mobile")))
                return "black berry";

            if (userAgent.Contains("Windows Phone"))
                return "windows phone";

            if (userAgent.Contains("Mac OS"))
                return "mac os";

            if (userAgent.Contains("Windows NT"))
                return "windows";

            if (userAgent.Contains("Linux"))
                return "linux";

            return "windows";
        }

        internal static bool Is64bitClient(string userAgent)
        {
            return userAgent.Contains("x64");
        }

        internal static long GetDriveFreeSize(string zipFileDirectory)
        {
            DriveInfo di = new DriveInfo(zipFileDirectory.Substring(0, 1));
            long availableFreeSpace = di.AvailableFreeSpace;
            long driveMaximumFileSize = GetFileSystemMaximumFileSize(di.DriveFormat);
            return Math.Min(availableFreeSpace, driveMaximumFileSize);
        }

        internal static long GetFileSystemMaximumFileSize(string driveFormat) => driveFormat.ToLower() switch
        {
            "fat16" => 4294967294,                  // 4 Gibibyte 
            "fat32" => 4294967294,                  // 4 Gibibyte
            "ntfs" => long.MaxValue,                // 16 Exabyte
            "refs" => long.MaxValue,                // 16 Exabyte
            "exfat" => long.MaxValue,               // 16 Exabyte
            "apfs" => long.MaxValue,                // 8 Exabyte
            "ext2" => 2199023255552,                // 2 Terabyte
            "ext3" => 2199023255552,                // 2 Terabyte
            "ext4" => 8796093022208,                // 16 Terabyte
            _ => 50
        };

    }
}
