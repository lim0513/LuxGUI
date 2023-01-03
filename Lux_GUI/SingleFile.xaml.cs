using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
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

namespace Lux_GUI
{
    /// <summary>
    /// SingleFile.xaml 的交互逻辑
    /// </summary>
    [PropertyChanged.AddINotifyPropertyChangedInterface]
    public partial class SingleFile : UserControl
    {
        public SingleFile()
        {
            InitializeComponent();
            this.DataContext = this;
        }

        public ObservableCollection<StreamInfo> Streams { get; } = new ObservableCollection<StreamInfo>();

        public ICollectionView ComboboxView { get; set; }

        public ICollectionView StreamsView { set; get; }

        public string DownloadUrl { get; set; }

        public string AdditionParam { get; set; }

        public bool OnComboFilterTriggered(object item)
        {
            if (!string.IsNullOrEmpty(SelectedQuality))
            {
                var lookup = item as StreamInfo;
                return lookup != null && lookup.Quality.Contains(SelectedQuality);
            }
            return true;
        }


        private string _selectedQuality;
        public string SelectedQuality
        {
            get
            {
                return _selectedQuality;
            }
            set
            {
                _selectedQuality = value;
                CollectionViewSource.GetDefaultView(Streams).Filter = OnComboFilterTriggered;
                StreamsView.Refresh();
            }
        }

        public void Parser(string InputUrl)
        {
            Streams.Clear();

            DownloadUrl = InputUrl;
            Task.Factory.StartNew(() =>
            {
                try
                {
                    var DlSite = "";
                    var DlTitle = "";
                    var DlType = "";
                    StreamInfo si = new StreamInfo(DownloadUrl);
                    var result = LuxHelper.Instance.ParserUrl(DownloadUrl).Split(System.Environment.NewLine.ToCharArray());
                    for (int i = 0; i < result.Length; i++)
                    {
                        var line = result[i];
                        if (line.StartsWith("Site:"))
                        {
                            DlSite = line.Trim().Substring(7);
                        }
                        else if (line.StartsWith(" Title:"))
                        {
                            DlTitle = line.Trim().Substring(7);
                        }
                        else if (line.StartsWith(" Type:"))
                        {
                            DlType = line.Split(':')[1].Trim();
                        }
                        else if (line.StartsWith(" Streams:"))
                        {

                        }
                        else if (line.Trim().StartsWith("["))
                        {
                            si = new StreamInfo(DownloadUrl)
                            {
                                Site = DlSite,
                                Title = DlTitle,
                                Type = DlType,
                                Name = line.Substring(line.IndexOf("[") + 1, line.IndexOf("]") - line.IndexOf("[") - 1)
                            };
                            Streams.AddEx(si);
                        }
                        else if (line.Trim().StartsWith("Quality:") && si != null)
                        {
                            si.Quality = line.Split(':')[1].Trim();
                        }
                        else if (line.Trim().StartsWith("Size:") && si != null)
                        {
                            si.Size = line.Split(':')[1].Trim();
                        }
                        else if (line.Trim().StartsWith("#") && si != null)
                        {
                            si.Args = line.Split(':')[1].Trim();
                        }
                    }
                }
                catch (Exception ex)
                {
                    this.Dispatcher.BeginInvoke(new Action(() =>
                    {
                        MessageBox.Show(ex.Message, "错误", button: MessageBoxButton.OK, icon: MessageBoxImage.Error);
                    }));
                }
                finally
                {
                    this.Dispatcher.BeginInvoke(new Action(() =>
                    {
                        datagrid.UpdateLayout();
                        DialogHost.Close("RootDialog");
                        ComboboxView.Refresh();
                    }));
                }
            });

        }

        private void BtnAction_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var info = ((Button)sender).DataContext as StreamInfo;
                var arg = info.Args.Substring(3);
                arg = arg.Replace("...", info.DownloadUrl);
                if (!string.IsNullOrEmpty(AdditionParam))
                    arg = AdditionParam + " " + arg;
                LuxHelper.Instance.Download(arg);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "错误", button: MessageBoxButton.OK, icon: MessageBoxImage.Error);
            }
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            StreamsView = CollectionViewSource.GetDefaultView(Streams);
            ComboboxView = CollectionViewSource.GetDefaultView(Streams.GroupBy(x => x.Quality).Select(g => g.Key));
        }
    }
}
