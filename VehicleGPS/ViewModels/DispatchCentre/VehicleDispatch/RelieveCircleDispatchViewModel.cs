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

namespace VehicleGPS.ViewModels.DispatchCentre.VehicleDispatch
{
    class RelieveCircleDispatchViewModel : NotificationObject
    {
        private string TaskId = null;//任务单ID
        public RelieveCircleDispatchViewModel(string taskId)
        {
            TaskId = taskId;
            InitVehicleList();
            this.ConfirmCommand = new DelegateCommand<Window>(this.ConfirmCommandExecute);
        }
        void InitVehicleList()
        {
            string sql = "select iv.VehicleId vehicleID,tt.id PlanDetailId,tt.tranTaskListId fPlanId,tt.vehicleNum vehicleNum,tt.transPerCap transCapPer,tt.driverId driverId,tt.driverName driverName,tt.assigncircle isassigncircle"
                + " from TranTaskListDetail tt,InfoVehicle iv where tt.vehicleNum=iv.Vehiclenum and tt.tranTaskListId='" + this.TaskId + "' and (tt.carStatus='4' or tt.carStatus='2' or tt.carStatus='3' or tt.carStatus='1') and tt.assigncircle='1'";
            string jsonStr = VehicleCommon.wcfDBHelper.BexcuteSql(StaticLoginInfo.GetInstance().UserName, sql);
            if (string.Compare(jsonStr, "error") != 0)
            {
                string jsonTmp = jsonStr.Substring(1, jsonStr.Length - 2);
                this.VehicleList = (List<BackToStationInfo>)JsonConvert.DeserializeObject(jsonTmp, typeof(List<BackToStationInfo>));
            }
        }
        private List<BackToStationInfo> vehiclelist;

        public List<BackToStationInfo> VehicleList
        {
            get { return vehiclelist; }
            set
            {
                vehiclelist = value;
                this.RaisePropertyChanged("VehicleList");
            }
        }
        public DelegateCommand<Window> ConfirmCommand { get; set; }
        void ConfirmCommandExecute(Window win)
        {
            List<string> failure = new List<string>();
            foreach (BackToStationInfo info in this.VehicleList)
            {
                if (info.IsChecked)
                {
                    string sql = "update TranTaskListDetail set assigncircle='0' where id='" + info.PlanDetailId + "'";
                    string jsonStr = VehicleCommon.wcfDBHelper.BexcuteSql(StaticLoginInfo.GetInstance().UserName, sql);
                    if (string.Compare(jsonStr, "error") == 0)
                    {
                        failure.Add(info.vehicleID);
                    }
                }
            }
            string msg = "";
            if (failure.Count>0)
            {
                msg += "以下车辆解除循环派车未成功：";
                foreach (string item in failure)
                {
                    msg += "\n" + item;
                }
            }
            else
            {
                msg = "解除循环派车成功！";
            }
            win.Close();
        }
    }
}
