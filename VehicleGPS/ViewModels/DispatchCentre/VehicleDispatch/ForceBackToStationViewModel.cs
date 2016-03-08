using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.Prism.ViewModel;
using VehicleGPS.Models.DispatchCentre.VehicleDispatch;
using Microsoft.Practices.Prism.Commands;
using System.Windows;
using VehicleGPS.Models;
using VehicleGPS.Models.Login;
using Newtonsoft.Json;
using VehicleGPS.zmq;
using System.ComponentModel;
using System.Threading;

namespace VehicleGPS.ViewModels.DispatchCentre.VehicleDispatch
{
    class ForceBackToStationViewModel : NotificationObject
    {
        TaskInfo taskinfo = new TaskInfo();
        public ForceBackToStationViewModel(TaskInfo TaskNum)
        {
            taskinfo = TaskNum;
            this.ConfirmCommand = new DelegateCommand<Window>(this.ConfirmCommandExecute);
            InitVehicleList();
        }

        #region 绑定数据
        private bool isbusy;

        public bool IsBusy
        {
            get { return isbusy; }
            set
            {
                isbusy = value;
                this.RaisePropertyChanged("IsBusy");
            }
        }


        //车辆列表
        private List<BackToStationInfo> listbackvehicle;

        public List<BackToStationInfo> VehicleList
        {
            get { return listbackvehicle; }
            set
            {
                listbackvehicle = value;
                this.RaisePropertyChanged("VehicleList");
            }
        }
        //是否增加方量
        private bool isaddcount;

        public bool IsAddCount
        {
            get { return isaddcount; }
            set
            {
                isaddcount = value;
                this.RaisePropertyChanged("IsAddCount");
            }
        }

        #endregion

        public DelegateCommand<Window> ConfirmCommand { get; set; }
        List<string> failure = null;
        private void ExecuteConfirm()
        {
            this.failure = new List<string>();
            foreach (BackToStationInfo info in this.VehicleList)
            {
                if (info.IsChecked)
                {//选中
                    string forceBackIns = "{\"cmd\":\"FORCECARBACK_TYPE\",\"simId\":\"" + info.sim + "\",\"cmdId\":\"" + info.sim + "_FORCECARBACK_TYPE" + "\",\"taskId\":\"" + this.taskinfo.FPlanId + "\",\"taskListId\":\"" + info.PlanDetailId + "\",\"isCircle\":\"" + info.isassigncircle + "\",\"isAddCount\":\"";
                    string update = "update TranTaskListDetail set carStatus='";
                    if (info.IsAddCount)
                    {//增加方量
                        update += "5";
                        forceBackIns += "1";
                    }
                    else
                    {
                        update += "6";
                        forceBackIns += "0";
                    }
                    update += "' where id='" + info.PlanDetailId + "'";
                    forceBackIns += "\"}";
                    string jsonStr = VehicleCommon.wcfDBHelper.BexcuteSql(StaticLoginInfo.GetInstance().UserName, update);
                    if (!zmq.zmqPackHelper.zmqInstructionsPack(info.sim, forceBackIns))
                    {
                        failure.Add(info.vehicleID);
                    }
                    else
                    {
                        //更新内存数据
                        foreach (var item in StaticDetailInfo.GetInstance().ListVehicleDetailInfo)
                        {
                            if (item.VehicleId.Equals(info.vehicleID))
                            {
                                item.VehicleState = "3";
                                break;
                            }
                        }
                        foreach (CVBasicInfo basic in StaticBasicInfo.GetInstance().ListVehicleBasicInfo)
                        {
                            if (basic.SIM.Equals(info.sim))
                            {
                                basic.TaskState = "进行任务中";
                                break;
                            }
                        }
                    }
                }
            }
            StaticTreeState.DispatchTreeRefresh = LoadingState.LOADING;
            StaticTreeState.VehicleBasicInfo = LoadingState.LOADING;
            DispatchTreeViewModel.GetInstance().TreeOperate.RefreshTree();//更新树形
            VehicleDispatchViewModel.GetInstance().InitDispatchInfo();
            while (true)
            {
                if (StaticTreeState.DispatchTreeRefresh == LoadingState.LOADCOMPLETE)
                {
                    Thread.Sleep(1000);
                    break;
                }
            }
        }
        void ConfirmCommandExecute(Window win)
        {
            bool CanSubmit = false;
            foreach (BackToStationInfo item in this.VehicleList)
            {
                if (item.IsChecked)
                {
                    CanSubmit = true;
                }
            }
            if (!CanSubmit)
            {
                MessageBox.Show("请先选择需要回站的车辆");
                return;
            }

            if (!(MessageBox.Show("是否强制回站？", "强制回站确认", MessageBoxButton.OKCancel, MessageBoxImage.Question) == MessageBoxResult.OK))
            {
                return;
            }
            BackgroundWorker worker = new BackgroundWorker();
            worker.DoWork += (o, ea) =>
            {
                this.ExecuteConfirm();
            };
            worker.RunWorkerCompleted += (o, ea) =>
            {
                this.IsBusy = false;
                string msg = "";
                if (this.failure.Count > 0)
                {
                    msg += "以下车辆强制回站失败：";
                    foreach (string item in this.failure)
                    {
                        msg += "\n" + item;
                    }
                }
                else
                {
                    msg = "强制回站成功！";
                }
                MessageBox.Show(msg);
                win.Close();
            };
            this.IsBusy = true;
            worker.RunWorkerAsync();
        }

        void InitVehicleList()
        {
            string sql = "select iv.VehicleId vehicleID,iv.SIM sim, tt.id PlanDetailId,tt.tranTaskListId fPlanId,tt.vehicleNum vehicleNum,tt.transPerCap transCapPer,tt.driverId driverId,tt.driverName driverName,tt.assigncircle isassigncircle"
                + " from TranTaskListDetail tt,InfoVehicle iv where tt.vehicleNum=iv.Vehiclenum and tt.tranTaskListId='" + this.taskinfo.FPlanId + "' and tt.carStatus not in (5,6)";
            string jsonStr = VehicleCommon.wcfDBHelper.BexcuteSql(StaticLoginInfo.GetInstance().UserName, sql);
            if (string.Compare(jsonStr, "error") != 0)
            {
                string jsonTmp = jsonStr.Substring(1, jsonStr.Length - 2);
                this.VehicleList = (List<BackToStationInfo>)JsonConvert.DeserializeObject(jsonTmp, typeof(List<BackToStationInfo>));
            }
        }
    }
}
