using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VehicleGPS.ViewModels.DispatchCentre;
using System.Windows;
using VehicleGPS.Models;
using System.Threading;
using VehicleGPS.ViewModels.MonitorCentre.RealTimeMonitor;

namespace VehicleGPS.Services.DispatchCentre
{
    class DispatchTreeOperate
    {
        public void ReadTree()
        {
            this.RefreshTree();
        }
        #region 加载树形结构（从数据库读取数据）
        /// <summary>
        /// 加载树形结构
        /// </summary>
        public void RefreshTree()
        {
            if (StaticTreeState.VehicleBasicInfo != LoadingState.LOADCOMPLETE
                || StaticTreeState.VehicleBasicInfo != LoadingState.LOADCOMPLETE
                || StaticTreeState.VehicleAllBasicInfo != LoadingState.LOADCOMPLETE)
            {
                StaticBasicInfo cvBasicInfo = StaticBasicInfo.GetInstance();
                cvBasicInfo.RefreshBasicInfo();
            }
            /*启动线程初始化树形*/
            Thread initTreeThread = new Thread(new ThreadStart(this.InitClientVehicleTree));
            initTreeThread.Start();
        }
        #endregion

        #region 初始化树形结构
        /// <summary>
        /// 初始化
        /// </summary>
        private void InitClientVehicleTree()
        {
            //Thread.Sleep(500);
            /*创建根节点*/
            DispatchTreeNodeViewModel rootNode = new DispatchTreeNodeViewModel();
            rootNode.NodeInfo = new CVBasicInfo();
            rootNode.nodeInfo.Name = "车辆调度";
            rootNode.NodeInfo.ID = "admin";
            rootNode.ListChildNodes = new List<DispatchTreeNodeViewModel>();
            /*加载用户基础信息*/
            List<DispatchTreeNodeViewModel> clientNodeList = new List<DispatchTreeNodeViewModel>();
            StaticBasicInfo basicInfo = StaticBasicInfo.GetInstance();

            bool loadSucess = false;
            int loadTimesCounter = 1;
            int loadTiems = 15;
            while (Monitor.TryEnter(StaticTreeState.ClientBasicMutex, 10000))
            {//等待10秒
                if (StaticTreeState.ClientBasicInfo != LoadingState.LOADCOMPLETE)
                {//加载用户失败
                    Monitor.Exit(StaticTreeState.ClientBasicMutex);
                    Thread.Sleep(50);
                    if (loadTimesCounter++ == loadTiems)//尝试15次
                    {
                        break;
                    }
                    continue;
                }
                if (StaticTreeState.ClientBasicInfo == LoadingState.LOADCOMPLETE)
                {
                    DispatchTreeNodeViewModel dispatchNode = new DispatchTreeNodeViewModel();
                    foreach (CVBasicInfo cbi in basicInfo.ListClientBasicInfo)
                    {
                        if (cbi.ID.Equals("root"))
                        {
                            continue;
                        }
                        DispatchTreeNodeViewModel clientNode = new DispatchTreeNodeViewModel();

                        if (basicInfo.ListVehicleOfClientBaseInfo.Contains(cbi.ID))//加载包含车辆的单位列表
                        {
                            if (cbi.ID == "RootNode")
                            {
                                continue;
                            }
                            InitClientNode(clientNode, cbi);
                            clientNodeList.Add(clientNode);
                        }
                    }
                    dispatchNode.NodeInfo = new CVBasicInfo();
                    dispatchNode.nodeInfo.Name = "车辆调度";
                    dispatchNode.isExpand = true; //第一级节点打开
                    dispatchNode.NodeInfo.ID = "RootNode";
                    dispatchNode.imageUrl = "pack://application:,,,/Images/monitor.png";
                    dispatchNode.ListChildNodes = new List<DispatchTreeNodeViewModel>();
                    dispatchNode.parentNode = rootNode;
                    rootNode.listChildNodes.Add(dispatchNode);
                    foreach (DispatchTreeNodeViewModel vtnv in clientNodeList)
                    {
                        dispatchNode.listChildNodes.Add(vtnv);
                        vtnv.parentNode = dispatchNode;
                        vtnv.isUsed = true;
                    }

                    loadSucess = true;
                }
                Monitor.Exit(StaticTreeState.ClientBasicMutex);
                break;
            }
            if (loadSucess == false)
            {//等待10秒超时或者加载用户信息失败
                StaticTreeState.ClientBasicInfo = LoadingState.LOADDINGFAIL;
                DispatchTreeViewModel.GetInstance().LoadTreeFail();
                StaticTreeState.DispatchTreeLoad = LoadingState.LOADCOMPLETE;
                return;
            }
            DispatchTreeViewModel.GetInstance().RootNode = rootNode;//先显示用户树

            loadSucess = false;
            while (true)
            {
                if (StaticTreeState.VehicleBasicInfo != LoadingState.LOADCOMPLETE)
                {//加载用户失败
                    Thread.Sleep(200);
                    if (loadTimesCounter++ == loadTiems)//尝试15次
                    {
                        break;
                    }
                    continue;
                }

                /*获取与用户权限关联的车辆节点*/
                List<DispatchTreeNodeViewModel> vehicleNodeList = new List<DispatchTreeNodeViewModel>();
                foreach (CVBasicInfo vbi in basicInfo.ListVehicleBasicInfo)
                {
                    DispatchTreeNodeViewModel vehicleNode = new DispatchTreeNodeViewModel();
                    InitVehicleNode(vehicleNode, vbi);
                    vehicleNodeList.Add(vehicleNode);
                }
                /*根据车辆的任务状态加入不同的任务节点*/
                foreach (DispatchTreeNodeViewModel childNode in rootNode.listChildNodes[0].listChildNodes)
                {
                    if (childNode.NodeInfo.Name.Equals("华新荆门站"))
                    {
                        int iiiii;
                    }
                    foreach (DispatchTreeNodeViewModel taskNode in childNode.listChildNodes)
                    {
                        List<DispatchTreeNodeViewModel> taskVehicleList = new List<DispatchTreeNodeViewModel>();
                        foreach (DispatchTreeNodeViewModel vChildNode in vehicleNodeList)
                        {
                            if (childNode.nodeInfo.ID == vChildNode.nodeInfo.ParentID)
                            {
                                if (taskNode.nodeInfo.Name.Equals(vChildNode.nodeInfo.TaskState))
                                {
                                    vChildNode.ParentNode = taskNode;
                                    DispatchTreeNodeViewModel vehicletmp = new DispatchTreeNodeViewModel();
                                    vehicletmp.ImageUrl = vChildNode.ImageUrl;
                                    vehicletmp.IsExpand = vChildNode.IsExpand;
                                    vehicletmp.IsFocus = vChildNode.IsFocus;
                                    vehicletmp.IsSelected = vChildNode.IsSelected;
                                    vehicletmp.isUsed = vChildNode.isUsed;
                                    vehicletmp.ListChildNodes = vChildNode.ListChildNodes;
                                    vehicletmp.NodeInfo = vChildNode.NodeInfo;
                                    vehicletmp.ParentNode = vChildNode.ParentNode;
                                    taskVehicleList.Add(vehicletmp);
                                }
                            }
                        }
                        if (taskNode.ListChildNodes!=null)
                        {
                            taskNode.ListChildNodes = null;
                        }
                        taskNode.ListChildNodes = taskVehicleList;
                    }
                }
                loadSucess = true;
                break;
            }


            if (loadSucess == false)
            {//等待10秒超时或者加载车辆失败

                DispatchTreeViewModel.GetInstance().LoadTreeFail();
                return;
            }
            DispatchTreeViewModel.GetInstance().RootNode = rootNode;
            DispatchTreeViewModel.GetInstance().InitAutoCompleteStation();
            RealTimeViewModel.GetInstance().SelectedGPSInfo();
            StaticTreeState.DispatchTreeLoad = LoadingState.LOADCOMPLETE;
            StaticTreeState.DispatchTreeRefresh = LoadingState.LOADCOMPLETE;
        }
        #endregion

        #region  初始化站点节点(样式)
        /// <summary>
        /// 初始化站点节点
        /// </summary>
        /// <param name="clientNode"></param>
        /// <param name="cbi"></param>
        private void InitClientNode(DispatchTreeNodeViewModel clientNode, CVBasicInfo cbi)
        {
            clientNode.nodeInfo = cbi;
            clientNode.IsExpand = true;
            clientNode.isSelected = false;
            clientNode.isUsed = false;
            clientNode.parentNode = null;
            clientNode.imageUrl = "pack://application:,,,/Images/Factory.png";
            clientNode.listChildNodes = new List<DispatchTreeNodeViewModel>();
            clientNode.listChildNodes.Add(GetTaskNode(clientNode, VehicleCommon.TaskNo, "pack://application:,,,/Images/IsFree_64.png"));
            clientNode.listChildNodes.Add(GetTaskNode(clientNode, VehicleCommon.TaskIng, "pack://application:,,,/Images/IsWorking_64.png"));
            clientNode.listChildNodes.Add(GetTaskNode(clientNode, VehicleCommon.TaskMaintain, "pack://application:,,,/Images/IsMataining_64.png"));
            clientNode.listChildNodes.Add(GetTaskNode(clientNode, VehicleCommon.TaskOff, "pack://application:,,,/Images/IsLeaving_64.png"));

        }
        /*任务节点*/
        /// <summary>
        /// 任务节点
        /// </summary>
        /// <param name="parentNode"></param>
        /// <param name="taskName"></param>
        /// <param name="imageUrl"></param>
        /// <returns></returns>
        private DispatchTreeNodeViewModel GetTaskNode(DispatchTreeNodeViewModel parentNode, string taskName, string imageUrl)
        {
            DispatchTreeNodeViewModel taskNode = new DispatchTreeNodeViewModel();
            taskNode.IsExpand = true;
            taskNode.nodeInfo = new CVBasicInfo();
            taskNode.nodeInfo.Name = taskName;
            taskNode.imageUrl = imageUrl;
            taskNode.nodeInfo.ID = "TaskNode";
            taskNode.listChildNodes = new List<DispatchTreeNodeViewModel>();
            taskNode.parentNode = parentNode;
            return taskNode;
        }
        #endregion

        /*初始化车辆节点*/
        private void InitVehicleNode(DispatchTreeNodeViewModel vehicleNode, CVBasicInfo vbi)
        {
            
            vehicleNode.nodeInfo = vbi;
            vehicleNode.isSelected = false;
            vehicleNode.isUsed = false;
            vehicleNode.parentNode = null;
            vehicleNode.listChildNodes = null;

            int loadTimesCounter = 0;
            int loadTimes = 5;
            string taskState = VehicleCommon.TaskOff;//存储车辆的任务状态
            string onlineState = VehicleCommon.VSOnlineOff;//存储车辆的在线状态
            while (StaticTreeState.VehicleAllBasicInfo != LoadingState.LOADCOMPLETE)
            {
                Thread.Sleep(200);
                if (++loadTimesCounter == loadTimes)
                {
                    taskState = VehicleCommon.TaskOff;
                }
            }

            foreach (CVDetailInfo info in StaticDetailInfo.GetInstance().ListVehicleDetailInfo)
            {
                if (info.SIM.Trim() == vehicleNode.nodeInfo.SIM.Trim())
                {
                    //设置车辆的任务状态
                    if (info.VehicleState == "0")
                    {
                        taskState = VehicleCommon.TaskNo;
                    }
                    else if (info.VehicleState == "1")
                    {
                        taskState = VehicleCommon.TaskIng;
                    }
                    else if (info.VehicleState == "2")
                    {
                        taskState = VehicleCommon.TaskMaintain;
                    }
                    else if (info.VehicleState == "3")
                    {
                        taskState = VehicleCommon.TaskOff;
                    }
                    //设置车辆的在线状态
                    if (info.VehicleGPSInfo != null && info.VehicleGPSInfo.OnlineStates != null)
                    {
                        onlineState = info.VehicleGPSInfo.OnlineStates;
                    }
                    break;
                }
            }
            vehicleNode.nodeInfo.TaskState = taskState;
            vehicleNode.nodeInfo.OnlineStatus = onlineState;
            vehicleNode.imageUrl = VehicleCommon.GetTaskImageURL(vehicleNode.nodeInfo);
        }

        #region 获取站点类型id
        /// <summary>
        /// 获取站点类型xq00002
        /// </summary>
        /// <returns></returns>
        private string GetXq00002()
        {
            /*车类型*/
            int loadTimesCounter = 0;
            int loadTimes = 5;
            while (StaticTreeState.BasicTypeInfo != LoadingState.LOADCOMPLETE)
            {
                Thread.Sleep(200);
                if (++loadTimesCounter == loadTimes)
                {
                    return "xq00002";
                }
            }
            if (StaticTreeState.BasicTypeInfo == LoadingState.LOADCOMPLETE)
            {
                foreach (BasicTypeInfo bti in StaticBasicType.GetInstance().ListBasicTypeInfo)
                {
                    if (bti.TypeName == "站点")
                    {
                        return bti.TypeID;
                    }
                }
            }
            return "xq00002";
        }
        #endregion
    }
}
