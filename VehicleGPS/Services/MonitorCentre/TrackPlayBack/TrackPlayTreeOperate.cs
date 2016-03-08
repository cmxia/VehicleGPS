using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VehicleGPS.Models;
using System.Threading;
using VehicleGPS.ViewModels.MonitorCentre.TrackPlayBack;
using System.Windows;

namespace VehicleGPS.Services.MonitorCentre.TrackPlayBack
{
    /// <summary>
    /// 轨迹回放树形结构数据操作
    /// </summary>
    class TrackPlayTreeOperate
    {
        private TrackPlayTreeViewModel treeVM;
        public TrackPlayTreeOperate(TrackPlayTreeViewModel treeVM)
        {
            this.treeVM = treeVM;
        }
        /// <summary>
        /// 读取树形结构
        /// </summary>
        public void ReadTree()
        {
            this.RefreshTree();
        }

        #region 获得树形结构相关的数据(操作数据库)
        /// <summary>
        /// 获得树形结构的相关数据
        /// </summary>
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
        #endregion

        #region 树形结构初始化
        /// <summary>
        /// 树形结构初始化（线程处理）
        /// </summary>
        private void InitClientVehicleTree()
        {
            //Thread.Sleep(1000);//确保树模型已经创建、获取车辆和用户基本信息线程已经启动
            /*创建根节点*/
            TrackPlayTreeNodeViewModel rootNode = new TrackPlayTreeNodeViewModel();
            rootNode.NodeInfo = new CVBasicInfo();
            rootNode.NodeInfo.ID = "admin";
            rootNode.ListChildNodes = new List<TrackPlayTreeNodeViewModel>();
            /*加载用户基础信息*/
            List<TrackPlayTreeNodeViewModel> clientNodeList = new List<TrackPlayTreeNodeViewModel>();
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
                        TrackPlayTreeNodeViewModel clientNode = new TrackPlayTreeNodeViewModel();
                        InitClientNode(clientNode, cbi);
                        clientNodeList.Add(clientNode);
                    }
                    int rollTimes = 0;//轮询次数，防止脏数据导致陷入死循环
                    while (UnUseCount(clientNodeList) != 0)
                    {
                        foreach (TrackPlayTreeNodeViewModel vtnv in clientNodeList)
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
                this.treeVM.LoadTreeFail();
                return;
            }
            this.treeVM.RootNode = rootNode;//先显示用户树

            loadSucess = false;
            loadTimesCounter = 1;

            /*获取与用户权限关联的车辆节点*/
            List<TrackPlayTreeNodeViewModel> vehicleNodeList = new List<TrackPlayTreeNodeViewModel>();
            foreach (CVBasicInfo vbi in basicInfo.ListVehicleBasicInfo)
            {
                TrackPlayTreeNodeViewModel vehicleNode = new TrackPlayTreeNodeViewModel();
                InitVehicleNode(vehicleNode, vbi);
                vehicleNodeList.Add(vehicleNode);
            }
            int _rollTimes = 0;//轮询次数，防止脏数据导致陷入死循环
            while (UnUseCount(vehicleNodeList) != 0)
            {
                foreach (TrackPlayTreeNodeViewModel vtnv in vehicleNodeList)
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
                foreach (TrackPlayTreeNodeViewModel vtnv in vehicleNodeList)
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
                this.treeVM.LoadTreeFail();
                return;
            }
            this.treeVM.RootNode = rootNode;
            this.treeVM.InitAutoComplete();/*初始化AutoComplete查询数据集*/
            rootNode.listChildNodes[0].isExpand = true;//第一层展开
            /*更新车辆状态*/
            Thread initCarOnlineState = new Thread(new ThreadStart(this.InitCarOnlineState));
            initCarOnlineState.Start();
        }
        /*初始化*/
        /// <summary>
        /// 树形结构初始化（线程处理）
        /// </summary>
        private void InitClientVehicleTree_old()
        {
            //Thread.Sleep(1000);//确保树模型已经创建、获取车辆和用户基本信息线程已经启动
            /*创建根节点*/
            TrackPlayTreeNodeViewModel rootNode = new TrackPlayTreeNodeViewModel();
            rootNode.NodeInfo = new CVBasicInfo();
            rootNode.NodeInfo.ID = "admin";
            rootNode.ListChildNodes = new List<TrackPlayTreeNodeViewModel>();
            /*加载用户基础信息*/
            List<TrackPlayTreeNodeViewModel> clientNodeList = new List<TrackPlayTreeNodeViewModel>();
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
                        TrackPlayTreeNodeViewModel clientNode = new TrackPlayTreeNodeViewModel();
                        InitClientNode(clientNode, cbi);
                        clientNodeList.Add(clientNode);
                    }
                    int rollTimes = 0;//轮询次数，防止脏数据导致陷入死循环
                    while (UnUseCount(clientNodeList) != 0)
                    {
                        foreach (TrackPlayTreeNodeViewModel vtnv in clientNodeList)
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
                this.treeVM.LoadTreeFail();
                return;
            }
            this.treeVM.RootNode = rootNode;//先显示用户树

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
                    List<TrackPlayTreeNodeViewModel> vehicleNodeList = new List<TrackPlayTreeNodeViewModel>();
                    foreach (CVBasicInfo vbi in basicInfo.ListVehicleBasicInfo)
                    {
                        TrackPlayTreeNodeViewModel vehicleNode = new TrackPlayTreeNodeViewModel();
                        InitVehicleNode(vehicleNode, vbi);
                        vehicleNodeList.Add(vehicleNode);
                    }
                    int rollTimes = 0;//轮询次数，防止脏数据导致陷入死循环
                    while (UnUseCount(vehicleNodeList) != 0)
                    {
                        foreach (TrackPlayTreeNodeViewModel vtnv in vehicleNodeList)
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
                        foreach (TrackPlayTreeNodeViewModel vtnv in vehicleNodeList)
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
                this.treeVM.LoadTreeFail();
                return;
            }
            this.treeVM.RootNode = rootNode;
            this.treeVM.InitAutoComplete();/*初始化AutoComplete查询数据集*/
            /*更新车辆状态*/
            Thread initCarOnlineState = new Thread(new ThreadStart(this.InitCarOnlineState));
            initCarOnlineState.Start();
        }
        #endregion

        #region 获取未使用的节点
        /*获取未使用的节点*/
        /// <summary>
        /// 获取未使用的节点
        /// </summary>
        /// <param name="nodeList"></param>
        /// <returns></returns>
        private int UnUseCount(List<TrackPlayTreeNodeViewModel> nodeList)
        {
            int count = 0;
            foreach (TrackPlayTreeNodeViewModel vtnv in nodeList)
            {
                if (vtnv.isUsed == false)
                {
                    count++;
                }
            }
            return count;
        }
        #endregion

        #region 车辆状态初始化
        /// <summary>
        /// 初始化车辆状态
        /// </summary>
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
                    InitVehicleState(this.treeVM.rootNode);
                    Monitor.Exit(StaticTreeState.VehicleGPSInfoMutex);
                    break;
                }
            }
        }
        #endregion

        #region 计算用户下的总车辆数以及车辆状态
        /*计算用户下的总车辆数以及车辆状态*/
        /// <summary>
        /// 计算权限用户的车辆总数及车辆状态
        /// </summary>
        /// <param name="rootNode"></param>
        /// <returns></returns>
        private string[] InitVehicleState(TrackPlayTreeNodeViewModel rootNode)
        {//返回类型string格式[1]count[2]onlineCount
            int count = 0;
            int onlineCount = 0;
            if (rootNode != null)
            {
                if (rootNode.ListChildNodes != null)
                {
                    foreach (TrackPlayTreeNodeViewModel vtnv in rootNode.ListChildNodes)
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
                                        vtnv.ImageUrl = VehicleCommon.GetVehicleImageURL(vtnv.nodeInfo.TypeID, detailInfo.VehicleGPSInfo.OnlineStates, detailInfo.VehicleGPSInfo.Datetime);
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
        #endregion

        #region 新增用户节点
        /*添加单位、站点、车队节点*/
        /// <summary>
        /// 添加用户节点
        /// </summary>
        /// <param name="node"></param>
        /// <param name="t_node"></param>
        /// <returns></returns>
        private bool AddClientTree(TrackPlayTreeNodeViewModel node, TrackPlayTreeNodeViewModel t_node)
        {
            if (node.NodeInfo.ID == t_node.NodeInfo.ParentID)
            {
                if (node.ListChildNodes == null)
                {
                    node.ListChildNodes = new List<TrackPlayTreeNodeViewModel>();
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
                        TrackPlayTreeNodeViewModel vtnv = node.ListChildNodes[i];
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
        /// <summary>
        /// 初始化用户结点
        /// </summary>
        /// <param name="clientNode"></param>
        /// <param name="cbi"></param>
        private void InitClientNode(TrackPlayTreeNodeViewModel clientNode, CVBasicInfo cbi)
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

        #region 新增车辆节点
        /*添加车辆节点*/
        /// <summary>
        /// 添加车辆结点
        /// </summary>
        /// <param name="node"></param>
        /// <param name="t_node"></param>
        /// <returns></returns>
        private bool AddVehicleTree(TrackPlayTreeNodeViewModel node, TrackPlayTreeNodeViewModel t_node)
        {
            if (node.NodeInfo.ID == t_node.NodeInfo.ParentID)
            {
                if (node.ListChildNodes == null)
                {
                    node.ListChildNodes = new List<TrackPlayTreeNodeViewModel>();
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
                        TrackPlayTreeNodeViewModel vtnv = node.ListChildNodes[i];
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
        /// <summary>
        /// 初始化车辆节点
        /// </summary>
        /// <param name="vehicleNode"></param>
        /// <param name="vbi"></param>
        private void InitVehicleNode(TrackPlayTreeNodeViewModel vehicleNode, CVBasicInfo vbi)
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
