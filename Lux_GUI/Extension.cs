using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Lux_GUI
{
    public static class Extension
    {
        public static void AddEx<T>(this ObservableCollection<T> collection, T item)
        {
            if (Application.Current.Dispatcher.CheckAccess())
            {
                collection.Add(item);
            }
            else
            {
                Application.Current.Dispatcher.BeginInvoke(new Action<T>(collection.Add), item);
            }
        }
    }
}
