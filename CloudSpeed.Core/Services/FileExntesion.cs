using System;
using System.IO;

namespace CloudSpeed.Services
{
    public static class FileExntesion
    {
        public static string ToFileSize(this long len)
        {
            var sizes = new[] { "B", "KiB", "MiB", "GiB", "TiB" };
            var order = 0;
            while (len >= 1024 && order < sizes.Length - 1)
            {
                order++;
                len = len / 1024;
            }
            return String.Format("{0:0.##} {1}", len, sizes[order]);
        }

        public static string ToFileSize(this FileInfo info)
        {
            if (info.Exists)
                return info.Length.ToFileSize();
            return string.Empty;
        }

        public static string GetMimeType(this string fileName)
        {
            return MimeTypes.GetMimeType(fileName);
        }
    }
}
