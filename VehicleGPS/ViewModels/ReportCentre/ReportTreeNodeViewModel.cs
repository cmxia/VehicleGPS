using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.Prism.ViewModel;
using VehicleGPS.Models;
using System.Windows;

namespace VehicleGPS.ViewModels.ReportCentre
{
    class ReportTreeNodeViewModel : NotificationObject
    {
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
        public bool isUsed;//用户判断该节点是否已经加载
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

        ///*是否显示选择RadioButton,用户不显示，车辆显示*/
        //public Visibility selectedVisible;
        //public Visibility SelectedVisible
        //{
        //    get { return selectedVisible; }
        //    set
        //    {
        //        if (selectedVisible != value)
        //        {
        //            selectedVisible = value;
        //            this.RaisePropertyChanged("SelectedVisible");
        //        }
        //    }
        //}

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

        /*节点选择状态*/
        public bool? isSelected;
        public bool? IsSelected
        {
            get { return isSelected; }
            set
            {
                if (isSelected != value)
                {
                    //if (DataLoadingState())
                    //{
                        isSelected = value;
                        ChangeChildNodes(this);
                        ChangeParentNodes(this);
                        this.RaisePropertyChanged("IsSelected");
                        //SelectedGPSInfo(this);
                    //}
                }
            }
        }

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

        /*子节点*/
        public List<ReportTreeNodeViewModel> listChildNodes;
        public List<ReportTreeNodeViewModel> ListChildNodes
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

        /*父节点*/
        public ReportTreeNodeViewModel parentNode;
        public ReportTreeNodeViewModel ParentNode
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
        /*判断数据加载情况*/
        public bool DataLoadingState()
        {
            if (StaticTreeState.ClientBasicInfo == LoadingState.LOADCOMPLETE &&
                StaticTreeState.VehicleBasicInfo == LoadingState.LOADCOMPLETE &&
                StaticTreeState.VehicleAllBasicInfo == LoadingState.LOADCOMPLETE &&
                StaticTreeState.VehicleGPSInfo == LoadingState.LOADCOMPLETE)
            {
                return true;
            }
            MessageBox.Show("正在努力加载车辆详细数据，请稍后...", "数据加载", MessageBoxButton.OKCancel);
            return false;
        }

        /*改变子节点的选择状态*/
        public void ChangeChildNodes(ReportTreeNodeViewModel CurrentNode)
        {
            if (CurrentNode.ListChildNodes != null)
            {
                foreach (ReportTreeNodeViewModel node in CurrentNode.ListChildNodes)
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

        /*改变父节点的选择状态*/
        public void ChangeParentNodes(ReportTreeNodeViewModel CurrentNode)
        {
            int selectedCount = 0;
            int noSelectedCount = 0;
            bool? parentNodeState = true;
            if (CurrentNode.parentNode != null)
            {
                foreach (ReportTreeNodeViewModel node in CurrentNode.parentNode.ListChildNodes)
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
    }
}