using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.Prism.Commands;
using VehicleGPS.Services.MonitorCentre.VehicleTrack;
using VehicleGPS.Models;
using System.Windows;
using Microsoft.Practices.Prism.ViewModel;
using System.Windows.Media;
using VehicleGPS.ViewModels.AutoComplete;
using System.Windows.Controls;

namespace VehicleGPS.ViewModels.MonitorCentre.VehicleTrack
{
    class VehicleTrackTreeViewModel: NotificationObject
    {
        public VehicleTrackViewModel ParentViewModel { get; set; }
        public VehicleTrackTreeViewModel(object parentViewModel)
        {
            this.AutoCompleteSelectedCommand = new DelegateCommand<object>(new Action<object>(this.AutoCompleteSelectedCommandExecute));
            this.ParentViewModel = (VehicleTrackViewModel)parentViewModel;
            this.ConfirmCommand = new DelegateCommand<Window>(new Action<Window>(ConfirmCommandExecute));
            this.CancelCommand = new DelegateCommand<Window>(new Action<Window>(CancelCommandExecute));
            this.RefreshCommand = new DelegateCommand(new Action(RefreshCommandExecute));
            /*穿入树模型，为了在树操作期间对树构造赋值*/
            TreeOperate = new VehicleTrackTreeOperate(this);
            TreeOperate.ReadTree();
        }
        public VehicleTrackTreeNodeViewModel rootNode;
        public VehicleTrackTreeNodeViewModel RootNode
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
        /*是否显示内部编号*/
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
                    this.SetInnerIDState(vb, this.rootNode);
                    this.RaisePropertyChanged("InnerIDVisibleSelected");
                }
            }
        }
        /*是否显示车牌号*/
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
                    this.SetNameState(vb, this.rootNode);
                    this.RaisePropertyChanged("NameVisibleSelected");
                }
            }
        }
        /*隐藏或者显示内部编号*/
        private void SetInnerIDState(Visibility vb, VehicleTrackTreeNodeViewModel rootNode)
        {
            if (rootNode.listChildNodes != null)
            {
                foreach (VehicleTrackTreeNodeViewModel vtnvm in rootNode.listChildNodes)
                {
                    SetInnerIDState(vb, vtnvm);
                }
            }
            else
            {
                rootNode.InnerIDVisible = vb;
            }
        }
        /*隐藏或者显示车辆名称*/
        private void SetNameState(Visibility vb, VehicleTrackTreeNodeViewModel rootNode)
        {
            if (rootNode.listChildNodes != null)
            {
                foreach (VehicleTrackTreeNodeViewModel vtnvm in rootNode.listChildNodes)
                {
                    SetNameState(vb, vtnvm);
                }
            }
            else
            {
                rootNode.NameVisible = vb;
            }
        }

        public VehicleTrackTreeOperate TreeOperate { set; get; }//树的操作类
        public DelegateCommand RefreshCommand { get; set; }//刷新树形
        private void RefreshCommandExecute()
        {
            this.RefreshState(false);//禁用刷新按钮
            StaticTreeState.BasicTypeInfo = LoadingState.NOLOADING;
            StaticTreeState.ClientBasicInfo = LoadingState.NOLOADING;
            StaticTreeState.VehicleBasicInfo = LoadingState.NOLOADING;
            StaticTreeState.VehicleAllBasicInfo = LoadingState.NOLOADING;
            StaticTreeState.VehicleGPSInfo = LoadingState.NOLOADING;

            TreeOperate.RefreshTree();
        }
        /*加载树形信息失败*/
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
                    tmpList.Add(item);
                }
                this.ListAutoComplete = tmpList;
            //}
        }
        #endregion

        #region 获取焦点和展开
        private void ExpandAndFocus(VehicleTrackTreeNodeViewModel root)
        {
            if (root.listChildNodes == null || root.listChildNodes.Count == 0)
            {
                return;
            }
            foreach (VehicleTrackTreeNodeViewModel node in root.listChildNodes)
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
        private void Expand(VehicleTrackTreeNodeViewModel node)
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

        #region 确定和取消
        public DelegateCommand<Window> ConfirmCommand { get; set; }
        public DelegateCommand<Window> CancelCommand { get; set; }
        public void ConfirmCommandExecute(Window win)
        {
            GetSelectedVehicle(this.rootNode);
            win.Close();
        }
        public void CancelCommandExecute(Window win)
        {
            win.Close();
        }
        /*遍历树，获得选择的车辆*/
        private bool GetSelectedVehicle(VehicleTrackTreeNodeViewModel node)
        {
            if (node.listChildNodes != null)
            {
                foreach (VehicleTrackTreeNodeViewModel vtnvm in node.listChildNodes)
                {
                    if (GetSelectedVehicle(vtnvm))
                    {
                        return true;
                    }
                }
            }
            else
            {
                if (node.nodeInfo.SIM != "0")//车辆节点
                {
                    if (node.isSelected == true)
                    {
                        foreach (CVDetailInfo info in StaticDetailInfo.GetInstance().ListVehicleDetailInfo)
                        {
                            if (info.VehicleId == node.NodeInfo.Name)
                            {
                                this.ParentViewModel.SelectedInfo = info;
                                return true;
                            }
                        }
                        /*如果找不到数据，则对车辆SIM和车牌号赋值--折中方案*/
                        CVDetailInfo detailInfo = new CVDetailInfo();
                        detailInfo.VehicleId = node.nodeInfo.Name;
                        detailInfo.SIM = node.nodeInfo.SIM;
                        this.ParentViewModel.SelectedInfo = detailInfo;
                        return true;
                    }
                }
            }
            return false;
        }
        #endregion
    }
}
