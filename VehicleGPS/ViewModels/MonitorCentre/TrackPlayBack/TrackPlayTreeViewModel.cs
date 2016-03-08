using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using Microsoft.Practices.Prism.ViewModel;
using Microsoft.Practices.Prism.Commands;
using VehicleGPS.Services.MonitorCentre.TrackPlayBack;
using VehicleGPS.Models;
using System.Windows.Media;
using System.Windows.Controls;
using VehicleGPS.ViewModels.AutoComplete;

namespace VehicleGPS.ViewModels.MonitorCentre.TrackPlayBack
{
    /// <summary>
    /// 轨迹回放ViewModel类
    /// </summary>
    class TrackPlayTreeViewModel: NotificationObject
    {
        private TrackPlayViewModel trackPlayVM;
        public TrackPlayTreeViewModel(object parentVM)
        {
            this.trackPlayVM = (TrackPlayViewModel)parentVM;
            this.AutoCompleteSelectedCommand = new DelegateCommand<object>(new Action<object>(this.AutoCompleteSelectedCommandExecute));
            this.ConfirmCommand = new DelegateCommand<Window>(new Action<Window>(ConfirmCommandExecute));
            this.CancelCommand = new DelegateCommand<Window>(new Action<Window>(CancelCommandExecute));
            this.RefreshCommand = new DelegateCommand(new Action(RefreshCommandExecute));
            TreeOperate = new TrackPlayTreeOperate(this);
            TreeOperate.ReadTree();
        }
        /// <summary>
        /// 树的根节点
        /// </summary>
        public TrackPlayTreeNodeViewModel rootNode;
        public TrackPlayTreeNodeViewModel RootNode
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

        #region 是否显示内部编号
        /*是否显示内部编号*/
        /// <summary>
        /// 是否显示内部编号
        /// </summary>
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
        #endregion 

        #region 是否显示车牌号
        /*是否显示车牌号*/
        /// <summary>
        /// 是否显示车牌号
        /// </summary>
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
        #endregion

        #region 隐藏或者显示内部编号
        /*隐藏或者显示内部编号*/
        /// <summary>
        /// 隐藏或者显示内部编号(函数)
        /// </summary>
        /// <param name="vb"></param>
        /// <param name="rootNode"></param>
        private void SetInnerIDState(Visibility vb, TrackPlayTreeNodeViewModel rootNode)
        {
            if (rootNode.listChildNodes != null)
            {
                foreach (TrackPlayTreeNodeViewModel vtnvm in rootNode.listChildNodes)
                {
                    SetInnerIDState(vb, vtnvm);
                }
            }
            else
            {
                rootNode.InnerIDVisible = vb;
            }
        }
        #endregion

        #region 隐藏或者显示车辆名称
        /*隐藏或者显示车辆名称*/
        /// <summary>
        /// 隐藏或者显示车辆名称
        /// </summary>
        /// <param name="vb"></param>
        /// <param name="rootNode"></param>
        private void SetNameState(Visibility vb, TrackPlayTreeNodeViewModel rootNode)
        {
            if (rootNode.listChildNodes != null)
            {
                foreach (TrackPlayTreeNodeViewModel vtnvm in rootNode.listChildNodes)
                {
                    SetNameState(vb, vtnvm);
                }
            }
            else
            {
                rootNode.NameVisible = vb;
            }
        }
        #endregion

        public TrackPlayTreeOperate TreeOperate { set; get; }//树的操作类

        #region 刷新树形Command
        public DelegateCommand RefreshCommand { get; set; }//刷新树形
        /// <summary>
        /// 刷新树形
        /// </summary>
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
        #endregion

        #region 加载树形信息失败
        /*加载树形信息失败*/
        /// <summary>
        /// 加载树形信息失败
        /// </summary>
        public void LoadTreeFail()
        {
            this.RefreshState(true);//启用刷新按钮
            string errorMsg = "";
            if (StaticTreeState.ClientBasicInfo == LoadingState.LOADDINGFAIL)
            {
                errorMsg += "(用户关键信息)";
            }
            //if (StaticTreeState.VehicleBasicInfo == LoadingState.LOADDINGFAIL)
            //{
            //    errorMsg += "(车辆关键信息)";
            //}
            if (StaticTreeState.VehicleAllBasicInfo == LoadingState.LOADDINGFAIL)
            {
                errorMsg += "(车辆基础信息)";
            }
            MessageBox.Show("加载" + errorMsg + "失败，请刷新重试", "数据加载失败", MessageBoxButton.OKCancel);
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
                    tmpList.Add(item);
                }
                this.ListAutoComplete = tmpList;
            //}
        }
        #endregion

        #region 获取焦点和展开
        private void ExpandAndFocus(TrackPlayTreeNodeViewModel root)
        {
            if (root.listChildNodes == null || root.listChildNodes.Count == 0)
            {
                return;
            }
            foreach (TrackPlayTreeNodeViewModel node in root.listChildNodes)
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
        private void Expand(TrackPlayTreeNodeViewModel node)
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
        /// <summary>
        /// 确定按钮
        /// </summary>
        /// <param name="win"></param>
        public void ConfirmCommandExecute(Window win)
        {
            GetSelectedVehicle(this.rootNode);
            win.Close();
        }
        /// <summary>
        /// 取消按钮
        /// </summary>
        /// <param name="win"></param>
        public void CancelCommandExecute(Window win)
        {
            win.Close();
        }
        /*遍历树，获得选择的车辆*/
        /// <summary>
        /// 选择车辆
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        private bool GetSelectedVehicle(TrackPlayTreeNodeViewModel node)
        {
            if (node.listChildNodes != null)
            {
                foreach (TrackPlayTreeNodeViewModel vtnvm in node.listChildNodes)
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
                        trackPlayVM.SelectedVehicle = node.nodeInfo;
                        return true;
                    }
                }
            }
            return false;
        }
        #endregion
    }
}
