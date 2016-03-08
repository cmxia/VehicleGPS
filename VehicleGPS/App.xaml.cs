using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;
using System.Diagnostics;
using System.Net;
using System.Text;
using System.Threading;
using System.IO;
using VehicleGPS.Update;
using VehicleGPS.Models;

namespace VehicleGPS
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {
        private string filepath = null;
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            if (!(e.Args.Length > 0))
            {

                //WebClient client = new WebClient();

                //string URLAddress = @"http://" + VehicleConfig.GetInstance().WEBIP + ":" + VehicleConfig.GetInstance().WEBPORT + "/test.txt";
                //string version = null;
                //version = client.DownloadString(URLAddress);
                //string oldversionPath = Environment.CurrentDirectory + "/../../Update/version.txt";
                //FileStream fs = new FileStream(oldversionPath, FileMode.Open);

                //StreamReader m_streamReader = new StreamReader(fs);

                //m_streamReader.BaseStream.Seek(0, SeekOrigin.Begin);// 从数据流中读取每一行，直到文件的最后一行

                //string old_version = m_streamReader.ReadLine();
                //if (string.Compare(version, old_version) != 0)
                //{//版本不一致
                //    if (MessageBox.Show("发现新版本，是否现在更新？", "确认更新", MessageBoxButton.OKCancel, MessageBoxImage.Question) == MessageBoxResult.OK)
                //    {
                //        Thread updateTH = new Thread(downloadfileTH);
                //        updateTH.Start();
                //        UpdateView win = new UpdateView();
                //        win.ShowDialog();

                //        App.Current.Shutdown();//关闭当前程序

                //        Process p = new Process();
                //        p.StartInfo.FileName = "cmd.exe";           //設定程序名
                //        p.StartInfo.Arguments = "/c " + "msiexec /i " + filepath;
                //        p.StartInfo.UseShellExecute = false;        //關閉Shell的使用
                //        p.StartInfo.RedirectStandardInput = true;   //重定向標準輸入
                //        p.StartInfo.RedirectStandardOutput = true;  //重定向標準輸出
                //        p.StartInfo.RedirectStandardError = true;   //重定向錯誤輸出
                //        p.StartInfo.CreateNoWindow = true;          //設置不顯示窗口
                //        p.Start();
                //    }

                //}

            }
        }
        private void downloadfileTH()
        {

            StaticTreeState.DownLoadComplete = LoadingState.LOADING;
            WebClient client = new WebClient();
            string URLAddress2 = @"http://" + VehicleConfig.GetInstance().WEBIP + ":" + VehicleConfig.GetInstance().WEBPORT + "/Setup.msi";//资源路径
            string filename = System.IO.Path.GetFileName(URLAddress2);//获取文件名
            string receivePath = Environment.CurrentDirectory + "/download/" + filename;//文件存放路径
            client.DownloadFile(URLAddress2, receivePath);//下载文件到本地
            this.filepath = receivePath;
            StaticTreeState.DownLoadComplete = LoadingState.LOADCOMPLETE;
        }
    }
}
