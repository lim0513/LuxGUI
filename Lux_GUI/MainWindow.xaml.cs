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
using System.Xml;
using System.Xml.XPath;

namespace Lux_GUI
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    [PropertyChanged.AddINotifyPropertyChangedInterface]
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = this;
        }

        public string LuxInfo { get; set; }

        public string BtnText => CanParser ? "解析" : "解析中";

        public bool IsPlayList
        {
            get
            {
                return LuxHelper.Instance.IsPlayList;
            }
            set
            {
                LuxHelper.Instance.IsPlayList = value;
            }
        }
        public bool CanBtnPlayList => IsPlayList && CanParser && !string.IsNullOrEmpty(DownloadUrl);

        public ObservableCollection<StreamInfo> Streams { get; } = new ObservableCollection<StreamInfo>();

        public string InputUrl { get; set; }

        public string DownloadUrl { get; set; }

        public bool CanParser { get; set; } = true;

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.IsEnabled = LuxHelper.Instance.IsLuxExist;
            LuxInfo = LuxHelper.Instance.GetLuxInfo();
        }
        private void BtnParser_Click(object sender, RoutedEventArgs e)
        {
            CanParser = false;
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
                                Name = line.Substring(line.IndexOf("[") + 1, line.IndexOf("]") - line.IndexOf("[") - 1),
                                CanDownload = !LuxHelper.Instance.IsPlayList,
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
                    CanParser = true;
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
                LuxHelper.Instance.Download(arg);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "错误", button: MessageBoxButton.OK, icon: MessageBoxImage.Error);
            }
        }

        private void BtnDLPlayList_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                LuxHelper.Instance.Download($"{DownloadUrl}");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "错误", button: MessageBoxButton.OK, icon: MessageBoxImage.Error);
            }
        }
    }
}
