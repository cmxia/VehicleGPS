using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.Prism.ViewModel;
using VehicleGPS.Models;
using System.Windows;
using VehicleGPS.Services.DispatchCentre.VehicleDispatch;
using VehicleGPS.Models.DispatchCentre.VehicleDispatch;
using Microsoft.Practices.Prism.Commands;

namespace VehicleGPS.ViewModels.DispatchCentre.VehicleDispatch
{
    class SetParameterViewModel : NotificationObject
    {
        private DispatchInfoViewModel ParentVM { get; set; }
        private VehicleDispatchDataOperate DataOperate { get; set; }//数据操作类
        public SetParameterViewModel(object selectedVM)
        {
            /*确定取消命令*/
            this.CancelCommand = new DelegateCommand<Window>(new Action<Window>(this.CancelCommandExecute));
            this.ConfirmCommand = new DelegateCommand<Window>(new Action<Window>(this.ConfirmCommandExecute));

            this.ParentVM = (DispatchInfoViewModel)selectedVM;
            this.DataOperate = new VehicleDispatchDataOperate();
            this.InitSelectedVehicle(this.ParentVM.SelectedTreeItem.NodeInfo);
            this.InitUnloadWay();
            this.InitTransObject(this.ParentVM.SelectedItem.TaskNumberInfo.ConcreteId);
            this.InitListTower(this.ParentVM.SelectedItem.TaskNumberInfo.RegionID);
            this.InitListDriver(this.ParentVM.SelectedItem.TaskNumberInfo.RegionID);

            /*选择项*/
            this.SelectedDriverIndex = 0;
            this.SelectedTowerIndex = 0;
            this.SelectedTransObjectIndex = 0;
            this.SelectedUnloadIndex = 0;
        }
        /*选择车辆*/
        private CVDetailInfo selectedVehicle;
        public CVDetailInfo SelectedVehicle
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
        /*初始化选择车辆*/
        private void InitSelectedVehicle(CVBasicInfo basicInfo)
        {
            foreach (CVDetailInfo info in StaticDetailInfo.GetInstance().ListVehicleDetailInfo)
            {
                if (info.VehicleId == basicInfo.Name)
                {//车牌号相等
                    this.SelectedVehicle = info;
                    this.PlanLoad = Convert.ToDouble(info.LoadAmount == "" ? "0" : info.LoadAmount).ToString("0.00");
                    this.RealLoad = this.planLoad;
                    break;
                }
            }
        }
        /*装料塔楼*/
        private int selectedTowerIndex;
        public int SelectedTowerIndex
        {
            get { return selectedTowerIndex; }
            set
            {
                if (selectedTowerIndex != value)
                {
                    selectedTowerIndex = value;
                    this.RaisePropertyChanged("SelectedTowerIndex");
                }
            }
        }
        private List<CVBasicInfo> listTower;
        public List<CVBasicInfo> ListTower
        {
            get { return listTower; }
            set
            {
                if (listTower != value)
                {
                    listTower = value;
                    this.RaisePropertyChanged("ListTower");
                }
            }
        }
        private void InitListTower(string stationID)
        {
            if (stationID != "")
            {
                //this.ListTower = this.DataOperate.GetTowerInfo(stationID);
            }
        }
        /*驾驶员信息*/
        private int selectedDriverIndex;
        public int SelectedDriverIndex
        {
            get { return selectedDriverIndex; }
            set
            {
                if (selectedDriverIndex != value)
                {
                    selectedDriverIndex = value;
                    this.RaisePropertyChanged("SelectedDriverIndex");
                }
            }
        }
        private List<DriverInfo> listDriver;
        public List<DriverInfo> ListDriver
        {
            get { return listDriver; }
            set
            {
                if (listDriver != value)
                {
                    listDriver = value;
                    this.RaisePropertyChanged("ListDriver");
                }
            }
        }
        /*初始化驾驶员信息*/
        private void InitListDriver(string stationID)
        {
            if (stationID != "")
            {
                List<DriverInfo> tmp = this.DataOperate.GetDriverInfo(stationID);
                foreach (DispatchInfoTreeItemViewModel itemType in this.ParentVM.ListTreeItem[0].ListTreeItem)
                {//this.ListTreeItem[0]为“车辆调度”节点,itemType为车辆类型节点
                    foreach (DispatchInfoTreeItemViewModel item in itemType.ListTreeItem)
                    {
                        if (item.IsSelected == true && item.Parameter != "您还未设置运输参数" )
                        {
                            foreach (DriverInfo driverInfo in tmp)
                            {
                                if (driverInfo.workID.Equals(item.parameterVM.listDriver[item.parameterVM.selectedDriverIndex].workID))
                                {//如果该司机已经被选择，则在下一步车辆的驾驶员调度数据中删除该驾驶员信息
                                    tmp.Remove(driverInfo);
                                    break;
                                }
                            }
                            
                        }
                    }
                }
                this.ListDriver = tmp;
            }
        }

        /*运输物品*/
        private int selectedTransObjectIndex;
        public int SelectedTransObjectIndex
        {
            get { return selectedTransObjectIndex; }
            set
            {
                if (selectedTransObjectIndex != value)
                {
                    selectedTransObjectIndex = value;
                    this.RaisePropertyChanged("SelectedTransObjectIndex");
                }
            }
        }
        private List<BasicTypeInfo> transObject;
        public List<BasicTypeInfo> TransObject
        {
            get { return transObject; }
            set
            {
                if (transObject != value)
                {
                    transObject = value;
                    this.RaisePropertyChanged("TransObject");
                }
            }
        }
        /*初始化运输物品*/
        private void InitTransObject(string objectIDs)
        {
            if (objectIDs != "")
            {
                this.TransObject = this.DataOperate.GetTransmitObject(objectIDs);
            }
        }

        /*卸料方式*/
        private int selectedUnloadIndex;
        public int SelectedUnloadIndex
        {
            get { return selectedUnloadIndex; }
            set
            {
                if (selectedUnloadIndex != value)
                {
                    selectedUnloadIndex = value;
                    this.RaisePropertyChanged("SelectedUnloadIndex");
                }
            }
        }
        private List<BasicTypeInfo> listUnloadWay;
        public List<BasicTypeInfo> ListUnloadWay
        {
            get { return listUnloadWay; }
            set
            {
                if (listUnloadWay != value)
                {
                    listUnloadWay = value;
                    this.RaisePropertyChanged("ListUnloadWay");
                }
            }
        }
        private void InitUnloadWay()
        {
            if (StaticTreeState.BasicTypeInfo == LoadingState.LOADCOMPLETE)
            {
                List<BasicTypeInfo> list = new List<BasicTypeInfo>();
                foreach (BasicTypeInfo info in StaticBasicType.GetInstance().ListBasicTypeInfo)
                {
                    if (info.TypeID.Substring(0, 2) == "xl")
                    {
                        list.Add(info);
                    }
                }
                this.ListUnloadWay = list;
            }
        }
        /*核载方量*/
        private string planLoad;
        public string PlanLoad
        {
            get { return planLoad; }
            set
            {
                if (planLoad != value)
                {
                    planLoad = value;
                    this.RaisePropertyChanged("PlanLoad");
                }
            }
        }
        
        /*实载方量*/
        private string realLoad;
        public string RealLoad
        {
            get { return realLoad; }
            set
            {
                if (realLoad != value)
                {
                    realLoad = value;
                    if (realLoad != "")
                    {
                        realLoad = Convert.ToDouble(value).ToString("0.00");
                    }
                    this.RaisePropertyChanged("RealLoad");

                    if (Convert.ToDouble(this.realLoad) > Convert.ToDouble(this.planLoad))
                    {
                        if (MessageBox.Show("运输方量已经超载，是否确定？", "提示", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                        {

                        }
                        else
                        {
                            this.RealLoad = this.planLoad;
                        }
                        //MessageBox.Show("实载方量不能大于核载方量", "数据错误", MessageBoxButton.OKCancel);
                        //this.RealLoad = this.planLoad;
                    }
                }
            }
        }
        #region 确定和取消
        public DelegateCommand<Window> CancelCommand { get; set; }
        private void CancelCommandExecute(Window win)
        {
            win.Close();
        }
        public DelegateCommand<Window> ConfirmCommand { get; set; }
        private void ConfirmCommandExecute(Window win)
        {
            if (this.RealLoad == "")
            {
                MessageBox.Show("请填写实载方量", "提示", MessageBoxButton.OK);
                return;
            }
            
            this.ParentVM.SelectedTreeItem.ParameterVM = this;
            win.Close();
        }
        #endregion
    }
}
