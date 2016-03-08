//RealTimeViewModel
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.Prism.ViewModel;
using VehicleGPS.Models;
using Microsoft.Practices.Prism.Commands;
using System.Windows;
using System.Windows.Controls;
using VehicleGPS.Services.MonitorCentre.RealTimeMonitor;
using VehicleGPS.Services;
using VehicleGPS.Services.MonitorCentre;

using System.Threading;
using VehicleGPS.Models.MonitorCentre;
using System.Timers;
using VehicleGPS.Views.Control.MonitorCentre.RealTimeMonitor;
using VehicleGPS.Models.Login;

namespace VehicleGPS.ViewModels.MonitorCentre.RealTimeMonitor
{
    /*监控中心模型单例模式*/
    class RealTimeViewModel : NotificationObject
    {
        #region 设置显示字段
        public ShowSettingViewModel showSettingVM;
        public ShowSettingViewModel ShowSettingVM
        {
            get { return showSettingVM; }
            set
            {
                showSettingVM = value;
                this.RaisePropertyChanged("ShowSettingVM");
            }
        }
        #endregion
        private static RealTimeViewModel instance = null;
        public int PageSize { get; set; }//分页大小
        private object MapMonitor = new object();

        #region 构造函数 并获得实例
        private RealTimeViewModel(WebBrowser webMap)
        {
            /*显示字段设置模型*/
            this.ShowSettingVM = ShowSettingViewModel.GetInstance();
            /*初始化分页命令*/
            this.ComeFirstCommand = new DelegateCommand(new Action(this.ComeFirstCommandExecute));
            this.ComeLastCommand = new DelegateCommand(new Action(this.ComeLastCommandExecute));
            this.ComeNextCommand = new DelegateCommand(new Action(this.ComeNextCommandExecute));
            this.ComePrevCommand = new DelegateCommand(new Action(this.ComePrevCommandExecute));
            this.PageSize = 10;
            this.TotalCount = 0;
            this.TotalPage = 0;
            this.CurrentPage = 0;
            this.CurrentStart = 0;
            this.CurrentEnd = 0;
            /*实时数据双击事件*/
            this.DoubleClickCommand = new DelegateCommand(new Action(this.DoubleClickCommandExecute));
            this.listVehicleInfo = new List<RealTimeItemViewModel>();

            //实时数据
            this.AlermInfoList = new List<WarnInfo>();
            this.SendMessageList = new List<MessageInfo>();
            this.ReceiveMessageList = new List<MessageInfo>();
            this.InstructionInfoList = new List<CommandInfo>();
            System.Timers.Timer timer = new System.Timers.Timer();
            timer.Elapsed += new ElapsedEventHandler(refreshData);
            timer.Interval = 1000;
            timer.Enabled = true;
            timer.AutoReset = true;
            timer.Start();

            this.MouseRightButtonUpCommand = new DelegateCommand(new Action(this.MouseRightButtonUpExecute));
            this.listVehicleInfoCurrentPage = new List<RealTimeItemViewModel>();
            RealTimeTreeVM = RealTimeTreeViewModel.GetInstance();
            this.WebMap = webMap;
            this.MapService = new RealTimeMapService(webMap);

            System.Timers.Timer timer2 = new System.Timers.Timer();
            timer2.Elapsed += new ElapsedEventHandler(timer2_Elapsed);
            timer2.Interval = 300000;
            timer2.Enabled = true;
            timer2.AutoReset = true;
            timer2.Start();
        }

        void timer2_Elapsed(object sender, ElapsedEventArgs e)
        {
            SelectedGPSInfo();
        }

        

        public DelegateCommand MouseRightButtonUpCommand { get; set; }
        private void MouseRightButtonUpExecute()
        {
            if (this.SelectedVehicle == null)
            {
                MessageBox.Show("请先选择一行数据！");
                return;
            }
            RealTimeTreeViewModel.GetInstance().isNodeExpand(RealTimeTreeViewModel.GetInstance().RootNode, SelectedVehicle.VehicleInfo.SIM);
        }
        public RealTimeTreeViewModel RealTimeTreeVM { get; set; }

        //刷新报警信息，发出短语，接收短语，指令状态列表
        private void refreshData(object sender, EventArgs e)
        {
            //刷新发送短语列表
            if (StaticTreeState.SendText == LoadingState.LOADCOMPLETE)
            {
                StaticTreeState.SendText = LoadingState.LOADING;
                List<MessageInfo> tmp = new List<MessageInfo>();
                foreach (MessageInfo msg in StaticMessageInfo.GetInstance().SendMessageList)
                {
                    tmp.Add(msg);
                }
                this.SendMessageList = tmp;
                StaticTreeState.SendText = LoadingState.LOADCOMPLETE;
            }

            //刷新接收短语列表
            if (StaticTreeState.ReceivedText == LoadingState.LOADCOMPLETE)
            {
                StaticTreeState.ReceivedText = LoadingState.LOADING;
                List<MessageInfo> tmp2 = new List<MessageInfo>();
                foreach (MessageInfo msg in StaticMessageInfo.GetInstance().ReceivedMessageList)
                {
                    tmp2.Add(msg);
                }
                this.ReceiveMessageList = tmp2;
                StaticTreeState.ReceivedText = LoadingState.LOADCOMPLETE;
            }
            if (StaticTreeState.CmdStatus == LoadingState.LOADCOMPLETE)
            {
                StaticTreeState.CmdStatus = LoadingState.LOADING;
                List<CommandInfo> tmpI = new List<CommandInfo>();
                foreach (CommandInfo cmd in StaticMessageInfo.GetInstance().CmdList)
                {
                    tmpI.Add(cmd);
                }
                this.InstructionInfoList = tmpI;
                StaticTreeState.CmdStatus = LoadingState.LOADCOMPLETE;
            }
            if (StaticTreeState.WarnInfo == LoadingState.LOADCOMPLETE)
            {
                StaticTreeState.WarnInfo = LoadingState.LOADING;
                List<WarnInfo> temp = new List<WarnInfo>();
                foreach (WarnInfo item in StaticWarnInfo.GetInstance().WarnInfoList)
                {
                    temp.Add(item);
                }
                this.AlermInfoList = temp;
                StaticTreeState.WarnInfo = LoadingState.LOADCOMPLETE;
            }
        }
        public void InitRegionInBaidu()
        {
            if (StaticTreeState.RegionBasicInfo == LoadingState.LOADCOMPLETE)
            {
                foreach (CRegionInfo regionInfo in StaticRegionInfo.GetInstance().ListRegionBasicInfo)
                {
                    if (regionInfo.Long == null || regionInfo.Long == "" || regionInfo.lat == null || regionInfo.lat == "")
                        continue;
                    this.MapService.addRegionByOne(regionInfo.zoneName, regionInfo.Long, regionInfo.lat, regionInfo.radio, "#f00");
                }
            }
        }
        public static RealTimeViewModel GetInstance(WebBrowser webMap = null)
        {
            if (instance == null)
            {
                instance = new RealTimeViewModel(webMap);
            }
            return instance;
        }
        #endregion

        /// <summary>
        /// 车辆信息含GPS信息（选择车辆的）
        /// </summary>
        private List<RealTimeItemViewModel> listVehicleInfo = null;
        public List<RealTimeItemViewModel> ListVehicleInfo
        {
            get { return listVehicleInfo; }
            set
            {
                if (listVehicleInfo != value)
                {
                    listVehicleInfo = value;
                    if (!StaticTreeState.IsRefreshRealTimeData)
                    {//如果是更新数据则不需要初始化页
                        this.InitPage();
                    }
                    //this.ParseAddress();
                }
            }
        }

        #region Added by 夏创铭

        /// <summary>
        /// 报警信息
        /// author： 夏创铭
        /// </summary>
        private List<WarnInfo> alerminfolist;

        public List<WarnInfo> AlermInfoList
        {
            get { return alerminfolist; }
            set
            {
                alerminfolist = value;
                this.RaisePropertyChanged("AlermInfoList");
            }
        }
        //发出短语
        private List<MessageInfo> sendmessagelist;

        public List<MessageInfo> SendMessageList
        {
            get { return sendmessagelist; }
            set
            {
                sendmessagelist = value;
                this.RaisePropertyChanged("SendMessageList");
            }
        }
        //接收短语
        private List<MessageInfo> receivemessagelist;

        public List<MessageInfo> ReceiveMessageList
        {
            get { return receivemessagelist; }
            set
            {
                receivemessagelist = value;
                this.RaisePropertyChanged("ReceiveMessageList");
            }
        }
        //指令状态
        private List<CommandInfo> instructioninfolist;

        public List<CommandInfo> InstructionInfoList
        {
            get { return instructioninfolist; }
            set
            {
                instructioninfolist = value;
                this.RaisePropertyChanged("InstructionInfoList");
            }
        }
        //选择的车辆
        public List<CVBasicInfo> listSelectedVehicleInfo = new List<CVBasicInfo>();

        #endregion

        /// <summary>
        /// 选择车辆的基本信息列表（节点信息）
        /// </summary>
        private List<CVBasicInfo> listSelectedStationInfo = new List<CVBasicInfo>();

        #region 定时更新Datagrid数据和Map数据
        public void RefreshDataGridMap()
        {//定时更新数据
            this.RefreshCurrentPage();
            this.InitBaiduMap();
            StaticTreeState.IsRefreshRealTimeData = false;
        }
        #endregion

        #region 绑定到Datagrid的当前页
        /*绑定当前页*/
        private List<RealTimeItemViewModel> listVehicleInfoCurrentPage = null;
        public List<RealTimeItemViewModel> ListVehicleInfoCurrentPage
        {
            get { return listVehicleInfoCurrentPage; }
            set
            {
                if (listVehicleInfoCurrentPage != value)
                {
                    listVehicleInfoCurrentPage = value;
                    this.RaisePropertyChanged("ListVehicleInfoCurrentPage");
                }
            }
        }

        /// <summary>
        /// 初始化页面
        /// </summary>
        private void InitPage()
        {
            this.TotalCount = this.ListVehicleInfo.Count;
            this.TotalPage = (this.TotalCount / this.PageSize) * this.PageSize < totalCount ? this.TotalCount / this.PageSize + 1 : this.TotalCount / this.PageSize;
            this.CurrentPage = 0;
            this.CurrentStart = 0;
            this.CurrentEnd = 0;
            if (this.ListVehicleInfoCurrentPage != null)
            {
                this.ListVehicleInfoCurrentPage.Clear();
            }
            this.ListVehicleInfoCurrentPage = null;
            ListVehicleInfoCurrentPage = this.ListVehicleInfo;
            //if (this.TotalCount != 0)
            //{
            //    this.ComeFirstCommandExecute();
            //}
        }

        private bool isBusy = false;//忙等待
        public bool IsBusy
        {
            get { return isBusy; }
            set
            {
                if (isBusy != value)
                {
                    isBusy = value;
                    this.RaisePropertyChanged("IsBusy");
                }
            }
        }
        #endregion

        #region 刷新当前页信息
        /*刷新当前页信息*/
        public void RefreshCurrentPage()
        {
            if (this.TotalCount == 0)
            {
                return;
            }
            List<RealTimeItemViewModel> pageTmp = new List<RealTimeItemViewModel>();
            for (int i = CurrentStart - 1; i < this.CurrentEnd; i++)
            {
                this.listVehicleInfo[i].VehicleInfo.Sequence = i + 1;
                pageTmp.Add(this.listVehicleInfo[i]);
            }
            if (this.ListVehicleInfoCurrentPage != null)
            {
                this.ListVehicleInfoCurrentPage.Clear();
            }
            this.ListVehicleInfoCurrentPage = pageTmp;
            this.SetPrevNextEnable();
        }
        #endregion

        #region 分页

        public DelegateCommand ComeFirstCommand { get; set; }//首页
        public DelegateCommand ComePrevCommand { get; set; }//前一页
        public DelegateCommand ComeNextCommand { get; set; }//下一页
        public DelegateCommand ComeLastCommand { get; set; }//末页

        /*前一页按钮是否可用*/
        private bool prevEnable = true;
        public bool PrevEnable
        {
            get { return prevEnable; }
            set
            {
                if (prevEnable != value)
                {
                    prevEnable = value;
                    this.RaisePropertyChanged("PrevEnable");
                }
            }
        }
        /*首页按钮是否可用*/
        private bool firstEnable = true;
        public bool FirstEnable
        {
            get { return firstEnable; }
            set
            {
                if (firstEnable != value)
                {
                    firstEnable = value;
                    this.RaisePropertyChanged("FirstEnable");
                }
            }
        }
        /*尾页按钮是否可用*/
        private bool lastEnable = true;
        public bool LastEnable
        {
            get { return lastEnable; }
            set
            {
                if (lastEnable != value)
                {
                    lastEnable = value;
                    this.RaisePropertyChanged("LastEnable");
                }
            }
        }
        /*下一页按钮是否可用*/
        private bool nextEnable = true;
        public bool NextEnable
        {
            get { return nextEnable; }
            set
            {
                if (nextEnable != value)
                {
                    nextEnable = value;
                    this.RaisePropertyChanged("NextEnable");
                }
            }
        }
        private void SetPrevNextEnable()
        {
            if (this.CurrentPage == 1)
            {
                this.PrevEnable = false;
                this.FirstEnable = false;
            }
            else
            {
                this.PrevEnable = true;
                this.FirstEnable = true;
            }
            if (this.CurrentPage == this.TotalPage)
            {
                this.NextEnable = false;
                this.LastEnable = false;
            }
            else
            {
                this.NextEnable = true;
                this.LastEnable = true;
            }
        }

        public void ComeFirstCommandExecute()
        {
            if (this.TotalCount == 0)
            {
                return;
            }
            this.CurrentPage = 1;
            this.CurrentStart = 1;
            this.CurrentEnd = this.PageSize > this.TotalCount ? this.TotalCount : this.PageSize;
            List<RealTimeItemViewModel> pageTmp = new List<RealTimeItemViewModel>();
            int sequence = (this.CurrentPage - 1) * this.PageSize + 1;//序号
            for (int i = CurrentStart - 1; i < this.CurrentEnd; i++)
            {
                //GPSInfo gpsInfo = this.listVehicleInfo[i].VehicleInfo.VehicleGPSInfo;
                //if (gpsInfo != null && gpsInfo.Latitude != "" && gpsInfo.Longitude != "")
                //{
                this.listVehicleInfo[i].VehicleInfo.Sequence = sequence++;
                pageTmp.Add(this.listVehicleInfo[i]);
                //}
            }
            if (this.ListVehicleInfoCurrentPage != null)
            {
                this.ListVehicleInfoCurrentPage.Clear();
            }
            this.ListVehicleInfoCurrentPage = pageTmp;

            SetPrevNextEnable();
        }
        public void ComePrevCommandExecute()
        {
            if (this.TotalCount == 0)
            {
                return;
            }
            if (this.CurrentPage > 1)
            {
                this.CurrentPage--;
                this.CurrentEnd = this.CurrentStart - 1;
                this.CurrentStart = (this.CurrentPage - 1) * this.PageSize + 1;
                List<RealTimeItemViewModel> pageTmp = new List<RealTimeItemViewModel>();
                int sequence = (this.CurrentPage - 1) * this.PageSize + 1;//序号
                for (int i = this.CurrentStart - 1; i < this.CurrentEnd; i++)
                {
                    //GPSInfo gpsInfo = this.listVehicleInfo[i].VehicleInfo.VehicleGPSInfo;
                    //if (gpsInfo != null && gpsInfo.Latitude != "" && gpsInfo.Longitude != "")
                    //{
                    this.listVehicleInfo[i].VehicleInfo.Sequence = sequence++;
                    pageTmp.Add(this.listVehicleInfo[i]);
                    //}
                }
                if (this.ListVehicleInfoCurrentPage != null)
                {
                    this.ListVehicleInfoCurrentPage.Clear();
                }
                this.ListVehicleInfoCurrentPage = pageTmp;
            }
            SetPrevNextEnable();
        }
        public void ComeNextCommandExecute()
        {
            if (this.TotalCount == 0)
            {
                return;
            }
            if (this.CurrentPage < this.TotalPage)
            {
                this.CurrentStart = this.CurrentPage * this.PageSize + 1;
                this.CurrentEnd = (this.CurrentStart + this.PageSize - 1) > this.TotalCount ? this.TotalCount : (this.CurrentStart + this.PageSize - 1);
                this.CurrentPage++;
                List<RealTimeItemViewModel> pageTmp = new List<RealTimeItemViewModel>();
                int sequence = (this.CurrentPage - 1) * this.PageSize + 1;//序号
                for (int i = CurrentStart - 1; i < this.CurrentEnd; i++)
                {
                    //GPSInfo gpsInfo = this.listVehicleInfo[i].VehicleInfo.VehicleGPSInfo;
                    //if (gpsInfo != null && gpsInfo.Latitude != "" && gpsInfo.Longitude != "")
                    //{
                    this.listVehicleInfo[i].VehicleInfo.Sequence = sequence++;
                    pageTmp.Add(this.listVehicleInfo[i]);
                    //}
                }
                if (this.ListVehicleInfoCurrentPage != null)
                {
                    this.ListVehicleInfoCurrentPage.Clear();
                }
                this.ListVehicleInfoCurrentPage = pageTmp;
            }
            SetPrevNextEnable();
        }
        public void ComeLastCommandExecute()
        {
            if (this.TotalCount == 0)
            {
                return;
            }
            this.CurrentPage = this.TotalPage;
            this.CurrentStart = this.TotalPage > 1 ? (this.CurrentPage - 1) * this.PageSize : 1;
            this.CurrentEnd = this.TotalCount;
            List<RealTimeItemViewModel> pageTmp = new List<RealTimeItemViewModel>();
            int sequence = (this.CurrentPage - 1) * this.PageSize + 1;//序号
            for (int i = CurrentStart; i < this.CurrentEnd; i++)
            {
                //GPSInfo gpsInfo = this.listVehicleInfo[i].VehicleInfo.VehicleGPSInfo;
                //if (gpsInfo != null && gpsInfo.Latitude != "" && gpsInfo.Longitude != "")
                //{
                this.listVehicleInfo[i].VehicleInfo.Sequence = sequence++;
                pageTmp.Add(this.listVehicleInfo[i]);
                //}
            }
            if (this.ListVehicleInfoCurrentPage != null)
            {
                this.ListVehicleInfoCurrentPage.Clear();
            }
            this.ListVehicleInfoCurrentPage = pageTmp;

            SetPrevNextEnable();
        }
        /*当前页*/
        private int currentPage;
        public int CurrentPage
        {
            get { return currentPage; }
            set
            {
                if (currentPage != value)
                {
                    currentPage = value;
                    this.RaisePropertyChanged("CurrentPage");
                }
            }
        }
        /*总共页数*/
        private int totalPage;
        public int TotalPage
        {
            get { return totalPage; }
            set
            {
                if (totalPage != value)
                {
                    totalPage = value;
                    this.RaisePropertyChanged("TotalPage");
                }
            }
        }
        /*当前页记录的开始条数*/
        private int currentStart;
        public int CurrentStart
        {
            get { return currentStart; }
            set
            {
                if (currentStart != value)
                {
                    currentStart = value;
                    this.RaisePropertyChanged("CurrentStart");
                }
            }
        }
        /*当前页记录的最后条数*/
        private int currentEnd;
        public int CurrentEnd
        {
            get { return currentEnd; }
            set
            {
                if (currentEnd != value)
                {
                    currentEnd = value;
                    this.RaisePropertyChanged("CurrentEnd");
                }
            }
        }
        /*总条数*/
        private int totalCount;
        public int TotalCount
        {
            get { return totalCount; }
            set
            {
                if (totalCount != value)
                {
                    totalCount = value;
                    this.RaisePropertyChanged("TotalCount");
                }
            }
        }
        #endregion

        #region 百度地图
        public WebBrowser WebMap { get; set; }
        public RealTimeMapService MapService { get; set; }

        public void ModMarker(CVDetailInfo info)
        {
            if (this.WebMap != null && StaticTreeState.RealTimeMapViewContruct)
            {
                string vehicleName = info.VehicleId;
                string onlineStatus = info.VehicleGPSInfo.OnlineStates;
                string updateTime = info.VehicleGPSInfo.Datetime;
                string speed = info.VehicleGPSInfo.Speed;
                string direction = info.VehicleGPSInfo.Direction;
                string curlocation = info.VehicleGPSInfo.CurLocation;
                string lng = info.VehicleGPSInfo.Longitude;
                string lat = info.VehicleGPSInfo.Latitude;
                string cont = VehicleCommon.GetHtml(vehicleName, onlineStatus, updateTime, speed, direction, curlocation);
                string icon = VehicleCommon.GetDirectionImageUrl(direction, info.VehicleGPSInfo.OnlineStates);
                RealTimeTreeViewModel realTimeTreeVM = RealTimeTreeViewModel.GetInstance();
                realTimeTreeVM.TreeOperate.InitCarOnlineState();
                if (lng == null || lng == "" || lat == null || lat == "")
                    return;
                Monitor.Enter(this.MapMonitor);
                this.WebMap.Dispatcher.Invoke((Action)delegate()
                {
                    this.MapService.ModMarker(vehicleName, Convert.ToDouble(lng), Convert.ToDouble(lat), cont, icon);
                });
                Monitor.Exit(this.MapMonitor);
            }
        }
        public void InitBaiduMap()
        {
            StaticTreeState.ClusterReady = LoadingState.NOLOADING;
            Thread nThread = new Thread(InitBaiduMapThread);
            nThread.Start();
        }
        private void InitBaiduMapThread()
        {
            Monitor.Enter(this.MapMonitor);
            this.WebMap.Dispatcher.Invoke((Action)delegate()
            {
                this.MapService.InitRealTimeMap();
                this.InitRegionInBaidu();
                foreach (RealTimeItemViewModel item in this.ListVehicleInfo)
                {
                    if (item.VehicleInfo.VehicleGPSInfo == null)
                    {
                        continue;//没有该车辆的实时gps信息
                    }
                    if (item.VehicleInfo.VehicleGPSInfo.Latitude == ""
                        || item.VehicleInfo.VehicleGPSInfo.Longitude == "")
                    {
                        continue;//没有该车辆的实时地理位置信息
                    }
                    string vehicleName = item.VehicleInfo.VehicleId;
                    string onlineStatus = item.VehicleInfo.VehicleGPSInfo.OnlineStates;
                    string updateTime = item.VehicleInfo.VehicleGPSInfo.Datetime;
                    string speed = item.VehicleInfo.VehicleGPSInfo.Speed;
                    string direction = item.VehicleInfo.VehicleGPSInfo.Direction;
                    string curlocation = item.VehicleInfo.VehicleGPSInfo.CurLocation;
                    string lng = item.VehicleInfo.VehicleGPSInfo.Longitude;
                    string lat = item.VehicleInfo.VehicleGPSInfo.Latitude;

                    string cont = VehicleCommon.GetHtml(vehicleName, onlineStatus, updateTime, speed, direction, curlocation);
                    /*获取显示状态的图片*/
                    //BusinessDataServiceWEB dataWeb = new BusinessDataServiceWEB();
                    string icon = VehicleCommon.GetDirectionImageUrl(direction, item.VehicleInfo.VehicleGPSInfo.OnlineStates);
                    if (lng == null || lng == "" || lat == "" || lat == null || lng == "null" || lat == "null")
                    {
                        continue;
                    }
                    this.MapService.SetMarker(vehicleName, Convert.ToDouble(lng), Convert.ToDouble(lat), cont, icon);
                }
                if (StaticRealTimeInfo.GetInstance().iscluster)
                {
                    this.MapService.SetMarkerCluster();
                }
                else
                {
                    this.MapService.AddMarkers();
                }
                VehicleGPS.Views.Control.MonitorCentre.MonitorCentre.cluster_CB.IsEnabled = true;
                RealTimeTree.treeStatic.IsEnabled = true;
                StaticTreeState.ClusterReady = LoadingState.LOADCOMPLETE;
            });
            Monitor.Exit(this.MapMonitor);
        }
        #endregion

        #region 右键菜单
        public RightClickOparate RightOperate { set; get; }//树的操作类
        /// <summary>
        /// 查看车辆信息
        /// </summary>
        public DelegateCommand<object> VehicleCommand { get; set; }
        /// <summary>
        /// 查看车辆信息
        /// </summary>
        /// <param name="e">无用</param>
        private void VehicleCommandExecute(object e)
        {

            if (this.selectedVehicle != null)//车辆节点
            {
                //MessageBox.Show(selectedNode.nodeInfo.Name);
                //MessageBox.Show(focusOne.nodeInfo.Name);
                RightOperate.getVehicleBaseInfo(this.selectedVehicle.VehicleInfo.SIM);
                Window win = new VehicleGPS.Views.Control.MonitorCentre.Instruction.VehicleInfo();
                win.Show();
            }
            else
            {
                MessageBox.Show("请选择列表项");
            }
        }

        /// <summary>
        /// 车辆跟踪
        /// </summary>
        public DelegateCommand<object> VehicleTrackCommand { get; set; }
        /// <summary>
        /// 车辆跟踪
        /// </summary>
        /// <param name="e">无用</param>
        private void VehicleTrackCommandExecute(object e)
        {
            if (this.selectedVehicle != null)//车辆节点
            {

                Window win = new VehicleGPS.Views.Control.MonitorCentre.VehicleTrack.VehicleTrack();
                win.Show();
            }
            else
            {
                MessageBox.Show("请选择列表项");
            }
        }

        /// <summary>
        /// 轨迹回放
        /// </summary>
        public DelegateCommand<object> TrackPlayCommand { get; set; }
        /// <summary>
        /// 轨迹回放
        /// </summary>
        /// <param name="e">无用</param>
        private void TrackPlayCommandExecute(object e)
        {
            if (this.selectedVehicle != null)//车辆节点
            {

                Window win = new VehicleGPS.Views.Control.MonitorCentre.TrackPlayBack.TrackPlayBack();
                win.Show();
            }
            else
            {
                MessageBox.Show("请选择列表项");
            }
        }
        #endregion

        #region 双击实时GPS数据
        public DelegateCommand DoubleClickCommand { get; set; }
        public void DoubleClickCommandExecute()
        {
            if (this.selectedVehicle != null)
            {
                if (this.selectedVehicle.VehicleInfo.VehicleGPSInfo == null)
                {
                    MessageBox.Show("没有该车辆的实时gps信息!", "GPS信息", MessageBoxButton.OKCancel);
                    return;
                }
                if (this.selectedVehicle.VehicleInfo.VehicleGPSInfo.Latitude == ""
                    || this.selectedVehicle.VehicleInfo.VehicleGPSInfo.Longitude == "")
                {
                    MessageBox.Show("没有该车辆的实时地理位置信息!", "GPS信息", MessageBoxButton.OKCancel);
                    return;
                }
                this.MapService.FocusMarker(Convert.ToDouble(this.selectedVehicle.VehicleInfo.VehicleGPSInfo.Longitude), Convert.ToDouble(this.selectedVehicle.VehicleInfo.VehicleGPSInfo.Latitude));
            }
        }
        #endregion

        #region 获取选择的车辆(树形结构的选择车辆)
        public void SelectedGPSInfo()
        {
            RealTimeTreeNodeViewModel parentNode = RealTimeTreeViewModel.GetInstance().RootNode;
            /*找到根节点*/
            //if (StaticLoginInfo.GetInstance().UserName.Equals("admin"))
            //{
                while (parentNode.nodeInfo.ID != "admin")
                {
                    parentNode = parentNode.parentNode;
                }
            //}
            //else
            //{
            //    while (parentNode.nodeInfo.ID != "root")
            //    {
            //        parentNode = parentNode.parentNode;
            //    }
            //}
            List<RealTimeItemViewModel> listTmp = new List<RealTimeItemViewModel>();
            List<CVBasicInfo> listStation = new List<CVBasicInfo>();
            List<WarnInfo> warntmp = new List<WarnInfo>();
            List<MessageInfo> sendtmp = new List<MessageInfo>();
            List<MessageInfo> receivetmp = new List<MessageInfo>();
            List<CommandInfo> cmdtmp = new List<CommandInfo>();
            this.listSelectedVehicleInfo.Clear();
            GetSelectedVehicleDetailInfo(parentNode, listTmp, listStation, warntmp, sendtmp, receivetmp, cmdtmp);

            //刷新报警信息
            this.AlermInfoList.Clear();
            this.AlermInfoList = warntmp;

            //refresh sended message information list
            this.SendMessageList.Clear();
            this.SendMessageList = sendtmp;

            //refresh received message information list
            this.ReceiveMessageList.Clear();
            this.ReceiveMessageList = receivetmp;

            //refresh instruction list
            this.InstructionInfoList.Clear();
            this.InstructionInfoList = cmdtmp;

            //刷新GPS信息
            this.ListVehicleInfo.Clear();
            this.ListVehicleInfo = listTmp;
            this.listSelectedStationInfo.Clear();
            this.listSelectedStationInfo = listStation;
        }

        /// <summary>
        /// 获取选中车辆的GPS、报警、发出短语、接收短语、指令状态等实时信息
        /// </summary>
        /// <param name="node"></param>
        /// <param name="listTmp">GPS信息</param>
        /// <param name="listStation"></param>
        /// <param name="warntmp">报警信息</param>
        /// <param name="sendtmp">发出短语</param>
        /// <param name="receivetmp">接收短语</param>
        /// <param name="cmdtmp">指令</param>
        private void GetSelectedVehicleDetailInfo(RealTimeTreeNodeViewModel node, List<RealTimeItemViewModel> listTmp,
            List<CVBasicInfo> listStation, List<WarnInfo> warntmp, List<MessageInfo> sendtmp, List<MessageInfo> receivetmp, List<CommandInfo> cmdtmp)
        {
            if (node.listChildNodes != null)
            {
                if (node.isSelected == true)
                {
                    listStation.Add(node.NodeInfo);
                }
                foreach (RealTimeTreeNodeViewModel vtnvm in node.listChildNodes)
                {
                    GetSelectedVehicleDetailInfo(vtnvm, listTmp, listStation, warntmp, sendtmp, receivetmp, cmdtmp);
                }
            }
            else
            {
                if (node.nodeInfo.SIM != "0")//车辆节点
                {
                    if (node.isSelected == true)
                    {
                        this.listSelectedVehicleInfo.Add(node.NodeInfo);
                        //车辆GPS信息
                        StaticDetailInfo detailInfo = StaticDetailInfo.GetInstance();
                        foreach (CVDetailInfo info in detailInfo.ListVehicleDetailInfo)
                        {
                            if (info.SIM == node.nodeInfo.SIM)
                            {
                                //if (info.VehicleGPSInfo != null)
                                //{
                                //    if (info.VehicleGPSInfo.Latitude != ""
                                //        && info.VehicleGPSInfo.Longitude != "")
                                //    {
                                RealTimeItemViewModel itemVM = new RealTimeItemViewModel();
                                itemVM.VehicleInfo = info;
                                string status = info.VehicleState;
                                if (status.Equals("0")||status.Equals("1")||status.Equals("2")||status.Equals("3"))
                                {
                                    itemVM.VehicleInfo.VehicleState = status == "0" ? "空闲无任务" : status == "1" ? "进行任务中" : status == "2" ? "维修不可用" : "无任务离场";
                                }
                                else
                                {
                                    itemVM.VehicleInfo.VehicleState = status;
                                }
                                listTmp.Add(itemVM);
                                //    }
                                //}
                                break;
                            }
                        }
                        #region add by 夏创铭

                        //车辆报警信息 modified by 夏创铭
                        StaticWarnInfo warnInfoList = StaticWarnInfo.GetInstance();
                        foreach (WarnInfo warninfo in warnInfoList.WarnInfoList)
                        {
                            if (warninfo.SimId == node.nodeInfo.SIM)
                            {
                                WarnInfo warn = new WarnInfo();
                                warn = warninfo;
                                warntmp.Add(warn);
                            }
                        }
                        #endregion
                    }
                }
            }
        }


        #endregion

        #region 在datagrid选择的车辆
        /*节点选择状态*/
        private RealTimeItemViewModel selectedVehicle;
        public RealTimeItemViewModel SelectedVehicle
        {
            get { return selectedVehicle; }
            set
            {
                if (selectedVehicle != value)
                {
                    selectedVehicle = value;
                    this.RaisePropertyChanged("SelectedVehicle");
                }
            }
        }
        #endregion

        #region 右键实时GPS数据
        public DelegateCommand RightClickCommand { get; set; }
        public void RightClickCommandExecute()
        {
            if (this.selectedVehicle != null)
            {
                if (this.selectedVehicle.VehicleInfo.VehicleGPSInfo == null)
                {
                    MessageBox.Show("没有该车辆的实时gps信息!", "GPS信息", MessageBoxButton.OKCancel);
                    return;
                }
                if (this.selectedVehicle.VehicleInfo.VehicleGPSInfo.Latitude == ""
                    || this.selectedVehicle.VehicleInfo.VehicleGPSInfo.Longitude == "")
                {
                    MessageBox.Show("没有该车辆的实时地理位置信息!", "GPS信息", MessageBoxButton.OKCancel);
                    return;
                }
                this.MapService.FocusMarker(Convert.ToDouble(this.selectedVehicle.VehicleInfo.VehicleGPSInfo.Longitude), Convert.ToDouble(this.selectedVehicle.VehicleInfo.VehicleGPSInfo.Latitude));
            }
        }
        #endregion

        #region 刷新地图
        public void RefreshRealtimeMap()
        {
            int i = 0;
            while (StaticTreeState.RegionBasicInfo != LoadingState.LOADCOMPLETE && i < 3)
            {
                Thread.Sleep(500);
                i++;
            }
            if (StaticTreeState.RegionBasicInfo == LoadingState.LOADCOMPLETE)
            {
                StaticTreeState.RegionBasicInfo = LoadingState.LOADING;
                (new BasicDataServiceWCF()).GetRegionRightInfo();
                RealTimeViewModel.GetInstance().MapService.RemoveAllMarkers();
                RealTimeViewModel.GetInstance().InitRegionInBaidu();
                RealTimeViewModel.GetInstance().InitBaiduMap();
            }

        }
        #endregion
    }
}
