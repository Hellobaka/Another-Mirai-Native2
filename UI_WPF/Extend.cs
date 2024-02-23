using Another_Mirai_Native.Model;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Another_Mirai_Native.UI
{
    public static class Extend
    {
        public static ObservableCollection<T> ToObservableCollection<T>(this IEnumerable<T> source)
        {
            ObservableCollection<T> list = new();
            foreach (T item in source)
            {
                list.Add(item);
            }
            return list;
        }

        public static string GetImageUrlOrPathFromCQCode(CQCode cqCode)
        {
            string ParseUrlFromCQImg(string filePath)
            {
                Regex regex = new("url=(.*)");
                var a = regex.Match(File.ReadAllText(filePath));
                if (a.Groups.Count > 1)
                {
                    string capture = a.Groups[1].Value;
                    capture = capture.Split('\r').First();
                    return capture;
                }
                else
                {
                    return "";
                }
            }

            if (cqCode.IsImageCQCode is false)
            {
                return "";
            }
            string file = cqCode.Items["file"];
            string basePath = @"data\image";

            string filePath = Path.Combine(basePath, file);
            if (File.Exists(filePath))
            {
                if (filePath.EndsWith(".cqimg"))
                {
                    return ParseUrlFromCQImg(filePath);
                }
                else
                {
                    return new FileInfo(filePath).FullName;
                }
            }
            else
            {
                filePath += ".cqimg";
                if (File.Exists(filePath))
                {
                    return ParseUrlFromCQImg(filePath);
                }
                else
                {
                    return "";
                }
            }
        }
    }
}
