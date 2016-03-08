using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.Prism.ViewModel;
using Microsoft.Practices.Prism.Commands;
using System.Windows;
using VehicleGPS.Services.ReportCentre;
using VehicleGPS.Models;
using System.Windows.Media;
using VehicleGPS.ViewModels.AutoComplete;
using System.Windows.Controls;

namespace VehicleGPS.ViewModels.ReportCentre
{
    class ReportTreeViewModel: NotificationObject
    {
        private static ReportTreeViewModel instance = null;
        private ReportTreeViewModel()
        {
            this.SelectedCommand = new DelegateCommand<object>(new Action<object>(this.SelectedCommandExecute));
            this.RefreshCommand = new DelegateCommand(new Action(RefreshCommandExecute));
            this.AutoCompleteSelectedCommand = new DelegateCommand<object>(new Action<object>(this.AutoCompleteSelectedCommandExecute));
            TreeOperate = new ReportTreeOperate();
            TreeOperate.ReadTree();
            StaticTreeState.ReportTreeContruct = true;
        }
        public static ReportTreeViewModel GetInstance()
        {
            if (instance == null)
            {
                instance = new ReportTreeViewModel();
            }
            return instance;
        }
        public ReportTreeNodeViewModel rootNode = null;
        public ReportTreeNodeViewModel RootNode
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
        private void SetInnerIDState(Visibility vb, ReportTreeNodeViewModel rootNode)
        {
            if (rootNode.listChildNodes != null)
            {
                foreach (ReportTreeNodeViewModel vtnvm in rootNode.listChildNodes)
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
        private void SetNameState(Visibility vb, ReportTreeNodeViewModel rootNode)
        {
            if (rootNode.listChildNodes != null)
            {
                foreach (ReportTreeNodeViewModel vtnvm in rootNode.listChildNodes)
                {
                    SetNameState(vb, vtnvm);
                }
            }
            else
            {
                rootNode.NameVisible = vb;
            }
        }

        public ReportTreeOperate TreeOperate { set; get; }//树的操作类
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

        /*选择的站点或者车辆*/
        public CVBasicInfo SelectedNode { get; set; }
        /*选择事件*/
        public DelegateCommand<object> SelectedCommand { get; set; }
        private void SelectedCommandExecute(object selectedItem)
        {
            ReportTreeNodeViewModel selectedVM = (ReportTreeNodeViewModel)selectedItem;
            this.SelectedNode = selectedVM.nodeInfo;
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
            if (StaticTreeState.VehicleBasicInfo == LoadingState.LOADCOMPLETE)
            {
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
            }
        }
        #endregion

        #region 获取焦点和展开
        private void ExpandAndFocus(ReportTreeNodeViewModel root)
        {
            if (root.listChildNodes == null || root.listChildNodes.Count == 0)
            {
                return;
            }
            foreach (ReportTreeNodeViewModel node in root.listChildNodes)
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
        private void Expand(ReportTreeNodeViewModel node)
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

        #region 获取选择的车辆
        public List<CVBasicInfo> GetSelectedVehicles()
        {
            List<CVBasicInfo> selectedList = new List<CVBasicInfo>();
            this.AddSelectedVehicles(this.rootNode, selectedList);
            return selectedList;
        }
        public void AddSelectedVehicles(ReportTreeNodeViewModel root,List<CVBasicInfo> list)
        {
            if (root.ListChildNodes != null && root.ListChildNodes.Count != 0)
            {
                foreach (ReportTreeNodeViewModel node in root.listChildNodes)
                {
                    if (node.nodeInfo.SIM != "0" && node.isSelected == true)
                    {
                        list.Add(node.nodeInfo);
                    }
                    else
                    {
                        AddSelectedVehicles(node, list);
                    }
                }
            }
            //else
            //{
            //    if (root.nodeInfo.SIM != "0" && root.isSelected == true)
            //    {
            //        list.Add(root.nodeInfo);
            //    }
            //    else
            //    {
            //        AddSelectedVehicles(root, list);
            //    }
            //}
        }
        #endregion
    }
}