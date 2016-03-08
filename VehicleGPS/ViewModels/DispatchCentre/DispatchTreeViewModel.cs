using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.Prism.ViewModel;
using Microsoft.Practices.Prism.Commands;
using VehicleGPS.Services.DispatchCentre;
using VehicleGPS.Models;
using System.Windows.Media;
using System.Windows;
using System.Windows.Controls;
using VehicleGPS.ViewModels.AutoComplete;
using VehicleGPS.Models.Login;
using Newtonsoft.Json;

namespace VehicleGPS.ViewModels.DispatchCentre
{
    class DispatchTreeViewModel : NotificationObject
    {
        private static DispatchTreeViewModel instance = null;
        private DispatchTreeViewModel()
        {
            this.AutoCompleteSelectedCommand = new DelegateCommand<object>(new Action<object>(this.AutoCompleteSelectedCommandExecute));
            this.ConditionChangedCommand = new DelegateCommand<object>(new Action<object>(this.ConditionChangedCommandExecute));
            /*选择命令*/
            this.SelectedCommand = new DelegateCommand<object>(new Action<object>(this.SelectedCommandExecute));
            /*刷新命令*/
            this.RefreshCommand = new DelegateCommand(new Action(RefreshCommandExecute));
            /*右键菜单*/
            this.VehicleDispatchCommand = new DelegateCommand(new Action(VehicleDispatchExecute));
            this.VehicleRepairCommand = new DelegateCommand(new Action(VehicleRepairExecute));
            TreeOperate = new DispatchTreeOperate();
            //锁住调度树形
            StaticTreeState.DispatchTreeLoad = LoadingState.LOADING;
            TreeOperate.ReadTree();
            StaticTreeState.DispatchTreeContruct = true;
        }
        public static DispatchTreeViewModel GetInstance()
        {
            if (instance == null)
            {
                instance = new DispatchTreeViewModel();
            }
            return instance;
        }
        public DispatchTreeNodeViewModel rootNode;
        public DispatchTreeNodeViewModel RootNode
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
        #region 右键菜单
        public DelegateCommand VehicleDispatchCommand { get; set; }//车辆调派
        private void VehicleDispatchExecute()
        {
            if (StaticTreeState.DispatchTreeLoad != LoadingState.LOADCOMPLETE)
            {
                MessageBox.Show("树形数据更新中，请稍后再试！");
                return;
            }
            if (SelectedNode != null && SelectedNode.ID.Contains("VEHI"))
            {
                if (SelectedNode.TaskState == VehicleCommon.TaskMaintain)
                {
                    StaticTreeState.DispatchTreeLoad = LoadingState.LOADING;
                    foreach (CVDetailInfo info in StaticDetailInfo.GetInstance().ListVehicleDetailInfo)
                    {
                        if (info.VehicleNum == SelectedNode.ID)
                        {
                            info.VehicleState = "0";
                            if (UpdateVehicleTaskStatus("0", info.VehicleNum))
                            {
                                SendMessage("0", info.SIM);
                            }
                            break;
                        }
                    }
                    this.TreeOperate.RefreshTree();
                }
            }
            StaticTreeState.DispatchTreeLoad = LoadingState.LOADCOMPLETE;
        }
        public DelegateCommand VehicleRepairCommand { get; set; }//车辆维修
        private void VehicleRepairExecute()
        {
            if (StaticTreeState.DispatchTreeLoad != LoadingState.LOADCOMPLETE)
            {
                MessageBox.Show("树形数据更新中，请稍后再试！");
                return;
            }
            if (SelectedNode != null && SelectedNode.ID.Contains("VEHI"))
            {
                if (SelectedNode.TaskState != VehicleCommon.TaskMaintain)
                {
                    StaticTreeState.DispatchTreeLoad = LoadingState.LOADING;
                    foreach (CVDetailInfo info in StaticDetailInfo.GetInstance().ListVehicleDetailInfo)
                    {
                        if (info.VehicleNum == SelectedNode.ID)
                        {
                            info.VehicleState = "2";
                            if (UpdateVehicleTaskStatus("2", info.VehicleNum))
                            {
                                SendMessage("2", info.SIM);
                            }
                            break;
                        }
                    }
                    this.TreeOperate.RefreshTree();
                    StaticTreeState.DispatchTreeLoad = LoadingState.LOADCOMPLETE;
                }
            }
        }

        private bool UpdateVehicleTaskStatus(string status, string vehicleNum)
        {
            string sql = "update InfoVehicle set VehicleState='" + status + "' where Vehiclenum='" + vehicleNum + "'";
            string updateStatus = VehicleCommon.wcfDBHelper.BexcuteSql(StaticLoginInfo.GetInstance().UserName, sql);
            return string.Compare(updateStatus, "error") == 0 ? false : true;
        }
        private bool SendMessage(string status, string SIM)
        {
            Dictionary<string, string> instruction = new Dictionary<string, string>();
            instruction.Add("cmd", "CARSTATUS_TYPE_FROMWEB");
            instruction.Add("simId", SIM);
            instruction.Add("cmdid", SIM + "_CARSTATUS_TYPE_FROMWEB");
            instruction.Add("vehiclestate", status);
            string insstring = JsonConvert.SerializeObject(instruction);
            return zmq.zmqPackHelper.zmqInstructionsPack(SIM, insstring);
        }
        #endregion
        public DispatchTreeOperate TreeOperate { set; get; }//树的操作类
        public DelegateCommand RefreshCommand { get; set; }//刷新树形
        public void RefreshCommandExecute()
        {
            this.RefreshState(false);//禁用刷新按钮
            StaticTreeState.BasicTypeInfo = LoadingState.NOLOADING;
            StaticTreeState.ClientBasicInfo = LoadingState.NOLOADING;
            StaticTreeState.VehicleAllBasicInfo = LoadingState.NOLOADING;
            //StaticTreeState.VehicleGPSInfo = LoadingState.NOLOADING;

            TreeOperate.RefreshTree();
        }
        /*加载树形信息失败*/
        public void LoadTreeFail()
        {
            this.RefreshState(true);//启用刷新按钮
            string errorMsg = "";
            if (StaticTreeState.ClientBasicInfo != LoadingState.LOADCOMPLETE)
            {
                errorMsg += "(站点关键信息)";
            }
            if (StaticTreeState.VehicleBasicInfo != LoadingState.LOADCOMPLETE)
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
            if (selectedItem.GetType().Name != "DispatchTreeNodeViewModel")
            {
                return;
            }
            DispatchTreeNodeViewModel selectedVM = (DispatchTreeNodeViewModel)selectedItem;
            if (selectedVM.nodeInfo.ID != "TaskNode")
            {//非任务点
                this.SelectedNode = selectedVM.nodeInfo;
            }
            else
            {
                this.SelectedNode = null;
            }
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
        //private AutoCompleteItem autoCompleteSelectedItem;
        //public AutoCompleteItem AutoCompleteSelectedItem
        //{
        //    get { return autoCompleteSelectedItem; }
        //    set
        //    {
        //        if (autoCompleteSelectedItem != value)
        //        {
        //            autoCompleteSelectedItem = value;
        //            this.RaisePropertyChanged("AutoCompleteSelectedItem");
        //        }
        //    }
        //}
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
            //if (StaticTreeState.VehicleBasicInfo == LoadingState.LOADCOMPLETE)
            //{
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
            // }
        }
        public void InitAutoCompleteStation()
        {
            if (StaticTreeState.ClientBasicInfo == LoadingState.LOADCOMPLETE)
            {
                StaticBasicInfo baseInfo = StaticBasicInfo.GetInstance();
                List<AutoCompleteItem> tmpList = new List<AutoCompleteItem>();
                foreach (CVBasicInfo info in baseInfo.ListClientBasicInfo)
                {
                    if (baseInfo.ListVehicleOfClientBaseInfo.Contains(info.ID))
                    {
                        AutoCompleteItem item = new AutoCompleteItem();
                        item.Name = info.Name;
                        item.SIM = info.ID;//对于站点SIM为站点ID
                        item.NameSim = info.Name + info.SIM;
                        tmpList.Add(item);
                    }
                }
                this.ListAutoComplete = tmpList;
            }
        }
        #endregion

        #region 获取焦点和展开
        private void ExpandAndFocus(DispatchTreeNodeViewModel root)
        {
            if (root.listChildNodes == null || root.listChildNodes.Count == 0)
            {
                return;
            }
            foreach (DispatchTreeNodeViewModel node in root.listChildNodes)
            {
                if (node.nodeInfo.Name == this.AutoCompleteSelectedItem.Name)
                {
                    this.Expand(node.ParentNode);  
                    if (node.ListChildNodes != null && node.ListChildNodes.Count > 0)
                    {
                        foreach (DispatchTreeNodeViewModel childnode in node.ListChildNodes)
                        {
                            childnode.IsExpand = true;
                        }
                    }
                    node.IsExpand = true;
                    node.IsFocus = true;
                    node.IsSelected = true;
                }
                else
                {
                    node.IsExpand = false;
                    ExpandAndFocus(node);
                }
            }
        }
        private void Expand(DispatchTreeNodeViewModel node)
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

