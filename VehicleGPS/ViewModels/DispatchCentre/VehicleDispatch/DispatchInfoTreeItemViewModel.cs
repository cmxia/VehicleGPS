using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.Prism.ViewModel;
using VehicleGPS.Models;
using System.Windows;
using VehicleGPS.Views.Control.DispatchCentre.VehicleDispatch;
using Microsoft.Practices.Prism.Commands;

namespace VehicleGPS.ViewModels.DispatchCentre.VehicleDispatch
{
    class DispatchInfoTreeItemViewModel : NotificationObject
    {
        public DispatchInfoTreeItemViewModel()
        {
            this.SetParameterCommand = new DelegateCommand<Window>(new Action<Window>(this.SetParameterCommandExecute));
            
        }
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
        public string ImageUrl { get; set; }

        private List<DispatchInfoTreeItemViewModel> listTreeItem = new List<DispatchInfoTreeItemViewModel>();
        public List<DispatchInfoTreeItemViewModel> ListTreeItem
        {
            get { return listTreeItem; }
            set
            {
                if (listTreeItem != value)
                {
                    listTreeItem = value;
                    this.RaisePropertyChanged("ListTreeItem");
                }
            }
        }

        private Visibility checkVisible;//是否显示CheckBox
        public Visibility CheckVisible
        {
            get { return checkVisible; }
            set
            {
                if (checkVisible != value)
                {
                    checkVisible = value;
                    this.RaisePropertyChanged("CheckVisible");
                }
            }
        }

        private bool isSelected;
        public bool IsSelected
        {
            get { return isSelected; }
            set
            {
                if (isSelected != value)
                {
                    isSelected = value;
                    this.RaisePropertyChanged("IsSelected");
                    this.Parameter = "您还未设置运输参数";
                    if (isSelected == true)
                    {
                        this.SetVisible = Visibility.Visible;
                        this.ShowVisible = Visibility.Visible;
                    }
                    else
                    {
                        this.SetVisible = Visibility.Collapsed;
                        this.ShowVisible = Visibility.Collapsed;
                    }
                }
            }
        }
        private Visibility setVisible;//显示设置按钮
        public Visibility SetVisible
        {
            get { return setVisible; }
            set
            {
                if (setVisible != value)
                {
                    setVisible = value;
                    this.RaisePropertyChanged("SetVisible");
                }
            }
        }
        
        private Visibility showVisible;//展示设置参数
        public Visibility ShowVisible
        {
            get { return showVisible; }
            set
            {
                if (showVisible != value)
                {
                    showVisible = value;
                    this.RaisePropertyChanged("ShowVisible");
                }
            }
        }
        private string parameter;//参数内容
        public string Parameter
        {
            get { return parameter; }
            set
            {
                if (parameter != value)
                {
                    parameter = value;
                    this.RaisePropertyChanged("Parameter");
                }
            }
        }
        /*点击设置运输参数按钮*/
        public DelegateCommand<Window> SetParameterCommand { get; set; }
        private void SetParameterCommandExecute(Window parentWin)
        {
            if (StaticTreeState.VehicleAllBasicInfo != LoadingState.LOADCOMPLETE)
            {//设置参数中需要车辆详细数据中的核载方量数据
                MessageBox.Show("车辆详细基础数据未加载完毕，请稍后", "数据加载", MessageBoxButton.OKCancel);
                return;
            }
            DispatchInfoViewModel parentVM = (DispatchInfoViewModel)parentWin.DataContext;
            parentVM.SelectedTreeItem = this;
            SetParameter setParameterWin = new SetParameter(parentWin.DataContext);
            setParameterWin.Owner = parentWin;
            setParameterWin.ShowDialog();
        }

        #region 参数设置的模型
        public SetParameterViewModel parameterVM;
        public SetParameterViewModel ParameterVM
        {
            get { return parameterVM; }
            set
            {
                if (parameterVM != value)
                {
                    parameterVM = value;
                    if (parameterVM != null)
                    {
                        //string parameterStr = "<table><tr><td>你的</td><td>我的</td></tr><tr><td>你的</td><td>我的</td></tr></table>";

                        string parameterStr = " 驾驶员编号：" + parameterVM.ListDriver[parameterVM.SelectedDriverIndex].workID
                                            + "\n 驾驶员姓名：" + parameterVM.ListDriver[parameterVM.SelectedDriverIndex].DriverName
                                            + "\n 运输物品：" + parameterVM.TransObject[parameterVM.SelectedTransObjectIndex].TypeName
                                            + "\n 卸料方式：" + parameterVM.ListUnloadWay[parameterVM.SelectedUnloadIndex].TypeName
                                            + "\n 装载塔楼：" + parameterVM.ListTower[parameterVM.SelectedTowerIndex].Name
                                            + "\n 实载方量：" + parameterVM.RealLoad + "方";
                        this.Parameter = parameterStr;
                    }
                }
            }
        }
        #endregion
    }
}
