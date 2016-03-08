using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.Prism.ViewModel;
using VehicleGPS.Models;
using VehicleGPS.Models.Login;
using System.Data;
using VehicleGPS.Services;

namespace VehicleGPS.ViewModels.DispatchCentre.VehicleDispatch
{
    class TracePlayBackViewModel : NotificationObject
    {
        string TaskId = null;
        public TracePlayBackViewModel(string taskid)
        {
            this.TaskId = taskid;
            InitVehicleList();
        }
        private List<TracePlayBackOneVM> vehiclelist;

        public List<TracePlayBackOneVM> VehicleList
        {
            get { return vehiclelist; }
            set
            {
                vehiclelist = value;
                this.RaisePropertyChanged("VehicleList");
            }
        }
        void InitVehicleList() {
            string sql = @"select iv.VehicleId,iv.SIM,iv.Vehiclenum,ttld.driverName,ttld.insertTime,ttld.endUnloadTime,ttld.triptime,ttld.transPerCap  
                from InfoVehicle iv,TranTaskListDetail ttld 
                where iv.Vehiclenum=ttld.vehicleNum and ttld.carStatus in (4,5) and ttld.tranTaskListId='" + TaskId + "'";
            string jsonStr = VehicleCommon.wcfDBHelper.BexcuteSql(StaticLoginInfo.GetInstance().UserName, sql);
            if (jsonStr!="error")
            {
                DataTable dt = new DataTable();
                dt = JsonHelper.JsonToDataTable(jsonStr);
                if (dt!=null && dt.Rows.Count>0)
                {
                    this.VehicleList = new List<TracePlayBackOneVM>();
                    foreach (DataRow row in dt.Rows)
                    {
                        TracePlayBackOneVM tp = new TracePlayBackOneVM();
                        tp.AssignTime = row["insertTime"].ToString() == "null" ? "" : row["insertTime"].ToString();
                        tp.tripTime = row["triptime"].ToString() == "null" ? "0" : row["triptime"].ToString();
                        tp.transCount = row["transPerCap"].ToString() == "null" ? "0" : row["transPerCap"].ToString();
                        tp.driverName = row["driverName"].ToString() == "null" ? "" : row["driverName"].ToString();
                        tp.FinishTime = row["endUnloadTime"].ToString() == "null" ? "" : row["endUnloadTime"].ToString();
                        tp.vehicleID = row["VehicleId"].ToString() == "null" ? "" : row["VehicleId"].ToString();
                        tp.SIM = row["SIM"].ToString() == "null" ? "" : row["SIM"].ToString();
                        tp.vehiclenum = row["vehicleNum"].ToString() == "null" ? "" : row["vehicleNum"].ToString();
                        this.VehicleList.Add(tp);
                    }
                }
            }
        }
    }
}
