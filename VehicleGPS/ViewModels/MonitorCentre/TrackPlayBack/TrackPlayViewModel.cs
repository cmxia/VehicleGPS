using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.Prism.ViewModel;
using Microsoft.Practices.Prism.Commands;
using VehicleGPS.Models;
using VehicleGPS.Services.MonitorCentre.TrackPlayBack;
using System.Windows.Controls;
using System.Windows;
using VehicleGPS.ViewModels.AutoComplete;
using VehicleGPS.Models.MonitorCentre;
using System.ComponentModel;
using VehicleGPS.ViewModels.MonitorCentre.RealTimeMonitor;
using System.Threading;
using System.Net;
using System.Data;
using VehicleGPS.Services;

namespace VehicleGPS.ViewModels.MonitorCentre.TrackPlayBack
{
    class TrackPlayViewModel : NotificationObject
    {
        public TrackPlayDataOperate DataOperate { get; set; }
        public TrackPlayMapService MapService { get; set; }
        private int PageSize { get; set; }//分页大小
        public TrackPlayViewModel(WebBrowser webMap = null)
        {
            /////*初始化分页命令*/
            ////this.ComeFirstCommand = new DelegateCommand(new Action(this.ComeFirstCommandExecute));
            ////this.ComeLastCommand = new DelegateCommand(new Action(this.ComeLastCommandExecute));
            ////this.ComeNextCommand = new DelegateCommand(new Action(this.ComeNextCommandExecute));
            ////this.ComePrevCommand = new DelegateCommand(new Action(this.ComePrevCommandExecute));
            ////this.PageSize = 10;
            //// this.listVehicleInfoCurrentPage = new List<TrackBackGpsInfo>();
            /*查询命令*/
            this.QueryCommand = new DelegateCommand(new Action(this.QueryCommandExecute));
            /*初始化播放命令*/
            this.PlayCommand = new DelegateCommand(new Action(this.PlayCommandExecute));
            this.PauseCommand = new DelegateCommand(new Action(this.PauseCommandExecute));
            this.StopCommand = new DelegateCommand(new Action(this.StopCommandExecute));
            this.SpeedDownCommand = new DelegateCommand(new Action(this.SpeedDownCommandExecute));
            this.SpeedUpCommand = new DelegateCommand(new Action(this.SpeedUpCommandExecute));
            /*双击历史轨迹数据时*/
            this.DoubleClickCommand = new DelegateCommand<object>(new Action<object>(this.DoubleClickCommandExecute));
            this.DoubleClickOnlineCommand = new DelegateCommand<object>(new Action<object>(this.DoubleClickOnlineCommandExecute));
            this.DoubleClickOverCommand = new DelegateCommand<object>(new Action<object>(this.DoubleClickOverCommandExecute));
            this.DoubleClickStopCommand = new DelegateCommand<object>(new Action<object>(this.DoubleClickStopCommandExecute));
            this.DoubleClickWarnCommand = new DelegateCommand<object>(new Action<object>(this.DoubleClickWarnCommandExecute));

            /*导出excel*/
            this.PrintDetailCommand = new DelegateCommand(new Action(this.PrintDetailCommandExecute));
            this.PrintOnlineCommand = new DelegateCommand(new Action(this.PrintDetailCommandExecute));
            this.PrintOverSpeedCommand = new DelegateCommand(new Action(this.PrintOverSpeedCommandExecute));
            this.PrintStopCommand = new DelegateCommand(new Action(this.PrintStopCommandExecute));
            this.PrintWarnCommand = new DelegateCommand(new Action(this.PrintWarnCommandExecute));
            /*历史轨迹数据*/
            this.listVehicleInfo = new List<TrackBackGpsInfo>();
            this.listOnLineInfo = new List<GpsOnlineInfo>();

            this.DataOperate = new TrackPlayDataOperate(this);//获取历史轨迹数据类
            this.MapService = new TrackPlayMapService(webMap);//地图操作类
            /*播放速度*/
            this.PlaySpeed = 5;
            this.StartEnable = true;//启用播放
            this.PauseEnable = false;//禁用暂停
            this.StopEnable = false;//启用停止

            RealTimeTreeNodeViewModel focusOne = RealTimeTreeViewModel.GetInstance().focusOne;
            if (focusOne != null)
            {
                CVBasicInfo selectedBasicInfo = new CVBasicInfo();
                selectedBasicInfo.ID = focusOne.NodeInfo.ID;
                selectedBasicInfo.SIM = focusOne.NodeInfo.SIM;
                selectedBasicInfo.Name = focusOne.NodeInfo.Name;
                this.SelectedVehicle = selectedBasicInfo;
            }
            /*获取选择的车辆 启用时去除注释即可*/
            //勾选树形结构车辆时rtvm.SelectedVehicle 并没有选中
            //选中griddata时选中
            //RealTimeViewModel rtvm = RealTimeViewModel.GetInstance();
            //if (rtvm.SelectedVehicle != null)
            //{
            //    CVBasicInfo selectedBasicInfo = new CVBasicInfo();
            //    selectedBasicInfo.ID = rtvm.SelectedVehicle.VehicleInfo.VehicleNum;
            //    selectedBasicInfo.SIM = rtvm.SelectedVehicle.VehicleInfo.SIM;
            //    selectedBasicInfo.Name = rtvm.SelectedVehicle.VehicleInfo.VehicleId;
            //    this.SelectedVehicle = selectedBasicInfo;
            //}
        }


        public TrackPlayViewModel(WebBrowser webMap, CVBasicInfo basic, DateTime starttime, DateTime endtime)
        {
            this.BeginTime = starttime;
            this.EndTime = endtime;
            this.SelectedVehicle = basic;
            this.QueryCommand = new DelegateCommand(new Action(this.QueryCommandExecute));
            /*初始化播放命令*/
            this.PlayCommand = new DelegateCommand(new Action(this.PlayCommandExecute));
            this.PauseCommand = new DelegateCommand(new Action(this.PauseCommandExecute));
            this.StopCommand = new DelegateCommand(new Action(this.StopCommandExecute));
            this.SpeedDownCommand = new DelegateCommand(new Action(this.SpeedDownCommandExecute));
            this.SpeedUpCommand = new DelegateCommand(new Action(this.SpeedUpCommandExecute));
            /*双击历史轨迹数据时*/
            this.DoubleClickCommand = new DelegateCommand<object>(new Action<object>(this.DoubleClickCommandExecute));
            /*历史轨迹数据*/
            this.listVehicleInfo = new List<TrackBackGpsInfo>();
            this.listOnLineInfo = new List<GpsOnlineInfo>();

            this.DataOperate = new TrackPlayDataOperate(this);//获取历史轨迹数据类
            this.MapService = new TrackPlayMapService(webMap);//地图操作类
            /*播放速度*/
            this.PlaySpeed = 5;
            this.StartEnable = true;//启用播放
            this.PauseEnable = false;//禁用暂停
            this.StopEnable = false;//启用停止
        }

        #region 导出Excel
        public DelegateCommand PrintDetailCommand { get; set; }//导出明细
        private void PrintDetailCommandExecute()
        {
            if (this.ListVehicleInfo == null || !(this.ListVehicleInfo.Count > 0))
            {
                MessageBox.Show("没有要导出的明细数据！");
                return;
            }
            if (StaticTreeState.GpsDetailAddrLoad != LoadingState.LOADCOMPLETE)
            {
                MessageBox.Show("明细数据地址转换未完成！请稍后再试");
                return;
            }

            Microsoft.Win32.SaveFileDialog savedl = new Microsoft.Win32.SaveFileDialog();
            savedl.Filter = "excel2007|*.xlsx|所有文件|*.*";//文件扩展名
            savedl.ShowDialog();
            string path = savedl.FileName;
            if (path == "")
            {
                return;
            }
            DataTable dt = new DataTable();
            dt.Columns.Add("Sequence", typeof(string));
            dt.Columns.Add("vehicleId", typeof(string));
            dt.Columns.Add("Sim", typeof(string));
            dt.Columns.Add("CurrentLocation", typeof(string));
            dt.Columns.Add("Longitude", typeof(string));
            dt.Columns.Add("Latitude", typeof(string));
            dt.Columns.Add("Datetime", typeof(string));
            dt.Columns.Add("Speed", typeof(string));
            dt.Columns.Add("Accstatus", typeof(string));
            dt.Columns.Add("Workstatus", typeof(string));
            dt.Columns.Add("Gpsmode", typeof(string));
            dt.Columns.Add("Oilwaystatus", typeof(string));
            dt.Columns.Add("Shakestatus", typeof(string));
            dt.Columns.Add("Hornstatus", typeof(string));
            dt.Columns.Add("Conditionerstatus", typeof(string));
            dt.Columns.Add("Vdstatus", typeof(string));
            dt.Columns.Add("Fdstatus", typeof(string));
            dt.Columns.Add("Bdstatus", typeof(string));
            dt.Columns.Add("Ltstatus", typeof(string));
            dt.Columns.Add("Rtstatus", typeof(string));
            dt.Columns.Add("Farlstatus", typeof(string));
            dt.Columns.Add("Nearlstatus", typeof(string));
            dt.Columns.Add("Vcstatus", typeof(string));
            dt.Columns.Add("Llsecret", typeof(string));
            dt.Columns.Add("Pnstatus", typeof(string));
            dt.Columns.Add("Enginestatus", typeof(string));
            dt.Columns.Add("Brakestatus", typeof(string));
            dt.Columns.Add("Protectstatus", typeof(string));
            dt.Columns.Add("Loadstatus", typeof(string));
            dt.Columns.Add("Busstatus", typeof(string));
            dt.Columns.Add("Gsmstatus", typeof(string));
            dt.Columns.Add("Lcstatus", typeof(string));
            dt.Columns.Add("Ffstatus", typeof(string));
            dt.Columns.Add("Bfstatus", typeof(string));
            dt.Columns.Add("Gpsantstatus", typeof(string));
            DataRow row = dt.NewRow();
            row["Sequence"] = "序号";
            row["vehicleId"] = "SIM卡号";
            row["Sim"] = "内部编号";
            row["CurrentLocation"] = "当前位置";
            row["Longitude"] = "经度";
            row["Latitude"] = "纬度";
            row["Datetime"] = "上传时间";
            row["Speed"] = "GPS速度";
            row["Accstatus"] = "ACC状态";
            row["Workstatus"] = "运营状态";
            row["Gpsmode"] = "GPS模式";
            row["Oilwaystatus"] = "油路状态";
            row["Shakestatus"] = "震动状态";
            row["Hornstatus"] = "喇叭状态";
            row["Conditionerstatus"] = "空调状态";
            row["Vdstatus"] = "车门状态";
            row["Fdstatus"] = "前车门状态";
            row["Bdstatus"] = "后车门状态";
            row["Ltstatus"] = "左转向状态";
            row["Rtstatus"] = "右转向状态";
            row["Farlstatus"] = "远光灯状态";
            row["Nearlstatus"] = "近光灯状态";
            row["Vcstatus"] = "车辆电路状态";
            row["Llsecret"] = "经纬度加密状态";
            row["Pnstatus"] = "正反转状态";
            row["Enginestatus"] = "发动机状态";
            row["Brakestatus"] = "刹车状态";
            row["Protectstatus"] = "安全状态";
            row["Loadstatus"] = "负载状态";
            row["Busstatus"] = "总线状态";
            row["Gsmstatus"] = "GSM模块状态";
            row["Lcstatus"] = "锁车电路状态";
            row["Ffstatus"] = "前雾灯状态";
            row["Bfstatus"] = "后雾灯状态";
            row["Gpsantstatus"] = "GPS天线状态";
            dt.Rows.Add(row);
            int i = 1;
            foreach (TrackBackGpsInfo info in this.ListVehicleInfo)
            {
                row = dt.NewRow();
                row["Sequence"] = info.Sequence;
                row["vehicleId"] = SelectedVehicle.Name;
                row["Sim"] = SelectedVehicle.SIM;
                row["CurrentLocation"] = info.CurrentLocation;
                row["Longitude"] = info.GpsInfo.Longitude;
                row["Latitude"] = info.GpsInfo.Latitude;
                row["Datetime"] = info.GpsInfo.Datetime;
                row["Speed"] = info.GpsInfo.Speed;
                row["Accstatus"] = info.GpsInfo.Accstatus;
                row["Workstatus"] = info.GpsInfo.Workstatus;
                row["Gpsmode"] = info.GpsInfo.Gpsmode;
                row["Oilwaystatus"] = info.GpsInfo.Oilwaystatus;
                row["Shakestatus"] = info.GpsInfo.Shakestatus;
                row["Hornstatus"] = info.GpsInfo.Hornstatus;
                row["Conditionerstatus"] = info.GpsInfo.Conditionerstatus;
                row["Vdstatus"] = info.GpsInfo.Vdstatus;
                row["Fdstatus"] = info.GpsInfo.Fdstatus;
                row["Bdstatus"] = info.GpsInfo.Bdstatus;
                row["Ltstatus"] = info.GpsInfo.Ltstatus;
                row["Rtstatus"] = info.GpsInfo.Rtstatus;
                row["Farlstatus"] = info.GpsInfo.Farlstatus;
                row["Nearlstatus"] = info.GpsInfo.Nearlstatus;
                row["Vcstatus"] = info.GpsInfo.Vcstatus;
                row["Llsecret"] = info.GpsInfo.Llsecret;
                row["Pnstatus"] = info.GpsInfo.Pnstatus;
                row["Enginestatus"] = info.GpsInfo.Enginestatus;
                row["Brakestatus"] = info.GpsInfo.Brakestatus;
                row["Protectstatus"] = info.GpsInfo.Protectstatus;
                row["Loadstatus"] = info.GpsInfo.Loadstatus;
                row["Busstatus"] = info.GpsInfo.Busstatus;
                row["Gsmstatus"] = info.GpsInfo.Gsmstatus;
                row["Lcstatus"] = info.GpsInfo.Lcstatus;
                row["Ffstatus"] = info.GpsInfo.Ffstatus;
                row["Bfstatus"] = info.GpsInfo.Bfstatus;
                row["Gpsantstatus"] = info.GpsInfo.Gpsantstatus;
                dt.Rows.Add(row);
            }
            ExcelHelper.ExportExcel(dt, path);

        }
        public DelegateCommand PrintWarnCommand { get; set; }//导出报警数据
        private void PrintWarnCommandExecute()
        {
            if (this.ListVehicleWarnInfo == null || !(this.ListVehicleWarnInfo.Count > 0))
            {
                MessageBox.Show("没有要导出的报警数据！");
                return;
            }
            if (StaticTreeState.GpsWarnAddrLoad != LoadingState.LOADCOMPLETE)
            {
                MessageBox.Show("报警数据地址转换未完成！请稍后再试");
                return;
            }
            Microsoft.Win32.SaveFileDialog savedl = new Microsoft.Win32.SaveFileDialog();
            savedl.Filter = "excel2007|*.xlsx|所有文件|*.*";//文件扩展名
            savedl.ShowDialog();
            string path = savedl.FileName;
            if (path == "")
            {
                return;
            }
            DataTable dt = new DataTable();
            dt.Columns.Add("Sequence", typeof(string));
            dt.Columns.Add("WarnTime", typeof(string));
            dt.Columns.Add("LastTime", typeof(string));
            dt.Columns.Add("WarnType", typeof(string));
            dt.Columns.Add("Address", typeof(string));
            DataRow row = dt.NewRow();
            row["Sequence"] = "序号";
            row["WarnTime"] = "报警时间";
            row["LastTime"] = "报警持续时间";
            row["WarnType"] = "报警类型";
            row["Address"] = "报警地点";
            dt.Rows.Add(row);
            foreach (GPSWarnInfo info in this.ListVehicleWarnInfo)
            {
                row = dt.NewRow();
                row["Sequence"] = info.Sequence;
                row["WarnTime"] = info.WarnTime;
                row["LastTime"] = info.LastTime;
                row["WarnType"] = info.WarnType;
                row["Address"] = info.Address;
                dt.Rows.Add(row);
            }
            ExcelHelper.ExportExcel(dt, path);

        }
        public DelegateCommand PrintStopCommand { get; set; }//导出停车数据明细
        private void PrintStopCommandExecute()
        {
            if (this.ListVehicleStopInfo == null || !(this.ListVehicleStopInfo.Count > 0))
            {
                MessageBox.Show("没有要导出的停车数据！");
                return;
            }
            if (StaticTreeState.StopAddrLoad != LoadingState.LOADCOMPLETE)
            {
                MessageBox.Show("停车数据地址转换未完成！请稍后再试");
                return;
            }
            Microsoft.Win32.SaveFileDialog savedl = new Microsoft.Win32.SaveFileDialog();
            savedl.Filter = "excel2007|*.xlsx|所有文件|*.*";//文件扩展名
            savedl.ShowDialog();
            string path = savedl.FileName;
            if (path == "")
            {
                return;
            }
            DataTable dt = new DataTable();
            dt.Columns.Add("Sequence", typeof(string));
            dt.Columns.Add("StartTime", typeof(string));
            dt.Columns.Add("EndTime", typeof(string));
            dt.Columns.Add("LastTime", typeof(string));
            dt.Columns.Add("Address", typeof(string));
            DataRow row = dt.NewRow();
            row["Sequence"] = "序号";
            row["StartTime"] = "开始时间";
            row["EndTime"] = "结束时间";
            row["LastTime"] = "持续时间";
            row["Address"] = "停车位置";
            dt.Rows.Add(row);
            int i = 1;
            foreach (GPSStopInfo info in this.ListVehicleStopInfo)
            {
                row = dt.NewRow();
                row["Sequence"] = info.Sequence;
                row["StartTime"] = info.StartTime;
                row["EndTime"] = info.EndTime;
                row["LastTime"] = info.LastTime;
                row["Address"] = info.Address;
                dt.Rows.Add(row);
            }
            ExcelHelper.ExportExcel(dt, path);

        }
        public DelegateCommand PrintOverSpeedCommand { get; set; }//导出超速明细
        private void PrintOverSpeedCommandExecute()
        {
            if (this.ListVehicleOverSpeedInfo == null || !(this.ListVehicleOverSpeedInfo.Count > 0))
            {
                MessageBox.Show("没有要导出的超速数据！");
                return;
            }
            if (StaticTreeState.StopAddrLoad != LoadingState.LOADCOMPLETE)
            {
                MessageBox.Show("超速数据地址转换未完成！请稍后再试");
                return;
            }
            Microsoft.Win32.SaveFileDialog savedl = new Microsoft.Win32.SaveFileDialog();
            savedl.Filter = "excel2007|*.xlsx|所有文件|*.*";//文件扩展名
            savedl.ShowDialog();
            string path = savedl.FileName;
            if (path == "")
            {
                return;
            }
            DataTable dt = new DataTable();
            dt.Columns.Add("Sequence", typeof(string));
            dt.Columns.Add("StartTime", typeof(string));
            dt.Columns.Add("EndTime", typeof(string));
            dt.Columns.Add("LastTime", typeof(string));
            dt.Columns.Add("StartAddress", typeof(string));
            dt.Columns.Add("EndAddress", typeof(string));
            DataRow row = dt.NewRow();
            row["Sequence"] = "序号";
            row["StartTime"] = "开始时间";
            row["EndTime"] = "结束时间";
            row["LastTime"] = "持续时间";
            row["StartAddress"] = "开始位置";
            row["EndAddress"] = "结束位置";
            dt.Rows.Add(row);
            foreach (GPSOverSpeedInfo info in this.ListVehicleOverSpeedInfo)
            {
                row = dt.NewRow();
                row["Sequence"] = info.Sequence;
                row["StartTime"] = info.StartTime;
                row["EndTime"] = info.EndTime;
                row["LastTime"] = info.LastTime;
                row["StartAddress"] = info.StartAddress;
                row["EndAddress"] = info.EndAddress;
                dt.Rows.Add(row);
            }
            ExcelHelper.ExportExcel(dt, path);

        }
        public DelegateCommand PrintOnlineCommand { get; set; }//导出上下线明细
        private void PrintOnlineCommandExecute()
        {
            if (this.ListVehicleInfo == null || !(this.ListVehicleInfo.Count > 0))
            {
                MessageBox.Show("没有要导出的超速数据！");
                return;
            }
            if (StaticTreeState.StopAddrLoad != LoadingState.LOADCOMPLETE)
            {
                MessageBox.Show("超速数据地址转换未完成！请稍后再试");
                return;
            }
            Microsoft.Win32.SaveFileDialog savedl = new Microsoft.Win32.SaveFileDialog();
            savedl.Filter = "excel2007|*.xlsx|所有文件|*.*";//文件扩展名
            savedl.ShowDialog();
            string path = savedl.FileName;
            if (path == "")
            {
                return;
            }
            DataTable dt = new DataTable();
            dt.Columns.Add("Sequence", typeof(string));
            dt.Columns.Add("StartTime", typeof(string));
            dt.Columns.Add("EndTime", typeof(string));
            dt.Columns.Add("LastTime", typeof(string));
            dt.Columns.Add("StartAddress", typeof(string));
            dt.Columns.Add("EndAddress", typeof(string));
            DataRow row = dt.NewRow();
            row["Sequence"] = "序号";
            row["StartTime"] = "开始时间";
            row["EndTime"] = "结束时间";
            row["LastTime"] = "持续时间";
            row["StartAddress"] = "开始位置";
            row["EndAddress"] = "结束位置";
            dt.Rows.Add(row);
            foreach (GPSOverSpeedInfo info in this.ListVehicleOverSpeedInfo)
            {
                row = dt.NewRow();
                row["Sequence"] = info.Sequence;
                row["StartTime"] = info.StartTime;
                row["EndTime"] = info.EndTime;
                row["LastTime"] = info.LastTime;
                row["StartAddress"] = info.StartAddress;
                row["EndAddress"] = info.EndAddress;
                dt.Rows.Add(row);
            }
            ExcelHelper.ExportExcel(dt, path);

        }
        #endregion

        #region 轨迹回放查询条件

        private bool initializeChecked;

        public bool InitializeChecked
        {
            get { return initializeChecked; }
            set
            {
                initializeChecked = value;
                this.RaisePropertyChanged("InitializeChecked");
                if (initializeChecked)
                {
                    InitializeBaiduMap();
                }
            }
        }
        //将历史轨迹显示到地图
        private void InitializeBaiduMap()
        {
            if (StaticTreeState.StopInfoLoad != LoadingState.LOADCOMPLETE && StaticTreeState.OverSpeedInfoLoad != LoadingState.LOADCOMPLETE)
            {
                this.DataOperate.InitOtherDataInfo();//根据历史轨迹信息初始化停车数据
            }
            Thread.Sleep(1000);
            Thread thread = new Thread(ParseStopAddress);
            thread.Start();
            this.MapService.WebMap.Dispatcher.Invoke(new Action(this.InitTrackPlayMap));//将历史轨迹显示到地图
        }

        /// <summary>
        /// 
        /// </summary>
        private CVBasicInfo selectedVehicle = null;
        public CVBasicInfo SelectedVehicle
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
        private DateTime beginTime = DateTime.Now.AddDays(-2);//开始时间为两天前
        public DateTime BeginTime
        {
            get { return beginTime; }
            set
            {
                if (beginTime != value)
                {
                    beginTime = value;
                    this.RaisePropertyChanged("BeginTime");
                }
            }
        }
        private DateTime endTime = DateTime.Now;
        public DateTime EndTime
        {
            get { return endTime; }
            set
            {
                if (endTime != value)
                {
                    endTime = value;
                    this.RaisePropertyChanged("EndTime");
                }
            }
        }
        /*查询命令*/
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
        /*地图是否可见*/
        private Visibility mapVisibility = Visibility.Visible;//忙等待
        public Visibility MapVisibility
        {
            get { return mapVisibility; }
            set
            {
                if (mapVisibility != value)
                {
                    mapVisibility = value;
                    this.RaisePropertyChanged("MapVisibility");
                }
            }
        }
        /// <summary>
        /// 查询命令
        /// </summary>
        public DelegateCommand QueryCommand { get; set; }
        public void QueryCommandExecute()
        {
            BackgroundWorker worker = new BackgroundWorker();
            worker.DoWork += (o, ea) =>
                {
                    this.InitializeChecked = false;
                    this.DataOperate.GetTrackPlayInfo();
                };
            worker.RunWorkerCompleted += (o, ea) =>
                {
                    this.IsBusy = false;
                    this.MapVisibility = Visibility.Visible;
                };
            this.IsBusy = true;
            this.MapVisibility = Visibility.Collapsed;
            worker.RunWorkerAsync();

        }
        #endregion

        #region 播放条件项
        /*播放速度*/
        /// <summary>
        /// 播放速度
        /// </summary>
        private int playSpeed;
        public int PlaySpeed
        {
            get { return playSpeed; }
            set
            {
                if (playSpeed != value)
                {
                    playSpeed = value;
                    this.RaisePropertyChanged("PlaySpeed");
                    if (this.MapService != null)
                    {//设置播放速度
                        this.MapService.SetPlaySpeed(playSpeed);
                    }
                }
            }
        }

        /*播放操作和可用树形*/
        /// <summary>
        /// 开始播放标志
        /// </summary>
        private bool startEnable;
        public bool StartEnable
        {
            get { return startEnable; }
            set
            {
                if (startEnable != value)
                {
                    startEnable = value;
                    this.RaisePropertyChanged("StartEnable");
                }
            }
        }
        /// <summary>
        /// 暂停播放标志
        /// </summary>
        private bool pauseEnable;
        public bool PauseEnable
        {
            get { return pauseEnable; }
            set
            {
                if (pauseEnable != value)
                {
                    pauseEnable = value;
                    this.RaisePropertyChanged("PauseEnable");
                }
            }
        }
        /// <summary>
        /// 停止播放标志
        /// </summary>
        private bool stopEnable;
        public bool StopEnable
        {
            get { return stopEnable; }
            set
            {
                if (stopEnable != value)
                {
                    stopEnable = value;
                    this.RaisePropertyChanged("StopEnable");
                }
            }
        }
        #endregion

        #region 播放操作（加速、减速、开始播放、暂停播放、停止播放）
        public DelegateCommand SpeedDownCommand { get; set; }
        public DelegateCommand SpeedUpCommand { get; set; }
        public void SpeedDownCommandExecute()
        {
            if (this.playSpeed > 1)
            {
                this.PlaySpeed--;
            }
        }
        public void SpeedUpCommandExecute()
        {
            if (this.playSpeed < 10)
            {
                this.PlaySpeed++;
            }
        }
        public DelegateCommand PlayCommand { get; set; }//开始播放
        public DelegateCommand PauseCommand { get; set; }//暂停播放
        public DelegateCommand StopCommand { get; set; }//停止播放
        public void PlayCommandExecute()
        {
            if (this.listVehicleInfo.Count != 0)
            {
                this.StartEnable = false;
                this.PauseEnable = true;
                this.StopEnable = true;
                this.MapService.StartTrackPlay();
            }
            else
            {
                MessageBox.Show("请选择车辆查询历史轨迹数据", "轨迹回放", MessageBoxButton.OKCancel);
            }
        }
        public void PauseCommandExecute()
        {

            this.StartEnable = true;
            this.PauseEnable = false;
            this.StopEnable = true;
            this.MapService.PauseTrackPlay();
        }
        public void StopCommandExecute()
        {
            this.PauseEnable = false;
            this.StartEnable = true;
            this.StopEnable = false;
            this.MapService.StopTrackPlay();
        }
        #endregion

        #region 绑定数据源

        /*明细数据*/
        private List<TrackBackGpsInfo> listVehicleInfo = null;
        public List<TrackBackGpsInfo> ListVehicleInfo
        {
            get { return listVehicleInfo; }
            set
            {
                if (listVehicleInfo != value)
                {
                    listVehicleInfo = value;
                    this.RaisePropertyChanged("ListVehicleInfo");
                   
                }
            }
        }
        private void GetAddressThread()
        {
            if (this.ListVehicleInfo != null && this.ListVehicleInfo.Count > 0)
            {
                TrackPlayDataOperate track = new TrackPlayDataOperate();
                foreach (TrackBackGpsInfo vehicleinfo in ListVehicleInfo)
                {
                    vehicleinfo.CurrentLocation = track.GetAddress(vehicleinfo);
                }
            }
            StaticTreeState.GpsDetailAddrLoad = LoadingState.LOADCOMPLETE;
        }

        #region 初始化历史轨迹回放地图

        /// <summary>
        /// 初始化轨迹回放地图
        /// </summary>
        private void InitTrackPlayMap()
        {
            this.MapService.InitTrackPlayMap();
            this.MapService.InitTrackPlayData(this);
            this.MapService.SetPlaySpeed(this.PlaySpeed);
        }
        #endregion

        /*停车数据*/
        private List<GPSStopInfo> listVehicleStopInfo = null;
        public List<GPSStopInfo> ListVehicleStopInfo
        {
            get { return listVehicleStopInfo; }
            set
            {
                if (listVehicleStopInfo != value)
                {
                    listVehicleStopInfo = value;
                    this.RaisePropertyChanged("ListVehicleStopInfo");
                }
            }
        }
        void ParseStopAddress()
        {
            StaticTreeState.StopAddrLoad = LoadingState.LOADING;
            StaticTreeState.GpsWarnAddrLoad = LoadingState.LOADING;
            StaticTreeState.GpsOnlineAddrLoad = LoadingState.LOADING;
            StaticTreeState.OverSpeedAddrLoad = LoadingState.LOADING;
            StaticTreeState.GpsDetailAddrLoad = LoadingState.LOADING;
            if (this.ListVehicleInfo != null && this.ListVehicleInfo.Count > 0)
            {
                TrackPlayDataOperate track2 = new TrackPlayDataOperate();
                foreach (GPSStopInfo vehicleinfo in ListVehicleStopInfo)
                {
                    vehicleinfo.Address = track2.ParseAddress(vehicleinfo.lng, vehicleinfo.lat);
                }
            }
            StaticTreeState.StopAddrLoad = LoadingState.LOADCOMPLETE;
            Thread thread = new Thread(ParseOverAddress);
            thread.Start();
        }
        /*报警数据*/
        private List<GPSWarnInfo> listVehicleWarnInfo = null;
        public List<GPSWarnInfo> ListVehicleWarnInfo
        {
            get { return listVehicleWarnInfo; }
            set
            {
                if (listVehicleWarnInfo != value)
                {
                    listVehicleWarnInfo = value;
                    this.RaisePropertyChanged("ListVehicleWarnInfo");
                }
            }
        }
        private void GetWarnAddress()
        {
            TrackPlayDataOperate track2 = new TrackPlayDataOperate();
            if (this.ListVehicleInfo != null && this.ListVehicleInfo.Count > 0)
            {
                foreach (GPSWarnInfo vehicleinfo in ListVehicleWarnInfo)
                {
                    vehicleinfo.Address = track2.ParseAddress(vehicleinfo.Longitude, vehicleinfo.Latitude);
                }
            }
            StaticTreeState.GpsWarnAddrLoad = LoadingState.LOADCOMPLETE;
            Thread onlineAddr = new Thread(ParseOnlineAddress);
            onlineAddr.Start();
        }
        /*超速数据*/
        private List<GPSOverSpeedInfo> listVehicleOverSpeedInfo = null;
        public List<GPSOverSpeedInfo> ListVehicleOverSpeedInfo
        {
            get { return listVehicleOverSpeedInfo; }
            set
            {
                if (listVehicleOverSpeedInfo != value)
                {
                    listVehicleOverSpeedInfo = value;
                    this.RaisePropertyChanged("ListVehicleOverSpeedInfo");
                }
            }
        }
        /*上下线明细数据*/
        private List<GpsOnlineInfo> listOnLineInfo = null;
        public List<GpsOnlineInfo> ListOnLineInfo
        {
            get { return listOnLineInfo; }
            set
            {
                if (listOnLineInfo != value)
                {
                    listOnLineInfo = value;
                    this.RaisePropertyChanged("ListOnLineInfo");
                }
            }
        }
        void ParseOverAddress()
        {
            try
            {
                foreach (var item in this.ListVehicleOverSpeedInfo)
                {
                    TrackPlayDataOperate track2 = new TrackPlayDataOperate();
                    item.StartAddress = track2.ParseAddress(item.Slng, item.Slat);
                    item.EndAddress = track2.ParseAddress(item.Elng, item.Elat);
                }
                StaticTreeState.OverSpeedAddrLoad = LoadingState.LOADCOMPLETE;
                Thread warnAddThread = new Thread(this.GetWarnAddress);
                warnAddThread.Start();
            }
            catch (NullReferenceException)
            {

            }

        }
        /*上下线明细数据*/
        private List<GpsOnlineInfo> listVehicleOnlineInfo;
        public List<GpsOnlineInfo> ListVehicleOnlineInfo
        {
            get { return listVehicleOnlineInfo; }
            set
            {
                listVehicleOnlineInfo = value;
                this.RaisePropertyChanged("ListVehicleOnlineInfo");
            }
        }
        void ParseOnlineAddress()
        {
            foreach (var item in this.ListVehicleOnlineInfo)
            {
                TrackPlayDataOperate track2 = new TrackPlayDataOperate();
                item.OnlineAddr = track2.ParseAddress(item.Slng, item.Slat);
                item.OfflineAddr = track2.ParseAddress(item.Elng, item.Elat);
            }
            StaticTreeState.GpsOnlineAddrLoad = LoadingState.LOADCOMPLETE;
            Thread addrConvert = new Thread(GetAddressThread);
            addrConvert.Start();
        }
        #endregion

        #region 统计数据（记录数）
        /*记录数*/
        /// <summary>
        /// 记录数
        /// </summary>
        private string recordCount;
        public string RecordCount
        {
            get { return recordCount; }
            set
            {
                if (recordCount != value)
                {
                    recordCount = value;
                    this.RaisePropertyChanged("RecordCount");
                }
            }
        }
        /*行驶时长*/
        private string driveTime;
        public string DriveTime
        {
            get { return driveTime; }
            set
            {
                if (driveTime != value)
                {
                    driveTime = value;
                    this.RaisePropertyChanged("DriveTime");
                }
            }
        }
        /*休息时长*/
        private string restTime;
        public string RestTime
        {
            get { return restTime; }
            set
            {
                if (restTime != value)
                {
                    restTime = value;
                    this.RaisePropertyChanged("RestTime");
                }
            }
        }
        /*行驶里程*/
        private string driveMileage;
        public string DriveMileage
        {
            get { return driveMileage; }
            set
            {
                if (driveMileage != value)
                {
                    driveMileage = value;
                    this.RaisePropertyChanged("DriveMileage");
                }
            }
        }
        /*最高速度*/
        private string maxSpeed;
        public string MaxSpeed
        {
            get { return maxSpeed; }
            set
            {
                if (maxSpeed != value)
                {
                    maxSpeed = value;
                    this.RaisePropertyChanged("MaxSpeed");
                }
            }
        }
        /*平均速度*/
        private string averageSpeed;
        public string AverageSpeed
        {
            get { return averageSpeed; }
            set
            {
                if (averageSpeed != value)
                {
                    averageSpeed = value;
                    this.RaisePropertyChanged("AverageSpeed");
                }
            }
        }
        #endregion

        #region 双击实时GPS数据
        private int selectedInfoIndex;
        /// <summary>
        /// 选中项的index
        /// </summary>
        public int SelectedInfoIndex
        {
            get { return selectedInfoIndex; }
            set
            {
                if (selectedInfoIndex != value)
                {
                    selectedInfoIndex = value;
                    this.RaisePropertyChanged("SelectedInfoIndex");
                }
            }
        }
        public DelegateCommand<object> DoubleClickCommand { get; set; }
        private void DoubleClickCommandExecute(object selectedItem)
        {
            TrackBackGpsInfo itemInfo = (TrackBackGpsInfo)selectedItem;
            if (itemInfo != null)
            {
                if (itemInfo.GpsInfo.Longitude == null || itemInfo.GpsInfo.Latitude == null || itemInfo.GpsInfo.Longitude == "" || itemInfo.GpsInfo.Latitude == "")
                    return;
                this.MapService.SetMarker(itemInfo.GpsInfo.Longitude, itemInfo.GpsInfo.Latitude,
                    VehicleCommon.GetHtml(this.SelectedVehicle.Name, this.SelectedVehicle.SIM, itemInfo.GpsInfo.Datetime,
                        itemInfo.GpsInfo.Speed, itemInfo.GpsInfo.Direction, itemInfo.GpsInfo.CurLocation)
                    , VehicleCommon.GetDirectionImageUrl(itemInfo.GpsInfo.Direction, VehicleCommon.VSOnlineRun));
            }
        }

        public DelegateCommand<object> DoubleClickWarnCommand { get; set; }//双击报警数据
        private void DoubleClickWarnCommandExecute(object selectedItem)
        {
            GPSWarnInfo itemInfo = (GPSWarnInfo)selectedItem;
            if (itemInfo != null)
            {
                foreach (var item in this.ListVehicleInfo)
                {
                    if (itemInfo.WarnTime.Equals(item.GpsInfo.Datetime))
                    {
                        if (item.GpsInfo.Longitude == null || item.GpsInfo.Latitude == null || item.GpsInfo.Longitude == "" || item.GpsInfo.Latitude == "")
                            return;
                        this.MapService.SetMarker(item.GpsInfo.Longitude, item.GpsInfo.Latitude,
                            VehicleCommon.GetHtml(this.SelectedVehicle.Name, this.SelectedVehicle.SIM, item.GpsInfo.Datetime,
                                item.GpsInfo.Speed, item.GpsInfo.Direction, item.GpsInfo.CurLocation)
                            , VehicleCommon.GetDirectionImageUrl(item.GpsInfo.Direction, VehicleCommon.VSOnlineRun));
                    }
                }
            }
        }
        public DelegateCommand<object> DoubleClickStopCommand { get; set; }//双击停车数据
        private void DoubleClickStopCommandExecute(object selectedItem)
        {
            GPSStopInfo itemInfo = (GPSStopInfo)selectedItem;
            if (itemInfo != null)
            {
                foreach (var item in this.ListVehicleInfo)
                {
                    if (itemInfo.StartTime.Equals(item.GpsInfo.Datetime))
                    {
                        if (item.GpsInfo.Longitude == null || item.GpsInfo.Latitude == null || item.GpsInfo.Longitude == "" || item.GpsInfo.Latitude == "")
                            return;
                        this.MapService.SetMarker(item.GpsInfo.Longitude, item.GpsInfo.Latitude,
                            VehicleCommon.GetHtml(this.SelectedVehicle.Name, this.SelectedVehicle.SIM, item.GpsInfo.Datetime,
                                item.GpsInfo.Speed, item.GpsInfo.Direction, item.GpsInfo.CurLocation)
                            , VehicleCommon.GetDirectionImageUrl(item.GpsInfo.Direction, VehicleCommon.VSOnlineRun));
                    }
                }
            }
        }
        public DelegateCommand<object> DoubleClickOverCommand { get; set; }//双击超速数据
        private void DoubleClickOverCommandExecute(object selectedItem)
        {
            GPSOverSpeedInfo itemInfo = (GPSOverSpeedInfo)selectedItem;
            if (itemInfo != null)
            {
                foreach (var item in this.ListVehicleInfo)
                {
                    if (itemInfo.StartTime.Equals(item.GpsInfo.Datetime))
                    {
                        if (item.GpsInfo.Longitude == null || item.GpsInfo.Latitude == null || item.GpsInfo.Longitude == "" || item.GpsInfo.Latitude == "")
                            return;
                        this.MapService.SetMarker(item.GpsInfo.Longitude, item.GpsInfo.Latitude,
                            VehicleCommon.GetHtml(this.SelectedVehicle.Name, this.SelectedVehicle.SIM, item.GpsInfo.Datetime,
                                item.GpsInfo.Speed, item.GpsInfo.Direction, item.GpsInfo.CurLocation)
                            , VehicleCommon.GetDirectionImageUrl(item.GpsInfo.Direction, VehicleCommon.VSOnlineRun));
                    }
                }
            }
        }
        public DelegateCommand<object> DoubleClickOnlineCommand { get; set; }//双击上下线数据
        private void DoubleClickOnlineCommandExecute(object selectedItem)
        {
            TrackBackGpsInfo itemInfo = (TrackBackGpsInfo)selectedItem;
            if (itemInfo != null)
            {
                if (itemInfo.GpsInfo.Longitude == null || itemInfo.GpsInfo.Latitude == null || itemInfo.GpsInfo.Longitude == "" || itemInfo.GpsInfo.Latitude == "")
                    return;
                this.MapService.SetMarker(itemInfo.GpsInfo.Longitude, itemInfo.GpsInfo.Latitude,
                    VehicleCommon.GetHtml(this.SelectedVehicle.Name, this.SelectedVehicle.SIM, itemInfo.GpsInfo.Datetime,
                        itemInfo.GpsInfo.Speed, itemInfo.GpsInfo.Direction, itemInfo.GpsInfo.CurLocation)
                    , VehicleCommon.GetDirectionImageUrl(itemInfo.GpsInfo.Direction, VehicleCommon.VSOnlineRun));
            }
        }
        #endregion

        #region 分页（弃用）
        ///*绑定当前页*/
        //private List<TrackBackGpsInfo> listVehicleInfoCurrentPage = null;
        //public List<TrackBackGpsInfo> ListVehicleInfoCurrentPage
        //{
        //    get { return listVehicleInfoCurrentPage; }
        //    set
        //    {
        //        if (listVehicleInfoCurrentPage != value)
        //        {
        //            listVehicleInfoCurrentPage = value;
        //            this.RaisePropertyChanged("ListVehicleInfoCurrentPage");
        //        }
        //    }
        //} 
        //private void InitPage()
        //{
        //    this.TotalCount = this.ListVehicleInfo.Count;
        //    this.TotalPage = (this.TotalCount / this.PageSize) * this.PageSize < totalCount ? this.TotalCount / this.PageSize + 1 : this.TotalCount / this.PageSize;
        //    this.CurrentPage = 0;
        //    this.CurrentStart = 0;
        //    this.CurrentEnd = 0;
        //    if (this.ListVehicleInfoCurrentPage != null)
        //    {
        //        this.ListVehicleInfoCurrentPage.Clear();
        //    }
        //    this.ListVehicleInfoCurrentPage = null;
        //    if (this.TotalCount != 0)
        //    {
        //        this.ComeFirstCommandExecute();
        //    }
        //}
        //public DelegateCommand ComeFirstCommand { get; set; }//首页
        //public DelegateCommand ComePrevCommand { get; set; }//前一页
        //public DelegateCommand ComeNextCommand { get; set; }//下一页
        //public DelegateCommand ComeLastCommand { get; set; }//末页

        ///*前一页按钮是否可用*/
        //private bool prevEnable = true;
        //public bool PrevEnable
        //{
        //    get { return prevEnable; }
        //    set
        //    {
        //        if (prevEnable != value)
        //        {
        //            prevEnable = value;
        //            this.RaisePropertyChanged("PrevEnable");
        //        }
        //    }
        //}
        ///*下一页按钮是否可用*/
        //private bool nextEnable = true;
        //public bool NextEnable
        //{
        //    get { return nextEnable; }
        //    set
        //    {
        //        if (nextEnable != value)
        //        {
        //            nextEnable = value;
        //            this.RaisePropertyChanged("NextEnable");
        //        }
        //    }
        //}
        //private void SetPrevNextEnable()
        //{
        //    if (this.CurrentPage == 1)
        //    {
        //        this.PrevEnable = false;
        //    }
        //    else
        //    {
        //        this.PrevEnable = true;
        //    }
        //    if (this.CurrentPage == this.TotalPage)
        //    {
        //        this.NextEnable = false;
        //    }
        //    else
        //    {
        //        this.NextEnable = true;
        //    }
        //}

        //public void ComeFirstCommandExecute()
        //{
        //    if (this.TotalCount == 0)
        //    {
        //        return;
        //    }
        //    this.CurrentPage = 1;
        //    this.CurrentStart = 1;
        //    this.CurrentEnd = this.PageSize > this.TotalCount ? this.TotalCount : this.PageSize;
        //    List<TrackBackGpsInfo> pageTmp = new List<TrackBackGpsInfo>();
        //    int sequence = (this.CurrentPage - 1) * this.PageSize + 1;//序号
        //    for (int i = CurrentStart - 1; i < this.CurrentEnd; i++)
        //    {
        //        //GPSInfo gpsInfo = this.listVehicleInfo[i].VehicleInfo.VehicleGPSInfo;
        //        //if (gpsInfo != null && gpsInfo.Latitude != "" && gpsInfo.Longitude != "")
        //        //{
        //        this.listVehicleInfo[i].GpsInfo.Sequence = sequence++;
        //        pageTmp.Add(this.listVehicleInfo[i]);
        //        //}
        //    }
        //    if (this.ListVehicleInfoCurrentPage != null)
        //    {
        //        this.ListVehicleInfoCurrentPage.Clear();
        //    }
        //    this.ListVehicleInfoCurrentPage = pageTmp;

        //    SetPrevNextEnable();
        //}
        //public void ComePrevCommandExecute()
        //{
        //    if (this.TotalCount == 0)
        //    {
        //        return;
        //    }
        //    if (this.CurrentPage > 1)
        //    {
        //        this.CurrentPage--;
        //        this.CurrentEnd = this.CurrentStart - 1;
        //        this.CurrentStart = (this.CurrentPage - 1) * this.PageSize + 1;
        //        List<TrackBackGpsInfo> pageTmp = new List<TrackBackGpsInfo>();
        //        int sequence = (this.CurrentPage - 1) * this.PageSize + 1;//序号
        //        for (int i = this.CurrentStart - 1; i < this.CurrentEnd; i++)
        //        {
        //            //GPSInfo gpsInfo = this.listVehicleInfo[i].VehicleInfo.VehicleGPSInfo;
        //            //if (gpsInfo != null && gpsInfo.Latitude != "" && gpsInfo.Longitude != "")
        //            //{
        //            this.listVehicleInfo[i].GpsInfo.Sequence = sequence++;
        //            pageTmp.Add(this.listVehicleInfo[i]);
        //            //}
        //        }
        //        if (this.ListVehicleInfoCurrentPage != null)
        //        {
        //            this.ListVehicleInfoCurrentPage.Clear();
        //        }
        //        this.ListVehicleInfoCurrentPage = pageTmp;
        //    }
        //    SetPrevNextEnable();
        //}
        //public void ComeNextCommandExecute()
        //{
        //    if (this.TotalCount == 0)
        //    {
        //        return;
        //    }
        //    if (this.CurrentPage < this.TotalPage)
        //    {
        //        this.CurrentStart = this.CurrentPage * this.PageSize + 1;
        //        this.CurrentEnd = (this.CurrentStart + this.PageSize - 1) > this.TotalCount ? this.TotalCount : (this.CurrentStart + this.PageSize - 1);
        //        this.CurrentPage++;
        //        List<TrackBackGpsInfo> pageTmp = new List<TrackBackGpsInfo>();
        //        int sequence = (this.CurrentPage - 1) * this.PageSize + 1;//序号
        //        for (int i = CurrentStart - 1; i < this.CurrentEnd; i++)
        //        {
        //            //GPSInfo gpsInfo = this.listVehicleInfo[i].VehicleInfo.VehicleGPSInfo;
        //            //if (gpsInfo != null && gpsInfo.Latitude != "" && gpsInfo.Longitude != "")
        //            //{
        //            this.listVehicleInfo[i].GpsInfo.Sequence = sequence++;
        //            pageTmp.Add(this.listVehicleInfo[i]);
        //            //}
        //        }
        //        if (this.ListVehicleInfoCurrentPage != null)
        //        {
        //            this.ListVehicleInfoCurrentPage.Clear();
        //        }
        //        this.ListVehicleInfoCurrentPage = pageTmp;
        //    }
        //    SetPrevNextEnable();
        //}
        //public void ComeLastCommandExecute()
        //{
        //    if (this.TotalCount == 0)
        //    {
        //        return;
        //    }
        //    this.CurrentPage = this.TotalPage;
        //    this.CurrentStart = this.TotalPage > 1 ? (this.CurrentPage - 1) * this.PageSize : 1;
        //    this.CurrentEnd = this.TotalCount;
        //    List<TrackBackGpsInfo> pageTmp = new List<TrackBackGpsInfo>();
        //    int sequence = (this.CurrentPage - 1) * this.PageSize + 1;//序号
        //    for (int i = CurrentStart - 1; i < this.CurrentEnd; i++)
        //    {
        //        //GPSInfo gpsInfo = this.listVehicleInfo[i].VehicleInfo.VehicleGPSInfo;
        //        //if (gpsInfo != null && gpsInfo.Latitude != "" && gpsInfo.Longitude != "")
        //        //{
        //        this.listVehicleInfo[i].GpsInfo.Sequence = sequence++;
        //        pageTmp.Add(this.listVehicleInfo[i]);
        //        //}
        //    }
        //    if (this.ListVehicleInfoCurrentPage != null)
        //    {
        //        this.ListVehicleInfoCurrentPage.Clear();
        //    }
        //    this.ListVehicleInfoCurrentPage = pageTmp;

        //    SetPrevNextEnable();
        //}
        ///*当前页*/
        //private int currentPage;
        //public int CurrentPage
        //{
        //    get { return currentPage; }
        //    set
        //    {
        //        if (currentPage != value)
        //        {
        //            currentPage = value;
        //            this.RaisePropertyChanged("CurrentPage");
        //        }
        //    }
        //}
        ///*总共页数*/
        //private int totalPage;
        //public int TotalPage
        //{
        //    get { return totalPage; }
        //    set
        //    {
        //        if (totalPage != value)
        //        {
        //            totalPage = value;
        //            this.RaisePropertyChanged("TotalPage");
        //        }
        //    }
        //}
        ///*当前页记录的开始条数*/
        //private int currentStart;
        //public int CurrentStart
        //{
        //    get { return currentStart; }
        //    set
        //    {
        //        if (currentStart != value)
        //        {
        //            currentStart = value;
        //            this.RaisePropertyChanged("CurrentStart");
        //        }
        //    }
        //}
        ///*当前页记录的最后条数*/
        //private int currentEnd;
        //public int CurrentEnd
        //{
        //    get { return currentEnd; }
        //    set
        //    {
        //        if (currentEnd != value)
        //        {
        //            currentEnd = value;
        //            this.RaisePropertyChanged("CurrentEnd");
        //        }
        //    }
        //}
        ///*总条数*/
        //private int totalCount;
        //public int TotalCount
        //{
        //    get { return totalCount; }
        //    set
        //    {
        //        if (totalCount != value)
        //        {
        //            totalCount = value;
        //            this.RaisePropertyChanged("TotalCount");
        //        }
        //    }
        //}
        #endregion
    }
}
