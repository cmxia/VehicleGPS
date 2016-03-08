using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.Prism.ViewModel;
using Microsoft.Practices.Prism.Commands;
using VehicleGPS.Models;
using System.Windows.Controls;
using VehicleGPS.Services.MonitorCentre.VehicleTrack;
using System.Windows;
using VehicleGPS.ViewModels.MonitorCentre.RealTimeMonitor;

namespace VehicleGPS.ViewModels.MonitorCentre.VehicleTrack
{
    class VehicleTrackViewModel : NotificationObject
    {
        /*选择跟踪的车辆详细信息*/
        private CVDetailInfo selectedInfo;
        public CVDetailInfo SelectedInfo
        {
            get { return selectedInfo; }
            set
            {
                if (selectedInfo != value)
                {
                    selectedInfo = value;
                    this.RaisePropertyChanged("SelectedInfo");
                }
            }
        }
        public VehicleTrackDataOperate DataOperate { get; set; }
        public VehicleTrackMapService MapService { get; set; }
        public VehicleTrackViewModel(WebBrowser webMap)
        {
            /*开始跟踪/停止跟踪*/
            this.StartCommand = new DelegateCommand(new Action(this.StartCommandExecute));
            this.StopCommand = new DelegateCommand(new Action(this.StopCommandExecute));
            /*双击实时轨迹数据时*/
            this.DoubleClickCommand = new DelegateCommand<object>(new Action<object>(this.DoubleClickCommandExecute));
            /*实时轨迹数据*/
            this.listVehicleInfo = new List<GPSInfo>();
            /*传入模型是为了在获取数据后对listVehicleInfo赋值*/
            this.DataOperate = new VehicleTrackDataOperate(this);//获取历史轨迹数据类
            this.MapService = new VehicleTrackMapService(webMap);//地图操作类
            this.getSelectedVehicle();
        }
        public VehicleTrackViewModel(WebBrowser webMap,string vehicleNum)
        {
            /*开始跟踪/停止跟踪*/
            this.StartCommand = new DelegateCommand(new Action(this.StartCommandExecute));
            this.StopCommand = new DelegateCommand(new Action(this.StopCommandExecute));
            /*双击实时轨迹数据时*/
            this.DoubleClickCommand = new DelegateCommand<object>(new Action<object>(this.DoubleClickCommandExecute));
            /*实时轨迹数据*/
            this.listVehicleInfo = new List<GPSInfo>();
            /*传入模型是为了在获取数据后对listVehicleInfo赋值*/
            this.DataOperate = new VehicleTrackDataOperate(this);//获取历史轨迹数据类
            this.MapService = new VehicleTrackMapService(webMap);//地图操作类

            foreach (CVDetailInfo item in StaticDetailInfo.GetInstance().ListVehicleDetailInfo)
            {
                if (item.VehicleId == vehicleNum)
                {
                    this.SelectedInfo = item;
                    break;
                }
            }
        }
        private void getSelectedVehicle()
        {
            CVDetailInfo vehicledetail = new CVDetailInfo();
            RealTimeTreeNodeViewModel RootNode = RealTimeTreeViewModel.GetInstance().RootNode;
            GetFocusNode(RootNode);
        }
        /// <summary>
        /// 获得焦点的车辆节点
        /// </summary>
        private void GetFocusNode(RealTimeTreeNodeViewModel root)
        {
            if (root.listChildNodes == null || root.listChildNodes.Count == 0)
            {
                return;
            }
            foreach (RealTimeTreeNodeViewModel node in root.listChildNodes)
            {
                if (node.nodeInfo.SIM != null && node.isFocus == true)
                {
                    foreach (CVDetailInfo item in StaticDetailInfo.GetInstance().ListVehicleDetailInfo)
                    {
                        if (item.VehicleNum==node.NodeInfo.ID)
                        {
                            this.SelectedInfo = item;
                            break;
                        }
                    }
                }
                else
                {
                    GetFocusNode(node);
                }
            }

        }
        private List<GPSInfo> listVehicleInfo;
        public List<GPSInfo> ListVehicleInfo
        {
            get { return listVehicleInfo; }
            set
            {
                if (listVehicleInfo != value)
                {
                    listVehicleInfo = value;
                    this.RaisePropertyChanged("ListVehicleInfo");
                    if (listVehicleInfo != null && listVehicleInfo.Count != 0)
                    {
                        this.MapService.WebMap.Dispatcher.Invoke(new Action(this.SetMarkerToMap));
                    }
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
            GPSInfo itemInfo = (GPSInfo)selectedItem;
            if (itemInfo != null)
            {
                this.MapService.FocusMarker(itemInfo.Longitude, itemInfo.Latitude);
            }
        }
        #endregion
        #region 开始跟踪/停止跟踪
        public DelegateCommand StartCommand { get; set; }
        public DelegateCommand StopCommand { get; set; }
        private void StartCommandExecute()
        {
            if (this.SelectedInfo == null)
            {
                MessageBox.Show("请选择要跟踪的车辆", "车辆跟踪", MessageBoxButton.OKCancel);
                return;
            }
            //if (StaticTreeState.VehicleGPSInfo != LoadingState.LOADCOMPLETE)
            //{
            //    MessageBox.Show("详细数据正在加载，请稍后", "车辆跟踪", MessageBoxButton.OKCancel);
            //    return;
            //}
            if (!this.IsVehicleOnline())
            {
                MessageBox.Show("跟踪的车辆不在线，请重新选择", "车辆跟踪", MessageBoxButton.OKCancel);
                return;
            }
            //this.StartEnable = false;
            //this.StopEnable = true;


            GetFirstGpsInfo();
            //this.DataOperate.InitDispatchTimer();
            //this.DataOperate.TestPubSubThread.Start();
            this.StartEnable = false;
            this.StopEnable = true;
            this.IsBusy = false;
            this.DataOperate.VehicleTrackInfoStart();
        }
        private void StopCommandExecute()
        {
            this.StartEnable = true;
            this.StopEnable = false;
            this.DataOperate.VehicleTrackInfoStop();
            //this.DataOperate.StopDispatchTimer();
            this.MapService.RemoveAllOverlay();
            this.ListVehicleInfo.Clear();
            this.ListVehicleInfo = null;
        }
        /*按钮是否可用*/
        private bool startEnable = true;
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
        private bool stopEnable = false;
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
        private void GetFirstGpsInfo()
        {
            if (StaticDetailInfo.GetInstance().ListVehicleDetailInfo != null)
                foreach (CVDetailInfo info in StaticDetailInfo.GetInstance().ListVehicleDetailInfo)
                {
                    if (info.VehicleId == this.SelectedInfo.VehicleId)
                    {
                        List<GPSInfo> tmpList = new List<GPSInfo>();
                        info.VehicleGPSInfo.Sequence = 1;
                        tmpList.Add(info.VehicleGPSInfo);
                        this.ListVehicleInfo = tmpList;
                        break;
                    }
                }
        }
        private bool IsVehicleOnline()
        {
            if (StaticTreeState.VehicleGPSInfo == LoadingState.LOADCOMPLETE)
            {
                foreach (CVDetailInfo info in StaticDetailInfo.GetInstance().ListVehicleDetailInfo)
                {
                    if (info.SIM == this.selectedInfo.SIM)
                    {
                        if (info.VehicleGPSInfo != null && info.VehicleGPSInfo.OnlineStates != VehicleCommon.VSOnlineOff)
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                }
            }
            return false;
        }
        #endregion

        #region 等待
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
        /*添加跟踪车辆标注点*/
        private void SetMarkerToMap()
        {
            if (this.listVehicleInfo == null || this.listVehicleInfo.Count == 0)
                return;
            GPSInfo lastInfo = this.listVehicleInfo[this.listVehicleInfo.Count - 1];
            this.SelectedInfoIndex = this.listVehicleInfo.Count - 1;
            string contStr = VehicleCommon.GetHtml(this.SelectedInfo.VehicleId, this.SelectedInfo.VehicleGPSInfo.OnlineStates, lastInfo.Datetime,
                lastInfo.Speed, lastInfo.Direction, lastInfo.CurLocation);
            string iconStr = VehicleCommon.GetDirectionImageUrl(lastInfo.Direction, lastInfo.OnlineStates);
            this.MapService.SetMarker(lastInfo.Longitude, lastInfo.Latitude, contStr, iconStr);
        }
    }
}
