//MainView
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
using VehicleGPS.Views.Control;
using VehicleGPS.Views.Control.MonitorCentre;
using VehicleGPS.Views.Control.DispatchCentre;
using VehicleGPS.Views.Control.ReportCentre;
using VehicleGPS.Models.Login;
using VehicleGPS.Models;
using VehicleGPS.Views.Warn;
using VehicleGPS.Views.Control.MessCenter;
using VehicleGPS.ViewModels.MessCenter;
using System.Windows.Media.Animation;
using System.Windows.Threading;
using Newtonsoft.Json;
using System.Net;
using System.IO;
using System.ComponentModel;
using Microsoft.Practices.Prism.ViewModel;
using System.Threading;
using VehicleGPS.Services;
using System.Media;
using VehicleGPS.ViewModels.MonitorCentre.RealTimeMonitor;
using VehicleGPS.Views.Control.MonitorCentre.RealTimeMonitor;
using VehicleGPS.ViewModels.Warn;
using Microsoft.Win32;
using System.Text.RegularExpressions;

namespace VehicleGPS.Views
{
    /// <summary>
    /// MainView.xaml 的交互逻辑
    /// </summary>
    public partial class MainView : Window
    {
        private string MsgCount;//消息中心消息总数
        private ImageButton selectedBtn = null;//当前选中的ImageButton
        private List<ImageButton> list_UserMenu;//用户拥有权限的菜单功能
        private List<ImageButton> list_CommonMenu;//获取通用菜单功能
        private bool isWarnShow = false;//是否显示报警

        private object WarningMutex = new object();
        private List<WarningInfo> realTimeWarnings = new List<WarningInfo>();
        private List<WarningInfo> currentRealTimeWarnings = new List<WarningInfo>();
        private int pageSize = 20;
        private int currentPage = 0;


        public MainView()
        {
            InitializeComponent();
            InitMenu();
            InitLoginInfo();
            InitTipMsgTimer();////////
            RealTimeMonitor.GetInstance();
            VehicleGPS.Views.Warn.WarnInfo.GetInstance();
            StaticMessageInfo.GetInstance();
            StaticGpsInfo.GetInstance();//启动实时刷新gps信息定时器
            MessageInfoViewModel.GetInstance(this);
            this.messagedialog.Show();
        }
        private void InitLoginInfo()
        {
            tb_UserID.Text = StaticLoginInfo.GetInstance().UserName;
            tb_LoginTime.Text = StaticLoginInfo.GetInstance().LoginTime;
        }
        private void InitMenu()
        {
            InitUserMenu();
            InitCommonMenu();////////////////////
        }
        /*初始化用户菜单功能*/
        private void InitUserMenu()
        {
            list_UserMenu = new List<ImageButton>();
            int colCount = 0;
            GetUserMenu();
            foreach (ImageButton imgBtn in list_UserMenu)
            {
                ColumnDefinition cd = new ColumnDefinition();
                cd.Width = GridLength.Auto;
                Grid_UserMenu.ColumnDefinitions.Add(cd);
                Grid_UserMenu.Children.Add(imgBtn);
                imgBtn.SetValue(Grid.ColumnProperty, colCount);
                colCount++;
            }
        }
        /*获取用户菜单功能*/
        private void GetUserMenu()
        {
            ImageButton imgBtn;
            if (StaticTreeState.RigthInfo == LoadingState.LOADCOMPLETE)
            {
                StaticRight rightInstance = StaticRight.GetInstance();
                foreach (RightInfo info in rightInstance.ListMenuRight)
                {
                    string xName = "";
                    string imageUrl = "";
                    string showName = "";
                    if (info.Name == "监控中心")
                    {
                        xName = "imgBtn_Monitor";
                        imageUrl = "/Images/MonitorCenter.png";
                        showName = info.Name;
                    }
                    else if (info.Name == "调度中心")
                    {
                        xName = "imgBtn_Dispatch";
                        imageUrl = "/Images/VehicleMonitor.png";
                        showName = info.Name;
                    }
                    else if (info.Name == "报表中心")
                    {
                        xName = "imgBtn_Report";
                        imageUrl = "/Images/InfoReports.png";
                        showName = info.Name;
                    }
                    else if (info.Name == "系统管理")
                    {
                        continue;
                    }
                    else
                    {
                        continue;
                    }
                    imgBtn = GetMenuImageButton(xName, imageUrl, showName);
                    imgBtn.AddHandler(ImageButton.MouseLeftButtonUpEvent, new MouseButtonEventHandler(Menu_Clicked), true);
                    list_UserMenu.Add(imgBtn);
                }
                imgBtn = GetMenuImageButton("imgBtn_Help", "/Images/help.png", "系统帮助");
                imgBtn.AddHandler(ImageButton.MouseLeftButtonUpEvent, new MouseButtonEventHandler(Menu_Clicked), true);
                list_UserMenu.Add(imgBtn);
            }
        }
        /*获取菜单ImageButton*/
        private ImageButton GetMenuImageButton(string xname, string path, string text)
        {
            ImageButton imgBtn = new ImageButton();
            imgBtn.Name = xname;
            imgBtn.Margin = new Thickness(0, 0, 10, 0);
            imgBtn.VerticalAlignment = VerticalAlignment.Center;
            imgBtn.HorizontalAlignment = HorizontalAlignment.Center;

            imgBtn.Image = new BitmapImage(new Uri(path, UriKind.Relative));
            imgBtn.ImageHeight = 20;
            imgBtn.ImageWidth = 20;
            imgBtn.ImageMargin = new Thickness(0, 0, 5, 0);

            imgBtn.Text = text;
            imgBtn.TextFontColor = new SolidColorBrush(Colors.White);
            imgBtn.TextFontFamily = new FontFamily("LiSu");
            imgBtn.TextFontSize = 20;

            imgBtn.MouseOverBorderBackground = new SolidColorBrush(Colors.LightSlateGray);
            imgBtn.MouseOverBorderCorner = new CornerRadius(3);

            return imgBtn;
        }
        /*初始化通用菜单功能*/
        private void InitCommonMenu()
        {
            list_CommonMenu = new List<ImageButton>();
            int colCount = 0;
            GetCommonMenu();
            foreach (ImageButton imgBtn in list_CommonMenu)
            {
                ColumnDefinition cd = new ColumnDefinition();
                cd.Width = GridLength.Auto;
                Grid_CommonMenu.ColumnDefinitions.Add(cd);
                Grid_CommonMenu.Children.Add(imgBtn);
                imgBtn.SetValue(Grid.ColumnProperty, colCount);
                colCount++;
            }
        }
        /*获取通用菜单功能*/
        private void GetCommonMenu()
        {
            /*消息中心*/
            ImageButton imgMes;
            //imgMes = GetCommonImageButton("imgMes", "/Images/MessageCenter.png", "消息中心");
            //imgMes.AddHandler(ImageButton.MouseLeftButtonUpEvent, new MouseButtonEventHandler(Mes_Clicked), true);
            //GetNewMessages gnm = new GetNewMessages();
            //MsgCount = gnm.MesCount;
            //imgMes.TextFontColor = new SolidColorBrush(Color.FromArgb(255, 255, 0, 0));

            //Binding MyBinding = new Binding();
            //MyBinding.Path = new PropertyPath("MesCount");

            //MyBinding.Source = gnm;
            //imgMes.DataContext = gnm;
            //imgMes.SetBinding(ImageButton.TextProperty, MyBinding);
            //imgMes.TextMargin = new Thickness(-15, -10, 0, 0);
            //list_CommonMenu.Add(imgMes);

            /*退出系统*/
            ImageButton imgBtn;
            imgBtn = GetCommonImageButton("imgBtn_Logout", "/Images/logout.png", "退出系统");
            imgBtn.AddHandler(ImageButton.MouseLeftButtonUpEvent, new MouseButtonEventHandler(Logout_Clicked), true);
            list_CommonMenu.Add(imgBtn);

        }
        /*获取通用imagbutton*/
        private ImageButton GetCommonImageButton(string xname, string path, string tip)
        {
            ImageButton imgBtn = new ImageButton();
            imgBtn.Name = xname;
            imgBtn.Margin = new Thickness(3);
            imgBtn.VerticalAlignment = VerticalAlignment.Center;
            imgBtn.HorizontalAlignment = HorizontalAlignment.Center;

            imgBtn.Image = new BitmapImage(new Uri(path, UriKind.Relative));
            imgBtn.ImageHeight = 20;
            imgBtn.ImageWidth = 20;
            imgBtn.ImageMargin = new Thickness(2);

            imgBtn.MouseOverBorderBackground = new SolidColorBrush(Colors.LightSlateGray);
            imgBtn.MouseOverBorderCorner = new CornerRadius(3);
            imgBtn.ToolTip = tip;

            return imgBtn;
        }
        /*点击菜单*/
        private void Menu_Clicked(object sender, RoutedEventArgs e)
        {
            SetImgBtnState(sender);
            LoadWindow();
        }
        /*根据不同菜单加载不同窗体*/
        private void LoadWindow()
        {
            UserControl uc = null;
            switch (selectedBtn.Name.ToString())
            {
                case "imgBtn_Monitor":
                    uc = MonitorCentre.GetInstance();
                    break;
                case "imgBtn_Dispatch":
                    uc = DispatchCentre.GetInstance();
                    break;
                case "imgBtn_Report":
                    RegistryKey key = Registry.ClassesRoot.OpenSubKey(@"http\shell\open\command\");
                    string s = key.GetValue("").ToString();
                    Regex reg = new Regex("\"([^\"]+)\"");
                    MatchCollection matchs = reg.Matches(s);

                    string filename = "";
                    if (matchs.Count > 0)
                    {
                        filename = matchs[0].Groups[1].Value;
                        System.Diagnostics.Process.Start(filename, "http://61.183.9.107:4014/ReportCenter/IndexWithoutLayoutAutoLogin?userId="
                            + StaticLoginInfo.GetInstance().UserName + "&password=" + StaticLoginInfo.GetInstance().Passwd);
                    }
                    else
                    {
                        MessageBox.Show("打开浏览器失败");
                    }
                    string explore = s.Substring(0, s.Length - 5);
                    //s就是你的默认浏览器，不过后面带了参数，把它截去
                    //uc = new ReportCentreWeb();
                    //uc = ReportCentre.GetInstance();
                    break;
                case "imgBtn_Help":
                    System.Windows.Forms.Help.ShowHelp(null, VehicleConfig.GetInstance().helpChmPath);
                    break;
                default:
                    break;
            }
            if (uc != null)
            {
                this.ContentGrid.Children.Clear();
                this.ContentGrid.Children.Add(uc);
            }
        }
        /*设置菜单选中状态*/
        private void SetImgBtnState(object sender)
        {
            if (selectedBtn != null)
            {
                selectedBtn.NormalBorderBackground = new SolidColorBrush(Colors.Transparent);//取消前一次菜单的选中状态
            }
            selectedBtn = (ImageButton)sender; ;
            selectedBtn.NormalBorderBackground = new SolidColorBrush(Colors.LightSlateGray);//设置当前菜单为选中状态
            selectedBtn.NormalBorderCorner = new CornerRadius(3);
        }
        /*消息中心*/
        private void Mes_Clicked(object sender, RoutedEventArgs e)
        {
            MessagesCenter mc = new MessagesCenter(MsgCount);
            mc.Show();
        }

        /*退出系统*/
        private void Logout_Clicked(object sender, RoutedEventArgs e)
        {
            if (MessageBoxResult.OK == MessageBox.Show("请确认是否退出系统！", "退出系统", MessageBoxButton.OKCancel))
            {
                if (VehicleGPS.Views.Warn.WarnInfo.GetInstance().IsWinShow())
                {
                    VehicleGPS.Views.Warn.WarnInfo.GetInstance().Close();
                }
                VehicleGPS.Views.Warn.WarnInfo.SetInstanceNull();
                this.Close();
                Application.Current.Shutdown();
                System.Diagnostics.Process.GetCurrentProcess().Kill();
            }
        }



        #region 浮窗
        private void img_FloatClose_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.HideFloat();
        }
        public List<TipMsg> tipMsgList = new List<TipMsg>();
        private DispatcherTimer tipMsgDispatchTimer = new DispatcherTimer();
        public bool isShowFloatWindow = true;// false;
        public bool isFloatWindowUse = true;//false;//是否使用浮窗
        private int msgCount = 0;
        VehicleGPS.Views.Warn.FloatWindow messagedialog = new VehicleGPS.Views.Warn.FloatWindow();
        /*初始化定时器*/
        private void InitTipMsgTimer()
        {
            this.tipMsgDispatchTimer.Interval = TimeSpan.FromSeconds(VehicleConfig.GetInstance().POPUPWINDOWINTERVAL);
            this.tipMsgDispatchTimer.Tick += new EventHandler(tipMsgDispatchTimer_Tick);
            this.tipMsgDispatchTimer.Start();
        }
        private void tipMsgDispatchTimer_Tick(object sender, EventArgs e)
        {
            List<TipMsg> msgList = new List<TipMsg>();
            int i = 1;
            foreach (var msg in StaticWarnInfo.GetInstance().MessageList)
            {
                TipMsg tip = new TipMsg();
                tip.Sequence = i++;
                tip.Msg = msg.VehicleNum + "(" + msg.VehicleId + ")" + ":" + msg.AlarmContent;
                tip.Time = msg.Time;
                msgList.Add(tip);
            }
            //if (msgCount != msgList.Count)
            if (msgList.Count > 0)
            {

                //this.tipMsgList = msgList;
                //if (tipMsgList.Count > 0)
                //{
                RefreshFloatWinData(msgList);
                if (!isShowFloatWindow)
                {
                    this.ShowFloat();
                }
                //}

            }
            else
            {
                HideFloat();
            }
        }
        /*刷新数据*/
        public void RefreshFloatWinData(List<TipMsg> list)
        {
            if (StaticTreeState.MessageInfo == LoadingState.LOADCOMPLETE)
            {
                StaticTreeState.MessageInfo = LoadingState.LOADING;
                StaticWarnInfo.GetInstance().MessageList.Clear();
                StaticTreeState.MessageInfo = LoadingState.LOADCOMPLETE;
            }
            string soundUrl = VehicleConfig.GetInstance().remindSoundPath;
            if (!File.Exists(soundUrl))
            {//如果声音文件不存在则播放默认的报警声音
                soundUrl = VehicleConfig.GetInstance().warnSoundPathDefault;
            }
            SoundPlayer palyer = new SoundPlayer(soundUrl);
            palyer.Play();
            messagedialog.tipMsgList = null;
            messagedialog.tipMsgList = list;
            //lv_TipMsgList.ItemsSource = null;
            //lv_TipMsgList.ItemsSource = this.tipMsgList;
        }

        /*显示浮窗*/
        public void ShowFloat()
        {
            if (!isShowFloatWindow && this.isFloatWindowUse)
            {
                this.isShowFloatWindow = true;
                messagedialog.RefreshData();
                messagedialog.Show();
                //DoubleAnimation heightAnimation = new DoubleAnimation(0, 180
                //    , new Duration(TimeSpan.FromSeconds(0.5)));
                //grid_TipMsg.BeginAnimation(Grid.HeightProperty, heightAnimation);
                //this.tipMsgDispatchTimer.Start();
            }

        }
        /*隐藏浮窗*/
        public void HideFloat()
        {
            if (this.isShowFloatWindow)
            {
                this.isShowFloatWindow = false;
                messagedialog.tipMsgList.Clear();
                messagedialog.RefreshData();
                messagedialog.Hide();
                //DoubleAnimation heightAnimation = new DoubleAnimation(180, 0
                //    , new Duration(TimeSpan.FromSeconds(0.5)));
                //grid_TipMsg.BeginAnimation(Grid.HeightProperty, heightAnimation);
                //this.tipMsgDispatchTimer.Stop();
            }
        }

        //private void grid_TipMsg_MouseEnter(object sender, MouseEventArgs e)
        //{
        //    this.tipMsgDispatchTimer.Stop();
        //}

        //private void grid_TipMsg_MouseLeave(object sender, MouseEventArgs e)
        //{
        //    this.tipMsgDispatchTimer.Start();
        //}
        #endregion

        private void rb_SelectedAll_Click(object sender, RoutedEventArgs e)
        {
            foreach (WarningInfo info in this.currentRealTimeWarnings)
            {
                info.IsSelected = true;
            }
        }

        private void rb_SelectedReverse_Click(object sender, RoutedEventArgs e)
        {
            foreach (WarningInfo info in this.currentRealTimeWarnings)
            {
                info.IsSelected = !info.IsSelected;
            }
        }

        private void Window_Closed(object sender, EventArgs e)
        {

            if (VehicleGPS.Views.Warn.WarnInfo.GetInstance().IsWinShow() || VehicleGPS.Views.Warn.WarnInfo.GetInstance() != null)
            {
                VehicleGPS.Views.Warn.WarnInfo.GetInstance().Close();
                VehicleGPS.Views.Warn.WarnInfo.SetInstanceNull();
            }
            Application.Current.Shutdown();
            System.Diagnostics.Process.GetCurrentProcess().Kill();
        }
    }
    public class WarningInfo : NotificationObject
    {
        private bool isSelected;
        public bool IsSelected
        {
            get { return isSelected; }
            set
            {
                isSelected = value;
                this.RaisePropertyChanged("IsSelected");
            }
        }
        public int sequence { get; set; }
        public string AlarmId { get; set; }
        public string VehicleId { get; set; }
        public string VehicleNum { get; set; }
        public string parentDepart { get; set; }
        public string Long { get; set; }
        public string Lat { get; set; }
        public string Time { get; set; }
        public string AlarmType { get; set; }
        public string AlarmContent { get; set; }
        public string ReleaseType { get; set; }
        public string SimId { get; set; }
        private string place;
        public string Place
        {
            get { return place; }
            set
            {
                place = value;
                this.RaisePropertyChanged("Place");
            }
        }
    }
}
