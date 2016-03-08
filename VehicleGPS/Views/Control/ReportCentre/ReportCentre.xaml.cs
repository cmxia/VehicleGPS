//ReportCentre.xaml.cs
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using VehicleGPS.Models;
using VehicleGPS.ViewModels.ReportCentre;
using VehicleGPS.Views.Control.ReportCentre.Reports;

namespace VehicleGPS.Views.Control.ReportCentre
{
    /// <summary>
    /// ReportCentre.xaml 的交互逻辑
    /// </summary>
    public partial class ReportCentre : UserControl
    {
        private ImageButton selectedBtn = null;//当前选中的ImageButton
        private List<ImageButton> list_Oil = null;//油耗分析报表
        private List<ImageButton> list_Mileage = null;//里程分析报表
        private List<ImageButton> list_Running = null;//运行分析报表
        private List<ImageButton> list_Alarm = null;//告警分析报表
        private List<ImageButton> list_Record = null;//行车记录报表
        private List<ImageButton> list_Common = null;//常用报表
        private string search_Mode = "None";
        private static ReportCentre instance;
        private ReportCentre()
        {
            InitializeComponent();
            InitReport();
            ResetCondition();
        }
        public static ReportCentre GetInstance()
        {
            if (instance == null)
            {
                instance = new ReportCentre();
            }
            return instance;
        }
        /*初始化所有报表功能*/
        private void InitReport()
        {
            InitOilReport();
            InitMileageReport();
            InitRunningReport();
            InitAlarmReport();
            InitRecordReport();
            InitCommonReport();
        }

        #region 油耗分析报表
        /*初始化报表*/
        private void InitOilReport()
        {
            GetOilReport();
            if (list_Oil.Count == 0)
            {//没有该权限就隐藏
                ep_Oil.Visibility = Visibility.Collapsed;
                return;
            }
            InitView(Grid_Oil, list_Oil);
        }
        /*获取报表*/
        private void GetOilReport()
        {
            list_Oil = new List<ImageButton>();
            if (StaticTreeState.RigthInfo == LoadingState.LOADCOMPLETE)
            {
                StaticRight rightInstance = StaticRight.GetInstance();
                string xName = "";
                string imageUrl = "";
                string showName = "";
                foreach (RightInfo info in rightInstance.ListOilRight)
                {
                    if (info.Name == "油耗统计")
                    {
                        xName = "imgBtn_OilStatistical";
                        imageUrl = "pack://application:,,,/Images/Report/yhtj.png";
                        showName = info.Name;
                    }
                    else if (info.Name == "油耗明细")
                    {
                        xName = "imgBtn_OilDetail";
                        imageUrl = "pack://application:,,,/Images/Report/yhmx.png";
                        showName = info.Name;
                    }
                    else if (info.Name == "油表查看")
                    {
                        xName = "imgBtn_OilCheck";
                        imageUrl = "pack://application:,,,/Images/Report/yhybck.png";
                        showName = info.Name;
                    }
                    if (xName != "" && imageUrl != "" && showName != "")
                    {
                        list_Oil.Add(GetImageButton(xName, showName, imageUrl, Oil_Clicked));
                    }
                }
            }
        }
        #endregion

        #region 常用报表
        /*初始化报表*/
        private void InitCommonReport()
        {
            GetCommonReport();
            if (list_Common.Count == 0)
            {//没有该权限就隐藏
                ep_Common.Visibility = Visibility.Collapsed;
                return;
            }
            InitView(Grid_Common, list_Common);
        }
        /*获取报表*/
        private void GetCommonReport()
        {
            list_Common = new List<ImageButton>();
            if (StaticTreeState.RigthInfo == LoadingState.LOADCOMPLETE)
            {
                StaticRight rightInstance = StaticRight.GetInstance();
                string xName = "";
                string imageUrl = "";
                string showName = "";
                foreach (RightInfo info in rightInstance.ListCommonRight)
                {
                    if (info.Name == "运输明细")
                    {
                        xName = "imgBtn_TransDetail";
                        imageUrl = "pack://application:,,,/Images/Report/yxmnlfx.png";
                        showName = info.Name;
                    }
                    else if (info.Name == "日行驶里程")
                    {
                        xName = "imgBtn_DayMileage";
                        imageUrl = "pack://application:,,,/Images/Report/lcxszlc.png";
                        showName = info.Name;
                    }
                    else if (info.Name == "违规作业")
                    {
                        xName = "imgBtn_Illegal";
                        imageUrl = "pack://application:,,,/Images/Report/jczlzdsy.png";
                        showName = info.Name;
                    }
                    else if (info.Name == "无任务离场")
                    {
                        xName = "imgBtn_NoTask";
                        imageUrl = "pack://application:,,,/Images/Report/jczlzdaz.png";
                        showName = info.Name;
                    }
                    else if (info.Name == "超速报警")
                    {
                        xName = "imgBtn_CommonOverSpeed";
                        imageUrl = "pack://application:,,,/Images/Report/jqbjmx.png";
                        showName = info.Name;
                    }
                    else if (info.Name == "指令下发历史")
                    {
                        xName = "imgBtn_History";
                        imageUrl = "pack://application:,,,/Images/Report/yhxjtj.png";
                        showName = info.Name;
                    }
                    if (xName != "" && imageUrl != "" && showName != "")
                    {
                        list_Common.Add(GetImageButton(xName, showName, imageUrl, Common_Clicked));
                    }
                }
            }
        }
        #endregion

        #region 里程分析报表
        /*初始化报表*/
        private void InitMileageReport()
        {
            GetMileageReport();
            if (list_Mileage.Count == 0)
            {//没有该权限就隐藏
                ep_Mileage.Visibility = Visibility.Collapsed;
                return;
            }
            InitView(Grid_Mileage, list_Mileage);
        }
        /*获取报表*/
        private void GetMileageReport()
        {
            list_Mileage = new List<ImageButton>();
            if (StaticTreeState.RigthInfo == LoadingState.LOADCOMPLETE)
            {
                StaticRight rightInstance = StaticRight.GetInstance();
                string xName = "";
                string imageUrl = "";
                string showName = "";
                foreach (RightInfo info in rightInstance.ListMileageRight)
                {
                    if (info.Name == "里程统计")
                    {
                        xName = "imgBtn_MileageStatistical";
                        imageUrl = "pack://application:,,,/Images/Report/lclctj.png";
                        showName = info.Name;
                    }
                    else if (info.Name == "里程明细")
                    {
                        xName = "imgBtn_MileageDetail";
                        imageUrl = "pack://application:,,,/Images/Report/lclcmx.png";
                        showName = info.Name;
                    }
                    else if (info.Name == "行驶总里程")
                    {
                        xName = "imgBtn_MileageSum";
                        imageUrl = "pack://application:,,,/Images/Report/lcxszlc.png";
                        showName = info.Name;
                    }
                    if (xName != "" && imageUrl != "" && showName != "")
                    {
                        list_Mileage.Add(GetImageButton(xName, showName, imageUrl, Mileage_Clicked));
                    }

                }
            }
        }
        #endregion

        #region 运行分析报表
        /*初始化报表*/
        private void InitRunningReport()
        {
            GetRunningReport();
            if (list_Running.Count == 0)
            {//没有该权限就隐藏
                ep_Run.Visibility = Visibility.Collapsed;
                return;
            }
            InitView(Grid_Running, list_Running);
        }
        /*获取报表*/
        private void GetRunningReport()
        {
            list_Running = new List<ImageButton>();
            if (StaticTreeState.RigthInfo == LoadingState.LOADCOMPLETE)
            {
                StaticRight rightInstance = StaticRight.GetInstance();
                string xName = "";
                string imageUrl = "";
                string showName = "";
                foreach (RightInfo info in rightInstance.ListRunRight)
                {
                    if (info.Name == "离线分析")
                    {
                        xName = "imgBtn_RunOffline";
                        imageUrl = "pack://application:,,,/Images/Report/yxlx.png";
                        showName = info.Name;
                    }
                    else if (info.Name == "停车统计")
                    {
                        xName = "imgBtn_RunPark";
                        imageUrl = "pack://application:,,,/Images/Report/yxtctj.png";
                        showName = info.Name;
                    }
                    else if (info.Name == "历史轨迹")
                    {
                        xName = "imgBtn_RunHistory";
                        imageUrl = "pack://application:,,,/Images/Report/yxlsgj.png";
                        showName = info.Name;
                    }
                    else if (info.Name == "进出区域明细")
                    {
                        xName = "imgBtn_RunInOut";
                        imageUrl = "pack://application:,,,/Images/Report/yxjcqy.png";
                        showName = info.Name;
                    }
                    else if (info.Name == "卸料统计")
                    {
                        xName = "imgBtn_RunUnload";
                        imageUrl = "pack://application:,,,/Images/Report/yxxctj.png";
                        showName = info.Name;
                    }
                    else if (info.Name == "上下线明细")
                    {
                        xName = "imgBtn_RunOffOnline";
                        imageUrl = "pack://application:,,,/Images/Report/yxsxxmx.png";
                        showName = info.Name;
                    }
                    else if (info.Name == "未熄火停车")
                    {
                        xName = "imgBtn_RunUnPowerOff";
                        imageUrl = "pack://application:,,,/Images/Report/yxwxhtctj.png";
                        showName = info.Name;
                    }
                    else if (info.Name == "ACC分析")
                    {
                        xName = "imgBtn_RunACC";
                        imageUrl = "pack://application:,,,/Images/Report/yxacc.png";
                        showName = info.Name;
                    }
                    else if (info.Name == "行车统计")
                    {
                        xName = "imgBtn_RunDrive";
                        imageUrl = "pack://application:,,,/Images/Report/yxxctj.png";
                        showName = info.Name;
                    }
                    else if (info.Name == "CAN数据明细")
                    {
                        xName = "imgBtn_RunCAN";
                        imageUrl = "pack://application:,,,/Images/Report/yxcan.png";
                        showName = info.Name;
                    }
                    if (xName != "" && imageUrl != "" && showName != "")
                    {
                        list_Running.Add(GetImageButton(xName, showName, imageUrl, Running_Clicked));
                    }
                }
            }
        }

        #endregion

        #region 告警分析报表
        /*初始化报表*/
        private void InitAlarmReport()
        {
            GetAlarmReport();
            if (list_Alarm.Count == 0)
            {//没有该权限就隐藏
                ep_Alarm.Visibility = Visibility.Collapsed;
                return;
            }
            InitView(Grid_Alarm, list_Alarm);
        }
        /*获取报表*/
        private void GetAlarmReport()
        {
            list_Alarm = new List<ImageButton>();
            if (StaticTreeState.RigthInfo == LoadingState.LOADCOMPLETE)
            {
                StaticRight rightInstance = StaticRight.GetInstance();
                string xName = "";
                string imageUrl = "";
                string showName = "";
                foreach (RightInfo info in rightInstance.ListAlarmRight)
                {
                    if (info.Name == "报警明细")
                    {
                        xName = "imgBtn_AlarmDetail";
                        imageUrl = "pack://application:,,,/Images/Report/jqbjmx.png";
                        showName = info.Name;
                    }
                    else if (info.Name == "单位报警统计")
                    {
                        xName = "imgBtn_AlarmStatistical";
                        imageUrl = "pack://application:,,,/Images/Report/jqdwbjtj.png";
                        showName = info.Name;
                    }
                    if (xName != "" && imageUrl != "" && showName != "")
                    {
                        list_Alarm.Add(GetImageButton(xName, showName, imageUrl, Alarm_Clicked));
                    }
                }
            }
        }
        #endregion

        #region 行驶记录报表
        /*初始化报表*/
        private void InitRecordReport()
        {
            GetRecordReport();
            if (list_Record.Count == 0)
            {//没有该权限就隐藏
                ep_Record.Visibility = Visibility.Collapsed;
                return;
            }
            InitView(Grid_Record, list_Record);
        }
        /*获取报表*/
        private void GetRecordReport()
        {
            list_Record = new List<ImageButton>();
            if (StaticTreeState.RigthInfo == LoadingState.LOADCOMPLETE)
            {
                StaticRight rightInstance = StaticRight.GetInstance();
                string xName = "";
                string imageUrl = "";
                string showName = "";
                foreach (RightInfo info in rightInstance.ListRecordRight)
                {
                    if (info.Name == "速度明细")
                    {
                        xName = "imgBtn_SpeedDetail";
                        imageUrl = "pack://application:,,,/Images/Report/xcsdmx.png";
                        showName = info.Name;
                    }
                    else if (info.Name == "平均速度分析")
                    {
                        xName = "imgBtn_AverageSpeed";
                        imageUrl = "pack://application:,,,/Images/Report/xcpjsd.png";
                        showName = info.Name;
                    }
                    else if (info.Name == "事故疑点数据")
                    {
                        xName = "imgBtn_AccidentData";
                        imageUrl = "pack://application:,,,/Images/Report/xcsdyd.png";
                        showName = info.Name;
                    }
                    if (info.Name == "单车区域超速")
                    {
                        xName = "imgBtn_RegionOverspeed";
                        imageUrl = "pack://application:,,,/Images/Report/xcdcqy.png";
                        showName = info.Name;
                    }
                    else if (info.Name == "区域超速汇总")
                    {
                        xName = "imgBtn_OverspeedReport";
                        imageUrl = "pack://application:,,,/Images/Report/xcqytj.png";
                        showName = info.Name;
                    }
                    if (xName != "" && imageUrl != "" && showName != "")
                    {
                        list_Record.Add(GetImageButton(xName, showName, imageUrl, Record_Clicked));
                    }
                }
            }
        }
        #endregion


        /*获取菜单ImageButton*/
        private ImageButton GetImageButton(string xname, string text, string path, MouseButtonEventHandler eventHandle)
        {
            ImageButton imgBtn = new ImageButton();
            imgBtn.Name = xname;
            imgBtn.Margin = new Thickness(3, 3, 3, 0);
            imgBtn.VerticalAlignment = VerticalAlignment.Center;
            imgBtn.HorizontalAlignment = HorizontalAlignment.Center;
            imgBtn.ControlOrientation = Orientation.Vertical;

            imgBtn.Text = text;
            imgBtn.TextFontColor = new SolidColorBrush(Colors.Black);
            imgBtn.TextMargin = new Thickness(3);

            imgBtn.Image = new BitmapImage(new Uri(path, UriKind.Absolute));
            imgBtn.ImageHeight = 40;
            imgBtn.ImageWidth = 40;
            imgBtn.ImageMargin = new Thickness(3);

            imgBtn.MouseOverBorderBackground = new ImageBrush(new BitmapImage(new Uri("pack://application:,,,/Images/Content_bg.jpg", UriKind.Absolute)));
            imgBtn.MouseOverBorderCorner = new CornerRadius(3);

            imgBtn.AddHandler(ImageButton.MouseLeftButtonUpEvent, new MouseButtonEventHandler(eventHandle), true);
            return imgBtn;
        }

        /*初始化报表界面*/
        private void InitView(Grid grid, List<ImageButton> list_element)
        {
            int colCount = 0;
            int rowCount = 0;
            foreach (ImageButton imgBtn in list_element)
            {
                /*添加ImageButton*/
                ColumnDefinition cd;
                RowDefinition rd;
                if (colCount == 0)
                {//换行
                    rd = new RowDefinition();
                    grid.RowDefinitions.Add(rd);

                }
                cd = new ColumnDefinition();
                cd.Width = GridLength.Auto;
                grid.ColumnDefinitions.Add(cd);
                grid.Children.Add(imgBtn);
                imgBtn.SetValue(Grid.ColumnProperty, colCount);
                imgBtn.SetValue(Grid.RowProperty, rowCount);
                colCount++;
                if (colCount == 6)//一行六个报表
                {
                    rowCount++;
                    colCount = 0;
                }
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
            selectedBtn.NormalBorderBackground = new ImageBrush(new BitmapImage(new Uri("pack://application:,,,/Images/Content_bg.jpg", UriKind.Absolute)));//设置当前菜单为选中状态
            selectedBtn.NormalBorderCorner = new CornerRadius(3);
        }

        private void ResetCondition()
        {
            this.dtp_BeginTime.Value = DateTime.Now;
            this.dtp_EndTime.Value = DateTime.Now;
            this.dp_BeginTime.Text = DateTime.Today.ToShortDateString();
            this.dp_EndTime.Text = DateTime.Today.ToShortDateString();
            this.cb_RegionType.SelectedIndex = 0;
            gb_ReportQuery.Visibility = Visibility.Collapsed;
            sp_Time.Visibility = Visibility.Collapsed;
            sp_TimeWithHM.Visibility = Visibility.Collapsed;
            sp_Overspeed.Visibility = Visibility.Collapsed;
            sp_SpeedLargerThan.Visibility = Visibility.Collapsed;
            sp_QueryButton.Visibility = Visibility.Collapsed;
            sp_RegionType.Visibility = Visibility.Collapsed;
            sp_InterValLargerThan.Visibility = Visibility.Collapsed;
        }

        private void InitCondition(string reportName, Visibility vTime, Visibility vTimeWihtHM, Visibility vIntervalLargerThan, Visibility vSpeedLargerThan, Visibility vOverspeed, Visibility vQueryButton, Visibility vRegionType)
        {
            ResetCondition();

            gb_ReportQuery.Visibility = Visibility.Visible;
            gb_ReportQuery.Header = reportName;
            sp_Time.Visibility = vTime;
            sp_TimeWithHM.Visibility = vTimeWihtHM;
            sp_Overspeed.Visibility = vOverspeed;
            sp_InterValLargerThan.Visibility = vIntervalLargerThan;
            sp_SpeedLargerThan.Visibility = vSpeedLargerThan;
            sp_QueryButton.Visibility = vQueryButton;
            sp_RegionType.Visibility = vRegionType;
        }
        #region 点击报表
        /*点击油耗分析报表*/
        private void Oil_Clicked(object sender, RoutedEventArgs e)
        {
            SetImgBtnState(sender);
            switch (((ImageButton)sender).Name)
            {
                case "imgBtn_OilStatistical"://油耗统计
                    InitCondition("油耗统计", Visibility.Collapsed, Visibility.Visible, Visibility.Collapsed, Visibility.Collapsed,
                        Visibility.Collapsed, Visibility.Visible, Visibility.Collapsed);
                    break;
                case "imgBtn_OilDetail"://油耗明细
                    InitCondition("油耗明细", Visibility.Collapsed, Visibility.Visible, Visibility.Collapsed, Visibility.Collapsed,
                        Visibility.Collapsed, Visibility.Visible, Visibility.Collapsed);
                    break;
                case "imgBtn_OilCheck"://油表查看
                    InitCondition("油表查看", Visibility.Collapsed, Visibility.Collapsed, Visibility.Collapsed, Visibility.Collapsed,
                        Visibility.Collapsed, Visibility.Visible, Visibility.Collapsed);

                    break;
                default:
                    break;
            }
            this.search_Mode = ((ImageButton)sender).Name;//add by shiwei for test
        }

        /*点击常用报表*/
        private void Common_Clicked(object sender, RoutedEventArgs e)
        {
            SetImgBtnState(sender);
            switch (((ImageButton)sender).Name)
            {
                case "imgBtn_TransDetail"://运输明细
                    InitCondition("运输明细", Visibility.Collapsed, Visibility.Visible, Visibility.Collapsed, Visibility.Collapsed,
                        Visibility.Collapsed, Visibility.Visible, Visibility.Collapsed);
                    break;
                case "imgBtn_DayMileage"://日行驶里程
                    InitCondition("日行驶里程", Visibility.Visible, Visibility.Collapsed, Visibility.Collapsed, Visibility.Collapsed,
                        Visibility.Collapsed, Visibility.Visible, Visibility.Collapsed);
                    break;
                case "imgBtn_Illegal"://违规作业
                    InitCondition("违规作业", Visibility.Collapsed, Visibility.Visible, Visibility.Collapsed, Visibility.Collapsed,
                        Visibility.Collapsed, Visibility.Visible, Visibility.Collapsed);
                    break;
                case "imgBtn_NoTask"://无任务离场
                    InitCondition("无任务离场", Visibility.Collapsed, Visibility.Visible, Visibility.Collapsed, Visibility.Collapsed,
                        Visibility.Collapsed, Visibility.Visible, Visibility.Collapsed);
                    break;
                case "imgBtn_CommonOverSpeed"://超速报警
                    InitCondition("超速报警", Visibility.Collapsed, Visibility.Visible, Visibility.Collapsed, Visibility.Collapsed,
                        Visibility.Visible, Visibility.Visible, Visibility.Collapsed);
                    break;
                case "imgBtn_History"://指令下发历史
                    InitCondition("指令下发历史", Visibility.Collapsed, Visibility.Visible, Visibility.Collapsed, Visibility.Collapsed,
                        Visibility.Collapsed, Visibility.Visible, Visibility.Collapsed);
                    break;
                default:
                    break;
            }
        }

        /*点击里程分析报表*/
        private void Mileage_Clicked(object sender, RoutedEventArgs e)
        {
            SetImgBtnState(sender);
            switch (((ImageButton)sender).Name)
            {
                case "imgBtn_MileageStatistical"://里程统计
                    InitCondition("里程统计", Visibility.Visible, Visibility.Collapsed, Visibility.Collapsed, Visibility.Collapsed,
                        Visibility.Collapsed, Visibility.Visible, Visibility.Collapsed);
                    break;
                case "imgBtn_MileageDetail"://里程明细
                    InitCondition("里程明细", Visibility.Visible, Visibility.Collapsed, Visibility.Collapsed, Visibility.Collapsed,
                        Visibility.Collapsed, Visibility.Visible, Visibility.Collapsed);
                    break;
                case "imgBtn_MileageSum"://行驶总里程
                    InitCondition("行驶总里程", Visibility.Collapsed, Visibility.Collapsed, Visibility.Collapsed, Visibility.Collapsed,
                        Visibility.Collapsed, Visibility.Visible, Visibility.Collapsed);
                    break;
                default:
                    break;
            }
        }
        /*点击运行报表*/
        private void Running_Clicked(object sender, RoutedEventArgs e)
        {
            SetImgBtnState(sender);
            switch (((ImageButton)sender).Name)
            {
                case "imgBtn_RunOffline"://离线分析
                    InitCondition("离线分析", Visibility.Collapsed, Visibility.Collapsed, Visibility.Collapsed, Visibility.Collapsed,
                        Visibility.Collapsed, Visibility.Visible, Visibility.Collapsed);
                    break;
                case "imgBtn_RunPark"://停车统计
                    InitCondition("停车统计", Visibility.Collapsed, Visibility.Visible, Visibility.Visible, Visibility.Collapsed,
                        Visibility.Collapsed, Visibility.Visible, Visibility.Collapsed);
                    break;
                case "imgBtn_RunHistory"://历史轨迹
                    InitCondition("历史轨迹", Visibility.Collapsed, Visibility.Visible, Visibility.Collapsed, Visibility.Visible,
                        Visibility.Collapsed, Visibility.Visible, Visibility.Collapsed);
                    break;
                case "imgBtn_RunInOut"://进出区域明细
                    InitCondition("进出区域明细", Visibility.Collapsed, Visibility.Visible, Visibility.Collapsed, Visibility.Collapsed,
                        Visibility.Collapsed, Visibility.Visible, Visibility.Visible);
                    break;
                case "imgBtn_RunUnload"://卸料统计
                    InitCondition("卸料统计", Visibility.Collapsed, Visibility.Collapsed, Visibility.Collapsed, Visibility.Collapsed,
                        Visibility.Collapsed, Visibility.Collapsed, Visibility.Collapsed);
                    break;
                case "imgBtn_RunOffOnline"://上下线明细
                    InitCondition("上下线明细", Visibility.Collapsed, Visibility.Visible, Visibility.Collapsed, Visibility.Collapsed,
                        Visibility.Collapsed, Visibility.Visible, Visibility.Collapsed);
                    break;
                case "imgBtn_RunUnPowerOff"://未熄火停车
                    InitCondition("未熄火停车", Visibility.Collapsed, Visibility.Visible, Visibility.Visible, Visibility.Collapsed,
                        Visibility.Collapsed, Visibility.Visible, Visibility.Collapsed);
                    break;
                case "imgBtn_RunACC"://ACC分析
                    InitCondition("ACC分析", Visibility.Collapsed, Visibility.Visible, Visibility.Collapsed, Visibility.Collapsed,
                        Visibility.Collapsed, Visibility.Visible, Visibility.Collapsed);
                    break;
                case "imgBtn_RunDrive"://行车统计
                    InitCondition("行车统计", Visibility.Collapsed, Visibility.Visible, Visibility.Visible, Visibility.Visible,
                        Visibility.Collapsed, Visibility.Visible, Visibility.Collapsed);
                    break;
                case "imgBtn_RunCAN"://CAN数据明细
                    InitCondition("CAN数据明细", Visibility.Collapsed, Visibility.Collapsed, Visibility.Collapsed, Visibility.Collapsed,
                        Visibility.Collapsed, Visibility.Collapsed, Visibility.Collapsed);
                    break;
                default:
                    break;
            }
        }
        /*点击告警分析报表*/
        private void Alarm_Clicked(object sender, RoutedEventArgs e)
        {
            SetImgBtnState(sender);
            switch (((ImageButton)sender).Name)
            {
                case "imgBtn_AlarmDetail"://报警明细
                    InitCondition("报警明细", Visibility.Collapsed, Visibility.Visible, Visibility.Collapsed, Visibility.Collapsed,
                        Visibility.Collapsed, Visibility.Visible, Visibility.Collapsed);
                    break;
                case "imgBtn_AlarmStatistical"://单位报警统计
                    InitCondition("单位报警统计", Visibility.Collapsed, Visibility.Visible, Visibility.Collapsed, Visibility.Collapsed,
                        Visibility.Collapsed, Visibility.Visible, Visibility.Collapsed);
                    break;
                default:
                    break;
            }
        }
        /*点击行车记录报表*/
        private void Record_Clicked(object sender, RoutedEventArgs e)
        {
            SetImgBtnState(sender);
            switch (((ImageButton)sender).Name)
            {
                case "imgBtn_SpeedDetail"://速度明细
                    InitCondition("速度明细", Visibility.Collapsed, Visibility.Visible, Visibility.Collapsed, Visibility.Collapsed,
                        Visibility.Visible, Visibility.Visible, Visibility.Collapsed);
                    break;
                case "imgBtn_AverageSpeed"://平均速度分析
                    InitCondition("平均速度分析", Visibility.Collapsed, Visibility.Collapsed, Visibility.Collapsed, Visibility.Collapsed,
                        Visibility.Collapsed, Visibility.Collapsed, Visibility.Collapsed);
                    break;
                case "imgBtn_AccidentData"://事故疑点数据
                    InitCondition("事故疑点数据", Visibility.Collapsed, Visibility.Collapsed, Visibility.Collapsed, Visibility.Collapsed,
                        Visibility.Collapsed, Visibility.Collapsed, Visibility.Collapsed);
                    break;
                case "imgBtn_RegionOverspeed"://单车区域超速
                    InitCondition("单车区域超速", Visibility.Collapsed, Visibility.Visible, Visibility.Collapsed, Visibility.Collapsed,
                        Visibility.Collapsed, Visibility.Visible, Visibility.Collapsed);
                    break;
                case "imgBtn_OverspeedReport"://区域超速汇报
                    InitCondition("区域超速汇报", Visibility.Collapsed, Visibility.Visible, Visibility.Collapsed, Visibility.Collapsed,
                        Visibility.Collapsed, Visibility.Visible, Visibility.Collapsed);
                    break;
                default:
                    break;
            }
        }
        #endregion

        #region 开始查询
        private void onSerach_Clicked(object sender, RoutedEventArgs e)
        {
            if (this.selectedBtn == null)
            {
                MessageBox.Show("提示", "请选择报表", MessageBoxButton.OK);
                return;
            }

            /*验证数据合法性*/
            List<CVBasicInfo> selectedList = ReportTreeViewModel.GetInstance().GetSelectedVehicles();
            if (selectedList == null || selectedList.Count == 0)
            {
                MessageBox.Show("请选择车辆", "提示", MessageBoxButton.OK);
                return;
            }
            DateTime sTime, eTime, sTimeHM, eTimeHM;
            sTime = Convert.ToDateTime(this.dp_BeginTime.Text);
            eTime = Convert.ToDateTime(this.dp_EndTime.Text);
            sTimeHM = (DateTime)dtp_BeginTime.Value;
            eTimeHM = (DateTime)dtp_EndTime.Value;
            if (sTime > eTime || sTime > DateTime.Now || eTime > DateTime.Now ||
                sTimeHM > eTimeHM || sTimeHM > DateTime.Now || eTimeHM > DateTime.Now)
            {
                MessageBox.Show("时间不合法", "提示", MessageBoxButton.OK);
                return;
            }

            string selectedReport = this.selectedBtn.Name.ToString();
            switch (selectedReport)
            {
                #region 常用报表
                case "imgBtn_TransDetail"://运输明细
                    TransportDetails tsdWin = new TransportDetails(selectedList, sTime, eTime);
                    tsdWin.Owner = Window.GetWindow(this);
                    tsdWin.ShowDialog();
                    break;
                case "imgBtn_DayMileage"://日行驶里程
                    MileageDetailsByDay cmdbdWin = new MileageDetailsByDay(selectedList, sTime, eTime);
                    cmdbdWin.Owner = Window.GetWindow(this);
                    cmdbdWin.ShowDialog();
                    break;
                case "imgBtn_Illegal"://违规作业
                    IllegalWork iw = new IllegalWork(selectedList, sTimeHM, eTimeHM);
                    iw.Owner = Window.GetWindow(this);
                    iw.ShowDialog();
                    break;
                case "imgBtn_NoTask"://无任务离场
                    LeaveWithoutTask lwt = new LeaveWithoutTask(selectedList, sTimeHM, eTimeHM);
                    lwt.Owner = Window.GetWindow(this);
                    lwt.ShowDialog();
                    break;
                case "imgBtn_CommonOverSpeed"://超速报警
                    string minSpeed = this.tb_MinSpeed.Text == "" ? "0" : this.tb_MinSpeed.Text;
                    string maxSpeed = this.tb_MaxSpeed.Text == "" ? "0" : this.tb_MaxSpeed.Text;
                    string overSpeedInterval = this.tb_OverSpeedInterval.Text == "" ? "0" : this.tb_OverSpeedInterval.Text;
                    SpeedDetails spsWin = new SpeedDetails(selectedList, sTimeHM, eTimeHM, overSpeedInterval, maxSpeed, minSpeed);
                    spsWin.Owner = Window.GetWindow(this);
                    spsWin.ShowDialog();
                    break;
                case "imgBtn_History"://指令下发历史
                    HistoryOfIssuedInstructions historyWin = new HistoryOfIssuedInstructions(selectedList, sTimeHM, eTimeHM);
                    historyWin.Owner = Window.GetWindow(this);
                    historyWin.ShowDialog();
                    break;
                #endregion
                #region 里程分析
                case "imgBtn_MileageStatistical"://里程统计
                    MileageDetailsByDay mdbdWin = new MileageDetailsByDay(selectedList, sTime, eTime);
                    mdbdWin.Owner = Window.GetWindow(this);
                    mdbdWin.ShowDialog();
                    break;
                case "imgBtn_MileageDetail"://里程明细
                    MileageDetails mdWin = new MileageDetails(selectedList, sTime, eTime);
                    mdWin.Owner = Window.GetWindow(this);
                    mdWin.ShowDialog();
                    break;
                case "imgBtn_MileageSum"://行驶总里程
                    TotalMileage tmWin = new TotalMileage(selectedList);
                    tmWin.Owner = Window.GetWindow(this);
                    tmWin.ShowDialog();
                    break;
                #endregion
                #region 油耗分析
                case "imgBtn_OilStatistical"://油耗统计
                    if (sTimeHM.AddDays(1) < eTimeHM)
                    {
                        MessageBox.Show("查询条件不能超过二十四小时", "提示", MessageBoxButton.OK);
                        break;
                    }
                    if (selectedList.Count != 1)
                    {
                        MessageBox.Show("只能选择一部车辆", "提示", MessageBoxButton.OK);
                        break;
                    }
                    OilStatistic osWin = new OilStatistic(selectedList, sTimeHM, eTimeHM);
                    osWin.Owner = Window.GetWindow(this);
                    osWin.ShowDialog();
                    break;
                case "imgBtn_OilDetail"://油耗明细
                    if (sTimeHM.AddDays(1) < eTimeHM)
                    {
                        MessageBox.Show("查询条件不能超过二十四小时", "提示", MessageBoxButton.OK);
                        break;
                    }
                    if (selectedList.Count != 1)
                    {
                        MessageBox.Show("只能选择一部车辆", "提示", MessageBoxButton.OK);
                        break;
                    }
                    OilCostDetails ocdWin = new OilCostDetails(selectedList, sTimeHM, eTimeHM);
                    ocdWin.Owner = Window.GetWindow(this);
                    ocdWin.ShowDialog();
                    break;
                case "imgBtn_OilCheck"://油表查看
                    CurrentOil coWin = new CurrentOil(selectedList);
                    coWin.Owner = Window.GetWindow(this);
                    coWin.ShowDialog();
                    break;
                #endregion
                #region 运行分析
                case "imgBtn_RunOffline"://离线分析
                    OfflineDetails oldWin = new OfflineDetails(selectedList);
                    oldWin.Owner = Window.GetWindow(this);
                    oldWin.ShowDialog();
                    break;
                case "imgBtn_RunPark"://停车统计
                    ParkingStatistic psWin = new ParkingStatistic(selectedList, sTimeHM, eTimeHM,
                        (this.tb_InterValLargerThan.Text == "" ? "0" : this.tb_InterValLargerThan.Text));
                    psWin.Owner = Window.GetWindow(this);
                    psWin.ShowDialog();
                    break;
                case "imgBtn_RunHistory"://历史轨迹
                    string speedHistory = this.tb_SpeedLargerThan.Text == "" ? "0" : this.tb_SpeedLargerThan.Text;
                    HistoryTrack htWin = new HistoryTrack(selectedList, sTimeHM, eTimeHM, speedHistory);
                    htWin.Owner = Window.GetWindow(this);
                    htWin.ShowDialog();
                    break;
                case "imgBtn_RunInOut"://进出区域明细
                    string regionTypeIndex = this.cb_RegionType.SelectedIndex.ToString();
                    InOutRegionReport iorWin = new InOutRegionReport(selectedList, sTimeHM, eTimeHM, regionTypeIndex);
                    iorWin.Owner = Window.GetWindow(this);
                    iorWin.ShowDialog();
                    break;
                case "imgBtn_RunUnload"://卸料统计

                    break;
                case "imgBtn_RunOffOnline"://上下线明细
                    OnlineOfflineReport ololWin = new OnlineOfflineReport(selectedList, sTimeHM, eTimeHM);
                    ololWin.Owner = Window.GetWindow(this);
                    ololWin.ShowDialog();
                    break;
                case "imgBtn_RunUnPowerOff"://未熄火停车
                    if (selectedList.Count != 1)
                    {
                        MessageBox.Show("只能选择一部车辆", "提示", MessageBoxButton.OK);
                        break;
                    }
                    string interval1 = this.tb_InterValLargerThan.Text == "" ? "0" : this.tb_InterValLargerThan.Text;
                    ParkingWithAccOn pwaoWin = new ParkingWithAccOn(selectedList, sTimeHM, eTimeHM, interval1);
                    pwaoWin.Owner = Window.GetWindow(this);
                    pwaoWin.ShowDialog();
                    break;
                case "imgBtn_RunACC"://ACC分析
                    if (selectedList.Count != 1)
                    {
                        MessageBox.Show("只能选择一部车辆", "提示", MessageBoxButton.OK);
                        break;
                    }
                    AccAnalysis aaWin = new AccAnalysis(selectedList, sTimeHM, eTimeHM);
                    aaWin.Owner = Window.GetWindow(this);
                    aaWin.ShowDialog();
                    break;
                case "imgBtn_RunDrive"://行车统计
                    if (selectedList.Count != 1)
                    {
                        MessageBox.Show("只能选择一部车辆", "提示", MessageBoxButton.OK);
                        break;
                    }
                    string interval2 = this.tb_InterValLargerThan.Text == "" ? "0" : this.tb_InterValLargerThan.Text;
                    string speed = this.tb_SpeedLargerThan.Text == "" ? "0" : this.tb_SpeedLargerThan.Text;
                    MovingAnalysis maWin = new MovingAnalysis(selectedList, sTimeHM, eTimeHM, interval2, speed);
                    maWin.Owner = Window.GetWindow(this);
                    maWin.ShowDialog();
                    break;
                case "imgBtn_RunCAN"://CAN数据明细

                    break;
                #endregion
                #region 行车记录
                case "imgBtn_SpeedDetail"://速度明细
                    string cminSpeed = this.tb_MinSpeed.Text == "" ? "0" : this.tb_MinSpeed.Text;
                    string cmaxSpeed = this.tb_MaxSpeed.Text == "" ? "0" : this.tb_MaxSpeed.Text;
                    string coverSpeedInterval = this.tb_OverSpeedInterval.Text == "" ? "0" : this.tb_OverSpeedInterval.Text;
                    SpeedDetails cspsWin = new SpeedDetails(selectedList, sTimeHM, eTimeHM, coverSpeedInterval, cmaxSpeed, cminSpeed);
                    cspsWin.Owner = Window.GetWindow(this);
                    cspsWin.ShowDialog();
                    break;
                case "imgBtn_AverageSpeed"://平均速度分析

                    break;
                case "imgBtn_AccidentData"://事故疑点数据

                    break;
                case "imgBtn_RegionOverspeed"://单车区域超速
                    string interval3 = this.tb_InterValLargerThan.Text == "" ? "0" : this.tb_InterValLargerThan.Text;
                    OverSpeedInArea osiaWin = new OverSpeedInArea(selectedList, sTimeHM, eTimeHM, interval3);
                    osiaWin.Owner = Window.GetWindow(this);
                    osiaWin.ShowDialog();
                    break;
                case "imgBtn_OverspeedReport"://区域超速汇报
                    string interval4 = this.tb_InterValLargerThan.Text == "" ? "0" : this.tb_InterValLargerThan.Text;
                    OverSpeedTotal ostWin = new OverSpeedTotal(selectedList, sTimeHM, eTimeHM, interval4);
                    ostWin.Owner = Window.GetWindow(this);
                    ostWin.ShowDialog();
                    break;
                #endregion
                default:
                    break;
            }
        }
        #endregion
        //#region 开始查询  add by shiwei for test 2014/01/11 13:18
        //private void onSerach_Clicked(object sender, RoutedEventArgs e)
        //{
        //    if(!search_Mode.ToString().Equals("None"))
        //    {
        //        CVBasicInfo basicInfo = null;
        //        basicInfo = ReportTreeViewModel.GetInstance().SelectedNode;//获取树型数据
        //        if (basicInfo == null)
        //        {
        //            MessageBox.Show("请先选择车辆");
        //        }
        //        else
        //        {
        //            Window win = null;
        //            switch (search_Mode.ToString())
        //            {
        //                case "imgBtn_OilStatistical"://油耗统计
        //                    win = new OilStatistic();
        //                    win.Owner = Window.GetWindow(this);
        //                    win.ShowDialog();
        //                    break;
        //                case "imgBtn_OilDetail"://油耗明细
        //                    win = new OilCostDetails();
        //                    win.Owner = Window.GetWindow(this);
        //                    win.ShowDialog();
        //                    break;
        //                case "imgBtn_OilCheck"://油表查看
        //                    //VehicleGPS.Services.Dialog.MessageBoxHelper.ShowMessage("油表查看测试");
        //                    //VehicleGPS.Services.ReportCentre.ReportTest getCurrentOils = new Services.ReportCentre.ReportTest();
        //                    //getCurrentOils.getCurrentOil();//查询结果查看debug/log/下的日志
        //                    win = new CurrentOil();
        //                    win.Owner = Window.GetWindow(this);
        //                    win.ShowDialog();
        //                    break;
        //                default:
        //                    break;
        //            };
        //        }

        //    }
        //}
        //#endregion
    }
}
