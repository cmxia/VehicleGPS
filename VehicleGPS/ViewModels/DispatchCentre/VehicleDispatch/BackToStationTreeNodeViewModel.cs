using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.Prism.ViewModel;
using VehicleGPS.Models;

namespace VehicleGPS.ViewModels.DispatchCentre.VehicleDispatch
{
    class BackToStationTreeNodeViewModel : NotificationObject
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
        /*节点选择状态*/
        public bool? isSelected;
        public bool? IsSelected
        {
            get { return isSelected; }
            set
            {
                if (isSelected != value)
                {

                    isSelected = value;
                    this.RaisePropertyChanged("IsSelected");
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
        public List<BackToStationTreeNodeViewModel> listChildNodes;
        public List<BackToStationTreeNodeViewModel> ListChildNodes
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
        public BackToStationTreeNodeViewModel parentNode;
        public BackToStationTreeNodeViewModel ParentNode
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
    }
}
