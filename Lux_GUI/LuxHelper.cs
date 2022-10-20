using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Navigation;

namespace Lux_GUI
{
    internal class LuxHelper
    {
        private string LuxFile { get; } = AppDomain.CurrentDomain.BaseDirectory + "lux.exe";
        private static readonly Lazy<LuxHelper> lazy = new Lazy<LuxHelper>(() => new LuxHelper());
        public static LuxHelper Instance { get { return lazy.Value; } }

        private LuxHelper()
        {
        }

        public bool IsLuxExist => System.IO.File.Exists(LuxFile);

        public string GetLuxInfo()
        {
            if (IsLuxExist)
            {
                using (Process p = new Process())
                {
                    AppendStartInfo(p);
                    p.StartInfo.Arguments = "-v";
                    p.Start();
                    var output = p.StandardOutput.ReadToEnd().Trim();
                    p.WaitForExit();//等待程序执行完退出进程
                    p.Close();
                    return output;
                }
            }
            else
            {
                return Properties.Resources.LbNoLuxHit;
            }
        }

        private void AppendStartInfo(Process p)
        {
            p.StartInfo.FileName = LuxFile;
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardOutput = true;  //由调用程序获取输出信息
            p.StartInfo.RedirectStandardError = true;   //重定向标准错误输出
            p.StartInfo.CreateNoWindow = true;          //不显示程序窗口
            p.StartInfo.StandardOutputEncoding = Encoding.UTF8;

        }

        internal string ParserUrl(string downloadUrl, bool isPlayList = false)
        {
            using (Process p = new Process())
            {
                AppendStartInfo(p);
                p.StartInfo.Arguments = (isPlayList ? "-p" : "") + $" -i {downloadUrl}";
                p.Start();

                var output = p.StandardOutput.ReadToEnd().Trim();
                p.WaitForExit();//等待程序执行完退出进程
                p.Close();
                return output;
            }
        }

        internal void Download(string arg)
        {
            Process p = new Process();
            p.StartInfo.FileName = LuxFile;
            p.StartInfo.Arguments = arg;
            p.Start();
        }

    }
}
