using System;
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
    }
}
