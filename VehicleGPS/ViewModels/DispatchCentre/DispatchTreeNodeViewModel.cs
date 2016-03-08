using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.Prism.ViewModel;
using VehicleGPS.Models;

namespace VehicleGPS.ViewModels.DispatchCentre
{
    class DispatchTreeNodeViewModel : NotificationObject
    {
        #region 节点信息
        /// <summary>
        /// 节点信息
        /// </summary>
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

        /// <summary>
        /// 用户判断该节点是否已经加载
        /// </summary>
        public bool isUsed;//用户判断该节点是否已经加载

        #region 是否展开
        /// <summary>
        /// 是否展开
        /// </summary>
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
        /// <summary>
        /// 是否获取可视焦点
        /// </summary>
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

        #region  节点选择状态
        /// <summary>
        /// 节点选择状态
        /// </summary>
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
        #endregion

        #region 节点图片Url
        /// <summary>
        /// 节点图片Url
        /// </summary>
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
        /// <summary>
        /// 子节点
        /// </summary>
        public List<DispatchTreeNodeViewModel> listChildNodes;
        public List<DispatchTreeNodeViewModel> ListChildNodes
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
        /// <summary>
        /// 父节点
        /// </summary>
        public DispatchTreeNodeViewModel parentNode;
        public DispatchTreeNodeViewModel ParentNode
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
    }
}
