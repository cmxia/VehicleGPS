//RealTimeTreeViewModel
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.Prism.ViewModel;
using Microsoft.Practices.Prism.Commands;
using VehicleGPS.Services.MonitorCentre;
using System.Windows;
using VehicleGPS.Models;
using System.Windows.Media;
using VehicleGPS.ViewModels.AutoComplete;
using System.Windows.Controls;
using VehicleGPS.Views.Control.MonitorCentre.Instruction;
using VehicleGPS.Views.Control.MonitorCentre.Instruction.HQ;

namespace VehicleGPS.ViewModels.MonitorCentre.RealTimeMonitor
{
    class RealTimeTreeViewModel : NotificationObject
    {
        private static RealTimeTreeViewModel instance = null;
        private RealTimeTreeViewModel()
        {
            /***********以下右键菜单***************/
            this.VehicleCommand = new DelegateCommand<object>(this.VehicleCommandExecute);
            this.VehicleTrackCommand = new DelegateCommand<object>(this.VehicleTrackCommandExecute);
            this.TrackPlayCommand = new DelegateCommand<object>(this.TrackPlayCommandExecute);
            this.StationLocateCommand = new DelegateCommand<object>(this.StationLocateCommandExecute);
            //指令
            //this.ServerSiteCommand = new DelegateCommand<string>(this.ServerSiteCommandExecute);
            this.CommonCommand = new DelegateCommand<string>(this.CommonCommandExecute);
            this.IPPortCommand = new DelegateCommand<string>(this.IPPortCommandExecute);
            this.SendTextCommand = new DelegateCommand<object>(this.SendTextCommandExecute);
            this.PosStrategyCommand = new DelegateCommand<object>(this.PosStrategyCommandExecute);
            this.VehicleColCommand = new DelegateCommand<object>(this.VehicleColCommandExecute);
            this.CommonRadioCommand = new DelegateCommand<string>(this.CommonRadioCommandExecute);
            this.AlarmCommand = new DelegateCommand<string>(this.AlarmCommandExecute);
            this.EventSetCommand = new DelegateCommand<object>(this.EventSetCommandExecute);
            this.PhoneSetCommand = new DelegateCommand<object>(this.PhoneSetCommandExecute);
            this.DelRegionCommand = new DelegateCommand<object>(this.DelRegionCommandExecute);
            this.ShotNowCommand = new DelegateCommand<object>(this.ShotNowCommandExecute);
            //地图指令
            this.SettingLineCommand = new DelegateCommand(this.SettingLineCommandExecute);
            this.SettingPolyCommand = new DelegateCommand(this.SettingPolyCommandExecute);
            this.SettingRectCommand = new DelegateCommand(this.SettingRectCommandExecute);
            this.SettingCircleCommand = new DelegateCommand(this.SettingCircleCommandExecute);
            //华强指令
            this.HQ_NumberCommand = new DelegateCommand<string>(this.HQ_NumberCommandExecute);
            this.HQ_Setting_C_I_Command = new DelegateCommand<string>(this.HQ_Setting_C_I_CommandExecute);
            this.HQ_WorkingStateCommand = new DelegateCommand<string>(this.HQ_WorkingStateCommandExecute);
            this.HQ_MonitorCommand = new DelegateCommand<object>(this.HQ_MonitorCommandExecute);
            this.HQ_NoneDataCommand = new DelegateCommand<string>(this.HQ_NoneDataCommandExecute);
            this.HQ_GPRSCommand = new DelegateCommand<object>(this.HQ_GPRSCommandExecute);
            this.HQ_VehicleMileageCommand = new DelegateCommand<object>(this.HQ_VehicleMileageCommandExecute);
            this.HQ_CompressionCommand = new DelegateCommand<object>(this.HQ_CompressionCommandExecute);
            this.HQ_DistanceBackCommand = new DelegateCommand<object>(this.HQ_DistanceBackCommandExecute);
            this.HQ_OneCallCommand = new DelegateCommand<object>(this.HQ_OneCallCommandExecute);
            this.HQ_FixSpeedCommand = new DelegateCommand<string>(this.HQ_FixSpeedCommandExecute);
            this.HQ_StandByCommand = new DelegateCommand<object>(this.HQ_StandByCommandExecute);
            this.HQ_EmptyBackCommand = new DelegateCommand<object>(this.HQ_EmptyBackCommandExecute);
            this.HQ_AnyInformationCommand = new DelegateCommand<object>(this.HQ_AnyInformationCommandExecute);
            this.HQ_Setting_ScopeCommand = new DelegateCommand<string>(this.HQ_Setting_ScopeCommandExecute);
            this.HQ_Setting_LogoCommand = new DelegateCommand<string>(this.HQ_Setting_LogoCommandExecute);
            this.HQ_Setting_MessageCommand = new DelegateCommand<string>(this.HQ_Setting_MessageCommandExecute);
            this.HQ_Setting_CallCommand = new DelegateCommand<string>(this.HQ_Setting_CallCommandExecute);
            this.HQ_Setting_PoloygnCommand = new DelegateCommand<string>(this.HQ_Setting_PoloygnCommandExecute);
            this.HQ_IPandPortCommand = new DelegateCommand<string>(this.HQ_IPandPortCommandExecute);
            this.HQ_OfflineGPRSCommand = new DelegateCommand<string>(this.HQ_OfflineGPRSCommandExecute);
            this.HQ_EmegentPicCommand = new DelegateCommand<string>(this.HQ_EmegentPicCommandExecute);
            this.HQ_UserCodeCommand = new DelegateCommand<object>(this.HQ_UserCodeCommandExecute);
            this.HQ_SettingParaCommand = new DelegateCommand<object>(this.HQ_SettingParaCommandExecute);
            RightOperate = new RightClickOparate();
            /***********以上右键菜单***************/
            this.DoubleClickCommand = new DelegateCommand<object>(new Action<object>(this.DoubleClickCommandExecute));
            this.RefreshCommand = new DelegateCommand(new Action(RefreshCommandExecute));
            this.AutoCompleteSelectedCommand = new DelegateCommand<object>(new Action<object>(this.AutoCompleteSelectedCommandExecute));
            TreeOperate = new RealTimeTreeOperate();
            TreeOperate.ReadTree();
            StaticTreeState.RealTimeTreeContruct = true;
            InitInstrutionRight();//初始化树形指令权限
        }

        #region 获得单例
        /// <summary>
        /// 获得树形结构操作的单例
        /// </summary>
        /// <returns></returns>
        public static RealTimeTreeViewModel GetInstance()
        {
            if (instance == null)
            {
                instance = new RealTimeTreeViewModel();
            }
            return instance;
        }
        #endregion

        #region 根节点
        /// <summary>
        /// 树的根节点 使用节点的父节点
        /// </summary>
        public RealTimeTreeNodeViewModel rootNode = null;
        public RealTimeTreeNodeViewModel RootNode
        {
            get { return rootNode; }
            set
            {
                if (rootNode != value)
                {
                    rootNode = value;
                    this.RaisePropertyChanged("RootNode");
                    RefreshState(true);//启用刷新按钮
                }
            }
        }
        #endregion

        #region 是否显示内部编号
        /// <summary>
        /// 是否显示内部编号
        /// </summary>
        public bool? innerIDVisibleSelected = true;
        public bool? InnerIDVisibleSelected
        {
            get { return innerIDVisibleSelected; }
            set
            {
                if (innerIDVisibleSelected != value)
                {
                    innerIDVisibleSelected = value;
                    Visibility vb;
                    if (value == true)
                    {
                        vb = Visibility.Visible;
                    }
                    else
                    {
                        vb = Visibility.Collapsed;
                    }
                    this.SetInnerIDState(vb, this.RootNode);
                    this.RaisePropertyChanged("InnerIDVisibleSelected");
                }
            }
        }
        #endregion

        #region 是否显示车牌号
        /// <summary>
        /// 是否显示车牌号
        /// </summary>
        public bool? nameVisibleSelected = true;
        public bool? NameVisibleSelected
        {
            get { return nameVisibleSelected; }
            set
            {
                if (nameVisibleSelected != value)
                {
                    nameVisibleSelected = value;
                    Visibility vb;
                    if (value == true)
                    {
                        vb = Visibility.Visible;
                    }
                    else
                    {
                        vb = Visibility.Collapsed;
                    }
                    this.SetNameState(vb, this.RootNode);
                    this.RaisePropertyChanged("NameVisibleSelected");
                }
            }
        }
        #endregion

        #region 隐藏或者显示内部编号
        /// <summary>
        /// 隐藏或者显示内部编号
        /// </summary>
        /// <param name="vb"></param>
        /// <param name="rootNode"></param>
        private void SetInnerIDState(Visibility vb, RealTimeTreeNodeViewModel rootNode)
        {
            if (rootNode.listChildNodes != null)
            {
                foreach (RealTimeTreeNodeViewModel vtnvm in rootNode.listChildNodes)
                {
                    SetInnerIDState(vb, vtnvm);
                }
            }
            else
            {
                rootNode.InnerIDVisible = vb;
            }
        }
        #endregion

        #region 隐藏或者显示车辆名称
        /// <summary>
        /// 隐藏或者显示车辆名称
        /// </summary>
        /// <param name="vb"></param>
        /// <param name="rootNode"></param>
        private void SetNameState(Visibility vb, RealTimeTreeNodeViewModel rootNode)
        {
            if (rootNode.listChildNodes != null)
            {
                foreach (RealTimeTreeNodeViewModel vtnvm in rootNode.listChildNodes)
                {
                    SetNameState(vb, vtnvm);
                }
            }
            else
            {
                rootNode.NameVisible = vb;
            }
        }
        #endregion

        public RealTimeTreeOperate TreeOperate { set; get; }//树的操作类
        public DelegateCommand RefreshCommand { get; set; }//刷新树形
        private void RefreshCommandExecute()
        {
            this.RefreshState(false);//禁用刷新按钮
            //StaticTreeState.BasicTypeInfo = LoadingState.NOLOADING;
            StaticTreeState.ClientBasicInfo = LoadingState.NOLOADING;
            //StaticTreeState.VehicleBasicInfo = LoadingState.NOLOADING;
            StaticTreeState.VehicleAllBasicInfo = LoadingState.NOLOADING;
            StaticTreeState.VehicleGPSInfo = LoadingState.NOLOADING;
            /*删除实时数据*/
            RealTimeViewModel realTimeVM = RealTimeViewModel.GetInstance();
            if (realTimeVM.ListVehicleInfoCurrentPage != null)
            {
                realTimeVM.ListVehicleInfoCurrentPage.Clear();
            }
            realTimeVM.ListVehicleInfoCurrentPage = null;

            TreeOperate.RefreshTree();
        }

        #region 获取车辆焦点，并在地图上显示
        public void FocusAndShowInMap(string SIM)
        {
            isNodeExpand(this.RootNode, SIM);
            this.RootNode.IsExpand = true;
            this.Mapfocus();
            this.ExpandAndFocus2(this.RootNode, SIM);
        }
        public bool isNodeExpand(RealTimeTreeNodeViewModel parentnode, string SIM)
        {
            if (parentnode.NodeInfo.ID.Contains("VEHI"))
            {
                if (parentnode.NodeInfo.SIM.Equals(SIM))
                {
                    parentnode.IsFocus = true;
                    parentnode.IsSelected = true;
                    this.SelectedNode = parentnode;
                    return true;
                }
            }
            else
            {
                if (parentnode.ListChildNodes != null && parentnode.ListChildNodes.Count > 0)
                {
                    foreach (RealTimeTreeNodeViewModel node in parentnode.ListChildNodes)
                    {
                        if (isNodeExpand(node, SIM))
                        {
                            parentnode.IsExpand = true;
                        }
                    }
                }
            }
            return false;
        }
        #endregion

        #region 加载树形信息失败
        /// <summary>
        /// 加载树形信息失败
        /// </summary>
        public void LoadTreeFail()
        {
            this.RefreshState(true);//启用刷新按钮
            string errorMsg = "";
            if (StaticTreeState.ClientBasicInfo == LoadingState.LOADDINGFAIL)
            {
                errorMsg += "(用户关键信息)";
            }
            if (StaticTreeState.VehicleBasicInfo == LoadingState.LOADDINGFAIL)
            {
                errorMsg += "(车辆关键信息)";
            }
            if (StaticTreeState.VehicleAllBasicInfo == LoadingState.LOADDINGFAIL)
            {
                errorMsg += "(车辆基础信息)";
            }
            MessageBox.Show("加载" + errorMsg + "失败，请刷新重试", "数据加载失败", MessageBoxButton.OKCancel);
        }
        #endregion

        #region 双击事件
        /*选择的节点*/
        public RealTimeTreeNodeViewModel SelectedNode { get; set; }
        public DelegateCommand<object> DoubleClickCommand { get; set; }
        private void DoubleClickCommandExecute(object tv)
        {
            RealTimeTreeNodeViewModel selectedNode = (RealTimeTreeNodeViewModel)((TreeView)tv).SelectedItem;
            if (selectedNode == null || selectedNode.nodeInfo == null)
            {
                return;
            }

            if (selectedNode.nodeInfo.SIM != "0")//车辆节点
            {
                this.SelectedNode = selectedNode;
                this.Mapfocus();
            }
        }

        private void Mapfocus()
        {

            CVDetailInfo selectedDetial = null;
            foreach (CVDetailInfo info in StaticDetailInfo.GetInstance().ListVehicleDetailInfo)
            {
                if (this.SelectedNode.nodeInfo.ID == info.VehicleNum)
                {
                    selectedDetial = info;
                    break;
                }
            }
            if (selectedDetial != null)
            {
                if (selectedDetial.VehicleGPSInfo == null)
                {
                    MessageBox.Show("没有该车辆的实时gps信息!", "GPS信息", MessageBoxButton.OKCancel);
                    return;
                }
                if (selectedDetial.VehicleGPSInfo.Latitude == ""
                    || selectedDetial.VehicleGPSInfo.Longitude == "")
                {
                    MessageBox.Show("没有该车辆的实时地理位置信息!", "GPS信息", MessageBoxButton.OKCancel);
                    return;
                }
                if (this.SelectedNode.IsSelected != true)
                {
                    this.SelectedNode.IsSelected = true;
                }
                RealTimeViewModel realTimeInstance = RealTimeViewModel.GetInstance();
                /*在DataGrid中选中该项*/
                int count = 0;
                foreach (RealTimeItemViewModel item in realTimeInstance.ListVehicleInfo)
                {
                    ++count;
                    if (item.VehicleInfo.VehicleNum == this.SelectedNode.nodeInfo.ID)
                    {
                        /*获取所在页*/
                        if (realTimeInstance.CurrentPage != count / realTimeInstance.PageSize + 1)
                        {
                            realTimeInstance.CurrentPage = count / realTimeInstance.PageSize + 1;
                            realTimeInstance.CurrentStart = (realTimeInstance.CurrentPage - 1) * realTimeInstance.PageSize + 1;
                            realTimeInstance.CurrentEnd = realTimeInstance.TotalCount < realTimeInstance.CurrentStart + realTimeInstance.PageSize - 1 ? realTimeInstance.TotalCount : realTimeInstance.CurrentStart + realTimeInstance.PageSize - 1;
                            realTimeInstance.RefreshCurrentPage();
                        }
                        //realTimeInstance.DoubleClickCommandExecute();
                        realTimeInstance.SelectedVehicle = item;

                        break;
                    }
                }

                realTimeInstance.MapService.FocusMarker(Convert.ToDouble(selectedDetial.VehicleGPSInfo.Longitude), Convert.ToDouble(selectedDetial.VehicleGPSInfo.Latitude));
            }
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
            focusOne = null;
            FocusItem(this.RootNode);
            if (focusOne != null)//车辆节点
            {
                //MessageBox.Show(selectedNode.nodeInfo.Name);
                //MessageBox.Show(focusOne.nodeInfo.Name);
                RightOperate.getVehicleBaseInfo(focusOne.nodeInfo.SIM);
                Window win = new VehicleGPS.Views.Control.MonitorCentre.Instruction.VehicleInfo();
                win.Show();
            }
            else
            {
                MessageBox.Show("请选择车辆结点");
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
            focusOne = null;
            FocusItem(this.RootNode);
            if (focusOne != null)//车辆节点
            {
                RightOperate.getVehicleBaseInfo(focusOne.nodeInfo.SIM);
                Window win = new VehicleGPS.Views.Control.MonitorCentre.VehicleTrack.VehicleTrack();
                win.Show();
            }
            else
            {
                MessageBox.Show("请选择车辆结点");
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
            focusOne = null;
            FocusItem(this.RootNode);
            if (focusOne != null)//车辆节点
            {

                Window win = new VehicleGPS.Views.Control.MonitorCentre.TrackPlayBack.TrackPlayBack();
                win.Show();
            }
            else
            {
                MessageBox.Show("请选择车辆结点");
            }
        }

        /// <summary>
        /// 基站定位
        /// </summary>
        public DelegateCommand<object> StationLocateCommand { get; set; }
        /// <summary>
        /// 基站定位
        /// </summary>
        /// <param name="e">无用</param>
        private void StationLocateCommandExecute(object e)
        {
            focusStation = null;  //车站焦点
            focusOne = null;   //车辆焦点
            FocusItem(this.RootNode);  //获取车辆焦点      
            if (focusOne != null)
            {
                string ins = "{\"simId\":\"" + focusOne.nodeInfo.SIM + "\"}";
                if (zmq.zmqPackHelper.StationLocate_zmq(focusOne.nodeInfo.SIM, ins))
                {
                    MessageBox.Show("发送成功");
                }
                else
                {
                    MessageBox.Show(focusOne.nodeInfo.InnerID + "发送失败");
                }
            }
            else //车辆焦点为空则进行车站焦点判断
            {
                try
                {
                    int count = 0;
                    bool flag = false;
                    FocusStation(this.RootNode); // 获取车站焦点
                    foreach (RealTimeTreeNodeViewModel node in focusStation.listChildNodes) //遍历车站焦点的子结点
                    {
                        if (node.nodeInfo.SIM != null) //SIM卡号不为空则结点为车辆结点
                        {
                            string ins = "{\"simId\":\"" + node.nodeInfo.SIM + "\"}";
                            if (zmq.zmqPackHelper.StationLocate_zmq(node.nodeInfo.SIM, ins)) //发送消息
                            {
                                count++; //计数发送成功的车辆数
                            }
                            else
                            {
                                MessageBox.Show(node.nodeInfo.InnerID + "发送失败");
                            }
                        }
                        else
                        {
                            flag = true; //若子结点SIM卡号为空，该结点下还有车站结点
                        }
                    }
                    if (count > 0)
                    {
                        MessageBox.Show("总共" + count + "辆车发送消息成功");
                    }
                    else if (flag == true && count == 0)//车站结点下没有直属车辆结点
                    {
                        MessageBox.Show("该站点下没有直属车辆");
                    }
                }
                catch (NullReferenceException ex)
                {
                    MessageBox.Show("该站点下没有直属车辆");
                }
            }
        }
        public RealTimeTreeNodeViewModel focusStation = null;
        /// <summary>
        /// 获得焦点的车站点
        /// </summary>
        private void FocusStation(RealTimeTreeNodeViewModel root)
        {
            if (root.listChildNodes == null || root.listChildNodes.Count == 0)
            {
                return;
            }
            foreach (RealTimeTreeNodeViewModel node in root.listChildNodes)
            {
                if (focusStation == null)
                {
                    if (node.nodeInfo.SIM == null && node.isFocus == true)
                    {
                        focusStation = node;
                        break;
                    }
                    else
                    {
                        FocusStation(node);
                    }
                }
            }

        }


        /// <summary>
        /// 指令下发-文本信息下发
        /// </summary>
        /// <returns></returns>
        public DelegateCommand<object> SendTextCommand { get; set; }
        private void SendTextCommandExecute(object e)
        {
            focusOne = null;
            FocusItem(this.RootNode);
            if (focusOne != null)//车辆节点
            {
                string onlinestatus = focusOne.ImageTip;
                if (onlinestatus.Equals("离线"))
                {
                    MessageBox.Show("车辆不在线，无法发送指令！");
                }
                else
                {
                    RightOperate.getVehicleBaseInfo(focusOne.nodeInfo.SIM);
                    SendText win = new VehicleGPS.Views.Control.MonitorCentre.Instruction.SendText();
                    if (win.isDisplay == false)
                    {
                        return;
                    }
                    else
                    {
                        win.Show();
                    }
                }
            }
            else
            {
                MessageBox.Show("请选择车辆结点");
            }
        }

        ///// <summary>
        ///// 指令下发-服务器设置
        ///// </summary>
        ///// <returns></returns>
        //public DelegateCommand<string> ServerSiteCommand { get; set; }
        //private void ServerSiteCommandExecute(string e)
        //{
        //    focusOne = null;
        //    FocusItem(this.RootNode);
        //    if (focusOne != null)//车辆节点
        //    {
        //        RightOperate.getVehicleBaseInfo(focusOne.nodeInfo.SIM);
        //        ServerSite win = new VehicleGPS.Views.Control.MonitorCentre.Instruction.ServerSite(e);
        //        if (win.isDisplay == false)
        //        {
        //            return;
        //        }
        //        else
        //        {
        //            win.Show();
        //        }
        //    }
        //    else
        //    {
        //        MessageBox.Show("请选择车辆结点");
        //    }
        //}

        /// <summary>
        /// 指令下发-通用
        /// </summary>
        /// <returns></returns>
        public DelegateCommand<string> CommonCommand { get; set; }
        private void CommonCommandExecute(string e)
        {
            focusOne = null;
            FocusItem(this.RootNode);
            if (focusOne != null)//车辆节点
            {
                string onlinestatus = focusOne.ImageTip;
                if (onlinestatus.Equals("离线"))
                {
                    MessageBox.Show("车辆不在线，无法发送指令！");
                }
                else
                {
                    RightOperate.getVehicleBaseInfo(focusOne.nodeInfo.SIM);
                    Common win = new VehicleGPS.Views.Control.MonitorCentre.Instruction.Common(e);
                    if (win.isDisplay == false)
                    {
                        return;
                    }
                    else
                    {
                        win.Show();
                    }
                }
            }
            else
            {
                MessageBox.Show("请选择车辆结点");
            }
        }
        //IP和端口号
        public DelegateCommand<string> IPPortCommand { get; set; }
        private void IPPortCommandExecute(string e)
        {
            focusOne = null;
            FocusItem(this.RootNode);
            if (focusOne != null)//车辆节点
            {
                string onlinestatus = focusOne.ImageTip;
                if (onlinestatus.Equals("离线"))
                {
                    MessageBox.Show("车辆不在线，无法发送指令！");
                }
                else
                {
                    RightOperate.getVehicleBaseInfo(focusOne.nodeInfo.SIM);
                    IPPort win = new VehicleGPS.Views.Control.MonitorCentre.Instruction.IPPort(e);
                    if (win.isDisplay == false)
                    {
                        return;
                    }
                    else
                    {
                        win.Show();
                    }
                }
            }
            else
            {
                MessageBox.Show("请选择车辆结点");
            }
        }

        /// <summary>
        /// 指令下发-车牌颜色
        /// </summary>
        /// <returns></returns>
        public DelegateCommand<object> VehicleColCommand { get; set; }
        private void VehicleColCommandExecute(object e)
        {
            focusOne = null;
            FocusItem(this.RootNode);
            if (focusOne != null)//车辆节点
            {
                string onlinestatus = focusOne.ImageTip;
                if (onlinestatus.Equals("离线"))
                {
                    MessageBox.Show("车辆不在线，无法发送指令！");
                }
                else
                {
                    RightOperate.getVehicleBaseInfo(focusOne.nodeInfo.SIM);
                    VehicleCol win = new VehicleGPS.Views.Control.MonitorCentre.Instruction.VehicleCol();
                    if (win.isDisplay == false)
                    {
                        return;
                    }
                    else
                    {
                        win.Show();
                    }
                }
            }
            else
            {
                MessageBox.Show("请选择车辆结点");
            }
        }

        /// <summary>
        /// 指令下发-位置汇报策略
        /// </summary>
        /// <returns></returns>
        public DelegateCommand<object> PosStrategyCommand { get; set; }
        private void PosStrategyCommandExecute(object e)
        {
            focusOne = null;
            FocusItem(this.RootNode);
            if (focusOne != null)//车辆节点
            {
                string onlinestatus = focusOne.ImageTip;
                if (onlinestatus.Equals("离线"))
                {
                    MessageBox.Show("车辆不在线，无法发送指令！");
                }
                else
                {
                    RightOperate.getVehicleBaseInfo(focusOne.nodeInfo.SIM);
                    PosStrategy win = new VehicleGPS.Views.Control.MonitorCentre.Instruction.PosStrategy();
                    if (win.isDisplay == false)
                    {
                        return;
                    }
                    else
                    {
                        win.Show();
                    }
                }
            }
            else
            {
                MessageBox.Show("请选择车辆结点");
            }
        }

        /// <summary>
        /// 指令下发-通用radio窗口（两个选项）
        /// </summary>
        /// <returns></returns>
        public DelegateCommand<string> CommonRadioCommand { get; set; }
        private void CommonRadioCommandExecute(string e)
        {
            focusOne = null;
            FocusItem(this.RootNode);
            if (focusOne != null)//车辆节点
            {
                string onlinestatus = focusOne.ImageTip;
                if (onlinestatus.Equals("离线"))
                {
                    MessageBox.Show("车辆不在线，无法发送指令！");
                }
                else
                {
                    RightOperate.getVehicleBaseInfo(focusOne.nodeInfo.SIM);
                    CommonRadio win = new VehicleGPS.Views.Control.MonitorCentre.Instruction.CommonRadio(e);
                    if (win.isDisplay == false)
                    {
                        return;
                    }
                    else
                    {
                        win.Show();
                    }
                }
            }
            else
            {
                MessageBox.Show("请选择车辆结点");
            }
        }

        /// <summary>
        /// 指令下发-通用报警窗口（一个checkbox）
        /// </summary>
        /// <returns></returns>
        public DelegateCommand<string> AlarmCommand { get; set; }
        private void AlarmCommandExecute(string e)
        {
            focusOne = null;
            FocusItem(this.RootNode);
            if (focusOne != null)//车辆节点
            {
                string onlinestatus = focusOne.ImageTip;
                if (onlinestatus.Equals("离线"))
                {
                    MessageBox.Show("车辆不在线，无法发送指令！");
                }
                else
                {
                    RightOperate.getVehicleBaseInfo(focusOne.nodeInfo.SIM);
                    Alarm win = new VehicleGPS.Views.Control.MonitorCentre.Instruction.Alarm(e);
                    if (win.isDisplay == false)
                    {
                        return;
                    }
                    else
                    {
                        win.Show();
                    }
                }
            }
            else
            {
                MessageBox.Show("请选择车辆结点");
            }
        }
        /// <summary>
        /// 指令下发-摄像头立即拍摄指令
        /// </summary>
        /// <returns></returns>
        public DelegateCommand<object> ShotNowCommand { get; set; }
        private void ShotNowCommandExecute(object e)
        {
            focusOne = null;
            FocusItem(this.rootNode);
            if (focusOne != null)//车辆节点
            {
                string onlinestatus = focusOne.ImageTip;
                if (onlinestatus.Equals("离线"))
                {
                    MessageBox.Show("车辆不在线，无法发送指令！");
                }
                else
                {
                    RightOperate.getVehicleBaseInfo(focusOne.nodeInfo.SIM);
                    ShotNow win = new VehicleGPS.Views.Control.MonitorCentre.Instruction.ShotNow();
                    if (win.isDisplay == false)
                    {
                        return;
                    }
                    else
                    {
                        win.Show();
                    }
                }
            }
            else
            {
                MessageBox.Show("请选择车辆结点");
            }
        }
        #region 地图指令

        public DelegateCommand SettingRectCommand { get; set; }//设置矩形区域
        public DelegateCommand SettingPolyCommand { get; set; }//设置多边形区域
        public DelegateCommand SettingLineCommand { get; set; }//设置线路
        public DelegateCommand SettingCircleCommand { get; set; }//设置圆形区域
        //设置矩形区域实现
        private void SettingRectCommandExecute()
        {
            focusOne = null;
            FocusItem(this.RootNode);
            if (focusOne != null)//车辆节点
            {
                string onlinestatus = focusOne.ImageTip;
                if (onlinestatus.Equals("离线"))
                {
                    MessageBox.Show("车辆不在线，无法发送指令！");
                }
                else
                {
                    RightOperate.getVehicleBaseInfo(focusOne.nodeInfo.SIM);
                    SetCirLineRectPoly win = new SetCirLineRectPoly("rect");
                    if (win.isDisplay == false)
                    {
                        return;
                    }
                    else
                    {
                        win.Show();
                    }
                }
            }
            else
            {
                MessageBox.Show("请选择车辆结点");
            }
        }
        //设置多边形区域实现
        private void SettingPolyCommandExecute()
        {
            focusOne = null;
            FocusItem(this.RootNode);
            if (focusOne != null)//车辆节点
            {
                string onlinestatus = focusOne.ImageTip;
                if (onlinestatus.Equals("离线"))
                {
                    MessageBox.Show("车辆不在线，无法发送指令！");
                }
                else
                {
                    RightOperate.getVehicleBaseInfo(focusOne.nodeInfo.SIM);
                    SetCirLineRectPoly win = new SetCirLineRectPoly("poly");
                    if (win.isDisplay == false)
                    {
                        return;
                    }
                    else
                    {
                        win.Show();
                    }
                }
            }
            else
            {
                MessageBox.Show("请选择车辆结点");
            }
        }
        //设置线路实现
        private void SettingLineCommandExecute()
        {
            focusOne = null;
            FocusItem(this.RootNode);
            if (focusOne != null)//车辆节点
            {
                string onlinestatus = focusOne.ImageTip;
                if (onlinestatus.Equals("离线"))
                {
                    MessageBox.Show("车辆不在线，无法发送指令！");
                }
                else
                {
                    RightOperate.getVehicleBaseInfo(focusOne.nodeInfo.SIM);
                    SetCirLineRectPoly win = new SetCirLineRectPoly("line");
                    if (win.isDisplay == false)
                    {
                        return;
                    }
                    else
                    {
                        win.Show();
                    }
                }
            }
            else
            {
                MessageBox.Show("请选择车辆结点");
            }
        }
        //设置圆形区域实现
        private void SettingCircleCommandExecute()
        {
            focusOne = null;
            FocusItem(this.RootNode);
            if (focusOne != null)//车辆节点
            {
                string onlinestatus = focusOne.ImageTip;
                if (onlinestatus.Equals("离线"))
                {
                    MessageBox.Show("车辆不在线，无法发送指令！");
                }
                else
                {
                    RightOperate.getVehicleBaseInfo(focusOne.nodeInfo.SIM);
                    SetCirLineRectPoly win = new SetCirLineRectPoly("circle");
                    if (win.isDisplay == false)
                    {
                        return;
                    }
                    else
                    {
                        win.Show();
                    }
                }
            }
            else
            {
                MessageBox.Show("请选择车辆结点");
            }
        }
        //事件设置
        public DelegateCommand<object> EventSetCommand { get; set; }
        private void EventSetCommandExecute(object e)
        {
            focusOne = null;
            FocusItem(this.RootNode);
            if (focusOne != null)//车辆节点
            {
                string onlinestatus = focusOne.ImageTip;
                if (onlinestatus.Equals("离线"))
                {
                    MessageBox.Show("车辆不在线，无法发送指令！");
                }
                else
                {
                    RightOperate.getVehicleBaseInfo(focusOne.nodeInfo.SIM);
                    EventSet win = new VehicleGPS.Views.Control.MonitorCentre.Instruction.EventSet();
                    if (win.isDisplay == false)
                    {
                        return;
                    }
                    else
                    {
                        win.Show();
                    }
                }
            }
            else
            {
                MessageBox.Show("请选择车辆结点");
            }
        }
        //设置电话本
        public DelegateCommand<object> PhoneSetCommand { get; set; }
        private void PhoneSetCommandExecute(object e)
        {
            focusOne = null;
            FocusItem(this.RootNode);
            if (focusOne != null)//车辆节点
            {
                string onlinestatus = focusOne.ImageTip;
                if (onlinestatus.Equals("离线"))
                {
                    MessageBox.Show("车辆不在线，无法发送指令！");
                }
                else
                {
                    RightOperate.getVehicleBaseInfo(focusOne.nodeInfo.SIM);
                    PhoneSet win = new VehicleGPS.Views.Control.MonitorCentre.Instruction.PhoneSet();
                    if (win.isDisplay == false)
                    {
                        return;
                    }
                    else
                    {
                        win.Show();
                    }
                }
            }
            else
            {
                MessageBox.Show("请选择车辆结点");
            }
        }
        //删除区域或路线
        public DelegateCommand<object> DelRegionCommand { get; set; }
        private void DelRegionCommandExecute(object e)
        {
            focusOne = null;
            FocusItem(this.RootNode);
            if (focusOne != null)//车辆节点
            {
                string onlinestatus = focusOne.ImageTip;
                if (onlinestatus.Equals("离线"))
                {
                    MessageBox.Show("车辆不在线，无法发送指令！");
                }
                else
                {
                    RightOperate.getVehicleBaseInfo(focusOne.nodeInfo.SIM);
                    DelRegion win = new VehicleGPS.Views.Control.MonitorCentre.Instruction.DelRegion();
                    if (win.isDisplay == false)
                    {
                        return;
                    }
                    else
                    {
                        win.Show();
                    }
                }
            }
            else
            {
                MessageBox.Show("请选择车辆结点");
            }
        }
        #endregion
        #region 华强指令
        /// <summary>
        /// 指令下发-华强设置类指令ABH
        /// </summary>
        /// <returns></returns>
        public DelegateCommand<string> HQ_NumberCommand { get; set; }
        private void HQ_NumberCommandExecute(string e)
        {
            focusOne = null;
            FocusItem(this.RootNode);
            if (focusOne != null)//车辆节点
            {
                string onlinestatus = focusOne.ImageTip;
                if (onlinestatus.Equals("离线"))
                {
                    MessageBox.Show("车辆不在线，无法发送指令！");
                }
                else
                {
                    RightOperate.getVehicleBaseInfo(focusOne.nodeInfo.SIM);
                    HQ_Number win = new VehicleGPS.Views.Control.MonitorCentre.Instruction.HQ.HQ_Number(e);
                    if (win.isDisplay == false)
                    {
                        return;
                    }
                    else
                    {
                        win.Show();
                    }
                }
            }
            else
            {
                MessageBox.Show("请选择车辆结点");
            }
        }
        /// <summary>
        /// 指令下发-华强设置类指令CI
        /// </summary>
        /// <returns></returns>
        public DelegateCommand<string> HQ_Setting_C_I_Command { get; set; }
        private void HQ_Setting_C_I_CommandExecute(string e)
        {
            focusOne = null;
            FocusItem(this.RootNode);
            if (focusOne != null)//车辆节点
            {
                string onlinestatus = focusOne.ImageTip;
                if (onlinestatus.Equals("离线"))
                {
                    MessageBox.Show("车辆不在线，无法发送指令！");
                }
                else
                {
                    RightOperate.getVehicleBaseInfo(focusOne.nodeInfo.SIM);
                    HQ_Setting_CI win = new VehicleGPS.Views.Control.MonitorCentre.Instruction.HQ.HQ_Setting_CI(e);
                    if (win.isDisplay == false)
                    {
                        return;
                    }
                    else
                    {
                        win.Show();
                    }
                }
            }
            else
            {
                MessageBox.Show("请选择车辆结点");
            }
        }

        /// <summary>
        /// 指令下发-华强控制类指令A控制终端工作状态 B-遥控智能锁车功能
        /// </summary>
        /// <returns></returns>
        public DelegateCommand<string> HQ_WorkingStateCommand { get; set; }
        private void HQ_WorkingStateCommandExecute(string e)
        {
            focusOne = null;
            FocusItem(this.RootNode);
            if (focusOne != null)//车辆节点
            {
                string onlinestatus = focusOne.ImageTip;
                if (onlinestatus.Equals("离线"))
                {
                    MessageBox.Show("车辆不在线，无法发送指令！");
                }
                else
                {
                    RightOperate.getVehicleBaseInfo(focusOne.nodeInfo.SIM);
                    HQ_WorkingState win = new VehicleGPS.Views.Control.MonitorCentre.Instruction.HQ.HQ_WorkingState(e);
                    if (win.isDisplay == false)
                    {
                        return;
                    }
                    else
                    {
                        win.Show();
                    }
                }
            }
            else
            {
                MessageBox.Show("请选择车辆结点");
            }
        }

        /// <summary>
        /// 指令下发-华强控制类指令F-监听功能
        /// </summary>
        /// <returns></returns>
        public DelegateCommand<object> HQ_MonitorCommand { get; set; }
        private void HQ_MonitorCommandExecute(object e)
        {
            focusOne = null;
            FocusItem(this.RootNode);
            if (focusOne != null)//车辆节点
            {
                string onlinestatus = focusOne.ImageTip;
                if (onlinestatus.Equals("离线"))
                {
                    MessageBox.Show("车辆不在线，无法发送指令！");
                }
                else
                {
                    RightOperate.getVehicleBaseInfo(focusOne.nodeInfo.SIM);
                    HQ_Monitor win = new VehicleGPS.Views.Control.MonitorCentre.Instruction.HQ.HQ_Monitor();
                    if (win.isDisplay == false)
                    {
                        return;
                    }
                    else
                    {
                        win.Show();
                    }
                }
            }
            else
            {
                MessageBox.Show("请选择车辆结点");
            }
        }

        /// <summary>
        /// 指令下发-华强控制类指令E-不带范围的点名信息C-解除终端报警G-查询车辆电瓶电压
        /// </summary>
        /// <returns></returns>
        public DelegateCommand<string> HQ_NoneDataCommand { get; set; }
        private void HQ_NoneDataCommandExecute(string e)
        {
            focusOne = null;
            FocusItem(this.RootNode);
            if (focusOne != null)//车辆节点
            {
                string onlinestatus = focusOne.ImageTip;
                if (onlinestatus.Equals("离线"))
                {
                    MessageBox.Show("车辆不在线，无法发送指令！");
                }
                else
                {
                    RightOperate.getVehicleBaseInfo(focusOne.nodeInfo.SIM);
                    HQ_NoneData win = new VehicleGPS.Views.Control.MonitorCentre.Instruction.HQ.HQ_NoneData(e);
                    if (win.isDisplay == false)
                    {
                        return;
                    }
                    else
                    {
                        win.Show();
                    }
                }
            }
            else
            {
                MessageBox.Show("请选择车辆结点");
            }
        }

        /// <summary>
        /// 指令下发-华强控制类指令D-要求终端进入GPRS模式
        /// </summary>
        /// <returns></returns>
        public DelegateCommand<object> HQ_GPRSCommand { get; set; }
        private void HQ_GPRSCommandExecute(object e)
        {
            focusOne = null;
            FocusItem(this.RootNode);
            if (focusOne != null)//车辆节点
            {
                string onlinestatus = focusOne.ImageTip;
                if (onlinestatus.Equals("离线"))
                {
                    MessageBox.Show("车辆不在线，无法发送指令！");
                }
                else
                {
                    RightOperate.getVehicleBaseInfo(focusOne.nodeInfo.SIM);
                    HQ_GPRS win = new VehicleGPS.Views.Control.MonitorCentre.Instruction.HQ.HQ_GPRS();
                    if (win.isDisplay == false)
                    {
                        return;
                    }
                    else
                    {
                        win.Show();
                    }
                }
            }
            else
            {
                MessageBox.Show("请选择车辆结点");
            }
        }

        /// <summary>
        /// 指令下发-华强控制类指令H-查询车辆里程信息
        /// </summary>
        /// <returns></returns>
        public DelegateCommand<object> HQ_VehicleMileageCommand { get; set; }
        private void HQ_VehicleMileageCommandExecute(object e)
        {
            focusOne = null;
            FocusItem(this.RootNode);
            if (focusOne != null)//车辆节点
            {
                string onlinestatus = focusOne.ImageTip;
                if (onlinestatus.Equals("离线"))
                {
                    MessageBox.Show("车辆不在线，无法发送指令！");
                }
                else
                {
                    RightOperate.getVehicleBaseInfo(focusOne.nodeInfo.SIM);
                    HQ_VehicleMileage win = new VehicleGPS.Views.Control.MonitorCentre.Instruction.HQ.HQ_VehicleMileage();
                    if (win.isDisplay == false)
                    {
                        return;
                    }
                    else
                    {
                        win.Show();
                    }
                }
            }
            else
            {
                MessageBox.Show("请选择车辆结点");
            }
        }

        /// <summary>
        /// 指令下发-华强控制类指令J-设置压缩回传参数
        /// </summary>
        /// <returns></returns>
        public DelegateCommand<object> HQ_CompressionCommand { get; set; }
        private void HQ_CompressionCommandExecute(object e)
        {
            focusOne = null;
            FocusItem(this.RootNode);
            if (focusOne != null)//车辆节点
            {
                string onlinestatus = focusOne.ImageTip;
                if (onlinestatus.Equals("离线"))
                {
                    MessageBox.Show("车辆不在线，无法发送指令！");
                }
                else
                {
                    RightOperate.getVehicleBaseInfo(focusOne.nodeInfo.SIM);
                    HQ_Compression win = new VehicleGPS.Views.Control.MonitorCentre.Instruction.HQ.HQ_Compression();
                    if (win.isDisplay == false)
                    {
                        return;
                    }
                    else
                    {
                        win.Show();
                    }
                }
            }
            else
            {
                MessageBox.Show("请选择车辆结点");
            }
        }

        /// <summary>
        /// 指令下发-华强控制类指令L-设置定距回传
        /// </summary>
        /// <returns></returns>
        public DelegateCommand<object> HQ_DistanceBackCommand { get; set; }
        private void HQ_DistanceBackCommandExecute(object e)
        {
            focusOne = null;
            FocusItem(this.RootNode);
            if (focusOne != null)//车辆节点
            {
                string onlinestatus = focusOne.ImageTip;
                if (onlinestatus.Equals("离线"))
                {
                    MessageBox.Show("车辆不在线，无法发送指令！");
                }
                else
                {
                    RightOperate.getVehicleBaseInfo(focusOne.nodeInfo.SIM);
                    HQ_DistanceBack win = new VehicleGPS.Views.Control.MonitorCentre.Instruction.HQ.HQ_DistanceBack();
                    if (win.isDisplay == false)
                    {
                        return;
                    }
                    else
                    {
                        win.Show();
                    }
                }
            }
            else
            {
                MessageBox.Show("请选择车辆结点");
            }
        }

        /// <summary>
        /// 指令下发-华强控制类指令M-带范围的单次点名信息
        /// </summary>
        /// <returns></returns>
        public DelegateCommand<object> HQ_OneCallCommand { get; set; }
        private void HQ_OneCallCommandExecute(object e)
        {
            focusOne = null;
            FocusItem(this.RootNode);
            if (focusOne != null)//车辆节点
            {
                string onlinestatus = focusOne.ImageTip;
                if (onlinestatus.Equals("离线"))
                {
                    MessageBox.Show("车辆不在线，无法发送指令！");
                }
                else
                {
                    RightOperate.getVehicleBaseInfo(focusOne.nodeInfo.SIM);
                    HQ_OneCall win = new VehicleGPS.Views.Control.MonitorCentre.Instruction.HQ.HQ_OneCall();
                    if (win.isDisplay == false)
                    {
                        return;
                    }
                    else
                    {
                        win.Show();
                    }
                }
            }
            else
            {
                MessageBox.Show("请选择车辆结点");
            }
        }

        /// <summary>
        /// 指令下发-华强控制类指令L-设置定距回传
        /// </summary>
        /// <returns></returns>
        public DelegateCommand<string> HQ_FixSpeedCommand { get; set; }
        private void HQ_FixSpeedCommandExecute(string e)
        {
            focusOne = null;
            FocusItem(this.RootNode);
            if (focusOne != null)//车辆节点
            {
                string onlinestatus = focusOne.ImageTip;
                if (onlinestatus.Equals("离线"))
                {
                    MessageBox.Show("车辆不在线，无法发送指令！");
                }
                else
                {
                    RightOperate.getVehicleBaseInfo(focusOne.nodeInfo.SIM);
                    HQ_FixSpeed win = new VehicleGPS.Views.Control.MonitorCentre.Instruction.HQ.HQ_FixSpeed(e);
                    if (win.isDisplay == false)
                    {
                        return;
                    }
                    else
                    {
                        win.Show();
                    }
                }
            }
            else
            {
                MessageBox.Show("请选择车辆结点");
            }
        }

        /// <summary>
        /// 指令下发-华强控制类指令K-设置在线待命时位置回传的时间间隔
        /// </summary>
        /// <returns></returns>
        public DelegateCommand<object> HQ_StandByCommand { get; set; }
        private void HQ_StandByCommandExecute(object e)
        {
            focusOne = null;
            FocusItem(this.RootNode);
            if (focusOne != null)//车辆节点
            {
                string onlinestatus = focusOne.ImageTip;
                if (onlinestatus.Equals("离线"))
                {
                    MessageBox.Show("车辆不在线，无法发送指令！");
                }
                else
                {
                    RightOperate.getVehicleBaseInfo(focusOne.nodeInfo.SIM);
                    HQ_StandBy win = new VehicleGPS.Views.Control.MonitorCentre.Instruction.HQ.HQ_StandBy();
                    if (win.isDisplay == false)
                    {
                        return;
                    }
                    else
                    {
                        win.Show();
                    }
                }
            }
            else
            {
                MessageBox.Show("请选择车辆结点");
            }
        }

        /// <summary>
        /// 指令下发-华强控制类指令P-设置空车时的定时回传时间间隔及次数
        /// </summary>
        /// <returns></returns>
        public DelegateCommand<object> HQ_EmptyBackCommand { get; set; }
        private void HQ_EmptyBackCommandExecute(object e)
        {
            focusOne = null;
            FocusItem(this.RootNode);
            if (focusOne != null)//车辆节点
            {
                string onlinestatus = focusOne.ImageTip;
                if (onlinestatus.Equals("离线"))
                {
                    MessageBox.Show("车辆不在线，无法发送指令！");
                }
                else
                {
                    RightOperate.getVehicleBaseInfo(focusOne.nodeInfo.SIM);
                    HQ_EmptyBack win = new VehicleGPS.Views.Control.MonitorCentre.Instruction.HQ.HQ_EmptyBack();
                    if (win.isDisplay == false)
                    {
                        return;
                    }
                    else
                    {
                        win.Show();
                    }
                }
            }
            else
            {
                MessageBox.Show("请选择车辆结点");
            }
        }

        /// <summary>
        /// 指令下发-华强控制类指令Z-华强中心下发给任意附件的的任意信息
        /// </summary>
        /// <returns></returns>
        public DelegateCommand<object> HQ_AnyInformationCommand { get; set; }
        private void HQ_AnyInformationCommandExecute(object e)
        {
            focusOne = null;
            FocusItem(this.RootNode);
            if (focusOne != null)//车辆节点
            {
                string onlinestatus = focusOne.ImageTip;
                if (onlinestatus.Equals("离线"))
                {
                    MessageBox.Show("车辆不在线，无法发送指令！");
                }
                else
                {
                    RightOperate.getVehicleBaseInfo(focusOne.nodeInfo.SIM);
                    HQ_AnyInformation win = new VehicleGPS.Views.Control.MonitorCentre.Instruction.HQ.HQ_AnyInformation();
                    if (win.isDisplay == false)
                    {
                        return;
                    }
                    else
                    {
                        win.Show();
                    }
                }
            }
            else
            {
                MessageBox.Show("请选择车辆结点");
            }
        }

        /// <summary>
        /// 指令下发-华强设置类指令 报警范围
        /// </summary>
        /// <returns></returns>
        public DelegateCommand<string> HQ_Setting_ScopeCommand { get; set; }
        private void HQ_Setting_ScopeCommandExecute(string e)
        {
            focusOne = null;
            FocusItem(this.RootNode);
            if (focusOne != null)//车辆节点
            {
                string onlinestatus = focusOne.ImageTip;
                if (onlinestatus.Equals("离线"))
                {
                    MessageBox.Show("车辆不在线，无法发送指令！");
                }
                else
                {
                    RightOperate.getVehicleBaseInfo(focusOne.nodeInfo.SIM);
                    HQ_Setting_Scope win = new VehicleGPS.Views.Control.MonitorCentre.Instruction.HQ.HQ_Setting_Scope(e);
                    if (win.isDisplay == false)
                    {
                        return;
                    }
                    else
                    {
                        win.Show();
                    }
                }
            }
            else
            {
                MessageBox.Show("请选择车辆结点");
            }
        }

        /// <summary>
        /// 指令下发-华强设置类指令 设置特定短信息菜单
        /// </summary>
        /// <returns></returns>
        public DelegateCommand<string> HQ_Setting_MessageCommand { get; set; }
        private void HQ_Setting_MessageCommandExecute(string e)
        {
            focusOne = null;
            FocusItem(this.RootNode);
            if (focusOne != null)//车辆节点
            {
                string onlinestatus = focusOne.ImageTip;
                if (onlinestatus.Equals("离线"))
                {
                    MessageBox.Show("车辆不在线，无法发送指令！");
                }
                else
                {
                    RightOperate.getVehicleBaseInfo(focusOne.nodeInfo.SIM);
                    HQ_Setting_Message win = new VehicleGPS.Views.Control.MonitorCentre.Instruction.HQ.HQ_Setting_Message(e);
                    if (win.isDisplay == false)
                    {
                        return;
                    }
                    else
                    {
                        win.Show();
                    }
                }
            }
            else
            {
                MessageBox.Show("请选择车辆结点");
            }
        }

        /// <summary>
        /// 指令下发-华强设置类指令 设置运行商LOGO
        /// </summary>
        /// <returns></returns>
        public DelegateCommand<string> HQ_Setting_LogoCommand { get; set; }
        private void HQ_Setting_LogoCommandExecute(string e)
        {
            focusOne = null;
            FocusItem(this.RootNode);
            if (focusOne != null)//车辆节点
            {
                string onlinestatus = focusOne.ImageTip;
                if (onlinestatus.Equals("离线"))
                {
                    MessageBox.Show("车辆不在线，无法发送指令！");
                }
                else
                {
                    RightOperate.getVehicleBaseInfo(focusOne.nodeInfo.SIM);
                    HQ_Setting_Logo win = new VehicleGPS.Views.Control.MonitorCentre.Instruction.HQ.HQ_Setting_Logo(e);
                    if (win.isDisplay == false)
                    {
                        return;
                    }
                    else
                    {
                        win.Show();
                    }
                }
            }
            else
            {
                MessageBox.Show("请选择车辆结点");
            }
        }

        /// <summary>
        /// 指令下发-华强设置类指令 设置特定短信息菜单
        /// </summary>
        /// <returns></returns>
        public DelegateCommand<string> HQ_Setting_CallCommand { get; set; }
        private void HQ_Setting_CallCommandExecute(string e)
        {
            focusOne = null;
            FocusItem(this.RootNode);
            if (focusOne != null)//车辆节点
            {
                string onlinestatus = focusOne.ImageTip;
                if (onlinestatus.Equals("离线"))
                {
                    MessageBox.Show("车辆不在线，无法发送指令！");
                }
                else
                {
                    RightOperate.getVehicleBaseInfo(focusOne.nodeInfo.SIM);
                    HQ_Setting_Call win = new VehicleGPS.Views.Control.MonitorCentre.Instruction.HQ.HQ_Setting_Call(e);
                    if (win.isDisplay == false)
                    {
                        return;
                    }
                    else
                    {
                        win.Show();
                    }
                }
            }
            else
            {
                MessageBox.Show("请选择车辆结点");
            }
        }

        /// <summary>
        /// 指令下发-华强设置类指令 设置多边形报警范围
        /// </summary>
        /// <returns></returns>
        public DelegateCommand<string> HQ_Setting_PoloygnCommand { get; set; }
        private void HQ_Setting_PoloygnCommandExecute(string e)
        {
            focusOne = null;
            FocusItem(this.RootNode);
            if (focusOne != null)//车辆节点
            {
                string onlinestatus = focusOne.ImageTip;
                if (onlinestatus.Equals("离线"))
                {
                    MessageBox.Show("车辆不在线，无法发送指令！");
                }
                else
                {
                    RightOperate.getVehicleBaseInfo(focusOne.nodeInfo.SIM);
                    HQ_Setting_Poloygn win = new VehicleGPS.Views.Control.MonitorCentre.Instruction.HQ.HQ_Setting_Poloygn(e);
                    if (win.isDisplay == false)
                    {
                        return;
                    }
                    else
                    {
                        win.Show();
                    }
                }
            }
            else
            {
                MessageBox.Show("请选择车辆结点");
            }
        }

        /// <summary>
        /// 指令下发-华强网络参数设置类指令A-更改IP、端口号
        /// </summary>
        /// <returns></returns>
        public DelegateCommand<string> HQ_IPandPortCommand { get; set; }
        private void HQ_IPandPortCommandExecute(string e)
        {
            focusOne = null;
            FocusItem(this.RootNode);
            if (focusOne != null)//车辆节点
            {
                string onlinestatus = focusOne.ImageTip;
                if (onlinestatus.Equals("离线"))
                {
                    MessageBox.Show("车辆不在线，无法发送指令！");
                }
                else
                {
                    RightOperate.getVehicleBaseInfo(focusOne.nodeInfo.SIM);
                    HQ_IPandPort win = new VehicleGPS.Views.Control.MonitorCentre.Instruction.HQ.HQ_IPandPort(e);
                    if (win.isDisplay == false)
                    {
                        return;
                    }
                    else
                    {
                        win.Show();
                    }
                }
            }
            else
            {
                MessageBox.Show("请选择车辆结点");
            }
        }

        /// <summary>
        /// 指令下发-华强网络参数设置类指令C-终端掉线后重新建立GPRS连接的最大时间 D-设置服务器UDP端口号
        /// </summary>
        /// <returns></returns>
        public DelegateCommand<string> HQ_OfflineGPRSCommand { get; set; }
        private void HQ_OfflineGPRSCommandExecute(string e)
        {
            focusOne = null;
            FocusItem(this.RootNode);
            if (focusOne != null)//车辆节点
            {
                string onlinestatus = focusOne.ImageTip;
                if (onlinestatus.Equals("离线"))
                {
                    MessageBox.Show("车辆不在线，无法发送指令！");
                }
                else
                {
                    RightOperate.getVehicleBaseInfo(focusOne.nodeInfo.SIM);
                    HQ_OfflineGPRS win = new VehicleGPS.Views.Control.MonitorCentre.Instruction.HQ.HQ_OfflineGPRS(e);
                    if (win.isDisplay == false)
                    {
                        return;
                    }
                    else
                    {
                        win.Show();
                    }
                }
            }
            else
            {
                MessageBox.Show("请选择车辆结点");
            }
        }

        /// <summary>
        /// 指令下发-华强多媒体设置类指令E-紧急情况下（盗警、劫警）图像抓拍的时间间隔以及抓拍次数
        /// </summary>
        /// <returns></returns>
        public DelegateCommand<string> HQ_EmegentPicCommand { get; set; }
        private void HQ_EmegentPicCommandExecute(string e)
        {
            focusOne = null;
            FocusItem(this.RootNode);
            if (focusOne != null)//车辆节点
            {
                string onlinestatus = focusOne.ImageTip;
                if (onlinestatus.Equals("离线"))
                {
                    MessageBox.Show("车辆不在线，无法发送指令！");
                }
                else
                {
                    RightOperate.getVehicleBaseInfo(focusOne.nodeInfo.SIM);
                    HQ_EmegentPic win = new VehicleGPS.Views.Control.MonitorCentre.Instruction.HQ.HQ_EmegentPic(e);
                    if (win.isDisplay == false)
                    {
                        return;
                    }
                    else
                    {
                        win.Show();
                    }
                }
            }
            else
            {
                MessageBox.Show("请选择车辆结点");
            }
        }

        /// <summary>
        /// 指令下发-华强网络参数设置类指令B-更改用户名、密码、拨号号码、APN
        /// </summary>
        /// <returns></returns>
        public DelegateCommand<object> HQ_UserCodeCommand { get; set; }
        private void HQ_UserCodeCommandExecute(object e)
        {
            focusOne = null;
            FocusItem(this.RootNode);
            if (focusOne != null)//车辆节点
            {
                string onlinestatus = focusOne.ImageTip;
                if (onlinestatus.Equals("离线"))
                {
                    MessageBox.Show("车辆不在线，无法发送指令！");
                }
                else
                {
                    RightOperate.getVehicleBaseInfo(focusOne.nodeInfo.SIM);
                    HQ_UserCode win = new VehicleGPS.Views.Control.MonitorCentre.Instruction.HQ.HQ_UserCode();
                    if (win.isDisplay == false)
                    {
                        return;
                    }
                    else
                    {
                        win.Show();
                    }
                }
            }
            else
            {
                MessageBox.Show("请选择车辆结点");
            }
        }

        /// <summary>
        /// 指令下发-华强设置类指令K-设置终端参数
        /// </summary>
        /// <returns></returns>
        public DelegateCommand<object> HQ_SettingParaCommand { get; set; }
        private void HQ_SettingParaCommandExecute(object e)
        {
            focusOne = null;
            FocusItem(this.RootNode);
            if (focusOne != null)//车辆节点
            {
                string onlinestatus = focusOne.ImageTip;
                if (onlinestatus.Equals("离线"))
                {
                    MessageBox.Show("车辆不在线，无法发送指令！");
                }
                else
                {
                    RightOperate.getVehicleBaseInfo(focusOne.nodeInfo.SIM);
                    HQ_SettingPara win = new VehicleGPS.Views.Control.MonitorCentre.Instruction.HQ.HQ_SettingPara();
                    if (win.isDisplay == false)
                    {
                        return;
                    }
                    else
                    {
                        win.Show();
                    }
                }
            }
            else
            {
                MessageBox.Show("请选择车辆结点");
            }
        }
        #endregion
        #endregion

        #region 获取选择的车辆
        public List<InstructionInfo> GetSelectedVehicles()
        {
            List<CVBasicInfo> selectedList = new List<CVBasicInfo>();
            this.AddSelectedVehicles(this.RootNode, selectedList);

            /*转为指令模型*/
            List<InstructionInfo> instructionList = new List<InstructionInfo>();
            if (StaticTreeState.VehicleAllBasicInfo == LoadingState.LOADCOMPLETE)
            {

                foreach (CVBasicInfo binfo in selectedList)
                {
                    foreach (CVDetailInfo dInfo in StaticDetailInfo.GetInstance().ListVehicleDetailInfo)
                    {
                        if (binfo.ID == dInfo.VehicleNum)
                        {
                            InstructionInfo iInfo = new InstructionInfo();
                            iInfo.sequence = instructionList.Count + 1;
                            iInfo.id = dInfo.VehicleNum;
                            iInfo.name = dInfo.VehicleId;
                            iInfo.sim = dInfo.SIM;
                            iInfo.gpsType = dInfo.GPSType;
                            iInfo.gpsVersion = dInfo.GPSVersion;
                            foreach (CVBasicInfo pInfo in StaticBasicInfo.GetInstance().ListClientBasicInfo)
                            {
                                if (binfo.ParentID == pInfo.ID)
                                {
                                    iInfo.parent = pInfo.Name;
                                    break;
                                }
                            }
                            instructionList.Add(iInfo);
                            break;
                        }
                    }
                }
            }
            else
            {
                MessageBox.Show("正在获取详细数据中，请稍后", "提示", MessageBoxButton.OK);
                return null;
            }
            return instructionList;
        }
        public void AddSelectedVehicles(RealTimeTreeNodeViewModel root, List<CVBasicInfo> list)
        {
            if (root.ListChildNodes != null && root.ListChildNodes.Count != 0)
            {
                foreach (RealTimeTreeNodeViewModel node in root.listChildNodes)
                {
                    if (node.nodeInfo.ID.Contains("VEHI") && node.isSelected == true)
                    {
                        list.Add(node.nodeInfo);
                    }
                    else
                    {
                        AddSelectedVehicles(node, list);
                    }
                }
            }
            //else
            //{
            //    if (root.nodeInfo.SIM != "0" && root.isSelected == true)
            //    {
            //        list.Add(root.nodeInfo);
            //    }
            //    else
            //    {
            //        AddSelectedVehicles(root, list);
            //    }
            //}
        }
        #endregion

        #region AutoComplete查询模型
        public DelegateCommand<object> AutoCompleteSelectedCommand { get; set; }
        private void AutoCompleteSelectedCommandExecute(object control)
        {
            if (((AutoCompleteBox)control).SelectedItem != null)
            {
                this.AutoCompleteSelectedItem = (AutoCompleteItem)((AutoCompleteBox)control).SelectedItem;
                this.ExpandAndFocus(this.RootNode);
            }
        }
        public AutoCompleteItem AutoCompleteSelectedItem { get; set; }
        private List<AutoCompleteItem> listAutoComplete;
        public List<AutoCompleteItem> ListAutoComplete
        {
            get { return listAutoComplete; }
            set
            {
                if (listAutoComplete != value)
                {
                    listAutoComplete = value;
                    this.RaisePropertyChanged("ListAutoComplete");
                }
            }
        }

        /// <summary>
        ///  初始化AutoComplete查询数据集
        ///  14-6-6
        /// </summary>
        public void InitAutoComplete()
        {
            List<AutoCompleteItem> tmpList = new List<AutoCompleteItem>();
            foreach (CVBasicInfo info in StaticBasicInfo.GetInstance().ListVehicleBasicInfo)
            {
                AutoCompleteItem item = new AutoCompleteItem();
                item.ID = info.ID;
                item.Name = info.Name;
                item.SIM = info.SIM;
                item.NameSim = info.Name + info.SIM;
                tmpList.Add(item);
            }
            this.ListAutoComplete = tmpList;
        }
        /*初始化AutoComplete查询数据集*/
        public void InitAutoComplete_old()
        {
            if (StaticTreeState.VehicleBasicInfo == LoadingState.LOADCOMPLETE)
            {
                List<AutoCompleteItem> tmpList = new List<AutoCompleteItem>();
                foreach (CVBasicInfo info in StaticBasicInfo.GetInstance().ListVehicleBasicInfo)
                {
                    AutoCompleteItem item = new AutoCompleteItem();
                    item.ID = info.ID;
                    item.Name = info.Name;
                    item.SIM = info.SIM;
                    tmpList.Add(item);
                }
                this.ListAutoComplete = tmpList;
            }
        }
        #endregion

        #region 获取焦点和展开
        private void ExpandAndFocus(RealTimeTreeNodeViewModel root)
        {
            if (root.listChildNodes == null || root.listChildNodes.Count == 0)
            {
                return;
            }
            foreach (RealTimeTreeNodeViewModel node in root.listChildNodes)
            {
                if (node.nodeInfo.Name == this.AutoCompleteSelectedItem.Name)
                {
                    this.Expand(node.ParentNode);
                    node.IsFocus = true;
                    node.IsSelected = true;
                }
                else
                {
                    ExpandAndFocus(node);
                }
            }
        }
        //给报警用的
        private void ExpandAndFocus2(RealTimeTreeNodeViewModel root, string sim)
        {
            if (root.listChildNodes == null || root.listChildNodes.Count == 0)
            {
                return;
            }
            foreach (RealTimeTreeNodeViewModel node in root.listChildNodes)
            {
                if (node.nodeInfo.SIM == sim)
                {
                    this.Expand(node.ParentNode);
                    node.IsFocus = true;
                    node.IsSelected = true;
                }
                else
                {
                    ExpandAndFocus2(node, sim);
                }
            }
        }
        public RealTimeTreeNodeViewModel focusOne = null;
        /// <summary>
        /// 获得焦点的树节点
        /// </summary>
        private void FocusItem(RealTimeTreeNodeViewModel root)
        {
            if (root == null)
            {
                root = RealTimeTreeViewModel.GetInstance().RootNode;
            }
            if (root.listChildNodes == null || root.listChildNodes.Count == 0)
            {
                return;
            }
            foreach (RealTimeTreeNodeViewModel node in root.listChildNodes)
            {
                if (focusOne == null)
                {
                    if (node.nodeInfo.SIM != null && node.isFocus == true)
                    {
                        focusOne = node;
                        break;
                    }
                    else
                    {
                        FocusItem(node);
                    }
                }
            }

        }
        private void Expand(RealTimeTreeNodeViewModel node)
        {
            node.IsExpand = true;
            if (node.nodeInfo.ID != "admin")
            {
                Expand(node.ParentNode);
            }
        }
        #endregion

        #region 刷新按钮状态
        public bool isRefreshEnable = false;
        public bool IsRefreshEnable
        {
            get { return isRefreshEnable; }
            set
            {
                if (isRefreshEnable != value)
                {
                    isRefreshEnable = value;
                    this.RaisePropertyChanged("IsRefreshEnable");
                }
            }
        }
        public Brush refreshFontColor = new SolidColorBrush(Colors.WhiteSmoke);
        public Brush RefreshFontColor
        {
            get { return refreshFontColor; }
            set
            {
                if (refreshFontColor != value)
                {
                    refreshFontColor = value;
                    this.RaisePropertyChanged("RefreshFontColor");
                }
            }
        }
        public void RefreshState(bool b)
        {
            this.IsRefreshEnable = b;
            this.RefreshFontColor = new SolidColorBrush((b == true ? Colors.Black : Colors.WhiteSmoke));
        }
        #endregion

        #region 指令显隐

        #region 部标指令
        private Visibility sendinstructionvisible = Visibility.Collapsed;
        public Visibility SendInstructionVisible
        {
            get { return sendinstructionvisible; }
            set
            {
                sendinstructionvisible = value;
                this.RaisePropertyChanged("SendInstructionVisible");
            }
        }
        private Visibility terminalparasetvisible = Visibility.Collapsed;

        public Visibility TerminalParaSetVisible
        {
            get { return terminalparasetvisible; }
            set
            {
                terminalparasetvisible = value;
                this.RaisePropertyChanged("TerminalParaSetVisible");
            }
        }
        private Visibility heartbeatvisible = Visibility.Collapsed;

        public Visibility HeartBeatVisible
        {
            get { return heartbeatvisible; }
            set
            {
                heartbeatvisible = value;
                this.RaisePropertyChanged("HeartBeatVisible");
            }
        }
        private Visibility tcpmessagesetvisible = Visibility.Collapsed;

        public Visibility TCPMessageSetVisible
        {
            get { return tcpmessagesetvisible; }
            set
            {
                tcpmessagesetvisible = value;
                this.RaisePropertyChanged("TCPMessageSetVisible");
            }
        }
        private Visibility tcpresponsevisible = Visibility.Collapsed;

        public Visibility TCPResponseVisible
        {
            get { return tcpresponsevisible; }
            set
            {
                tcpresponsevisible = value;
                this.RaisePropertyChanged("TCPResponseVisible");
            }
        }
        private Visibility tcpagainvisible = Visibility.Collapsed;

        public Visibility TCPAgainVisible
        {
            get { return tcpagainvisible; }
            set
            {
                tcpagainvisible = value;
                this.RaisePropertyChanged("TCPAgainVisible");
            }
        }
        private Visibility udpMessageSetVisible = Visibility.Collapsed;

        public Visibility UDPMessageSetVisible
        {
            get { return udpMessageSetVisible; }
            set
            {
                udpMessageSetVisible = value;
                this.RaisePropertyChanged("UDPMessageSetVisible");
            }
        }
        private Visibility udpResponseVisible = Visibility.Collapsed;

        public Visibility UDPResponseVisible
        {
            get { return udpResponseVisible; }
            set
            {
                udpResponseVisible = value;
                this.RaisePropertyChanged("UPPResponseVisible");
            }
        }
        private Visibility udpAgainVisible = Visibility.Collapsed;

        public Visibility UDPAgainVisible
        {
            get { return udpAgainVisible; }
            set
            {
                udpAgainVisible = value;
                this.RaisePropertyChanged("UDPAgainVisible");
            }
        }
        private Visibility smsMessageSetVisible = Visibility.Collapsed;

        public Visibility SMSMessageSetVisible
        {
            get { return smsMessageSetVisible; }
            set
            {
                smsMessageSetVisible = value;
                this.RaisePropertyChanged("SMSMessageSetVisible");
            }
        }
        private Visibility smsResponseVisible = Visibility.Collapsed;

        public Visibility SMSResponseVisible
        {
            get { return smsResponseVisible; }
            set
            {
                smsResponseVisible = value;
                this.RaisePropertyChanged("SMSResponseVisible");
            }
        }
        private Visibility smsAgainVisible = Visibility.Collapsed;

        public Visibility SMSAgainVisible
        {
            get { return smsAgainVisible; }
            set
            {
                smsAgainVisible = value;
                this.RaisePropertyChanged("SMSAgainVisible");
            }
        }
        private Visibility serverSetVisible = Visibility.Collapsed;

        public Visibility ServerSetVisible
        {
            get { return serverSetVisible; }
            set
            {
                serverSetVisible = value;
                this.RaisePropertyChanged("ServerSetVisible");
            }
        }
        private Visibility mainServerVisible = Visibility.Collapsed;

        public Visibility MainServerVisible
        {
            get { return mainServerVisible; }
            set
            {
                mainServerVisible = value;
                this.RaisePropertyChanged("MainServerVisible");
            }
        }
        private Visibility mainSiteVisible = Visibility.Collapsed;

        public Visibility MainSiteVisible
        {
            get { return mainSiteVisible; }
            set
            {
                mainSiteVisible = value;
                this.RaisePropertyChanged("MainSiteVisible");
            }
        }
        private Visibility mainUserVisible = Visibility.Collapsed;

        public Visibility MainUserVisible
        {
            get { return mainUserVisible; }
            set
            {
                mainUserVisible = value;
                this.RaisePropertyChanged("MainUserVisible");
            }
        }
        private Visibility mainCodeVisible = Visibility.Collapsed;

        public Visibility MainCodeVisible
        {
            get { return mainCodeVisible; }
            set
            {
                mainCodeVisible = value;
                this.RaisePropertyChanged("MainCodeVisible");
            }
        }
        private Visibility mainIPVisible = Visibility.Collapsed;

        public Visibility MainIPVisible
        {
            get { return mainIPVisible; }
            set
            {
                mainIPVisible = value;
                this.RaisePropertyChanged("MainIPVisible");
            }
        }
        private Visibility viceServerVisible = Visibility.Collapsed;

        public Visibility ViceServerVisible
        {
            get { return viceServerVisible; }
            set
            {
                viceServerVisible = value;
                this.RaisePropertyChanged("ViceServerVisible");
            }
        }
        private Visibility viceSiteVisible = Visibility.Collapsed;

        public Visibility ViceSiteVisible
        {
            get { return viceSiteVisible; }
            set
            {
                viceSiteVisible = value;
                this.RaisePropertyChanged("ViceSiteVisible");
            }
        }
        private Visibility viceUserVisible = Visibility.Collapsed;

        public Visibility ViceUserVisible
        {
            get { return viceUserVisible; }
            set
            {
                viceUserVisible = value;
                this.RaisePropertyChanged("ViceUserVisible");
            }
        }
        private Visibility viceCodeVisible = Visibility.Collapsed;

        public Visibility ViceCodeVisible
        {
            get { return viceCodeVisible; }
            set
            {
                viceCodeVisible = value;
                this.RaisePropertyChanged("ViceCodeVisible");
            }
        }
        private Visibility viceIPVisible = Visibility.Collapsed;

        public Visibility ViceIPVisible
        {
            get { return viceIPVisible; }
            set
            {
                viceIPVisible = value;
                this.RaisePropertyChanged("ViceIPVisible");
            }
        }
        private Visibility tcpSiteVisible = Visibility.Collapsed;

        public Visibility TCPSiteVisible
        {
            get { return tcpSiteVisible; }
            set
            {
                tcpSiteVisible = value;
                this.RaisePropertyChanged("TCPSiteVisible");
            }
        }
        private Visibility udpSiteVisible = Visibility.Collapsed;

        public Visibility UDPSiteVisible
        {
            get { return udpSiteVisible; }
            set
            {
                udpSiteVisible = value;
                this.RaisePropertyChanged("UDPSiteVisible");
            }
        }
        private Visibility posSetVisible = Visibility.Collapsed;

        public Visibility PosSetVisible
        {
            get { return posSetVisible; }
            set
            {
                posSetVisible = value;
                this.RaisePropertyChanged("PosSetVisible");
            }
        }
        private Visibility posStrategyVisible = Visibility.Collapsed;

        public Visibility PosStrategyVisible
        {
            get { return posStrategyVisible; }
            set
            {
                posStrategyVisible = value;
                this.RaisePropertyChanged("PosStrategyVisible");
            }
        }
        private Visibility posSolutionVisible = Visibility.Collapsed;

        public Visibility PosSolutionVisible
        {
            get { return posSolutionVisible; }
            set
            {
                posSolutionVisible = value;
                this.RaisePropertyChanged("PosSolutionVisible");
            }
        }
        private Visibility reportTimeSetVisible = Visibility.Collapsed;

        public Visibility ReportTimeSetVisible
        {
            get { return reportTimeSetVisible; }
            set
            {
                reportTimeSetVisible = value;
                this.RaisePropertyChanged("ReportTimeSetVisible");
            }
        }
        private Visibility unloginTimeVisible = Visibility.Collapsed;

        public Visibility UnloginTimeVisible
        {
            get { return unloginTimeVisible; }
            set
            {
                unloginTimeVisible = value;
                this.RaisePropertyChanged("UnloginTimeVisible");
            }
        }
        private Visibility restTimeVisible = Visibility.Collapsed;

        public Visibility RestTimeVisible
        {
            get { return restTimeVisible; }
            set
            {
                restTimeVisible = value;
                this.RaisePropertyChanged("RestTimeVisible");
            }
        }
        private Visibility emergenceTimeVisible = Visibility.Collapsed;

        public Visibility EmergenceTimeVisible
        {
            get { return emergenceTimeVisible; }
            set
            {
                emergenceTimeVisible = value;
                this.RaisePropertyChanged("EmergenceTimeVisible");
            }
        }
        private Visibility lackTimeVisible = Visibility.Collapsed;

        public Visibility LackTimeVisible
        {
            get { return lackTimeVisible; }
            set
            {
                lackTimeVisible = value;
                this.RaisePropertyChanged("LackTimeVisible");
            }
        }
        private Visibility reportDistanceSetVisible = Visibility.Collapsed;

        public Visibility ReportDistanceSetVisible
        {
            get { return reportDistanceSetVisible; }
            set
            {
                reportDistanceSetVisible = value;
                this.RaisePropertyChanged("ReportDistanceSetVisible");
            }
        }
        private Visibility unloginDistanceVisible = Visibility.Collapsed;

        public Visibility UnloginDistanceVisible
        {
            get { return unloginDistanceVisible; }
            set
            {
                unloginDistanceVisible = value;
                this.RaisePropertyChanged("UnloginDistanceVisible");
            }
        }
        private Visibility restDistanceVisible = Visibility.Collapsed;

        public Visibility RestDistanceVisible
        {
            get { return restDistanceVisible; }
            set
            {
                restDistanceVisible = value;
                this.RaisePropertyChanged("RestDistanceVisible");
            }
        }
        private Visibility emergenceDistanceVisible = Visibility.Collapsed;

        public Visibility EmergenceDistanceVisible
        {
            get { return emergenceDistanceVisible; }
            set
            {
                emergenceDistanceVisible = value;
                this.RaisePropertyChanged("EmergenceDistanceVisible");
            }
        }
        private Visibility lackDistanceVisible = Visibility.Collapsed;

        public Visibility LackDistanceVisible
        {
            get { return lackDistanceVisible; }
            set
            {
                lackDistanceVisible = value;
                this.RaisePropertyChanged("LackDistanceVisible");
            }
        }
        private Visibility inflectionAngleVisible = Visibility.Collapsed;

        public Visibility InflectionAngleVisible
        {
            get { return inflectionAngleVisible; }
            set
            {
                inflectionAngleVisible = value;
                this.RaisePropertyChanged("InflectionAngleVisible");
            }
        }
        private Visibility callSetVisible = Visibility.Collapsed;

        public Visibility CallSetVisible
        {
            get { return callSetVisible; }
            set
            {
                callSetVisible = value;
                this.RaisePropertyChanged("CallSetVisible");
            }
        }
        private Visibility platformNumVisible = Visibility.Collapsed;

        public Visibility PlatformNumVisible
        {
            get { return platformNumVisible; }
            set
            {
                platformNumVisible = value;
                this.RaisePropertyChanged("PlatformNumVisible");
            }
        }
        private Visibility retNumVisible = Visibility.Collapsed;

        public Visibility RetNumVisible
        {
            get { return retNumVisible; }
            set
            {
                retNumVisible = value;
                this.RaisePropertyChanged("RetNumVisible");
            }
        }
        private Visibility factorySettingNumVisible = Visibility.Collapsed;

        public Visibility FactorySettingNumVisible
        {
            get { return factorySettingNumVisible; }
            set
            {
                factorySettingNumVisible = value;
                this.RaisePropertyChanged("FactorySettingNumVisible");
            }
        }
        private Visibility platformSMSNumVisible = Visibility.Collapsed;

        public Visibility PlatformSMSNumVisible
        {
            get { return platformSMSNumVisible; }
            set
            {
                platformSMSNumVisible = value;
                this.RaisePropertyChanged("PlatformSMSNumVisible");
            }
        }
        private Visibility terminalSMSNumVisible = Visibility.Collapsed;

        public Visibility TerminalSMSNumVisible
        {
            get { return terminalSMSNumVisible; }
            set
            {
                terminalSMSNumVisible = value;
                this.RaisePropertyChanged("TerminalSMSNumVisible");
            }
        }
        private Visibility terminalStrategyVisible = Visibility.Collapsed;

        public Visibility TerminalStrategyVisible
        {
            get { return terminalStrategyVisible; }
            set
            {
                terminalStrategyVisible = value;
                this.RaisePropertyChanged("TerminalStrategyVisible");
            }
        }
        private Visibility onceMaxTimeVisible = Visibility.Collapsed;

        public Visibility OnceMaxTimeVisible
        {
            get { return onceMaxTimeVisible; }
            set
            {
                onceMaxTimeVisible = value;
                this.RaisePropertyChanged("OnceMaxTimeVisible");
            }
        }
        private Visibility monthMaxTimeVisible = Visibility.Collapsed;

        public Visibility MonthMaxTimeVisible
        {
            get { return monthMaxTimeVisible; }
            set
            {
                monthMaxTimeVisible = value;
                this.RaisePropertyChanged("MonthMaxTimeVisible");
            }
        }
        private Visibility monitorNumVisible = Visibility.Collapsed;

        public Visibility MonitorNumVisible
        {
            get { return monitorNumVisible; }
            set
            {
                monitorNumVisible = value;
                this.RaisePropertyChanged("MonitorNumVisible");
            }
        }
        private Visibility privilegeNumVisible = Visibility.Collapsed;

        public Visibility PrivilegeNumVisible
        {
            get { return privilegeNumVisible; }
            set
            {
                privilegeNumVisible = value;
                this.RaisePropertyChanged("PrivilegeNumVisible");
            }
        }
        private Visibility alarmSetVisible = Visibility.Collapsed;

        public Visibility AlarmSetVisible
        {
            get { return alarmSetVisible; }
            set
            {
                alarmSetVisible = value;
                this.RaisePropertyChanged("AlarmSetVisible");
            }
        }
        private Visibility alarmShieldVisible = Visibility.Collapsed;

        public Visibility AlarmShieldVisible
        {
            get { return alarmShieldVisible; }
            set
            {
                alarmShieldVisible = value;
                this.RaisePropertyChanged("AlarmShieldVisible");
            }
        }
        private Visibility alarmTextSMSVisible = Visibility.Collapsed;

        public Visibility AlarmTextSMSVisible
        {
            get { return alarmTextSMSVisible; }
            set
            {
                alarmTextSMSVisible = value;
                this.RaisePropertyChanged("AlarmTextSMSVisible");
            }
        }
        private Visibility alarmShootSwitchVisible = Visibility.Collapsed;

        public Visibility AlarmShootSwitchVisible
        {
            get { return alarmShootSwitchVisible; }
            set
            {
                alarmShootSwitchVisible = value;
                this.RaisePropertyChanged("AlarmShootSwitchVisible");
            }
        }
        private Visibility alarmStorSignVisible = Visibility.Collapsed;

        public Visibility AlarmStorSignVisible
        {
            get { return alarmStorSignVisible; }
            set
            {
                alarmStorSignVisible = value;
                this.RaisePropertyChanged("AlarmStorSignVisible");
            }
        }
        private Visibility alarmKeySignVisible = Visibility.Collapsed;

        public Visibility AlarmKeySignVisible
        {
            get { return alarmKeySignVisible; }
            set
            {
                alarmKeySignVisible = value;
                this.RaisePropertyChanged("AlarmKeySignVisible");
            }
        }
        private Visibility overSpeedSetVisible = Visibility.Collapsed;

        public Visibility OverSpeedSetVisible
        {
            get { return overSpeedSetVisible; }
            set
            {
                overSpeedSetVisible = value;
                this.RaisePropertyChanged("OverSpeedSetVisible");
            }
        }
        private Visibility highSpeedVisible = Visibility.Collapsed;

        public Visibility HighSpeedVisible
        {
            get { return highSpeedVisible; }
            set
            {
                highSpeedVisible = value;
                this.RaisePropertyChanged("HighSpeedVisible");
            }
        }
        private Visibility overSpeedTimeVisible = Visibility.Collapsed;

        public Visibility OverSpeedTimeVisible
        {
            get { return overSpeedTimeVisible; }
            set
            {
                overSpeedTimeVisible = value;
                this.RaisePropertyChanged("OverSpeedTimeVisible");
            }
        }
        private Visibility fatigueDriveSet = Visibility.Collapsed;

        public Visibility FatigueDriveSet
        {
            get { return fatigueDriveSet; }
            set
            {
                fatigueDriveSet = value;
                this.RaisePropertyChanged("FatigueDriveSet");
            }
        }
        private Visibility continuousDriveTimeVisible = Visibility.Collapsed;

        public Visibility ContinuousDriveTimeVisible
        {
            get { return continuousDriveTimeVisible; }
            set
            {
                continuousDriveTimeVisible = value;
                this.RaisePropertyChanged("ContinuousDriveTimeVisible");
            }
        }
        private Visibility dayTotalTimeVisible = Visibility.Collapsed;

        public Visibility DayTotalTimeVisible
        {
            get { return dayTotalTimeVisible; }
            set
            {
                dayTotalTimeVisible = value;
                this.RaisePropertyChanged("DayTotalTimeVisible");
            }
        }
        private Visibility minRestTimeVisible = Visibility.Collapsed;

        public Visibility MinRestTimeVisible
        {
            get { return minRestTimeVisible; }
            set
            {
                minRestTimeVisible = value;
                this.RaisePropertyChanged("MinRestTimeVisible");
            }
        }
        private Visibility maxStopTimeVisible = Visibility.Collapsed;

        public Visibility MaxStopTimeVisible
        {
            get { return maxStopTimeVisible; }
            set
            {
                maxStopTimeVisible = value;
                this.RaisePropertyChanged("MaxStopTimeVisible");
            }
        }
        private Visibility photoParaVisible = Visibility.Collapsed;

        public Visibility PhotoParaVisible
        {
            get { return photoParaVisible; }
            set
            {
                photoParaVisible = value;
                this.RaisePropertyChanged("PhotoParaVisible");
            }
        }
        private Visibility imageVideoVisible = Visibility.Collapsed;

        public Visibility ImageVideoVisible
        {
            get { return imageVideoVisible; }
            set
            {
                imageVideoVisible = value;
                this.RaisePropertyChanged("ImageVideoVisible");
            }
        }
        private Visibility brightnessVisible = Visibility.Collapsed;

        public Visibility BrightnessVisible
        {
            get { return brightnessVisible; }
            set
            {
                brightnessVisible = value;
                this.RaisePropertyChanged("BrightnessVisible");
            }
        }
        private Visibility contrastVisible = Visibility.Collapsed;

        public Visibility ContrastVisible
        {
            get { return contrastVisible; }
            set
            {
                contrastVisible = value;
                this.RaisePropertyChanged("ContrastVisible");
            }
        }
        private Visibility saturationVisible = Visibility.Collapsed;

        public Visibility SaturationVisible
        {
            get { return saturationVisible; }
            set
            {
                saturationVisible = value;
                this.RaisePropertyChanged("SaturationVisible");
            }
        }
        private Visibility colorVisible = Visibility.Collapsed;

        public Visibility ColorVisible
        {
            get { return colorVisible; }
            set
            {
                colorVisible = value;
                this.RaisePropertyChanged("ColorVisible");
            }
        }
        private Visibility vehicleParaSetVisible = Visibility.Collapsed;

        public Visibility VehicleParaSetVisible
        {
            get { return vehicleParaSetVisible; }
            set
            {
                vehicleParaSetVisible = value;
                this.RaisePropertyChanged("VehicleParaSetVisible");
            }
        }
        private Visibility mileNumVisible = Visibility.Collapsed;

        public Visibility MileNumVisible
        {
            get { return mileNumVisible; }
            set
            {
                mileNumVisible = value;
                this.RaisePropertyChanged("MileNumVisible");
            }
        }
        private Visibility provinceIDVisible = Visibility.Collapsed;

        public Visibility ProvinceIDVisible
        {
            get { return provinceIDVisible; }
            set
            {
                provinceIDVisible = value;
                this.RaisePropertyChanged("ProvinceIDVisible");
            }
        }
        private Visibility cityIDVisible = Visibility.Collapsed;

        public Visibility CityIDVisible
        {
            get { return cityIDVisible; }
            set
            {
                cityIDVisible = value;
                this.RaisePropertyChanged("CityIDVisible");
            }
        }
        private Visibility vehicleNumVisible = Visibility.Collapsed;

        public Visibility VehicleNumVisible
        {
            get { return vehicleNumVisible; }
            set
            {
                vehicleNumVisible = value;
                this.RaisePropertyChanged("VehicleNumVisible");
            }
        }
        private Visibility vehicleColVisible = Visibility.Collapsed;

        public Visibility VehicleColVisible
        {
            get { return vehicleColVisible; }
            set
            {
                vehicleColVisible = value;
                this.RaisePropertyChanged("VehicleColVisible");
            }
        }
        private Visibility sendTextVisible = Visibility.Collapsed;

        public Visibility SendTextVisible
        {
            get { return sendTextVisible; }
            set
            {
                sendTextVisible = value;
                this.RaisePropertyChanged("SendTextVisible");
            }
        }
        private Visibility eventSetVisible = Visibility.Collapsed;

        public Visibility EventSetVisible
        {
            get { return eventSetVisible; }
            set
            {
                eventSetVisible = value;
                this.RaisePropertyChanged("EventSetVisible");
            }
        }
        private Visibility phoneSetVisible = Visibility.Collapsed;

        public Visibility PhoneSetVisible
        {
            get { return phoneSetVisible; }
            set
            {
                phoneSetVisible = value;
                this.RaisePropertyChanged("PhoneSetVisible");
            }
        }
        private Visibility regionLineSetVisible = Visibility.Collapsed;

        public Visibility RegionLineSetVisible
        {
            get { return regionLineSetVisible; }
            set
            {
                regionLineSetVisible = value;
                this.RaisePropertyChanged("RegionLineSetVisible");
            }
        }
        private Visibility setCircleVisible = Visibility.Collapsed;

        public Visibility SetCircleVisible
        {
            get { return setCircleVisible; }
            set
            {
                setCircleVisible = value;
                this.RaisePropertyChanged("SetCircleVisible");
            }
        }
        private Visibility setRectVisible = Visibility.Collapsed;

        public Visibility SetRectVisible
        {
            get { return setRectVisible; }
            set
            {
                setRectVisible = value;
                this.RaisePropertyChanged("SetRectVisible");
            }
        }
        private Visibility setPolyVisible = Visibility.Collapsed;

        public Visibility SetPolyVisible
        {
            get { return setPolyVisible; }
            set
            {
                setPolyVisible = value;
                this.RaisePropertyChanged("SetPolyVisible");
            }
        }
        private Visibility setLineVisible = Visibility.Collapsed;

        public Visibility SetLineVisible
        {
            get { return setLineVisible; }
            set
            {
                setLineVisible = value;
                this.RaisePropertyChanged("SetLineVisible");
            }
        }
        private Visibility deleteRegionVisible = Visibility.Collapsed;

        public Visibility DeleteRegionVisible
        {
            get { return deleteRegionVisible; }
            set
            {
                deleteRegionVisible = value;
                this.RaisePropertyChanged("DeleteRegionVisible");
            }
        }

        private Visibility shotNowVisible = Visibility.Collapsed;

        public Visibility ShotNowVisible
        {
            get { return shotNowVisible; }
            set
            {
                shotNowVisible = value;
                this.RaisePropertyChanged("ShotNowVisible");
            }
        }

        #endregion

        #region 华强指令
        private Visibility hqSettingVisible = Visibility.Collapsed;

        public Visibility HQSettingVisible
        {
            get { return hqSettingVisible; }
            set
            {
                hqSettingVisible = value;
                this.RaisePropertyChanged("HQSettingVisible");
            }
        }
        private Visibility hqNumberVisible = Visibility.Collapsed;

        public Visibility HQNumberVisible
        {
            get { return hqNumberVisible; }
            set
            {
                hqNumberVisible = value;
                this.RaisePropertyChanged("HQNumberVisible");
            }
        }
        private Visibility hqCodeVisible = Visibility.Collapsed;

        public Visibility HQCodeVisible
        {
            get { return hqCodeVisible; }
            set
            {
                hqCodeVisible = value;
                this.RaisePropertyChanged("HQCodeVisible");
            }
        }
        private Visibility hqAnswerVisible = Visibility.Collapsed;

        public Visibility HQAnswerVisible
        {
            get { return hqAnswerVisible; }
            set
            {
                hqAnswerVisible = value;
                this.RaisePropertyChanged("HQAnswerVisible");
            }
        }
        private Visibility hqOtherSetVisible = Visibility.Collapsed;

        public Visibility HQOtherSetVisible
        {
            get { return hqOtherSetVisible; }
            set
            {
                hqOtherSetVisible = value;
                this.RaisePropertyChanged("HQOtherSetVisible");
            }
        }
        private Visibility hqSetSwitchVisible = Visibility.Collapsed;

        public Visibility HQSetSwitchVisible
        {
            get { return hqSetSwitchVisible; }
            set
            {
                hqSetSwitchVisible = value;
                this.RaisePropertyChanged("HQSetSwitchVisible");
            }
        }
        private Visibility hqSetAlarmRangeVisible = Visibility.Collapsed;

        public Visibility HQSetAlarmRangeVisible
        {
            get { return hqSetAlarmRangeVisible; }
            set
            {
                hqSetAlarmRangeVisible = value;
                this.RaisePropertyChanged("HQSetAlarmRangeVisible");
            }
        }
        private Visibility hqMessageMenuVisible = Visibility.Collapsed;

        public Visibility HQMessageMenuVisible
        {
            get { return hqMessageMenuVisible; }
            set
            {
                hqMessageMenuVisible = value;
                this.RaisePropertyChanged("HQMessageMenuVisible");
            }
        }
        private Visibility hqOperatorLogoVisible = Visibility.Collapsed;

        public Visibility HQOperatorLogoVisible
        {
            get { return hqOperatorLogoVisible; }
            set
            {
                hqOperatorLogoVisible = value;
                this.RaisePropertyChanged("HQOperatorLogoVisible");
            }
        }
        private Visibility hqCallLimitVisible = Visibility.Collapsed;

        public Visibility HQCallLimitVisible
        {
            get { return hqCallLimitVisible; }
            set
            {
                hqCallLimitVisible = value;
                this.RaisePropertyChanged("HQCallLimitVisible");
            }
        }
        private Visibility hqPolyAlarmRangeVisible = Visibility.Collapsed;

        public Visibility HQPolyAlarmRangeVisible
        {
            get { return hqPolyAlarmRangeVisible; }
            set
            {
                hqPolyAlarmRangeVisible = value;
                this.RaisePropertyChanged("HQPolyAlarmRangeVisible");
            }
        }
        private Visibility hqQueryPara1Visible = Visibility.Collapsed;

        public Visibility HQQueryPara1Visible
        {
            get { return hqQueryPara1Visible; }
            set
            {
                hqQueryPara1Visible = value;
                this.RaisePropertyChanged("HQQueryPara1Visible");
            }
        }
        private Visibility hqQueryPara2Visible = Visibility.Collapsed;

        public Visibility HQQueryPara2Visible
        {
            get { return hqQueryPara2Visible; }
            set
            {
                hqQueryPara2Visible = value;
                this.RaisePropertyChanged("HQQueryPara2Visible");
            }
        }
        private Visibility hqQueryProductIDVisible = Visibility.Collapsed;

        public Visibility HQQueryProductIDVisible
        {
            get { return hqQueryProductIDVisible; }
            set
            {
                hqQueryProductIDVisible = value;
                this.RaisePropertyChanged("HQQueryProductIDVisible");
            }
        }
        private Visibility hqSetProductIDVisible = Visibility.Collapsed;

        public Visibility HQSetProductIDVisible
        {
            get { return hqSetProductIDVisible; }
            set
            {
                hqSetProductIDVisible = value;
                this.RaisePropertyChanged("HQSetProductIDVisible");
            }
        }
        private Visibility hqSetParaVisible = Visibility.Collapsed;

        public Visibility HQSetParaVisible
        {
            get { return hqSetParaVisible; }
            set
            {
                hqSetParaVisible = value;
                this.RaisePropertyChanged("HQSetParaVisible");
            }
        }
        private Visibility hqControlVisible = Visibility.Collapsed;

        public Visibility HQControlVisible
        {
            get { return hqControlVisible; }
            set
            {
                hqControlVisible = value;
                this.RaisePropertyChanged("HQControlVisible");
            }
        }
        private Visibility hqWorkStateVisible = Visibility.Collapsed;

        public Visibility HQWorkStateVisible
        {
            get { return hqWorkStateVisible; }
            set
            {
                hqWorkStateVisible = value;
                this.RaisePropertyChanged("HQWorkStateVisible");
            }
        }
        private Visibility hqLockCarVisible = Visibility.Collapsed;

        public Visibility HQLockCarVisible
        {
            get { return hqLockCarVisible; }
            set
            {
                hqLockCarVisible = value;
                this.RaisePropertyChanged("HQLockCarVisible");
            }
        }
        private Visibility hqMonitorVisible = Visibility.Collapsed;

        public Visibility HQMonitorVisible
        {
            get { return hqMonitorVisible; }
            set
            {
                hqMonitorVisible = value;
                this.RaisePropertyChanged("HQMonitorVisible");
            }
        }
        private Visibility hqTerminalSetVisible = Visibility.Collapsed;

        public Visibility HQTerminalSetVisible
        {
            get { return hqTerminalSetVisible; }
            set
            {
                hqTerminalSetVisible = value;
                this.RaisePropertyChanged("HQTerminalSetVisible");
            }
        }
        private Visibility hqNoRangeCallVisible = Visibility.Collapsed;

        public Visibility HQNoRangeCallVisible
        {
            get { return hqNoRangeCallVisible; }
            set
            {
                hqNoRangeCallVisible = value;
                this.RaisePropertyChanged("HQNoRangeCallVisible");
            }
        }
        private Visibility hqRemoveTerminalAlarmVisible = Visibility.Collapsed;

        public Visibility HQRemoveTerminalAlarmVisible
        {
            get { return hqRemoveTerminalAlarmVisible; }
            set
            {
                hqRemoveTerminalAlarmVisible = value;
                this.RaisePropertyChanged("HQRemoveTerminalAlarmVisible");
            }
        }
        private Visibility hqQueryVoltageVisible = Visibility.Collapsed;

        public Visibility HQQueryVoltageVisible
        {
            get { return hqQueryVoltageVisible; }
            set
            {
                hqQueryVoltageVisible = value;
                this.RaisePropertyChanged("HQQueryVoltageVisible");
            }
        }
        private Visibility hqGPRSVisible = Visibility.Collapsed;

        public Visibility HQGPRSVisible
        {
            get { return hqGPRSVisible; }
            set
            {
                hqGPRSVisible = value;
                this.RaisePropertyChanged("HQGPRSVisible");
            }
        }
        private Visibility hqQueryMileVisible = Visibility.Collapsed;

        public Visibility HQQueryMileVisible
        {
            get { return hqQueryMileVisible; }
            set
            {
                hqQueryMileVisible = value;
                this.RaisePropertyChanged("HQQueryMileVisible");
            }
        }
        private Visibility hqCompressionBackVisible = Visibility.Collapsed;

        public Visibility HQCompressionBackVisible
        {
            get { return hqCompressionBackVisible; }
            set
            {
                hqCompressionBackVisible = value;
                this.RaisePropertyChanged("HQCompressionBackVisible");
            }
        }
        private Visibility hqDistanceBackVisible = Visibility.Collapsed;

        public Visibility HQDistanceBackVisible
        {
            get { return hqDistanceBackVisible; }
            set
            {
                hqDistanceBackVisible = value;
                this.RaisePropertyChanged("HQDistanceBackVisible");
            }
        }
        private Visibility hqOneCallVisible = Visibility.Collapsed;

        public Visibility HQOneCallVisible
        {
            get { return hqOneCallVisible; }
            set
            {
                hqOneCallVisible = value;
                this.RaisePropertyChanged("HQOneCallVisible");
            }
        }
        private Visibility hqFixSpeedBackVisible = Visibility.Collapsed;

        public Visibility HQFixSpeedBackVisible
        {
            get { return hqFixSpeedBackVisible; }
            set
            {
                hqFixSpeedBackVisible = value;
                this.RaisePropertyChanged("HQFixSpeedBackVisible");
            }
        }
        private Visibility hqStandByVisible = Visibility.Collapsed;

        public Visibility HQStandByVisible
        {
            get { return hqStandByVisible; }
            set
            {
                hqStandByVisible = value;
                this.RaisePropertyChanged("HQStandByVisible");
            }
        }
        private Visibility hqEmptyBackVisible = Visibility.Collapsed;

        public Visibility HQEmptyBackVisible
        {
            get { return hqEmptyBackVisible; }
            set
            {
                hqEmptyBackVisible = value;
                this.RaisePropertyChanged("HQEmptyBackVisible");
            }
        }
        private Visibility hqAnyInformationVisible = Visibility.Collapsed;

        public Visibility HQAnyInformationVisible
        {
            get { return hqAnyInformationVisible; }
            set
            {
                hqAnyInformationVisible = value;
                this.RaisePropertyChanged("HQAnyInformationVisible");
            }
        }
        private Visibility hqNetParaVisible = Visibility.Collapsed;

        public Visibility HQNetParaVisible
        {
            get { return hqNetParaVisible; }
            set
            {
                hqNetParaVisible = value;
                this.RaisePropertyChanged("HQNetParaVisible");
            }
        }
        private Visibility hqIPandPortVisible = Visibility.Collapsed;

        public Visibility HQIPandPortVisible
        {
            get { return hqIPandPortVisible; }
            set
            {
                hqIPandPortVisible = value;
                this.RaisePropertyChanged("HQIPandPortVisible");
            }
        }
        private Visibility hqViceIPandPortVisible = Visibility.Collapsed;

        public Visibility HQViceIPandPortVisible
        {
            get { return hqViceIPandPortVisible; }
            set
            {
                hqViceIPandPortVisible = value;
                this.RaisePropertyChanged("HQViceIPandPortVisible");
            }
        }
        private Visibility hqUserCodeVisible = Visibility.Collapsed;

        public Visibility HQUserCodeVisible
        {
            get { return hqUserCodeVisible; }
            set
            {
                hqUserCodeVisible = value;
                this.RaisePropertyChanged("HQUserCodeVisible");
            }
        }
        private Visibility hqOffLineGPRSVisible = Visibility.Collapsed;

        public Visibility HQOffLineGPRSVisible
        {
            get { return hqOffLineGPRSVisible; }
            set
            {
                hqOffLineGPRSVisible = value;
                this.RaisePropertyChanged("HQOffLineGPRSVisible");
            }
        }
        private Visibility hqUDPportVisible = Visibility.Collapsed;

        public Visibility HQUDPportVisible
        {
            get { return hqUDPportVisible; }
            set
            {
                hqUDPportVisible = value;
                this.RaisePropertyChanged("HQUDPportVisible");
            }
        }
        private Visibility hqMultimediaVisible = Visibility.Collapsed;

        public Visibility HQMultimediaVisible
        {
            get { return hqMultimediaVisible; }
            set
            {
                hqMultimediaVisible = value;
                this.RaisePropertyChanged("HQMultimediaVisible");
            }
        }
        private Visibility hqOneImageVisible = Visibility.Collapsed;

        public Visibility HQOneImageVisible
        {
            get { return hqOneImageVisible; }
            set
            {
                hqOneImageVisible = value;
                this.RaisePropertyChanged("HQOneImageVisible");
            }
        }
        private Visibility hqSomeImageVisible = Visibility.Collapsed;

        public Visibility HQSomeImageVisible
        {
            get { return hqSomeImageVisible; }
            set
            {
                hqSomeImageVisible = value;
                this.RaisePropertyChanged("HQSomeImageVisible");
            }
        }
        private Visibility hqEmegentPicVisible = Visibility.Collapsed;

        public Visibility HQEmegentPicVisible
        {
            get { return hqEmegentPicVisible; }
            set
            {
                hqEmegentPicVisible = value;
                this.RaisePropertyChanged("HQEmegentPicVisible");
            }
        }
        private Visibility hqTimerShotVisible = Visibility.Collapsed;

        public Visibility HQTimerShotVisible
        {
            get { return hqTimerShotVisible; }
            set
            {
                hqTimerShotVisible = value;
                this.RaisePropertyChanged("HQTimerShotVisible");
            }
        }

        #endregion

        //初始化指令权限
        private void InitInstrutionRight()
        {
            if (StaticTreeState.InstructionInfo == LoadingState.LOADCOMPLETE)
            {
                InstructionRight rightInstance = InstructionRight.GetInstance();
                foreach (RightInfo item in rightInstance.ListInstructionRight)
                {
                    switch (item.ID)
                    {
                        case "6":
                            SendInstructionVisible = Visibility.Visible;
                            break;
                        case "61":
                            TerminalParaSetVisible = Visibility.Visible;
                            break;
                        case "611":
                            HeartBeatVisible = Visibility.Visible;
                            break;
                        case "612":
                            TCPMessageSetVisible = Visibility.Visible;
                            break;
                        case "6121":
                            TCPResponseVisible = Visibility.Visible;
                            break;
                        case "6122":
                            TCPAgainVisible = Visibility.Visible;
                            break;
                        case "613":
                            UDPMessageSetVisible = Visibility.Visible;
                            break;
                        case "6131":
                            UDPResponseVisible = Visibility.Visible;
                            break;
                        case "6132":
                            UDPAgainVisible = Visibility.Visible;
                            break;
                        case "614":
                            SMSMessageSetVisible = Visibility.Visible;
                            break;
                        case "6141":
                            SMSResponseVisible = Visibility.Visible;
                            break;
                        case "6142":
                            SMSAgainVisible = Visibility.Visible;
                            break;
                        case "62":
                            ServerSetVisible = Visibility.Visible;
                            break;
                        case "621":
                            MainServerVisible = Visibility.Visible;
                            break;
                        case "6211":
                            MainSiteVisible = Visibility.Visible;
                            break;
                        case "6212":
                            MainUserVisible = Visibility.Visible;
                            break;
                        case "6213":
                            MainCodeVisible = Visibility.Visible;
                            break;
                        case "6214":
                            MainIPVisible = Visibility.Visible;
                            break;
                        case "622":
                            ViceServerVisible = Visibility.Visible;
                            break;
                        case "6221":
                            ViceSiteVisible = Visibility.Visible;
                            break;
                        case "6222":
                            ViceUserVisible = Visibility.Visible;
                            break;
                        case "6223":
                            ViceCodeVisible = Visibility.Visible;
                            break;
                        case "6224":
                            ViceIPVisible = Visibility.Visible;
                            break;
                        case "623":
                            TCPSiteVisible = Visibility.Visible;
                            break;
                        case "624":
                            UDPSiteVisible = Visibility.Visible;
                            break;
                        case "63":
                            PosSetVisible = Visibility.Visible;
                            break;
                        case "631":
                            PosStrategyVisible = Visibility.Visible;
                            break;
                        case "632":
                            PosSolutionVisible = Visibility.Visible;
                            break;
                        case "633":
                            ReportTimeSetVisible = Visibility.Visible;
                            break;
                        case "6331":
                            UnloginTimeVisible = Visibility.Visible;
                            break;
                        case "6332":
                            RestTimeVisible = Visibility.Visible;
                            break;
                        case "6333":
                            EmergenceTimeVisible = Visibility.Visible;
                            break;
                        case "6334":
                            LackTimeVisible = Visibility.Visible;
                            break;
                        case "634":
                            ReportDistanceSetVisible = Visibility.Visible;
                            break;
                        case "6341":
                            UnloginDistanceVisible = Visibility.Visible;
                            break;
                        case "6342":
                            RestDistanceVisible = Visibility.Visible;
                            break;
                        case "6343":
                            EmergenceDistanceVisible = Visibility.Visible;
                            break;
                        case "6344":
                            LackDistanceVisible = Visibility.Visible;
                            break;
                        case "635":
                            InflectionAngleVisible = Visibility.Visible;
                            break;
                        case "64":
                            CallSetVisible = Visibility.Visible;
                            break;
                        case "641":
                            PlatformNumVisible = Visibility.Visible;
                            break;
                        case "642":
                            RetNumVisible = Visibility.Visible;
                            break;
                        case "643":
                            FactorySettingNumVisible = Visibility.Visible;
                            break;
                        case "644":
                            PlatformSMSNumVisible = Visibility.Visible;
                            break;
                        case "645":
                            TerminalSMSNumVisible = Visibility.Visible;
                            break;
                        case "646":
                            TerminalStrategyVisible = Visibility.Visible;
                            break;
                        case "647":
                            OnceMaxTimeVisible = Visibility.Visible;
                            break;
                        case "648":
                            MonthMaxTimeVisible = Visibility.Visible;
                            break;
                        case "649":
                            MonitorNumVisible = Visibility.Visible;
                            break;
                        case "64A":
                            PrivilegeNumVisible = Visibility.Visible;
                            break;
                        case "65":
                            AlarmSetVisible = Visibility.Visible;
                            break;
                        case "651":
                            AlarmShieldVisible = Visibility.Visible;
                            break;
                        case "652":
                            AlarmTextSMSVisible = Visibility.Visible;
                            break;
                        case "653":
                            AlarmShootSwitchVisible = Visibility.Visible;
                            break;
                        case "654":
                            AlarmStorSignVisible = Visibility.Visible;
                            break;
                        case "655":
                            AlarmKeySignVisible = Visibility.Visible;
                            break;
                        case "66":
                            OverSpeedSetVisible = Visibility.Visible;
                            break;
                        case "661":
                            HighSpeedVisible = Visibility.Visible;
                            break;
                        case "662":
                            OverSpeedTimeVisible = Visibility.Visible;
                            break;
                        case "67":
                            FatigueDriveSet = Visibility.Visible;
                            break;
                        case "671":
                            ContinuousDriveTimeVisible = Visibility.Visible;
                            break;
                        case "672":
                            DayTotalTimeVisible = Visibility.Visible;
                            break;
                        case "673":
                            MinRestTimeVisible = Visibility.Visible;
                            break;
                        case "674":
                            MaxStopTimeVisible = Visibility.Visible;
                            break;
                        case "68":
                            PhotoParaVisible = Visibility.Visible;
                            break;
                        case "681":
                            ImageVideoVisible = Visibility.Visible;
                            break;
                        case "682":
                            BrightnessVisible = Visibility.Visible;
                            break;
                        case "683":
                            ContrastVisible = Visibility.Visible;
                            break;
                        case "684":
                            SaturationVisible = Visibility.Visible;
                            break;
                        case "685":
                            ColorVisible = Visibility.Visible;
                            break;
                        case "69":
                            VehicleParaSetVisible = Visibility.Visible;
                            break;
                        case "691":
                            MileNumVisible = Visibility.Visible;
                            break;
                        case "692":
                            ProvinceIDVisible = Visibility.Visible;
                            break;
                        case "693":
                            CityIDVisible = Visibility.Visible;
                            break;
                        case "694":
                            VehicleNumVisible = Visibility.Visible;
                            break;
                        case "695":
                            VehicleColVisible = Visibility.Visible;
                            break;
                        case "6A":
                            SendTextVisible = Visibility.Visible;
                            break;
                        case "6B":
                            EventSetVisible = Visibility.Visible;
                            break;
                        case "6C":
                            PhoneSetVisible = Visibility.Visible;
                            break;
                        case "6D":
                            RegionLineSetVisible = Visibility.Visible;
                            break;
                        case "6D1":
                            SetCircleVisible = Visibility.Visible;
                            break;
                        case "6D2":
                            SetRectVisible = Visibility.Visible;
                            break;
                        case "6D3":
                            SetPolyVisible = Visibility.Visible;
                            break;
                        case "6D4":
                            SetLineVisible = Visibility.Visible;
                            break;
                        case "6D5":
                            DeleteRegionVisible = Visibility.Visible;
                            break;
                        case "6E":
                            HQSettingVisible = Visibility.Visible;
                            break;
                        case "6E1":
                            HQNumberVisible = Visibility.Visible;
                            break;
                        case "6E2":
                            HQCodeVisible = Visibility.Visible;
                            break;
                        case "6E3":
                            HQAnswerVisible = Visibility.Visible;
                            break;
                        case "6E4":
                            HQOtherSetVisible = Visibility.Visible;
                            break;
                        case "6E5":
                            HQSetSwitchVisible = Visibility.Visible;
                            break;
                        case "6E6":
                            HQSetAlarmRangeVisible = Visibility.Visible;
                            break;
                        case "6E7":
                            HQMessageMenuVisible = Visibility.Visible;
                            break;
                        case "6E8":
                            HQOperatorLogoVisible = Visibility.Visible;
                            break;
                        case "6E9":
                            HQCallLimitVisible = Visibility.Visible;
                            break;
                        case "6EA":
                            HQPolyAlarmRangeVisible = Visibility.Visible;
                            break;
                        case "6EB":
                            HQQueryPara1Visible = Visibility.Visible;
                            break;
                        case "6EC":
                            HQQueryPara2Visible = Visibility.Visible;
                            break;
                        case "6ED":
                            HQQueryProductIDVisible = Visibility.Visible;
                            break;
                        case "6EE":
                            HQSetProductIDVisible = Visibility.Visible;
                            break;
                        case "6EF":
                            HQSetParaVisible = Visibility.Visible;
                            break;
                        case "6F":
                            HQControlVisible = Visibility.Visible;
                            break;
                        case "6F1":
                            HQWorkStateVisible = Visibility.Visible;
                            break;
                        case "6F2":
                            HQLockCarVisible = Visibility.Visible;
                            break;
                        case "6F3":
                            HQMonitorVisible = Visibility.Visible;
                            break;
                        case "6F4":
                            HQTerminalSetVisible = Visibility.Visible;
                            break;
                        case "6F5":
                            HQNoRangeCallVisible = Visibility.Visible;
                            break;
                        case "6F6":
                            HQRemoveTerminalAlarmVisible = Visibility.Visible;
                            break;
                        case "6F7":
                            HQQueryVoltageVisible = Visibility.Visible;
                            break;
                        case "6F8":
                            HQGPRSVisible = Visibility.Visible;
                            break;
                        case "6F9":
                            HQQueryMileVisible = Visibility.Visible;
                            break;
                        case "6FA":
                            HQCompressionBackVisible = Visibility.Visible;
                            break;
                        case "6FB":
                            HQDistanceBackVisible = Visibility.Visible;
                            break;
                        case "6FC":
                            HQOneCallVisible = Visibility.Visible;
                            break;
                        case "6FD":
                            HQFixSpeedBackVisible = Visibility.Visible;
                            break;
                        case "6FE":
                            HQStandByVisible = Visibility.Visible;
                            break;
                        case "6FF":
                            HQEmptyBackVisible = Visibility.Visible;
                            break;
                        case "6FG":
                            HQAnyInformationVisible = Visibility.Visible;
                            break;
                        case "6G":
                            HQNetParaVisible = Visibility.Visible;
                            break;
                        case "6G1":
                            HQIPandPortVisible = Visibility.Visible;
                            break;
                        case "6G2":
                            HQViceIPandPortVisible = Visibility.Visible;
                            break;
                        case "6G3":
                            HQUserCodeVisible = Visibility.Visible;
                            break;
                        case "6G4":
                            HQOffLineGPRSVisible = Visibility.Visible;
                            break;
                        case "6G5":
                            HQUDPportVisible = Visibility.Visible;
                            break;
                        case "6H":
                            HQMultimediaVisible = Visibility.Visible;
                            break;
                        case "6H1":
                            HQOneImageVisible = Visibility.Visible;
                            break;
                        case "6H2":
                            HQSomeImageVisible = Visibility.Visible;
                            break;
                        case "6H3":
                            HQEmegentPicVisible = Visibility.Visible;
                            break;
                        case "6H4":
                            HQTimerShotVisible = Visibility.Visible;
                            break;
                        case "6I":
                            ShotNowVisible = Visibility.Visible;
                            break;
                    }
                }
            }
        }

        #endregion
    }
}
