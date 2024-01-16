using System.Collections.Generic;
using System.Collections.ObjectModel;

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
    }
}
