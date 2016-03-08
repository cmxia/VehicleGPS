using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.Prism.ViewModel;
using VehicleGPS.Models;
using System.Windows;

namespace VehicleGPS.ViewModels.MonitorCentre.ImageCheck
{
    class ImageCheckTreeNodeViewModel : NotificationObject
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
        private List<ImageCheckTreeNodeViewModel> listchildnodes;

        public List<ImageCheckTreeNodeViewModel> ListChildNodes
        {
            get { return listchildnodes; }
            set
            {
                listchildnodes = value;
                this.RaisePropertyChanged("ListChildNodes");
            }
        }
        private ImageCheckTreeNodeViewModel parentnode;

        public ImageCheckTreeNodeViewModel ParentNode
        {
            get { return parentnode; }
            set
            {
                parentnode = value;
                this.RaisePropertyChanged("ParentNode");
            }
        }
        #region 节点选择状态
        /*节点选择状态*/
        public bool isSelected;
        public bool IsSelected
        {
            get { return isSelected; }
            set
            {
                isSelected = value;
                if (isSelected)
                {
                    ImageCheckTreeViewModel.GetInstance().SelectedNode = this;
                }
                this.RaisePropertyChanged("IsSelected");
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
    }
}
