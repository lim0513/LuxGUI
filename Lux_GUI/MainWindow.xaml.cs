using MahApps.Metro.Controls;
using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml;
using System.Xml.XPath;

namespace Lux_GUI
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    [PropertyChanged.AddINotifyPropertyChangedInterface]
    public partial class MainWindow : MetroWindow
    {
        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = this;
        }

        public string LuxInfo { get; set; }

        public int SelectPage { get; set; } = 0;

        public string InputUrl { get; set; }

        public bool IsPlayList { get; set; }

        public bool HasLux => LuxHelper.Instance.IsLuxExist;

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            LuxInfo = LuxHelper.Instance.GetLuxInfo();
        }

        private void BtnParser_Click(object sender, RoutedEventArgs e)
        {
            if (IsPlayList)
            {
                this.SelectPage = 1;
                playList.Parser(InputUrl);
            }
            else
            {
                this.SelectPage = 0;
                singleFile.Parser(InputUrl);
            }

        }
    }
}
