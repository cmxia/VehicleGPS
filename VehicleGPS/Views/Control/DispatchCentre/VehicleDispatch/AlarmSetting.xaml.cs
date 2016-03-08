using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Collections.ObjectModel;
using System.Net;
using System.IO;
using System.Diagnostics;
using VehicleGPS.Models;
using VehicleGPS.Models.Login;
using Newtonsoft.Json;
using System.Xml;
using Microsoft.Practices.Prism.ViewModel;

namespace VehicleGPS.Views.Control.DispatchCentre.VehicleDispatch
{
    /// <summary>
    /// AlarmSetting.xaml 的交互逻辑
    /// </summary>
    public partial class AlarmSetting : Window
    {
        private static AlarmSetting instance;
        public List<AlarmSettingInfo> warnSettingList = new List<AlarmSettingInfo>();
        public List<bool> warnSettingList_tmp = new List<bool>();//用于备份
        private string path = VehicleConfig.GetInstance().warnSettingPath;

        public System.Windows.Forms.OpenFileDialog open = new System.Windows.Forms.OpenFileDialog();
        public int[] items = null;
        int j = 0;
        CheckBox cb = null;
        public AlarmSetting()
        {
            InitializeComponent();
            this.ReadWarnSetting();
            this.dg_ColumnsList.ItemsSource = null;
            this.dg_ColumnsList.ItemsSource = this.warnSettingList;

            this.voice.IsReadOnly = true;
            string soundUrl = VehicleConfig.GetInstance().warnSoundPath;
            int i = soundUrl.LastIndexOf('/');
            this.voice.Text = soundUrl.Substring(soundUrl.LastIndexOf('/') + 1);
        }
        public static AlarmSetting GetInstance(Window parentWin = null)
        {
            if (instance == null)
            {
                instance = new AlarmSetting();
            }

            instance.warnSettingList_tmp.Clear();
            foreach (AlarmSettingInfo info in instance.warnSettingList)
            {
                instance.warnSettingList_tmp.Add(info.IsOpen);
            }
            //instance.showFloat_tmp = instance.showFloat;
            return instance;
        }
        /*设置“全选”“反选”“上移”“下移”样式*/
        private void InitButton()
        {
            SetButton(btn_allSelect);
            SetButton(btn_inverse);
        }
        private void SetButton(Button btn)
        {
            btn.VerticalAlignment = VerticalAlignment.Center;
            btn.HorizontalAlignment = HorizontalAlignment.Center;
        }
        /*全选*/
        private void all_Select_Click(object sender, RoutedEventArgs e)
        {
            foreach (AlarmSettingInfo info in this.warnSettingList)
            {
                info.IsOpen = true;
            }
        }
        /*反选*/
        private void inverse_Click(object sender, RoutedEventArgs e)
        {
            foreach (AlarmSettingInfo info in this.warnSettingList)
            {
                info.IsOpen = !info.IsOpen;
            }
        }

        ///*选择报警声音*/
        string destinationPath = VehicleConfig.GetInstance().warnSoundPathDefault;
        string filePath = null;
        private void voicefile_Click(object sender, RoutedEventArgs e)
        {

            open = new System.Windows.Forms.OpenFileDialog();//定义打开文本框实体
            open.Title = "打开文件";//对话框标题
            open.Filter = "波形文件|*.wav|所有文件|*.*";//文件扩展名
            if (open.ShowDialog() == System.Windows.Forms.DialogResult.OK)//打开
            {
                //String.
                string voiceName = open.FileName.Substring(open.FileName.LastIndexOf('\\') + 1);
                //成功后的处理
                this.filePath = open.FileName;
                this.destinationPath = VehicleConfig.GetInstance().voicePath + "/" + voiceName;
                voice.Text = voiceName;

            }
        }
        private void ReadWarnSetting()
        {
            if (!File.Exists(path))
            {
                return;
            }
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(path);
            XmlNode xn = xmlDoc.SelectSingleNode("Setting");
            // 得到根节点的所有子节点
            XmlNodeList xnl = xn.ChildNodes;
            foreach (XmlNode node in xnl)
            {
                // 将节点转换为元素，便于得到节点的属性值
                XmlElement xe = (XmlElement)node;
                if (xe.GetAttribute("userName").ToString() == "Default")
                {
                    //得到默认设置的所有子节点
                    XmlNodeList xnlDefault = xe.ChildNodes;
                    foreach (XmlNode nodeDefualt in xnlDefault)
                    {
                        if (nodeDefualt.Name == "voicePath")
                        {
                            //this.showFloat = (nodeDefualt.InnerText == "0" ? false : true);
                            //cb_FLoatWindow.IsChecked = this.showFloat;
                            continue;
                        }
                        if (nodeDefualt.Name == "showFloat")
                        {
                            //this.showFloat = (nodeDefualt.InnerText == "0" ? false : true);
                            //cb_FLoatWindow.IsChecked = this.showFloat;
                            continue;
                        }
                        //if (nodeDefualt.Name == "warnNum")
                        //{
                        //    this.number.Text = nodeDefualt.InnerText;
                        //        //cb_FLoatWindow.IsChecked = this.showFloat;
                        //    continue;
                        //}
                        AlarmSettingInfo info = new AlarmSettingInfo();
                        info.WarnName = nodeDefualt.Name;
                        info.WarnID = nodeDefualt.Attributes["id"].Value;
                        info.IsOpen = (nodeDefualt.InnerText == "0" ? false : true);
                        this.warnSettingList.Add(info);

                    }
                    break;
                }
            }
            /*报警过滤字符串*/
            List<string> tmp = new List<string>();
            foreach (AlarmSettingInfo info in this.warnSettingList)
            {
                if (info.IsOpen == true)
                {
                    tmp.Add(info.WarnID);
                }
            }
            //this.warnInfoWindow.SetFilter(tmp);
            //this.warnInfoWindow.SetUserFloatWindow(false);//是否浮显示
        }
        private void ConfirmCommand_Click(object sender, RoutedEventArgs e)
        {
            if (filePath != null && !(filePath.Equals(destinationPath)))
            {
                try
                {
                    System.IO.File.Copy(filePath, destinationPath, true);//复制声音文件到指定目录
                    VehicleConfig.GetInstance().warnSoundPath = destinationPath;
                }
                catch (Exception)
                {
                    //MessageBox.Show("您选则的声音文件正在使用中，请选择其他文件！");
                    //return;
                }
            }
            StaticTreeState.WarnSettinInfo = LoadingState.NOLOADING;
            if (!File.Exists(path))
            {
                return;
            }
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(path);
            XmlNode xn = xmlDoc.SelectSingleNode("Setting");
            // 得到根节点的所有子节点
            XmlNodeList xnl = xn.ChildNodes;
            foreach (XmlNode node in xnl)
            {
                // 将节点转换为元素，便于得到节点的属性值
                XmlElement xe = (XmlElement)node;
                if (xe.GetAttribute("userName").ToString() == "Default")
                {
                    //得到默认设置的所有子节点
                    XmlNodeList xnlDefault = xe.ChildNodes;
                    foreach (XmlNode nodeDefualt in xnlDefault)
                    {
                        if (nodeDefualt.Name == "showFloat")
                        {
                            //this.showFloat = (bool)this.cb_FLoatWindow.IsChecked;
                            nodeDefualt.InnerText = "0";
                            continue;
                        }
                        if (nodeDefualt.Name == "voicePath")
                        {
                            //this.showFloat = (bool)this.cb_FLoatWindow.IsChecked;
                            nodeDefualt.InnerText = destinationPath;
                            continue;
                        }
                        foreach (AlarmSettingInfo info in this.warnSettingList)
                        {
                            if (info.WarnName == nodeDefualt.Name)
                            {
                                nodeDefualt.InnerText = (info.IsOpen == true ? "1" : "0");
                                break;
                            }
                        }
                    }
                    break;
                }
            }
            xmlDoc.Save(this.path);
            /*报警过滤字符串*/
            List<string> tmp = new List<string>();
            List<AlarmSettingInfo> tmpwarn = new List<AlarmSettingInfo>();
            foreach (AlarmSettingInfo info in this.warnSettingList)
            {
                if (info.IsOpen == true)
                {
                    tmp.Add(info.WarnID);
                    tmpwarn.Add(info);
                }
            }
            StaticWarnInfo.warnsetlist = tmpwarn;
            //this.warnInfoWindow.SetFilter(tmp);
            //this.warnInfoWindow.SetUserFloatWindow(false);//是否浮显示
            MessageBox.Show("保存设置成功", "提示", MessageBoxButton.OK);
            StaticWarnInfo.GetInstance().RefreshWarnInfo();
            StaticTreeState.WarnSettinInfo = LoadingState.LOADCOMPLETE;
            this.Visibility = Visibility.Collapsed;
        }
        private void close_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }


    public class AlarmSettingInfo : NotificationObject
    {
        public string WarnID { get; set; }
        public string WarnName { get; set; }
        private bool isOpen;
        public bool IsOpen
        {
            get { return isOpen; }
            set
            {
                this.isOpen = value;
                this.RaisePropertyChanged("IsOpen");
            }
        }
    }
    public class AlarmDicInfo : NotificationObject
    {
        public string DICID { get; set; }
        public string DICNAME { get; set; }
        private bool isOpen;
        public bool IsOpen
        {
            get { return isOpen; }
            set
            {
                this.isOpen = value;
                this.RaisePropertyChanged("IsOpen");
            }
        }
    }
}
