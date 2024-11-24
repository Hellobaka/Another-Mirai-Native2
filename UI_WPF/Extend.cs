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
            ObservableCollection<T> list = [.. source];
            return list;
        }
    }
}
