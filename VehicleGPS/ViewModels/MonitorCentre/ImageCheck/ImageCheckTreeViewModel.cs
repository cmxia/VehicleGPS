using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.Prism.ViewModel;
using System.Windows;
using Microsoft.Practices.Prism.Commands;
using VehicleGPS.Models;
using VehicleGPS.Services.MonitorCentre.ImageCheck;
using System.Windows.Media;
using VehicleGPS.ViewModels.AutoComplete;
using System.Windows.Controls;

namespace VehicleGPS.ViewModels.MonitorCentre.ImageCheck
{
    class ImageCheckTreeViewModel : NotificationObject
    {
        private static ImageCheckTreeViewModel instance = null;
        public static ImageCheckTreeViewModel GetInstance()
        {
            if (instance==null)
            {
                instance = new ImageCheckTreeViewModel();
            }
            return instance;
        }
        public ImageCheckTreeViewModel()
        {
            this.TreeOperate = new ImageCheckOperate();
            TreeOperate.ReadTree();
            this.RefreshCommand = new DelegateCommand(new Action(RefreshCommandExecute));
            this.AutoCompleteSelectedCommand = new DelegateCommand<object>(new Action<object>(this.AutoCompleteSelectedCommandExecute));
        }
        /*选中的节点*/
        public ImageCheckTreeNodeViewModel SelectedNode { get; set; }

        #region 根节点
        private ImageCheckTreeNodeViewModel rootnode;

        public ImageCheckTreeNodeViewModel RootNode
        {
            get { return rootnode; }
            set
            {
                rootnode = value;
                this.RaisePropertyChanged("RootNode");
            }
        }
        #endregion

        #region 是否显示内部编号
        /// <summary>
        /// 是否显示内部编号
        /// </summary>
        public bool innerIDVisibleSelected = true;
        public bool InnerIDVisibleSelected
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
                    this.SetInnerIDState(vb, this.RootNode);
                    this.RaisePropertyChanged("InnerIDVisibleSelected");
                }
            }
        }
        #endregion

        #region 是否显示车牌号
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
                    this.SetNameState(vb, this.RootNode);
                    this.RaisePropertyChanged("NameVisibleSelected");
                }
            }
        }
        #endregion

        #region 隐藏或者显示内部编号
        /// <summary>
        /// 隐藏或者显示内部编号
        /// </summary>
        /// <param name="vb"></param>
        /// <param name="rootNode"></param>
        private void SetInnerIDState(Visibility vb, ImageCheckTreeNodeViewModel rootNode)
        {
            if (rootNode.ListChildNodes != null)
            {
                foreach (ImageCheckTreeNodeViewModel vtnvm in rootNode.ListChildNodes)
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
        /// <summary>
        /// 隐藏或者显示车辆名称
        /// </summary>
        /// <param name="vb"></param>
        /// <param name="rootNode"></param>
        private void SetNameState(Visibility vb, ImageCheckTreeNodeViewModel rootNode)
        {
            if (rootNode.ListChildNodes != null)
            {
                foreach (ImageCheckTreeNodeViewModel vtnvm in rootNode.ListChildNodes)
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
        private void ExpandAndFocus(ImageCheckTreeNodeViewModel root)
        {
            if (root.ListChildNodes == null || root.ListChildNodes.Count == 0)
            {
                return;
            }
            foreach (ImageCheckTreeNodeViewModel node in root.ListChildNodes)
            {
                if (node.nodeInfo.Name == this.AutoCompleteSelectedItem.Name)
                {
                    this.Expand(node.ParentNode);
                    node.IsFocus = true;
                    node.IsSelected = true;
                }
                else
                {
                    ExpandAndFocus(node);
                }
            }
        }
        public DelegateCommand<object> AutoCompleteSelectedCommand { get; set; }
        private void AutoCompleteSelectedCommandExecute(object control)
        {
            if (((AutoCompleteBox)control).SelectedItem != null)
            {
                this.AutoCompleteSelectedItem = (AutoCompleteItem)((AutoCompleteBox)control).SelectedItem;
                this.ExpandAndFocus(this.RootNode);
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
        /// <summary>
        ///  初始化AutoComplete查询数据集
        ///  14-6-6
        /// </summary>
        public void InitAutoComplete()
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

        private void Expand(ImageCheckTreeNodeViewModel node)
        {
            node.IsExpand = true;
            if (node.nodeInfo.ID != "admin")
            {
                Expand(node.ParentNode);
            }
        }
        public ImageCheckOperate TreeOperate { set; get; }//树的操作类
        public DelegateCommand RefreshCommand { get; set; }//刷新树形
        private void RefreshCommandExecute()
        {
            
        }
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
    }
}
