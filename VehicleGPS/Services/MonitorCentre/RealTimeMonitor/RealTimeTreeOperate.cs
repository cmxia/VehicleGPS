using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VehicleGPS.Models;
using System.Threading;
using VehicleGPS.ViewModels.MonitorCentre.RealTimeMonitor;
using System.Windows;
using VehicleGPS.Models.Login;

namespace VehicleGPS.Services.MonitorCentre
{
    class RealTimeTreeOperate
    {
        public void ReadTree()
        {
            this.RefreshTree();
        }

        #region 准备初始化
        /// <summary>
        /// 初始化树形结构(登录初始化加载了的此处就不加载了)
        /// </summary>
        public void RefreshTree()
        {
            /*获取单位的基本信息*/
            if (StaticTreeState.ClientBasicInfo != LoadingState.LOADCOMPLETE)
            {
                StaticBasicInfo cvBasicInfo = StaticBasicInfo.GetInstance();
                cvBasicInfo.RefreshClientBasicInfo();
            }
            /*获取车辆的基本信息*/
            if (StaticTreeState.VehicleAllBasicInfo != LoadingState.LOADCOMPLETE)
            {
                StaticBasicInfo cvBasicInfo = StaticBasicInfo.GetInstance();
                cvBasicInfo.RefreshVehicleDetailInfo();
            }
            /*获取车辆最近GPS信息*/
            if (StaticTreeState.VehicleGPSInfo != LoadingState.LOADCOMPLETE)
            {
                StaticDetailInfo cvDetailInfo = StaticDetailInfo.GetInstance();
                cvDetailInfo.RefreshGPSInfo();
            }
            /*启动线程初始化树形*/
            Thread initTreeThread = new Thread(new ThreadStart(this.InitClientVehicleTree));
            initTreeThread.Start();
        }
        #endregion

        #region 初始化
        /// <summary>
        /// 初始化
        /// </summary>
        private void InitClientVehicleTree()
        {
            /*创建根节点*/
            RealTimeTreeNodeViewModel RootNode = new RealTimeTreeNodeViewModel();
            RootNode.NodeInfo = new CVBasicInfo();
            RootNode.NodeInfo.ID = "admin";
            RootNode.ListChildNodes = new List<RealTimeTreeNodeViewModel>();

            /*加载用户车辆基础信息*/
            List<RealTimeTreeNodeViewModel> clientNodeList = new List<RealTimeTreeNodeViewModel>();
            List<RealTimeTreeNodeViewModel> vehicleNodeList = new List<RealTimeTreeNodeViewModel>();
            StaticBasicInfo basicInfo = StaticBasicInfo.GetInstance();

            bool loadSucess = false;
            int loadTimesCounter = 1;
            int loadTiems = 15;
            while (Monitor.TryEnter(StaticTreeState.ClientBasicMutex, 10000))
            {//等待10秒
                if (StaticTreeState.ClientBasicInfo != LoadingState.LOADCOMPLETE)
                {//加载用户失败
                    Monitor.Exit(StaticTreeState.ClientBasicMutex);
                    Thread.Sleep(200);
                    if (loadTimesCounter++ == loadTiems)//尝试15次
                    {
                        break;
                    }
                    continue;
                }
                if (StaticTreeState.ClientBasicInfo == LoadingState.LOADCOMPLETE)
                {
                    /*获取用户节点*/
                    foreach (CVBasicInfo cbi in basicInfo.ListClientBasicInfo)
                    {
                        RealTimeTreeNodeViewModel clientNode = new RealTimeTreeNodeViewModel();
                        InitClientNode(clientNode, cbi);
                        clientNodeList.Add(clientNode);
                    }
                    int rollTimes = 0;//轮询次数，防止脏数据导致陷入死循环
                    while (UnUseCount(clientNodeList) != 0)
                    {
                        foreach (RealTimeTreeNodeViewModel vtnv in clientNodeList)
                        {
                            if (vtnv.isUsed == false)
                            {
                                if (AddClientTree(RootNode, vtnv))
                                {
                                    vtnv.isUsed = true;
                                }
                            }
                        }
                        if (++rollTimes == 5)//最多轮询五次
                        {
                            break;
                        }
                    }
                    loadSucess = true;
                }
                Monitor.Exit(StaticTreeState.ClientBasicMutex);
                break;
            }
            if (loadSucess == false)
            {//等待10秒超时或者加载用户信息失败
                StaticTreeState.ClientBasicInfo = LoadingState.LOADDINGFAIL;
                RealTimeTreeViewModel.GetInstance().LoadTreeFail();
                return;
            }
            RealTimeTreeViewModel.GetInstance().RootNode = RootNode;
            /*****************车辆信息**************/
            int _rollTimes = 0;//轮询次数，防止脏数据导致陷入死循环
            foreach (CVBasicInfo vbi in basicInfo.ListVehicleBasicInfo)
            {
                RealTimeTreeNodeViewModel vehicleNode = new RealTimeTreeNodeViewModel();
                InitVehicleNode(vehicleNode, vbi);
                vehicleNodeList.Add(vehicleNode);
            }
            while (UnUseCount(vehicleNodeList) != 0)
            {
                foreach (RealTimeTreeNodeViewModel vtnv in vehicleNodeList)
                {
                    if (vtnv.isUsed == false)
                    {
                        if (AddVehicleTree(RootNode, vtnv))
                        {
                            vtnv.isUsed = true;
                        }
                    }
                }
                if (++_rollTimes == 1)//最多轮询1次
                {
                    break;
                }
            }
            //如果没找到父节点，全部放到根节点下一节点（我的商砼）目录下
            if (UnUseCount(vehicleNodeList) != 0)
            {
                try
                {
                    foreach (RealTimeTreeNodeViewModel vtnv in vehicleNodeList)
                    {
                        if (vtnv.isUsed == false)
                        {
                            vtnv.parentNode = new RealTimeTreeNodeViewModel();
                            vtnv.parentNode = RootNode.listChildNodes[0];//将其父节点设为（我的商砼）
                            vtnv.nodeInfo.ParentID = RootNode.listChildNodes[0].nodeInfo.ID;
                            if (AddVehicleTree(RootNode.listChildNodes[0], vtnv))
                            {
                                vtnv.isUsed = true;
                            }
                        }
                    }
                }
                catch (Exception)
                {
                    throw;
                }
            }
            RealTimeTreeViewModel.GetInstance().RootNode = RootNode;
            RealTimeTreeViewModel.GetInstance().InitAutoComplete();/*初始化AutoComplete查询数据集*/
            if (RootNode.ListChildNodes.Count > 0)
            {
                RootNode.listChildNodes[0].isExpand = true;
            }


            /*更新车辆状态*/
            Thread initCarOnlineState = new Thread(new ThreadStart(this.InitCarOnlineState));
            initCarOnlineState.Start();
        }

        #endregion

        #region 获取未使用的节点
        /*获取未使用的节点*/
        private int UnUseCount(List<RealTimeTreeNodeViewModel> nodeList)
        {
            int count = 0;
            foreach (RealTimeTreeNodeViewModel vtnv in nodeList)
            {
                if (vtnv.isUsed == false)
                {
                    count++;
                }
            }
            return count;
        }
        #endregion

        #region 初始化车辆数据
        public void InitCarOnlineState()
        {
            int tryTimes = 0;
            while (Monitor.TryEnter(StaticTreeState.VehicleGPSInfoMutex, 10000))
            {
                if (StaticTreeState.VehicleGPSInfo != LoadingState.LOADCOMPLETE)
                {
                    Monitor.Exit(StaticTreeState.VehicleGPSInfoMutex);
                    Thread.Sleep(500);
                    if (tryTimes++ == 5)//尝试5次
                    {
                        break;
                    }
                    continue;
                }
                else
                {
                    InitVehicleState(RealTimeTreeViewModel.GetInstance().rootNode);
                    Monitor.Exit(StaticTreeState.VehicleGPSInfoMutex);
                    break;
                }
            }
        }
        #endregion


        #region 计算用户下的总车辆数以及车辆状态
        /*计算用户下的总车辆数以及车辆状态*/
        private string[] InitVehicleState(RealTimeTreeNodeViewModel rootNode)
        {//返回类型string格式[1]count[2]onlineCount
            int count = 0;
            int onlineCount = 0;
            if (rootNode.ListChildNodes != null)
            {
                try
                {  
                    foreach (RealTimeTreeNodeViewModel vtnv in rootNode.ListChildNodes)
                    {
                        if (vtnv.onlineNumberVisible == Visibility.Collapsed)
                        {//车辆
                            count++;
                            foreach (CVDetailInfo detailInfo in StaticDetailInfo.GetInstance().ListVehicleDetailInfo)
                            {
                                if (detailInfo != null && vtnv.nodeInfo.SIM.Equals(detailInfo.SIM))
                                {
                                    if (detailInfo.VehicleGPSInfo != null)
                                    {
                                        if (detailInfo.VehicleGPSInfo.OnlineStates == VehicleCommon.VSOnlineOff)
                                        {//离线

                                        }
                                        if (detailInfo.VehicleGPSInfo.OnlineStates == VehicleCommon.VSOnlinePark)
                                        {//停车
                                            onlineCount++;
                                        }
                                        if (detailInfo.VehicleGPSInfo.OnlineStates == VehicleCommon.VSOnlineRun)
                                        {//行驶中
                                            onlineCount++;
                                        }
                                        if (detailInfo.VehicleGPSInfo.OnlineStates == VehicleCommon.VSOnlineWarn)
                                        {//报警
                                            onlineCount++;
                                        }
                                        vtnv.nodeInfo.TypeID = detailInfo.VehicleType;
                                        vtnv.ImageUrl = VehicleCommon.GetVehicleImageURL(vtnv.nodeInfo.TypeID, detailInfo.VehicleGPSInfo.OnlineStates, detailInfo.VehicleGPSInfo.Datetime);
                                        vtnv.ImageTip = VehicleCommon.GetVehicleImageTip(vtnv.nodeInfo.TypeID, detailInfo.VehicleGPSInfo.OnlineStates, detailInfo.VehicleGPSInfo.Datetime);
                                        //vtnv.ImageUrl = VehicleCommon.GetVehicleImageURL(detailInfo.VehicleType, detailInfo.VehicleGPSInfo.OnlineStates);
                                        //vtnv.ImageTip = VehicleCommon.GetVehicleImageTip(detailInfo.VehicleType, detailInfo.VehicleGPSInfo.OnlineStates);
                                    }
                                    else
                                    {
                                        //不确定状态的车辆
                                    }
                                    break;
                                }
                            }
                        }
                        else
                        {//用户单位
                            string[] ret = InitVehicleState(vtnv);
                            count += Convert.ToInt32(ret[0]);
                            onlineCount += Convert.ToInt32(ret[1]);
                        }
                    }
                }
                catch (Exception e)
                {  

                }
            }
            rootNode.NodeInfo.OnlineCount = onlineCount;
            rootNode.NodeInfo.Count = count;
            rootNode.RaiseNodeInfoChanged();//通知节点信息发生变化
            string[] retStrs = new string[2];
            retStrs[0] = count.ToString();
            retStrs[1] = onlineCount.ToString();
            return retStrs;
        }
        #endregion

        #region 用户节点
        /*添加单位、站点、车队节点*/
        private bool AddClientTree(RealTimeTreeNodeViewModel node, RealTimeTreeNodeViewModel t_node)
        {
            if (node.NodeInfo.ID == t_node.NodeInfo.ParentID)
            {
                if (node.ListChildNodes == null)
                {
                    node.ListChildNodes = new List<RealTimeTreeNodeViewModel>();
                }
                t_node.parentNode = node;
                node.ListChildNodes.Add(t_node);
                return true;
            }
            else
            {
                if (node.ListChildNodes != null)
                {
                    for (int i = 0; i < node.ListChildNodes.Count; i++)
                    {
                        RealTimeTreeNodeViewModel vtnv = node.ListChildNodes[i];
                        if (AddClientTree(vtnv, t_node))
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        /*初始化单位、车队、经营商、站点节点*/
        private void InitClientNode(RealTimeTreeNodeViewModel clientNode, CVBasicInfo cbi)
        {
            clientNode.nodeInfo = cbi;
            clientNode.isSelected = false;
            clientNode.isUsed = false;
            clientNode.parentNode = null;
            clientNode.ListChildNodes = null;
            clientNode.onlineNumberVisible = Visibility.Visible;
            clientNode.innerIDVisible = Visibility.Collapsed;
            clientNode.nameVisible = Visibility.Visible;
        }
        #endregion

        #region 车辆节点
        /*添加车辆节点*/
        private bool AddVehicleTree(RealTimeTreeNodeViewModel node, RealTimeTreeNodeViewModel t_node)
        {
            if (node.NodeInfo.ID == t_node.NodeInfo.ParentID)
            {
                if (node.ListChildNodes == null)
                {
                    node.ListChildNodes = new List<RealTimeTreeNodeViewModel>();
                }
                t_node.parentNode = node;
                node.ListChildNodes.Add(t_node);
                return true;
            }
            else
            {
                if (node.ListChildNodes != null)
                {
                    for (int i = 0; i < node.ListChildNodes.Count; i++)
                    {
                        RealTimeTreeNodeViewModel vtnv = node.ListChildNodes[i];
                        if (AddVehicleTree(vtnv, t_node))
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }
        /*初始化车辆节点*/
        private void InitVehicleNode(RealTimeTreeNodeViewModel vehicleNode, CVBasicInfo vbi)
        {
            vehicleNode.nodeInfo = vbi;
            vehicleNode.isSelected = false;
            vehicleNode.isUsed = false;
            vehicleNode.parentNode = null;
            vehicleNode.listChildNodes = null;
            vehicleNode.onlineNumberVisible = Visibility.Collapsed;
            vehicleNode.innerIDVisible = Visibility.Visible;
            vehicleNode.nameVisible = Visibility.Visible;
            vehicleNode.imageUrl = "pack://application:,,,/Images/Car/offline.png";
            vehicleNode.ImageTip = "离线";
        }
        #endregion
    }
}
