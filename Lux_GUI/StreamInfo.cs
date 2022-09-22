using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lux_GUI
{
    [PropertyChanged.AddINotifyPropertyChangedInterface]
    public class StreamInfo
    {
        public StreamInfo(string downloadUrl)
        {
            DownloadUrl = downloadUrl;
        }

        public string Name { get; set; }

        public string Quality { get; set; }

        public string Size { get; set; }

        public string Args { get; set; }

        public string DownloadUrl { get; }
    }
}
