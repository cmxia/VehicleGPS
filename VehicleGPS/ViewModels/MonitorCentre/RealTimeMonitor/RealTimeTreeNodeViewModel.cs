//RealTimeTreeNodeViewModel
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.Prism.ViewModel;
using VehicleGPS.Models;
using System.Windows;
using System.Threading;
using VehicleGPS.Views.Control.MonitorCentre.RealTimeMonitor;

namespace VehicleGPS.ViewModels.MonitorCentre.RealTimeMonitor
{
    class RealTimeTreeNodeViewModel : NotificationObject
    {
        #region 节点信息
        public CVBasicInfo nodeInfo;
        public CVBasicInfo NodeInfo//节点信息
        {
            get { return nodeInfo; }
            set
            {
                if (nodeInfo != value)
                {
                    nodeInfo = value;
                    this.RaisePropertyChanged("NodeInfo");
                }
            }
        }
        public void RaiseNodeInfoChanged()
        {
            this.RaisePropertyChanged("NodeInfo");
        }
        #endregion

        public bool isUsed;//用户判断该节点是否已经加载

        #region 是否展开
        /*是否展开*/
        public bool isExpand;
        public bool IsExpand
        {
            get { return isExpand; }
            set
            {
                if (isExpand != value)
                {
                    isExpand = value;
                    this.RaisePropertyChanged("IsExpand");
                }
            }
        }
        #endregion

        #region 是否获取可视焦点
        /*是否获取可视焦点*/
        public bool isFocus;
        public bool IsFocus
        {
            get { return isFocus; }
            set
            {
                if (isFocus != value)
                {
                    isFocus = value;
                    this.RaisePropertyChanged("IsFocus");
                }
            }
        }
        #endregion

        #region 是否车辆还是用户,用于隐藏或者显示单位在线车辆和总车辆数量
        /*是否车辆还是用户,用于隐藏或者显示单位在线车辆和总车辆数量*/
        public Visibility onlineNumberVisible;
        public Visibility OnlineNumberVisible
        {
            get { return onlineNumberVisible; }
            set
            {
                if (onlineNumberVisible != value)
                {
                    onlineNumberVisible = value;
                    this.RaisePropertyChanged("OnlineNumberVisible");
                }
            }
        }
       #endregion

        #region 是否显示内部编号
        /*是否显示内部编号*/
        public Visibility innerIDVisible;
        public Visibility InnerIDVisible
        {
            get { return innerIDVisible; }
            set
            {
                if (innerIDVisible != value)
                {
                    innerIDVisible = value;
                    this.RaisePropertyChanged("InnerIDVisible");
                }
            }
        }
        #endregion

        #region 是否显示车牌号
        /*是否显示车牌号*/
        public Visibility nameVisible;
        public Visibility NameVisible
        {
            get { return nameVisible; }
            set
            {
                if (nameVisible != value)
                {
                    nameVisible = value;
                    this.RaisePropertyChanged("NameVisible");
                }
            }
        }
        #endregion

        #region 节点选择状态
        /*节点选择状态*/
        public bool? isSelected;
        public bool? IsSelected
        {
            get { return isSelected; }
            set
            {
                if (isSelected != value)
                {

                    if (DataLoadingState())
                    {
                        isSelected = value;
                        ChangeChildNodes(this);
                        ChangeParentNodes(this);
                        this.RaisePropertyChanged("IsSelected");

                        RealTimeTree.treeStatic.IsEnabled = false;
                        VehicleGPS.Views.Control.MonitorCentre.MonitorCentre.cluster_CB.IsEnabled = false;
                        RealTimeViewModel realTimeVM = RealTimeViewModel.GetInstance();
                        realTimeVM.SelectedVehicle = null;//选择的车辆置空
                        realTimeVM.SelectedGPSInfo();//获取选择车辆的实时gps数据 以及报警数据
                        realTimeVM.InitBaiduMap();//将实时gps数据显示在地图上
                    }
                }
            }
        }
        #endregion

        #region 节点图片Url
        /*节点图片Url*/
        public string imageTip;
        public string ImageTip
        {
            get { return imageTip; }
            set
            {
                if (imageTip != value)
                {
                    imageTip = value;
                    this.RaisePropertyChanged("ImageTip");
                }
            }
        }
        #endregion

        #region 节点图片提示
        /*节点图片Url*/
        public string imageUrl;
        public string ImageUrl
        {
            get { return imageUrl; }
            set
            {
                if (imageUrl != value)
                {
                    imageUrl = value;
                    this.RaisePropertyChanged("ImageUrl");
                }
            }
        }
        #endregion

        #region 子节点
        /*子节点*/
        public List<RealTimeTreeNodeViewModel> listChildNodes;
        public List<RealTimeTreeNodeViewModel> ListChildNodes
        {
            get { return listChildNodes; }
            set
            {
                if (listChildNodes != value)
                {
                    listChildNodes = value;
                    this.RaisePropertyChanged("ListChildNodes");
                }
            }
        }
        #endregion

        #region 父节点
        /*父节点*/
        public RealTimeTreeNodeViewModel parentNode;
        public RealTimeTreeNodeViewModel ParentNode
        {
            get { return parentNode; }
            set
            {
                if (parentNode != value)
                {
                    parentNode = value;
                    this.RaisePropertyChanged("ParentNode");
                }
            }
        }
        #endregion

        #region 判断数据加载情况
        /*判断数据加载情况*/
        /// <summary>
        /// 判断数据加载情况
        /// </summary>
        /// <returns></returns>
        public bool DataLoadingState()
        {
            //if (StaticTreeState.ClientBasicInfo == LoadingState.LOADCOMPLETE &&
            //    StaticTreeState.VehicleBasicInfo == LoadingState.LOADCOMPLETE &&
            //    StaticTreeState.VehicleAllBasicInfo == LoadingState.LOADCOMPLETE &&
            //    StaticTreeState.VehicleGPSInfo == LoadingState.LOADCOMPLETE && 
            //    !StaticTreeState.IsRefreshRealTimeData)
            if (StaticTreeState.ClientBasicInfo == LoadingState.LOADCOMPLETE &&
              
              StaticTreeState.VehicleAllBasicInfo == LoadingState.LOADCOMPLETE &&
              
              !StaticTreeState.IsRefreshRealTimeData)
            {
                return true;
            }
            if (StaticTreeState.IsRefreshRealTimeData)
            {//更新数据
                MessageBox.Show("正在定时更新车辆详细数据和地图数据，请稍后...", "数据更新", MessageBoxButton.OKCancel);
                //return true;
            }
            else
            {
                MessageBox.Show("正在努力加载车辆详细数据，请稍后...", "数据加载", MessageBoxButton.OKCancel);
            }
            return false;
        }
        #endregion

        #region 改变子节点的选择状态
        /*改变子节点的选择状态*/
        public void ChangeChildNodes(RealTimeTreeNodeViewModel CurrentNode)
        {
            if (CurrentNode.ListChildNodes != null)
            {
                foreach (RealTimeTreeNodeViewModel node in CurrentNode.ListChildNodes)
                {
                    // if (node.IsSelected != CurrentNode.IsSelected)
                    //{
                    /*此处的是isSelected字段而不是IsSelected属性，防止父节点触发访问器而触发Setter
                     * 通过直接调用函数实现向下遍历
                     */
                    node.isSelected = CurrentNode.IsSelected;
                    node.RaisePropertyChanged("IsSelected");//通知页面刷新
                    //CurrentNode.RaisePropertyChanged("IsSelected");
                    //}
                    if (node.ListChildNodes != null)
                    {
                        node.ChangeChildNodes(node);//遍历方向：向下遍历
                    }
                }
            }
        }
        #endregion

        #region 改变父节点的选择状态
        /*改变父节点的选择状态*/
        public void ChangeParentNodes(RealTimeTreeNodeViewModel CurrentNode)
        {
            int selectedCount = 0;
            int noSelectedCount = 0;
            bool? parentNodeState = true;
            if (CurrentNode.parentNode != null)
            {
                foreach (RealTimeTreeNodeViewModel node in CurrentNode.parentNode.ListChildNodes)
                {
                    if (node.IsSelected == true)
                    {
                        selectedCount++;
                    }
                    if (node.IsSelected == false)
                    {
                        noSelectedCount++;
                    }
                }
                /*此处的是isSelected字段而不是IsSelected属性，防止父节点触发访问器而触发Setter
                 * 通过直接调用函数实现向上遍历
                 */
                if (selectedCount == CurrentNode.parentNode.ListChildNodes.Count)
                {//全选
                    parentNodeState = true;
                }
                else if (noSelectedCount == CurrentNode.parentNode.ListChildNodes.Count)
                {//全不选
                    parentNodeState = false;
                }
                else
                {//部分选
                    parentNodeState = null;
                }
                CurrentNode.parentNode.isSelected = parentNodeState;
                CurrentNode.parentNode.RaisePropertyChanged("IsSelected");//通知页面刷新

                if (CurrentNode.parentNode.parentNode != null)
                {
                    CurrentNode.parentNode.ChangeParentNodes(CurrentNode.parentNode);//遍历方向：向上遍历
                }
            }
        }
        #endregion

    }
}