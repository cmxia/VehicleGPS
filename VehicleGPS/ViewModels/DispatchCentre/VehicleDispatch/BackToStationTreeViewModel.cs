using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.Prism.ViewModel;
using Microsoft.Practices.Prism.Commands;
using VehicleGPS.Models;
using System.Windows;
using System.Windows.Controls;
using VehicleGPS.ViewModels.AutoComplete;
using System.Windows.Media;
using VehicleGPS.Services.DispatchCentre.VehicleDispatch;
using VehicleGPS.Models.DispatchCentre.VehicleDispatch;

namespace VehicleGPS.ViewModels.DispatchCentre.VehicleDispatch
{
    class BackToStationTreeViewModel : NotificationObject
    {
        public BackToStationViewModel StationVM { get; set; }
        public BackToStationDataOperate DataOperate { get; set; }
        public BackToStationTreeViewModel()
        {
            this.AutoCompleteSelectedCommand = new DelegateCommand<object>(new Action<object>(this.AutoCompleteSelectedCommandExecute));
            this.ConditionChangedCommand = new DelegateCommand<object>(new Action<object>(this.ConditionChangedCommandExecute));
            /*选择命令*/
            this.SelectedCommand = new DelegateCommand<object>(new Action<object>(this.SelectedCommandExecute));
            /*刷新命令*/
            this.RefreshCommand = new DelegateCommand(new Action(RefreshCommandExecute));
            TreeOperate = new BackToStationTreeOperate(this);
            DataOperate = new BackToStationDataOperate();
            TreeOperate.ReadTree();
        }

        public BackToStationTreeNodeViewModel rootNode;
        public BackToStationTreeNodeViewModel RootNode
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

        public BackToStationTreeOperate TreeOperate { set; get; }//树的操作类
        public DelegateCommand RefreshCommand { get; set; }//刷新树形
        private void RefreshCommandExecute()
        {
            this.RefreshState(false);//禁用刷新按钮
            StaticTreeState.BasicTypeInfo = LoadingState.NOLOADING;
            StaticTreeState.ClientBasicInfo = LoadingState.NOLOADING;
            StaticTreeState.VehicleBasicInfo = LoadingState.NOLOADING;
            StaticTreeState.VehicleAllBasicInfo = LoadingState.NOLOADING;
            //StaticTreeState.VehicleGPSInfo = LoadingState.NOLOADING;
            TreeOperate.RefreshTree();
        }
        /*加载树形信息失败*/
        public void LoadTreeFail()
        {
            this.RefreshState(true);//启用刷新按钮
            string errorMsg = "";
            if (StaticTreeState.ClientBasicInfo == LoadingState.LOADDINGFAIL)
            {
                errorMsg += "(站点关键信息)";
            }
            if (StaticTreeState.VehicleBasicInfo == LoadingState.LOADDINGFAIL)
            {
                errorMsg += "(车辆关键信息)";
            }
            MessageBox.Show("加载" + errorMsg + "失败，请刷新重试", "数据加载失败", MessageBoxButton.OKCancel);
        }

        /*选择的站点或者车辆*/
        public CVBasicInfo SelectedNode { get; set; }
        /*选择事件*/
        public DelegateCommand<object> SelectedCommand { get; set; }
        private void SelectedCommandExecute(object selectedItem)
        {
            if (selectedItem.GetType().Name != "BackToStationTreeNodeViewModel")
            {
                return;
            }
            BackToStationTreeNodeViewModel selectedVM = (BackToStationTreeNodeViewModel)selectedItem;
            if (selectedVM.nodeInfo.TypeID == null || selectedVM.nodeInfo.TypeID == "")
            {
                return;
            }
            if (selectedVM.nodeInfo.ID.Contains("VEHI"))
            {//车辆节点
                this.SelectedNode = selectedVM.nodeInfo;
                BackToStationInfo info = this.DataOperate.GetTransConDetailByVehicleNum(this.SelectedNode.ID);
                if (info != null)
                {
                    this.InitStationVM(info);
                }
                else
                {
                    MessageBox.Show("找不到该车的任务单消息，请刷新树形重试", "强制回站", MessageBoxButton.OK);
                    return;
                }
            }
        }
        /*初始化强制回站模型*/
        private void InitStationVM(BackToStationInfo info)
        {
            this.StationVM.FPlanId = info.fPlanId;
            this.StationVM.SIM = info.sim;
            this.StationVM.VehicleID = info.vehicleID;
            this.StationVM.VehicleTypeName = info.vehicleTypeName;
            this.StationVM.StartPoint = info.startPoint;
            this.StationVM.EndPoint = info.endPoint;
            this.StationVM.TransCapPer = info.transCapPer;
            this.StationVM.LoadAmount = info.loadAmount;
            this.StationVM.ConcreteName = info.concreteName;
            this.StationVM.CarsStatus = info.carsStatus;
            this.StationVM.OffTypeName = info.offTypeName;
            this.StationVM.DriverId = info.driverId;
            this.StationVM.DriverName = info.driverName;
            this.StationVM.IsVisibility = Visibility.Visible;
            this.StationVM.ImageUrl = VehicleCommon.GetVehicleImageURL(this.SelectedNode.TypeID, VehicleCommon.VSOnlineRun);
        }
        #region 选择查询条件
        public DelegateCommand<object> ConditionChangedCommand { get; set; }
        private void ConditionChangedCommandExecute(object selectedItem)
        {
            if (selectedItem != null)
            {
                string selectedStr = ((ComboBoxItem)selectedItem).Content.ToString();
                if (selectedStr == "车辆")
                {
                    this.InitAutoComplete();
                }
                if (selectedStr == "站点")
                {
                    this.InitAutoCompleteStation();
                }
            }
        }
        #endregion
        #region AutoComplete查询模型
        public DelegateCommand<object> AutoCompleteSelectedCommand { get; set; }
        private void AutoCompleteSelectedCommandExecute(object control)
        {
            if (((AutoCompleteBox)control).SelectedItem != null)
            {
                this.AutoCompleteSelectedItem = (AutoCompleteItem)((AutoCompleteBox)control).SelectedItem;
                this.ExpandAndFocus(this.rootNode);
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
        /*初始化AutoComplete查询数据集*/
        public void InitAutoComplete()
        {
            if (this.rootNode.listChildNodes[0].listChildNodes != null && this.rootNode.listChildNodes[0].listChildNodes.Count != 0)
            {
                List<AutoCompleteItem> tmpList = new List<AutoCompleteItem>();
                foreach (BackToStationTreeNodeViewModel stationInfo in this.rootNode.listChildNodes[0].listChildNodes)
                {
                    foreach (BackToStationTreeNodeViewModel vehicleInfo in stationInfo.listChildNodes)
                    {
                        AutoCompleteItem item = new AutoCompleteItem();
                        item.Name = vehicleInfo.nodeInfo.Name;
                        item.SIM = vehicleInfo.nodeInfo.ID;
                        item.NameSim = vehicleInfo.nodeInfo.Name + vehicleInfo.nodeInfo.SIM;
                        tmpList.Add(item);
                    }
                }
                this.ListAutoComplete = tmpList;
            }
        }
        public void InitAutoCompleteStation()
        {
            if (this.rootNode.listChildNodes[0].listChildNodes != null && this.rootNode.listChildNodes[0].listChildNodes.Count != 0)
            {
                List<AutoCompleteItem> tmpList = new List<AutoCompleteItem>();
                foreach (BackToStationTreeNodeViewModel info in this.rootNode.listChildNodes[0].listChildNodes)
                {
                    AutoCompleteItem item = new AutoCompleteItem();
                    item.Name = info.nodeInfo.Name;
                    item.SIM = info.nodeInfo.ID;//对于站点SIM为站点ID
                    item.NameSim = info.nodeInfo.Name + info.nodeInfo.SIM;
                    tmpList.Add(item);
                }
                this.ListAutoComplete = tmpList;
            }
        }
        #endregion

        #region 获取焦点和展开
        private void ExpandAndFocus(BackToStationTreeNodeViewModel root)
        {
            if (root.listChildNodes == null || root.listChildNodes.Count == 0)
            {
                return;
            }
            foreach (BackToStationTreeNodeViewModel node in root.listChildNodes)
            {
                if (node.nodeInfo.Name == this.AutoCompleteSelectedItem.Name)
                {
                    this.Expand(node.ParentNode);
                    node.IsFocus = true;
                }
                else
                {
                    ExpandAndFocus(node);
                }
            }
        }
        private void Expand(BackToStationTreeNodeViewModel node)
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
    }
}