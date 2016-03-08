using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VehicleGPS.Models;
using System.Threading;
using VehicleGPS.ViewModels.MonitorCentre.VehicleTrack;
using System.Windows;

namespace VehicleGPS.Services.MonitorCentre.VehicleTrack
{
    class VehicleTrackTreeOperate
    {
        private VehicleTrackTreeViewModel TreeViewModel { get; set; }
        public VehicleTrackTreeOperate(VehicleTrackTreeViewModel treeViewModel)
        {
            this.TreeViewModel = treeViewModel;
        }
        public void ReadTree()
        {
            this.RefreshTree();
        }
        public void RefreshTree()
        {
            ///*获取所有基本类型*/
            //if (StaticTreeState.BasicTypeInfo != LoadingState.LOADCOMPLETE)
            //{
            //    StaticBasicType basicType = StaticBasicType.GetInstance();
            //    basicType.RefreshBasicInfo();
            //}
            ///*获取车辆以及用户的基本信息*/
            //if (StaticTreeState.VehicleBasicInfo != LoadingState.LOADCOMPLETE
            //    || StaticTreeState.ClientBasicInfo != LoadingState.LOADCOMPLETE
            //    || StaticTreeState.VehicleAllBasicInfo != LoadingState.LOADCOMPLETE)
            //{
            //    StaticBasicInfo cvBasicInfo = StaticBasicInfo.GetInstance();
            //    cvBasicInfo.RefreshBasicInfo();
            //}
            ///*获取车辆最近GPS信息*/
            //if (StaticTreeState.VehicleGPSInfo != LoadingState.LOADCOMPLETE)
            //{
            //    StaticDetailInfo cvDetailInfo = StaticDetailInfo.GetInstance();
            //    cvDetailInfo.RefreshGPSInfo();
            //}

            /*获取车辆以及用户的基本信息*/
            if (StaticTreeState.ClientBasicInfo != LoadingState.LOADCOMPLETE
                || StaticTreeState.VehicleAllBasicInfo != LoadingState.LOADCOMPLETE)
            {
                StaticBasicInfo cvBasicInfo = StaticBasicInfo.GetInstance();
                cvBasicInfo.RefreshBasicInfo();
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
        /// <summary>
        /// 初始化
        /// </summary>
        private void InitClientVehicleTree()
        {
            //Thread.Sleep(1000);//确保树模型已经创建、获取车辆和用户基本信息线程已经启动
            /*创建根节点*/
            VehicleTrackTreeNodeViewModel rootNode = new VehicleTrackTreeNodeViewModel();
            rootNode.NodeInfo = new CVBasicInfo();
            rootNode.NodeInfo.ID = "admin";
            rootNode.ListChildNodes = new List<VehicleTrackTreeNodeViewModel>();
            /*加载用户基础信息*/
            List<VehicleTrackTreeNodeViewModel> clientNodeList = new List<VehicleTrackTreeNodeViewModel>();
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
                        VehicleTrackTreeNodeViewModel clientNode = new VehicleTrackTreeNodeViewModel();
                        InitClientNode(clientNode, cbi);
                        clientNodeList.Add(clientNode);
                    }
                    int rollTimes = 0;//轮询次数，防止脏数据导致陷入死循环
                    while (UnUseCount(clientNodeList) != 0)
                    {
                        foreach (VehicleTrackTreeNodeViewModel vtnv in clientNodeList)
                        {
                            if (vtnv.isUsed == false)
                            {
                                if (AddClientTree(rootNode, vtnv))
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
                this.TreeViewModel.LoadTreeFail();
                return;
            }
            this.TreeViewModel.RootNode = rootNode;//先显示用户树

            loadSucess = false;
            loadTimesCounter = 1;
          
                    /*获取与用户权限关联的车辆节点*/
                    List<VehicleTrackTreeNodeViewModel> vehicleNodeList = new List<VehicleTrackTreeNodeViewModel>();
                    foreach (CVBasicInfo vbi in basicInfo.ListVehicleBasicInfo)
                    {
                        VehicleTrackTreeNodeViewModel vehicleNode = new VehicleTrackTreeNodeViewModel();
                        InitVehicleNode(vehicleNode, vbi);
                        vehicleNodeList.Add(vehicleNode);
                    }
                    int _rollTimes = 0;//轮询次数，防止脏数据导致陷入死循环
                    while (UnUseCount(vehicleNodeList) != 0)
                    {
                        foreach (VehicleTrackTreeNodeViewModel vtnv in vehicleNodeList)
                        {
                            if (vtnv.isUsed == false)
                            {
                                if (AddVehicleTree(rootNode, vtnv))
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
                        foreach (VehicleTrackTreeNodeViewModel vtnv in vehicleNodeList)
                        {
                            if (vtnv.isUsed == false)
                            {
                                vtnv.parentNode = rootNode.listChildNodes[0];//将其父节点设为（我的商砼）
                                vtnv.nodeInfo.ParentID = rootNode.listChildNodes[0].nodeInfo.ID;
                                if (AddVehicleTree(rootNode.listChildNodes[0], vtnv))
                                {
                                    vtnv.isUsed = true;
                                }
                            }
                        }
                    }
                    loadSucess = true;
             
            if (loadSucess == false)
            {//等待10秒超时或者加载车辆失败
                StaticTreeState.VehicleBasicInfo = LoadingState.LOADDINGFAIL;
                this.TreeViewModel.LoadTreeFail();
                return;
            }
            this.TreeViewModel.RootNode = rootNode;
            this.TreeViewModel.InitAutoComplete();
            rootNode.listChildNodes[0].isExpand = true;//第一层展开
            ///*更新车辆状态*/
            Thread initCarOnlineState = new Thread(new ThreadStart(this.InitCarOnlineState));
            initCarOnlineState.Start();
        }
        /*初始化*/
        private void InitClientVehicleTree_old()
        {
            //Thread.Sleep(1000);//确保树模型已经创建、获取车辆和用户基本信息线程已经启动
            /*创建根节点*/
            VehicleTrackTreeNodeViewModel rootNode = new VehicleTrackTreeNodeViewModel();
            rootNode.NodeInfo = new CVBasicInfo();
            rootNode.NodeInfo.ID = "admin";
            rootNode.ListChildNodes = new List<VehicleTrackTreeNodeViewModel>();
            /*加载用户基础信息*/
            List<VehicleTrackTreeNodeViewModel> clientNodeList = new List<VehicleTrackTreeNodeViewModel>();
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
                        VehicleTrackTreeNodeViewModel clientNode = new VehicleTrackTreeNodeViewModel();
                        InitClientNode(clientNode, cbi);
                        clientNodeList.Add(clientNode);
                    }
                    int rollTimes = 0;//轮询次数，防止脏数据导致陷入死循环
                    while (UnUseCount(clientNodeList) != 0)
                    {
                        foreach (VehicleTrackTreeNodeViewModel vtnv in clientNodeList)
                        {
                            if (vtnv.isUsed == false)
                            {
                                if (AddClientTree(rootNode, vtnv))
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
                this.TreeViewModel.LoadTreeFail();
                return;
            }
            this.TreeViewModel.RootNode = rootNode;//先显示用户树

            loadSucess = false;
            loadTimesCounter = 1;
            while (Monitor.TryEnter(StaticTreeState.VehicleBasicMutex, 10000))
            {//等待10秒
                /*加载车辆基础信息*/
                if (StaticTreeState.VehicleBasicInfo != LoadingState.LOADCOMPLETE)
                {//加载车辆失败
                    Monitor.Exit(StaticTreeState.VehicleBasicMutex);
                    Thread.Sleep(200);
                    if (loadTimesCounter++ == loadTiems)//尝试15次
                    {
                        break;
                    }
                    return;
                }
                if (StaticTreeState.VehicleBasicInfo == LoadingState.LOADCOMPLETE)
                {
                    /*获取与用户权限关联的车辆节点*/
                    List<VehicleTrackTreeNodeViewModel> vehicleNodeList = new List<VehicleTrackTreeNodeViewModel>();
                    foreach (CVBasicInfo vbi in basicInfo.ListVehicleBasicInfo)
                    {
                        VehicleTrackTreeNodeViewModel vehicleNode = new VehicleTrackTreeNodeViewModel();
                        InitVehicleNode(vehicleNode, vbi);
                        vehicleNodeList.Add(vehicleNode);
                    }
                    int rollTimes = 0;//轮询次数，防止脏数据导致陷入死循环
                    while (UnUseCount(vehicleNodeList) != 0)
                    {
                        foreach (VehicleTrackTreeNodeViewModel vtnv in vehicleNodeList)
                        {
                            if (vtnv.isUsed == false)
                            {
                                if (AddVehicleTree(rootNode, vtnv))
                                {
                                    vtnv.isUsed = true;
                                }
                            }
                        }
                        if (++rollTimes == 1)//最多轮询1次
                        {
                            break;
                        }
                    }
                    //如果没找到父节点，全部放到根节点下一节点（我的商砼）目录下
                    if (UnUseCount(vehicleNodeList) != 0)
                    {
                        foreach (VehicleTrackTreeNodeViewModel vtnv in vehicleNodeList)
                        {
                            if (vtnv.isUsed == false)
                            {
                                vtnv.parentNode = rootNode.listChildNodes[0];//将其父节点设为（我的商砼）
                                vtnv.nodeInfo.ParentID = rootNode.listChildNodes[0].nodeInfo.ID;
                                if (AddVehicleTree(rootNode.listChildNodes[0], vtnv))
                                {
                                    vtnv.isUsed = true;
                                }
                            }
                        }
                    }
                    loadSucess = true;
                }
                Monitor.Exit(StaticTreeState.VehicleBasicMutex);
                break;
            }
            if (loadSucess == false)
            {//等待10秒超时或者加载车辆失败
                StaticTreeState.VehicleBasicInfo = LoadingState.LOADDINGFAIL;
                this.TreeViewModel.LoadTreeFail();
                return;
            }
            this.TreeViewModel.RootNode = rootNode;
            this.TreeViewModel.InitAutoComplete();
            /*更新车辆状态*/
            Thread initCarOnlineState = new Thread(new ThreadStart(this.InitCarOnlineState));
            initCarOnlineState.Start();
        }
        /*获取未使用的节点*/
        private int UnUseCount(List<VehicleTrackTreeNodeViewModel> nodeList)
        {
            int count = 0;
            foreach (VehicleTrackTreeNodeViewModel vtnv in nodeList)
            {
                if (vtnv.isUsed == false)
                {
                    count++;
                }
            }
            return count;
        }
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
                    InitVehicleState(this.TreeViewModel.rootNode);
                    Monitor.Exit(StaticTreeState.VehicleGPSInfoMutex);
                    break;
                }
            }
        }
        /*计算用户下的总车辆数以及车辆状态*/
        private string[] InitVehicleState(VehicleTrackTreeNodeViewModel rootNode)
        {//返回类型string格式[1]count[2]onlineCount
            int count = 0;
            int onlineCount = 0;
            if (rootNode != null)
            {
                if (rootNode.ListChildNodes != null)
                {
                    foreach (VehicleTrackTreeNodeViewModel vtnv in rootNode.ListChildNodes)
                    {
                        if (vtnv.onlineNumberVisible == Visibility.Collapsed)
                        {//车辆
                            count++;
                            foreach (CVDetailInfo detailInfo in StaticDetailInfo.GetInstance().ListVehicleDetailInfo)
                            {
                                if (vtnv.nodeInfo.SIM == detailInfo.SIM)
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
                                        vtnv.ImageUrl = VehicleCommon.GetVehicleImageURL(vtnv.nodeInfo.TypeID, detailInfo.VehicleGPSInfo.OnlineStates,detailInfo.VehicleGPSInfo.Datetime);
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
                rootNode.NodeInfo.OnlineCount = onlineCount;
                rootNode.NodeInfo.Count = count;
                rootNode.RaiseNodeInfoChanged();//通知节点信息发生变化
            }
            string[] retStrs = new string[2];
            retStrs[0] = count.ToString();
            retStrs[1] = onlineCount.ToString();
            return retStrs;
        }

        #region 用户节点
        /*添加单位、站点、车队节点*/
        private bool AddClientTree(VehicleTrackTreeNodeViewModel node, VehicleTrackTreeNodeViewModel t_node)
        {
            if (node.NodeInfo.ID == t_node.NodeInfo.ParentID)
            {
                if (node.ListChildNodes == null)
                {
                    node.ListChildNodes = new List<VehicleTrackTreeNodeViewModel>();
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
                        VehicleTrackTreeNodeViewModel vtnv = node.ListChildNodes[i];
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
        private void InitClientNode(VehicleTrackTreeNodeViewModel clientNode, CVBasicInfo cbi)
        {
            clientNode.nodeInfo = cbi;
            clientNode.isSelected = false;
            clientNode.isUsed = false;
            clientNode.parentNode = null;
            clientNode.ListChildNodes = null;
            clientNode.selectedVisible = Visibility.Collapsed;
            clientNode.onlineNumberVisible = Visibility.Visible;
            clientNode.innerIDVisible = Visibility.Collapsed;
            clientNode.nameVisible = Visibility.Visible;
            if (clientNode.nodeInfo.ParentID == "admin")
            {
                clientNode.imageUrl = "pack://application:,,,/Images/monitor.png";
            }
            else
            {
                clientNode.imageUrl = "pack://application:,,,/Images/trackclient.png";
            }
        }
        #endregion
        #region 车辆节点
        /*添加车辆节点*/
        private bool AddVehicleTree(VehicleTrackTreeNodeViewModel node, VehicleTrackTreeNodeViewModel t_node)
        {
            if (node.NodeInfo.ID == t_node.NodeInfo.ParentID)
            {
                if (node.ListChildNodes == null)
                {
                    node.ListChildNodes = new List<VehicleTrackTreeNodeViewModel>();
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
                        VehicleTrackTreeNodeViewModel vtnv = node.ListChildNodes[i];
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
        private void InitVehicleNode(VehicleTrackTreeNodeViewModel vehicleNode, CVBasicInfo vbi)
        {
            vehicleNode.nodeInfo = vbi;
            vehicleNode.isSelected = false;
            vehicleNode.isUsed = false;
            vehicleNode.parentNode = null;
            vehicleNode.listChildNodes = null;
            vehicleNode.onlineNumberVisible = Visibility.Collapsed;
            vehicleNode.selectedVisible = Visibility.Visible;
            vehicleNode.innerIDVisible = Visibility.Visible;
            vehicleNode.nameVisible = Visibility.Visible;
            vehicleNode.imageUrl = "pack://application:,,,/Images/Car/unknow.png";
        }
        #endregion
    }
}