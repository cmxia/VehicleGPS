using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.Prism.ViewModel;
using Microsoft.Practices.Prism.Commands;
using VehicleGPS.Models;
using VehicleGPS.Services.MonitorCentre.AbnormalStatictic;
using System.Windows.Controls;
using System.Windows;
using VehicleGPS.ViewModels.AutoComplete;
using VehicleGPS.Models.MonitorCentre;
using System.ComponentModel;
using VehicleGPS.ViewModels.MonitorCentre.RealTimeMonitor;
using System.Threading;
using VehicleGPS.Services;
using System.Net;
using Newtonsoft.Json;
using System.Data;

namespace VehicleGPS.ViewModels.MonitorCentre.AbnormalStatictic
{
    class AbnormalStaticticMonitorViewModel : NotificationObject
    {
        public AbnormalStatictcMapService MapService { get; set; }
        private static AbnormalStaticticMonitorViewModel instance = null;
        private int PageSize { get; set; }//分页大小

        #region 构造函数
        private AbnormalStaticticMonitorViewModel(WebBrowser webMap)
        {
            /*初始化分页命令*/
            this.ComeFirstCommand = new DelegateCommand(new Action(this.ComeFirstCommandExecute));
            this.ComeLastCommand = new DelegateCommand(new Action(this.ComeLastCommandExecute));
            this.ComeNextCommand = new DelegateCommand(new Action(this.ComeNextCommandExecute));
            this.ComePrevCommand = new DelegateCommand(new Action(this.ComePrevCommandExecute));
            this.PageSize = 20;
            /*查询命令*/
            this.QueryCommand = new DelegateCommand(new Action(this.QueryCommandExecute));
            /*双击历数据时*/
            this.DoubleClickCommand = new DelegateCommand<object>(new Action<object>(this.DoubleClickCommandExecute));

            this.ExportExcelCommand = new DelegateCommand(new Action(this.ExportExcelExecute));
            /*历史轨迹数据*/
            //this.listVehicleInfo = new List<TrackBackGpsInfo>();
            //this.listVehicleInfoCurrentPage = new List<TrackBackGpsInfo>();

            //this.DataOperate = new TrackPlayDataOperate(this);//获取历史轨迹数据类
            this.MapService = new AbnormalStatictcMapService(webMap);//地图操作类
            if (ListVehicleInfo != null)
            {
                ListVehicleInfo.Clear();
            }
            /*获取选择的车辆*/
            RealTimeViewModel rtvm = RealTimeViewModel.GetInstance();
            if (rtvm.SelectedVehicle != null)
            {
                CVBasicInfo selectedBasicInfo = new CVBasicInfo();
                selectedBasicInfo.ID = rtvm.SelectedVehicle.VehicleInfo.VehicleNum;
                selectedBasicInfo.SIM = rtvm.SelectedVehicle.VehicleInfo.SIM;
                selectedBasicInfo.Name = rtvm.SelectedVehicle.VehicleInfo.VehicleId;
                //this.SelectedVehicle = selectedBasicInfo;
            }
        }
        #endregion

        #region 获得实例
        public static AbnormalStaticticMonitorViewModel GetInstance(WebBrowser webMap = null)
        {
            if (instance == null)
            {
                instance = new AbnormalStaticticMonitorViewModel(webMap);
            }
            return instance;
        }
        #endregion

        #region 查询条件

        private int days = 1;//开始时间为两天前
        public int Days
        {
            get { return days; }
            set
            {
                if (days != value)
                {
                    days = value;
                    this.RaisePropertyChanged("Days");
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
        #endregion

        #region 查询进行
        public DelegateCommand QueryCommand { get; set; }
        public void QueryCommandExecute()
        {
            this.IsBusy = true;
            BackgroundWorker worker = new BackgroundWorker();
            worker.DoWork += (o, ea) =>
            {
                GetResult();
            };
            worker.RunWorkerCompleted += (o, ea) =>
            {
                this.IsBusy = false;
                //this.MapVisibility = Visibility.Visible;
            };
            //this.MapVisibility = Visibility.Collapsed;
            worker.RunWorkerAsync();

        }

        private void GetResult()
        {
            if (ListVehicleInfo != null)
            {
                ListVehicleInfo.Clear();
            }
            List<RealTimeItemViewModel> tmp = new List<RealTimeItemViewModel>();
            foreach (CVDetailInfo model in StaticDetailInfo.GetInstance().ListVehicleDetailInfo)
            {
                if (model.VehicleGPSInfo == null || model.VehicleGPSInfo.Datetime == null)
                {
                    RealTimeItemViewModel rtItem = new RealTimeItemViewModel();
                    rtItem.VehicleInfo = model;
                    tmp.Add(rtItem);
                    continue;
                }
                DateTime now = DateTime.Now;
                DateTime last = DateTime.Parse(model.VehicleGPSInfo.Datetime);
                TimeSpan ts = now - last;
                if (ts.Days > this.Days)
                {
                    RealTimeItemViewModel rtItem = new RealTimeItemViewModel();
                    rtItem.VehicleInfo = model;
                    tmp.Add(rtItem);
                }
            }
            ListVehicleInfo = tmp;
            Thread parseAddrTH = new Thread(ParseAddress);
            parseAddrTH.Start();
        }
        private void ParseAddress()
        {
            foreach (RealTimeItemViewModel vehicle in this.ListVehicleInfo)
            {
                if (vehicle.VehicleInfo.VehicleGPSInfo != null && vehicle.VehicleInfo.VehicleGPSInfo.Latitude != null &&
                    vehicle.VehicleInfo.VehicleGPSInfo.Longitude != null && string.IsNullOrEmpty(vehicle.VehicleInfo.VehicleGPSInfo.CurLocation))
                {
                    vehicle.VehicleInfo.VehicleGPSInfo.CurLocation = (new BusinessDataServiceWEB()).ParseOneAddress(vehicle.VehicleInfo.VehicleGPSInfo.Longitude, vehicle.VehicleInfo.VehicleGPSInfo.Latitude);
                }
            }
        }
        #endregion

        public DelegateCommand ExportExcelCommand { get; set; }
        private void ExportExcelExecute()
        {
            if (this.ListVehicleInfo==null||this.ListVehicleInfo.Count==0)
            {
                MessageBox.Show("没有需要导出的数据！");
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
            dt.Columns.Add("sequence", typeof(string));
            dt.Columns.Add("vehicleId", typeof(string));
            dt.Columns.Add("InnerId", typeof(string));
            dt.Columns.Add("UnitName", typeof(string));
            dt.Columns.Add("OnlineStatus", typeof(string));
            dt.Columns.Add("Lng", typeof(string));
            dt.Columns.Add("Lat", typeof(string));
            dt.Columns.Add("Time", typeof(string));
            dt.Columns.Add("Sim", typeof(string));
            dt.Columns.Add("Location", typeof(string));
            DataRow row = dt.NewRow();
            row["sequence"] = "序号";
            row["vehicleId"] = "车牌号";
            row["InnerId"] = "内部编号";
            row["UnitName"] = "所属单位";
            row["OnlineStatus"] = "在线状态";
            row["Lng"] = "经度";
            row["Lat"] = "纬度";
            row["Time"] = "上传时间";
            row["Sim"] = "SIM卡号";
            row["Location"] = "当前位置";
            dt.Rows.Add(row);
            int i = 1;
            foreach (RealTimeItemViewModel info in this.ListVehicleInfo)
            {
                row = dt.NewRow();
                row["sequence"] = (i++).ToString();
                row["vehicleId"] = info.VehicleInfo.VehicleId;
                row["InnerId"] = info.VehicleInfo.FInnerId;
                row["UnitName"] = info.VehicleInfo.ParentUnitName;
                row["OnlineStatus"] = info.VehicleInfo.VehicleGPSInfo == null ? "" : info.VehicleInfo.VehicleGPSInfo.OnlineStates;
                row["Location"] = info.VehicleInfo.VehicleGPSInfo == null ? "" : info.VehicleInfo.VehicleGPSInfo.CurLocation;
                row["Lng"] = info.VehicleInfo.VehicleGPSInfo == null ? "" : info.VehicleInfo.VehicleGPSInfo.Longitude;
                row["Lat"] = info.VehicleInfo.VehicleGPSInfo == null ? "" : info.VehicleInfo.VehicleGPSInfo.Latitude;
                row["Time"] = info.VehicleInfo.VehicleGPSInfo == null ? "" : info.VehicleInfo.VehicleGPSInfo.Datetime;
                row["Sim"] = info.VehicleInfo.SIM;
                dt.Rows.Add(row);
            }
            ExcelHelper.ExportExcel(dt,path);
        }

        private List<RealTimeItemViewModel> listVehicleInfo = new List<RealTimeItemViewModel>();
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

        #region 双击实时GPS数据
        private int selectedInfoIndex;
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
            //TrackBackGpsInfo itemInfo = (TrackBackGpsInfo)selectedItem;
            //if (itemInfo != null)
            //{
            //    this.MapService.SetMarker(itemInfo.GpsInfo.Longitude, itemInfo.GpsInfo.Latitude,
            //        VehicleCommon.GetHtml(this.SelectedVehicle.Name, this.SelectedVehicle.SIM, itemInfo.GpsInfo.Datetime,
            //            itemInfo.GpsInfo.Speed, itemInfo.GpsInfo.Direction, itemInfo.GpsInfo.CurLocation)
            //        , VehicleCommon.GetDirectionImageUrl(itemInfo.GpsInfo.Direction, VehicleCommon.VSOnlineRun));
            //}
        }
        #endregion

        #region 分页
        /*绑定当前页*/
        private List<TrackBackGpsInfo> listVehicleInfoCurrentPage = null;
        public List<TrackBackGpsInfo> ListVehicleInfoCurrentPage
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
            if (this.TotalCount != 0)
            {
                this.ComeFirstCommandExecute();
            }
        }
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
            }
            else
            {
                this.PrevEnable = true;
            }
            if (this.CurrentPage == this.TotalPage)
            {
                this.NextEnable = false;
            }
            else
            {
                this.NextEnable = true;
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
            List<TrackBackGpsInfo> pageTmp = new List<TrackBackGpsInfo>();
            int sequence = (this.CurrentPage - 1) * this.PageSize + 1;//序号
            for (int i = CurrentStart - 1; i < this.CurrentEnd; i++)
            {
                TrackBackGpsInfo tbgi = new TrackBackGpsInfo();
                tbgi.Sequence = i + 1;
                tbgi.GpsInfo = new GPSInfo();
                tbgi.SIM = ListVehicleInfo[i].VehicleInfo.SIM;
                tbgi.VehicleId = ListVehicleInfo[i].VehicleInfo.VehicleId;
                tbgi.VehicleTypeName = ListVehicleInfo[i].VehicleInfo.VehicleType;
                tbgi.FInnerId = ListVehicleInfo[i].VehicleInfo.FInnerId;
                tbgi.CustomerName = ListVehicleInfo[i].VehicleInfo.ParentUnitName;
                tbgi.GpsInfo = this.listVehicleInfo[i].VehicleInfo.VehicleGPSInfo;
                pageTmp.Add(tbgi);
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
                List<TrackBackGpsInfo> pageTmp = new List<TrackBackGpsInfo>();
                int sequence = (this.CurrentPage - 1) * this.PageSize + 1;//序号
                for (int i = this.CurrentStart - 1; i < this.CurrentEnd; i++)
                {
                    TrackBackGpsInfo tbgi = new TrackBackGpsInfo();
                    tbgi.Sequence = i + 1;
                    tbgi.GpsInfo = new GPSInfo();
                    tbgi.SIM = ListVehicleInfo[i].VehicleInfo.SIM;
                    tbgi.VehicleId = ListVehicleInfo[i].VehicleInfo.VehicleId;
                    tbgi.VehicleTypeName = ListVehicleInfo[i].VehicleInfo.VehicleType;
                    tbgi.FInnerId = ListVehicleInfo[i].VehicleInfo.FInnerId;
                    tbgi.CustomerName = ListVehicleInfo[i].VehicleInfo.ParentUnitName;
                    tbgi.GpsInfo = this.listVehicleInfo[i].VehicleInfo.VehicleGPSInfo;
                    pageTmp.Add(tbgi);
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
                List<TrackBackGpsInfo> pageTmp = new List<TrackBackGpsInfo>();
                int sequence = (this.CurrentPage - 1) * this.PageSize + 1;//序号
                for (int i = CurrentStart - 1; i < this.CurrentEnd; i++)
                {
                    TrackBackGpsInfo tbgi = new TrackBackGpsInfo();
                    tbgi.Sequence = i + 1;
                    tbgi.GpsInfo = new GPSInfo();
                    tbgi.SIM = ListVehicleInfo[i].VehicleInfo.SIM;
                    tbgi.VehicleId = ListVehicleInfo[i].VehicleInfo.VehicleId;
                    tbgi.VehicleTypeName = ListVehicleInfo[i].VehicleInfo.VehicleType;
                    tbgi.FInnerId = ListVehicleInfo[i].VehicleInfo.FInnerId;
                    tbgi.CustomerName = ListVehicleInfo[i].VehicleInfo.ParentUnitName;
                    tbgi.GpsInfo = this.listVehicleInfo[i].VehicleInfo.VehicleGPSInfo;
                    pageTmp.Add(tbgi);
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
            List<TrackBackGpsInfo> pageTmp = new List<TrackBackGpsInfo>();
            int sequence = (this.CurrentPage - 1) * this.PageSize + 1;//序号
            for (int i = CurrentStart - 1; i < this.CurrentEnd; i++)
            {
                TrackBackGpsInfo tbgi = new TrackBackGpsInfo();
                tbgi.Sequence = i + 1;
                tbgi.GpsInfo = new GPSInfo();
                tbgi.SIM = ListVehicleInfo[i].VehicleInfo.SIM;
                tbgi.VehicleId = ListVehicleInfo[i].VehicleInfo.VehicleId;
                tbgi.VehicleTypeName = ListVehicleInfo[i].VehicleInfo.VehicleType;
                tbgi.FInnerId = ListVehicleInfo[i].VehicleInfo.FInnerId;
                tbgi.CustomerName = ListVehicleInfo[i].VehicleInfo.ParentUnitName;
                tbgi.GpsInfo = this.listVehicleInfo[i].VehicleInfo.VehicleGPSInfo;
                pageTmp.Add(tbgi);
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
    }
}
