using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.Prism.ViewModel;
using VehicleGPS.Models.DispatchCentre.VehicleDispatch;
using Microsoft.Practices.Prism.Commands;
using VehicleGPS.Services.DispatchCentre.VehicleDispatch;
using System.Windows.Controls;
using VehicleGPS.Models;
using System.Windows;
using VehicleGPS.Views.Control.DispatchCentre.VehicleDispatch;
using VehicleGPS.Models.Login;
using VehicleGPS.Services;
using System.Data;
using Newtonsoft.Json;

namespace VehicleGPS.ViewModels.DispatchCentre.VehicleDispatch
{
    public class VehicleDispatchItemViewModel : NotificationObject
    {
        //private VehicleDispatchDataOperate DataOperate { get; set; }
        public VehicleDispatchItemViewModel()
        {
            //初始化后
            this.DispatchGridLoadedCommand = new DelegateCommand<object>(new Action<object>(this.DispatchGridLoadedCommandExecute));
            /*宽度大小改变*/
            this.DispatchGridSizeChangedCommand = new DelegateCommand<object>(new Action<object>(this.DispatchGridSizeChangedCommandExecute));
            //this.DataOperate = new VehicleDispatchDataOperate();
            /*进度和交付方量*/
            this.TransedCap = 0;
            this.TransedProgress = "0";

            /*右键*/
            this.DispatchInfoQueryCommand = new DelegateCommand(new Action(this.DispatchInfoQueryExecute));
            this.TimeRemindSetCommand = new DelegateCommand(new Action(this.TimeRemindSetExecute));
            this.BackToStationCommand = new DelegateCommand(new Action(this.BackToStationExecute));
            this.ReliveCircleDispatchCommand = new DelegateCommand(new Action(this.ReliveCircleDispatchExecute));
            this.TracePlayBackCommand = new DelegateCommand(new Action(this.TracePlayBackExecute));
            this.TaskFinishCommand = new DelegateCommand(new Action(this.TaskFinishExecute));
        }

        #region 绑定字段
        //下拉列表的可见性
        private Visibility comboBoxVisibility = Visibility.Collapsed;

        public Visibility ComboBoxVisibility
        {
            get { return comboBoxVisibility; }
            set
            {
                comboBoxVisibility = value;
                this.RaisePropertyChanged("ComboBoxVisibility");
            }
        }
        //区域内的车辆的listview的可见性
        private Visibility listViewVisibility = Visibility.Collapsed;

        public Visibility ListViewVisibility
        {
            get { return listViewVisibility; }
            set
            {
                listViewVisibility = value;
                this.RaisePropertyChanged("ListViewVisibility");
            }
        }

        public TaskInfo TaskNumberInfo { get; set; }
        /*已经交付的方量*/
        private double transedCap;
        public double TransedCap
        {
            get { return transedCap; }
            set
            {
                if (transedCap != value)
                {
                    transedCap = value;
                    this.RaisePropertyChanged("TransedCap");
                }
            }
        }
        /*完成进度*/
        private string transedProgress;
        public string TransedProgress
        {
            get { return transedProgress; }
            set
            {
                if (transedProgress != value)
                {
                    transedProgress = value;
                    this.RaisePropertyChanged("TransedProgress");
                }
            }
        }

        /*已发车次*/
        private int hasDispatch;
        public int HasDispatch
        {
            get { return hasDispatch; }
            set
            {
                if (hasDispatch != value)
                {
                    hasDispatch = value;
                    this.RaisePropertyChanged("HasDispatch");
                }
            }
        }

        /*途中标注点*/
        private List<DispatchPointViewModel> listDispatchPoint;
        public List<DispatchPointViewModel> ListDispatchPoint
        {
            get { return listDispatchPoint; }
            set
            {
                if (listDispatchPoint != value)
                {
                    listDispatchPoint = value;
                    this.RaisePropertyChanged("ListDispatchPoint");
                }
            }
        }

        /*区域内标注点*/
        private List<DispatchPointViewModel> listInRegionPoint;
        public List<DispatchPointViewModel> ListInRegionPoint
        {
            get { return listInRegionPoint; }
            set
            {
                if (listInRegionPoint != value)
                {
                    listInRegionPoint = value;
                    this.RaisePropertyChanged("ListInRegionPoint");
                }
            }
        }

        /*工地内标注点*/
        private List<DispatchPointViewModel> listInSitePoint;
        public List<DispatchPointViewModel> ListInSitePoint
        {
            get { return listInSitePoint; }
            set
            {
                if (listInSitePoint != value)
                {
                    listInSitePoint = value;
                    this.RaisePropertyChanged("ListInSitePoint");
                }
            }
        }

        /*所有途中的车辆信息*/
        private List<DispatchVehicleViewModel> listDispatchVehicle;
        public List<DispatchVehicleViewModel> ListDispatchVehicle
        {
            get { return listDispatchVehicle; }
            set
            {
                if (listDispatchVehicle != value)
                {
                    listDispatchVehicle = value;
                    this.RaisePropertyChanged("ListDispatchVehicle");
                }
            }
        }
        /*所有到达的车辆*/
        private List<DispatchVehicleViewModel> listInSiteVehicle;
        public List<DispatchVehicleViewModel> ListInSiteVehicle
        {
            get { return listInSiteVehicle; }
            set
            {
                if (listInSiteVehicle != value)
                {
                    listInSiteVehicle = value;
                    this.RaisePropertyChanged("ListInSiteVehicle");
                }
            }
        }
        /*所有未出发的车辆*/
        private List<DispatchVehicleViewModel> listInRegionVehicle;
        public List<DispatchVehicleViewModel> ListInRegionVehicle
        {
            get { return listInRegionVehicle; }
            set
            {
                if (listInRegionVehicle != value)
                {
                    listInRegionVehicle = value;
                    this.RaisePropertyChanged("ListInRegionVehicle");
                }
            }
        }

        /*所有返回的车辆*/
        private List<DispatchVehicleViewModel> listRetRegionVehicle;
        public List<DispatchVehicleViewModel> ListRetRegionVehicle
        {
            get { return listRetRegionVehicle; }
            set
            {
                if (listRetRegionVehicle != value)
                {
                    listRetRegionVehicle = value;
                    this.RaisePropertyChanged("ListRetRegionVehicle");
                }
            }
        }
        //调度宽度
        private double dispatchWidth;
        public double DispatchWidth
        {
            get
            {
                return dispatchWidth;
            }
            set
            {
                dispatchWidth = value;
            }
        }

        #endregion

        #region 绑定操作

        //回单
        public DelegateCommand TaskFinishCommand { get; set; }
        private void TaskFinishExecute()
        {
            StaticTreeState.DispatchCenter = LoadingState.LOADING;
            string car_sql = "select id,vehicleNum from TranTaskListDetail where tranTaskListId='" + TaskNumberInfo.FPlanId + "'and(carStatus='1' or carStatus='2' or carStatus='3')";
            string car_jsonStr = VehicleCommon.wcfDBHelper.BexcuteSql(StaticLoginInfo.GetInstance().UserName, car_sql);
            if (string.Compare(car_jsonStr, "error") == 0)
            {
                return;
            }
            DataTable dt = JsonHelper.JsonToDataTable(car_jsonStr);
            if (dt != null)
            {
                MessageBox.Show("该任务单仍有派车信息，无法回单！！", "提示");
                return;
            }
            else
            {
                if (MessageBox.Show("您确定要将此任务单更改为完成状态？", "回单确认", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                {
                    if (MessageBox.Show("该任务单未完成计划方量，是否确认将其更改为完成状态？", "回单确认", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                    {
                        string sql = "update TranTaskList set taskStatus='2' where taskListId='" + TaskNumberInfo.FPlanId + "'";
                        string jsonStr = VehicleCommon.wcfDBHelper.BexcuteSql(StaticLoginInfo.GetInstance().UserName, sql);
                        if (jsonStr == "error")
                        {
                            MessageBox.Show("回单失败，请重试！");
                        }
                        else
                        {
                            MessageBox.Show("回单成功！");
                            Dictionary<string, string> instruction = new Dictionary<string, string>();
                            instruction.Add("cmd", "DISPATCH_TYPE");
                            instruction.Add("cmdid", "123_DISPATCH_TYPE");
                            instruction.Add("type", "TASK");
                            instruction.Add("ID", TaskNumberInfo.FPlanId);
                            instruction.Add("OPERATETYPE ", "3");
                            string insstring = JsonConvert.SerializeObject(instruction);
                            zmq.zmqPackHelper.zmqInstructionsPack("", insstring);
                            VehicleDispatchViewModel.GetInstance().QueryCommandExecute();
                        }
                    }
                }
            }
            StaticTreeState.DispatchCenter = LoadingState.LOADCOMPLETE;
            DispatchTreeViewModel.GetInstance().TreeOperate.RefreshTree();//更新树形
        }
        //派车单查询
        public DelegateCommand DispatchInfoQueryCommand { get; set; }
        private void DispatchInfoQueryExecute()
        {
            StaticTreeState.DispatchCenter = LoadingState.LOADING;
            DispatchVehicleQuery win = new DispatchVehicleQuery(TaskNumberInfo.FPlanId);
            win.ShowDialog();
            StaticTreeState.DispatchCenter = LoadingState.LOADCOMPLETE;
        }
        //时间提醒设置
        public DelegateCommand TimeRemindSetCommand { get; set; }
        private void TimeRemindSetExecute()
        {
            StaticTreeState.DispatchCenter = LoadingState.LOADING;
            TimeRemindSetting win = new TimeRemindSetting(TaskNumberInfo.FPlanId);
            win.ShowDialog();
            StaticTreeState.DispatchCenter = LoadingState.LOADCOMPLETE;
        }
        //强制回站
        public DelegateCommand BackToStationCommand { get; set; }
        private void BackToStationExecute()
        {
            StaticTreeState.DispatchCenter = LoadingState.LOADING;
            string sql = "select  COUNT (*) Num"
               + " from TranTaskListDetail where tranTaskListId='" + TaskNumberInfo.FPlanId + "' and carStatus not in (5,6)";
            string jsonStr = VehicleCommon.wcfDBHelper.BexcuteSql(StaticLoginInfo.GetInstance().UserName, sql);
            if (string.Compare(jsonStr, "error") != 0)
            {
                DataTable dt = new DataTable();
                dt = JsonHelper.JsonToDataTable(jsonStr);
                if (dt == null || dt.Rows.Count >= 0)
                {
                    if (!(Convert.ToInt32(dt.Rows[0]["Num"].ToString()) > 0))
                    {
                        MessageBox.Show("该任务单没有车辆需要强制回站！");
                        return;
                    }
                }
            }
            else
            {
                MessageBox.Show("数据库查询错误！");
                return;
            }
            ForceBackToStation win = new ForceBackToStation(TaskNumberInfo);
            win.ShowDialog();
            StaticTreeState.DispatchCenter = LoadingState.LOADCOMPLETE;
        }
        //解除循环派车
        public DelegateCommand ReliveCircleDispatchCommand { get; set; }
        private void ReliveCircleDispatchExecute()
        {
            StaticTreeState.DispatchCenter = LoadingState.LOADING;
            RelieveCircleDispatch win = new RelieveCircleDispatch(TaskNumberInfo.FPlanId);
            win.ShowDialog();
            StaticTreeState.DispatchCenter = LoadingState.LOADCOMPLETE;
        }
        //趟次轨迹回放
        public DelegateCommand TracePlayBackCommand { get; set; }
        private void TracePlayBackExecute()
        {
            StaticTreeState.DispatchCenter = LoadingState.LOADING;
            TracePlayBack win = new TracePlayBack(TaskNumberInfo.FPlanId);
            win.ShowDialog();
            StaticTreeState.DispatchCenter = LoadingState.LOADCOMPLETE;
        }
        //调度界面加载完成
        public DelegateCommand<object> DispatchGridLoadedCommand { get; set; }
        private void DispatchGridLoadedCommandExecute(object param)
        {
            this.DispatchWidth = ((Grid)param).ActualWidth;
        }

        #endregion

        #region 调度中心屏幕宽度改变
        public DelegateCommand<object> DispatchGridSizeChangedCommand { get; set; }
        private void DispatchGridSizeChangedCommandExecute(object param)
        {
            double oldWidth = this.DispatchWidth;
            this.DispatchWidth = ((Grid)param).ActualWidth;
            if (this.listDispatchPoint != null)
            {
                foreach (DispatchPointViewModel vm in this.listDispatchPoint)
                {
                    vm.MarginDistance = vm.MarginDistance * this.DispatchWidth / oldWidth;
                }
            }
        }
        public double GetMarginDistance(double rate)
        {
            return this.dispatchWidth * rate;
        }
        #endregion
    }
}
