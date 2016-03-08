using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.Prism.ViewModel;
using VehicleGPS.Models;
using System.Windows;

namespace VehicleGPS.ViewModels.MonitorCentre.TrackPlayBack
{
    /// <summary>
    /// 轨迹回放树的节点类
    /// </summary>
    class TrackPlayTreeNodeViewModel:NotificationObject
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

        /*是否显示选择RadioButton,用户不显示，车辆显示*/
        public Visibility selectedVisible;
        public Visibility SelectedVisible
        {
            get { return selectedVisible; }
            set
            {
                if (selectedVisible != value)
                {
                    selectedVisible = value;
                    this.RaisePropertyChanged("SelectedVisible");
                }
            }
        }

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
                    if (DataLoadingState())
                    {
                        isSelected = value;
                        this.RaisePropertyChanged("IsSelected");
                        //SelectedGPSInfo(this);
                    }
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
        public List<TrackPlayTreeNodeViewModel> listChildNodes;
        public List<TrackPlayTreeNodeViewModel> ListChildNodes
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
        public TrackPlayTreeNodeViewModel parentNode;
        public TrackPlayTreeNodeViewModel ParentNode
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
                //StaticTreeState.VehicleBasicInfo == LoadingState.LOADCOMPLETE &&
                StaticTreeState.VehicleAllBasicInfo == LoadingState.LOADCOMPLETE &&
                StaticTreeState.VehicleGPSInfo == LoadingState.LOADCOMPLETE)
            {
                return true;
            }
            MessageBox.Show("正在努力加载车辆详细数据，请稍后...", "数据加载", MessageBoxButton.OKCancel);
            return false;
        }
    }
}
