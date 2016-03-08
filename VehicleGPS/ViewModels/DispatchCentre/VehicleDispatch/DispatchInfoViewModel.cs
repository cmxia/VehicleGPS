using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.Prism.ViewModel;
using System.Windows;
using VehicleGPS.Models;
using Microsoft.Practices.Prism.Commands;
using VehicleGPS.Views.Control.DispatchCentre.VehicleDispatch;
using System.Xml;
using VehicleGPS.Services.DispatchCentre.VehicleDispatch;
using System.IO;

namespace VehicleGPS.ViewModels.DispatchCentre.VehicleDispatch
{
    class DispatchInfoViewModel:NotificationObject
    {
        private VehicleDispatchViewModel ParentViewModel {get;set;}
        public DispatchInfoViewModel(object parentVM)
        {
            /*确定取消命令*/
            this.CancelCommand = new DelegateCommand<Window>(new Action<Window>(this.CancelCommandExecute));
            this.ConfirmCommand = new DelegateCommand<Window>(new Action<Window>(this.ConfirmCommandExecute));
            ///*树形选择事件*/
            //this.SelectedCommand = new DelegateCommand<object>(new Action<object>(this.SelectedCommandExecute));
            this.ParentViewModel = (VehicleDispatchViewModel)parentVM;
            this.SelectedItem = this.ParentViewModel.SelectedItem;
            this.InitVehicleTree();
            this.ListHolidayType = VehicleCommon.GetHolidayType();

            /*节假日*/
            this.SelectedHolidayIndex = 0;
        }
        /*当前选择的调度Item*/
        private VehicleDispatchItemViewModel selectedItem;
        public VehicleDispatchItemViewModel SelectedItem
        {
            get { return selectedItem; }
            set
            {
                if (selectedItem != value)
                {
                    selectedItem = value;
                    this.RaisePropertyChanged("SelectedItem");
                }
            }
        }
        ///*树节点选择事件*/
        //public DispatchInfoTreeItemViewModel SelectedNode { get; set; }
        //public DelegateCommand<object> SelectedCommand { get; set; }
        //private void SelectedCommandExecute(object selectedItem)
        //{
        //    DispatchInfoTreeItemViewModel selectedVM = (DispatchInfoTreeItemViewModel)selectedItem;
        //    this.SelectedNode = selectedVM;
        //}
        /*树节点*/
        private List<DispatchInfoTreeItemViewModel> listTreeItem;
        public List<DispatchInfoTreeItemViewModel> ListTreeItem
        {
            get { return listTreeItem; }
            set
            {
                if (listTreeItem != value)
                {
                    listTreeItem = value;
                    this.RaisePropertyChanged("ListTreeItem");
                }
            }
        }
        /*当前选择的树节点*/
        public DispatchInfoTreeItemViewModel SelectedTreeItem { get; set; }
        /*初始化树节点*/
        private void InitVehicleTree()
        {
            if (this.selectedItem != null)
            {
                if (this.ListTreeItem != null)
                {
                    this.ListTreeItem.Clear();
                }
                List<DispatchInfoTreeItemViewModel> tmpList = new List<DispatchInfoTreeItemViewModel>();
                DispatchInfoTreeItemViewModel root = this.GetVehicleNode("dispatch", "车辆调度",null, Visibility.Collapsed, Visibility.Collapsed, Visibility.Collapsed);
                root.ImageUrl = "pack://application:,,,/Images/Factory.png";
                AddVehicleType(root);
                AddVehicle(root);
                tmpList.Add(root);
                this.ListTreeItem = tmpList;
            }
        }

        private DispatchInfoTreeItemViewModel GetVehicleNode(string nodeID,string nodeName,string nodeTypeID, Visibility setVisible, Visibility checkVisible, Visibility showVisible)
        {
            DispatchInfoTreeItemViewModel item = new DispatchInfoTreeItemViewModel();
            item.NodeInfo = new CVBasicInfo();
            item.NodeInfo.ID = nodeID;
            item.NodeInfo.Name = nodeName;
            item.NodeInfo.TypeID = nodeTypeID;
            item.CheckVisible = checkVisible;
            item.SetVisible = setVisible;
            item.ShowVisible = showVisible;
            item.ImageUrl = VehicleCommon.GetVehicleImageURL(nodeTypeID, VehicleCommon.VSOnlineRun);
            return item;
        }
        /*添加车辆类型节点*/
        private void AddVehicleType(DispatchInfoTreeItemViewModel root) 
        {
            if (StaticTreeState.BasicTypeInfo != LoadingState.LOADCOMPLETE)
            {
                MessageBox.Show("车辆类型基础数据未加载成功,请稍后或者退出刷新树形", "数据加载", MessageBoxButton.OKCancel);
                return;
            }
            StaticBasicType basicType = StaticBasicType.GetInstance();
            foreach (BasicTypeInfo info in basicType.ListBasicTypeInfo)
            {
                if (info.TypeID.Substring(0, 2) == "cl")
                {
                    if (info.TypeID == "cl00001")//搅拌车放在第一位
                    {
                        root.ListTreeItem.Insert(0, this.GetVehicleNode(info.TypeID, info.TypeName, info.TypeID, Visibility.Collapsed, Visibility.Collapsed, Visibility.Collapsed));
                    }
                    else
                    {
                        root.ListTreeItem.Add(this.GetVehicleNode(info.TypeID, info.TypeName, info.TypeID, Visibility.Collapsed, Visibility.Collapsed, Visibility.Collapsed));
                    }
                }
            }
        }
        /*添加所有空闲车辆*/
        private void AddVehicle(DispatchInfoTreeItemViewModel root)
        {
            if (StaticTreeState.VehicleAllBasicInfo != LoadingState.LOADCOMPLETE)
            {//车辆任务需要加载全部基础数据才能获取，然后存入关键信息中CVBasicInfo
                MessageBox.Show("车辆所有基础数据未加载成功,请稍后或者退出刷新树形", "数据加载", MessageBoxButton.OKCancel);
                return;
            }
            StaticBasicInfo basicInfo = StaticBasicInfo.GetInstance();
            foreach (CVBasicInfo info in basicInfo.ListVehicleBasicInfo)
            {
                if (info.ParentID == this.SelectedItem.TaskNumberInfo.RegionID && info.TaskState == VehicleCommon.TaskNo)
                {//空闲并且属于该站点的车辆
                    foreach (DispatchInfoTreeItemViewModel item in root.ListTreeItem)
                    {
                        if (item.NodeInfo.ID == info.TypeID)
                        {//属于该车类型
                            item.ListTreeItem.Add(this.GetVehicleNode(info.ID, info.Name, info.TypeID, Visibility.Collapsed, Visibility.Visible, Visibility.Collapsed));
                        }
                    }
                }
            }
        }
        /*是否循环派车*/
        private bool isLoopDispatch;
        public bool IsLoopDispatch
        {
            get { return isLoopDispatch; }
            set
            {
                if (isLoopDispatch != value)
                {
                    isLoopDispatch = value;
                    this.RaisePropertyChanged("IsLoopDispatch");
                }
            }
        }
        /*上班类型*/
        private int selectedHolidayIndex;
        public int SelectedHolidayIndex
        {
            get { return selectedHolidayIndex; }
            set
            {
                if (selectedHolidayIndex != value)
                {
                    selectedHolidayIndex = value;
                    this.RaisePropertyChanged("SelectedHolidayIndex");
                }
            }
        }
        private List<string> listHolidayType;
        public List<string> ListHolidayType
        {
            get { return listHolidayType; }
            set
            {
                if (listHolidayType != value)
                {
                    listHolidayType = value;
                    this.RaisePropertyChanged("ListHolidayType");
                }
            }
        }

        #region 确定和取消
        public DelegateCommand<Window> CancelCommand { get; set; }
        private void CancelCommandExecute(Window win)
        {
            if (MessageBox.Show("调度数据还未提交，确定关闭吗？", "确认信息", MessageBoxButton.YesNo) == MessageBoxResult.No)
            {
                return;
            }
            win.Close();
        }
        public DelegateCommand<Window> ConfirmCommand { get; set; }
        private void ConfirmCommandExecute(Window win)
        {
            bool hasSelectedVehicle = false;//是否选择了调度车辆
            foreach (DispatchInfoTreeItemViewModel itemType in this.ListTreeItem[0].ListTreeItem)
            {//this.ListTreeItem[0]为“车辆调度”节点,itemType为车辆类型节点
                foreach (DispatchInfoTreeItemViewModel item in itemType.ListTreeItem)
                {
                    if (item.IsSelected == true && item.Parameter == "您还未设置运输参数")
                    {
                        MessageBox.Show("您选择的调度车辆还没有设置运输参数", "运输参数", MessageBoxButton.OKCancel);
                        return;
                    }
                    if (item.IsSelected == true)
                    {//已经选择了调度车辆，可以“确定”保存
                        hasSelectedVehicle = true;
                    }
                }
            }
            if (!hasSelectedVehicle)
            {
                MessageBox.Show("您还没有选择的调度车辆，不能提交调度数据", "调度车辆", MessageBoxButton.OKCancel);
                return;
            }
            if (MessageBox.Show("确定提交调度数据吗？", "确认信息", MessageBoxButton.YesNo) == MessageBoxResult.No)
            {
                return;
            }

            string xmlStr = this.CreatInsertXML();
            VehicleDispatchDataOperate dataOperate = new VehicleDispatchDataOperate();
            dataOperate.InsertDispatchInfo(xmlStr);
            win.Close();
        }
        #endregion

        #region 创建插入数据的XML
        private string CreatInsertXML()
        {
            XmlDocument xmlDoc = new XmlDocument();
            /*创建编码格式*/
            XmlDeclaration xmlDec = xmlDoc.CreateXmlDeclaration("1.0", "gb2312", null);
            xmlDoc.AppendChild(xmlDec);
            /*创建根节点*/
            XmlElement root = xmlDoc.CreateElement("Root");
            xmlDoc.AppendChild(root);

            foreach (DispatchInfoTreeItemViewModel itemType in this.ListTreeItem[0].ListTreeItem)
            {//this.ListTreeItem[0]是“车辆调度”节点,itemType为车类型节点
                foreach (DispatchInfoTreeItemViewModel item in itemType.ListTreeItem)
                {//item为车辆节点
                    if (item.IsSelected == true)
                    {
                        /*创建任务节点*/
                        XmlElement task = xmlDoc.CreateElement("Task");
                        root.AppendChild(task);
                        /*任务单号*/
                        XmlElement FPlanId = xmlDoc.CreateElement("FPlanId");
                        FPlanId.InnerText = this.SelectedItem.TaskNumberInfo.FPlanId;
                        task.AppendChild(FPlanId);
                        /*车辆编号*/
                        XmlElement Vehiclenum = xmlDoc.CreateElement("Vehiclenum");
                        Vehiclenum.InnerText = item.NodeInfo.ID;
                        task.AppendChild(Vehiclenum);
                        /*运输方量*/
                        XmlElement TransCapPer = xmlDoc.CreateElement("TransCapPer");
                        TransCapPer.InnerText = item.ParameterVM.RealLoad.ToString();
                        task.AppendChild(TransCapPer);
                        /*上班时间*/
                        XmlElement Holiday = xmlDoc.CreateElement("Holiday");
                        Holiday.InnerText = this.ListHolidayType[this.SelectedHolidayIndex];
                        task.AppendChild(Holiday);
                        /*车辆状态*/
                        XmlElement CarsStatus = xmlDoc.CreateElement("CarsStatus");
                        CarsStatus.InnerText = "1";//默认在区域内
                        task.AppendChild(CarsStatus);
                        /*卸料方式*/
                        XmlElement offtype = xmlDoc.CreateElement("offtype");
                        offtype.InnerText = item.ParameterVM.ListUnloadWay[item.ParameterVM.SelectedUnloadIndex].TypeID;
                        task.AppendChild(offtype);
                        /*运输物品编号*/
                        string concreteIdText = item.ParameterVM.TransObject[item.ParameterVM.SelectedTransObjectIndex].TypeID;
                        if (concreteIdText == "noselected")
                        {//没有选择运输物品
                            concreteIdText = "";
                        }
                        XmlElement ConcreteID = xmlDoc.CreateElement("ConcreteID");
                        ConcreteID.InnerText = concreteIdText;
                        task.AppendChild(ConcreteID);
                        /*车牌号*/
                        XmlElement VehicleID = xmlDoc.CreateElement("VehicleID");
                        VehicleID.InnerText = item.NodeInfo.Name;
                        task.AppendChild(VehicleID);
                        /*起始点区域ID*/
                        XmlElement Startid = xmlDoc.CreateElement("Startid");
                        Startid.InnerText = this.SelectedItem.TaskNumberInfo.StartID;
                        task.AppendChild(Startid);
                        /*结束点工地ID*/
                        XmlElement Endid = xmlDoc.CreateElement("Endid");
                        Endid.InnerText = this.SelectedItem.TaskNumberInfo.EndID;
                        task.AppendChild(Endid);
                        /*是否循环派车*/
                        XmlElement TransType = xmlDoc.CreateElement("TransType");
                        TransType.InnerText = (this.IsLoopDispatch == true ? "1" : "0");
                        task.AppendChild(TransType);
                        /*塔楼ID*/
                        string towerIdText = item.ParameterVM.ListTower[item.ParameterVM.SelectedTowerIndex].ID;
                        string towerNameText = item.ParameterVM.ListTower[item.ParameterVM.SelectedTowerIndex].Name;
                        if (towerIdText == "noselected")
                        {//没有选择塔楼
                            towerIdText = "";
                            towerNameText = "";
                        }
                        XmlElement TowerID = xmlDoc.CreateElement("TowerID");
                        TowerID.InnerText = towerIdText;
                        task.AppendChild(TowerID);
                        /*塔楼Name*/
                        XmlElement TowerName = xmlDoc.CreateElement("TowerName");
                        TowerName.InnerText = towerNameText;
                        task.AppendChild(TowerName);
                        /*驾驶员工作号*/
                        string driverIdText = item.ParameterVM.ListDriver[item.ParameterVM.SelectedDriverIndex].workID;
                        string driverNameText = item.ParameterVM.ListDriver[item.ParameterVM.SelectedDriverIndex].DriverName;
                        if(driverIdText == "不选择驾驶员")
                        {
                            driverIdText = "";
                            driverNameText = "";
                        }
                        XmlElement DriverID = xmlDoc.CreateElement("DriverID");
                        DriverID.InnerText = driverIdText;
                        task.AppendChild(DriverID);
                        /*驾驶员姓名*/
                        XmlElement DriverName = xmlDoc.CreateElement("DriverName");
                        DriverName.InnerText = driverNameText;
                        task.AppendChild(DriverName);
                        /*车辆类型ID*/
                        XmlElement VehicleType = xmlDoc.CreateElement("VehicleType");
                        VehicleType.InnerText = item.NodeInfo.TypeID;
                        task.AppendChild(VehicleType);

                        /*添加一条调度车辆数据*/
                        root.AppendChild(task);
                    }
                }
            }
            return xmlDoc.InnerXml;
        }
        #endregion

    }
}
