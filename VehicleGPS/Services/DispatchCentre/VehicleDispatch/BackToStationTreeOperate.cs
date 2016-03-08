using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VehicleGPS.Models;
using System.Threading;
using VehicleGPS.ViewModels.DispatchCentre.VehicleDispatch;

namespace VehicleGPS.Services.DispatchCentre.VehicleDispatch
{
    class BackToStationTreeOperate
    {
        private BackToStationTreeViewModel TreeViewModel { get; set; }
        public BackToStationTreeOperate(BackToStationTreeViewModel vm)
        {
            this.TreeViewModel = vm;
        }
        public void ReadTree()
        {
            this.RefreshTree();
        }
        public void RefreshTree()
        {
            /*获取所有基本类型*/
            if (StaticTreeState.BasicTypeInfo != LoadingState.LOADCOMPLETE)
            {
                StaticBasicType basicType = StaticBasicType.GetInstance();
                basicType.RefreshBasicInfo();
            }
            /*获取车辆以及用户的基本信息*/
            if (StaticTreeState.VehicleBasicInfo != LoadingState.LOADCOMPLETE
                || StaticTreeState.ClientBasicInfo != LoadingState.LOADCOMPLETE
                || StaticTreeState.VehicleAllBasicInfo != LoadingState.LOADCOMPLETE)
            {
                StaticBasicInfo cvBasicInfo = StaticBasicInfo.GetInstance();
                cvBasicInfo.RefreshBasicInfo();
            }
            /*启动线程初始化树形*/
            Thread initTreeThread = new Thread(new ThreadStart(this.InitClientVehicleTree));
            initTreeThread.Start();
        }
        /*初始化*/
        private void InitClientVehicleTree()
        {
            //Thread.Sleep(1000);//确保树模型已经创建、获取车辆和用户基本信息线程已经启动
            /*创建根节点*/
            BackToStationTreeNodeViewModel rootNode = new BackToStationTreeNodeViewModel();
            rootNode.NodeInfo = new CVBasicInfo();
            rootNode.nodeInfo.Name = "强制回站";
            rootNode.NodeInfo.ID = "admin";
            rootNode.ListChildNodes = new List<BackToStationTreeNodeViewModel>();

            BackToStationTreeNodeViewModel backNode = new BackToStationTreeNodeViewModel();
            backNode.NodeInfo = new CVBasicInfo();
            backNode.nodeInfo.Name = "强制回站";
            backNode.NodeInfo.ID = "admin";
            backNode.imageUrl = "pack://application:,,,/Images/monitor.png";
            backNode.parentNode = rootNode;
            rootNode.listChildNodes.Add(backNode);
            /*加载用户基础信息*/
            List<BackToStationTreeNodeViewModel> clientNodeList = new List<BackToStationTreeNodeViewModel>();
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
                    string typeID = "Unit";//站点类型ID
                    /*获取用户节点*/
                    foreach (CVBasicInfo cbi in basicInfo.ListClientBasicInfo)
                    {
                        BackToStationTreeNodeViewModel clientNode = new BackToStationTreeNodeViewModel();
                        if (cbi.TypeID.Trim() == typeID.Trim())
                        {//站点节点                           
                            InitClientNode(backNode, clientNode, cbi);
                            clientNodeList.Add(clientNode);
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
            backNode.ListChildNodes = clientNodeList;
            this.TreeViewModel.RootNode = rootNode;//先显示用户树

            loadSucess = false;
            loadTimesCounter = 1;
            /*获取与用户权限关联的车辆节点*/
            List<BackToStationTreeNodeViewModel> vehicleNodeList = new List<BackToStationTreeNodeViewModel>();
            foreach (CVBasicInfo vbi in basicInfo.ListVehicleBasicInfo)
            {
                BackToStationTreeNodeViewModel vehicleNode = new BackToStationTreeNodeViewModel();
                if (InitVehicleNode(vehicleNode, vbi))
                {
                    vehicleNodeList.Add(vehicleNode);
                }
            }
            /*添加车辆到站点*/
            foreach (BackToStationTreeNodeViewModel childNode in rootNode.listChildNodes[0].listChildNodes)
            {
                foreach (BackToStationTreeNodeViewModel vChildNode in vehicleNodeList)
                {
                    if (childNode.nodeInfo.ID == vChildNode.nodeInfo.ParentID)
                    {
                        vChildNode.parentNode = childNode;
                        childNode.listChildNodes.Add(vChildNode);
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
        }
        private void InitClientNode(BackToStationTreeNodeViewModel parentNode, BackToStationTreeNodeViewModel clientNode, CVBasicInfo cbi)
        {
            clientNode.nodeInfo = cbi;
            clientNode.isSelected = false;
            clientNode.isUsed = false;
            clientNode.parentNode = parentNode;
            clientNode.imageUrl = "pack://application:,,,/Images/Factory.png";
            clientNode.listChildNodes = new List<BackToStationTreeNodeViewModel>();
        }

        /*初始化车辆节点*/
        private bool InitVehicleNode(BackToStationTreeNodeViewModel vehicleNode, CVBasicInfo vbi)
        {
            vehicleNode.nodeInfo = vbi;
            vehicleNode.isSelected = false;
            vehicleNode.isUsed = false;
            vehicleNode.parentNode = null;
            vehicleNode.listChildNodes = null;

            int loadTimesCounter = 0;
            int loadTimes = 5;
            string taskState = VehicleCommon.TaskOff;
            string onlineState = VehicleCommon.VSOnlineOff;
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
            }
            if (taskState != VehicleCommon.TaskIng)
            {//强制回站树形只加载有任务的车
                return false;
            }
            vehicleNode.nodeInfo.TaskState = taskState;
            vehicleNode.nodeInfo.OnlineStatus = onlineState;
            vehicleNode.imageUrl = VehicleCommon.GetTaskImageURL(vehicleNode.nodeInfo);
            return true;
        }

        /*获取站点类型id*/
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
    }
}
