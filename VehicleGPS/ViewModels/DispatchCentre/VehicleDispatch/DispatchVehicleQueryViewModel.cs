using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.Prism.ViewModel;
using VehicleGPS.Models;
using VehicleGPS.Models.Login;
using System.Data;
using VehicleGPS.Services;
using VehicleGPS.Models.DispatchCentre.VehicleDispatch;

namespace VehicleGPS.ViewModels.DispatchCentre.VehicleDispatch
{
    class DispatchVehicleQueryViewModel : NotificationObject
    {
        string Taskid = null;
        public DispatchVehicleQueryViewModel(string taskid)
        {
            this.Taskid = taskid;
            InitVehicleInfoList();
        }
        public DispatchVehicleQueryViewModel(List<DispatchVehicleInfo> dispatchList)
        {
            this.VehicleList = new List<DispatchVehicleQueryOneVM>();
            foreach (DispatchVehicleInfo item in dispatchList)
            {
                DispatchVehicleQueryOneVM one = new DispatchVehicleQueryOneVM();
                one.TaskInfo = item;
                VehicleList.Add(one);
            }
        }
        void InitVehicleInfoList()
        {
            string sql = @"select ttl.unitName,ttl.taskListId,ttl.startRegName,ttl.endRegName,ttl.concretNum,ttl.transDistance,ttl.site,
                ttld.driverName,ttld.insertTime,ttld.transPerCap,ttld.triptime,ttld.outStartRegTime,ttld.inEndRegTime,ttld.startUnloadTime,ttld.endUnloadTime,ttld.towerName,
                iv.VehicleId 
                from TranTaskList ttl, TranTaskListDetail ttld,InfoVehicle iv 
                where ttl.taskListId=ttld.tranTaskListId and (ttld.carStatus in (1,2,3,4)) and iv.Vehiclenum=ttld.vehicleNum and ttl.taskListId='" + Taskid + "'";
            string jsonStr = VehicleCommon.wcfDBHelper.BexcuteSql(StaticLoginInfo.GetInstance().UserName, sql);
            if (jsonStr != "error")
            {
                DataTable dt = new DataTable();
                dt = JsonHelper.JsonToDataTable(jsonStr);
                if (dt != null && dt.Rows.Count > 0)
                {
                    this.VehicleList = new List<DispatchVehicleQueryOneVM>();
                    foreach (DataRow row in dt.Rows)
                    {
                        DispatchVehicleQueryOneVM one = new DispatchVehicleQueryOneVM();
                        one.TaskInfo = new Models.DispatchCentre.VehicleDispatch.DispatchVehicleInfo();
                        one.TaskInfo.ConcreteName = row["concretNum"].ToString() == "null" ? "" : row["concretNum"].ToString();
                        one.TaskInfo.DriverName = row["driverName"].ToString() == "null" ? "" : row["driverName"].ToString();
                        one.TaskInfo.EndRegion = row["endRegName"].ToString() == "null" ? "" : row["endRegName"].ToString();
                        one.TaskInfo.FPlanId = row["taskListId"].ToString() == "null" ? "" : row["taskListId"].ToString();
                        one.TaskInfo.InsertTime = row["insertTime"].ToString() == "null" ? "" : row["insertTime"].ToString();
                        one.TaskInfo.StartRegion = row["startRegName"].ToString() == "null" ? "" : row["startRegName"].ToString();
                        one.TaskInfo.TransCapPer = row["transPerCap"].ToString() == "null" ? "" : row["transPerCap"].ToString();
                        one.TaskInfo.UnitName = row["unitName"].ToString() == "null" ? "" : row["unitName"].ToString();
                        one.TaskInfo.VehicleID = row["VehicleId"].ToString() == "null" ? "" : row["VehicleId"].ToString();
                        one.TaskInfo.TripTime = row["triptime"].ToString() == "null" ? "" : row["triptime"].ToString();
                        one.TaskInfo.OutStartRegionTime = row["outStartRegTime"].ToString() == "null" ? "" : row["outStartRegTime"].ToString();
                        one.TaskInfo.InEndRegionTime = row["inEndRegTime"].ToString() == "null" ? "" : row["inEndRegTime"].ToString();
                        one.TaskInfo.transDistance = row["transDistance"].ToString() == "null" ? "" : row["transDistance"].ToString();
                        one.TaskInfo.Position = row["site"].ToString() == "null" ? "" : row["site"].ToString();
                        one.TaskInfo.StartUnloadTime = row["startUnloadTime"].ToString() == "null" ? "" : row["startUnloadTime"].ToString();
                        one.TaskInfo.EndUnloadTime = row["endUnloadTime"].ToString() == "null" ? "" : row["endUnloadTime"].ToString();
                        one.TaskInfo.TowerName = row["towerName"].ToString() == "null" ? "" : row["towerName"].ToString();
                        this.VehicleList.Add(one);
                    }
                }
            }
        }
        private List<DispatchVehicleQueryOneVM> vehiclelist;

        public List<DispatchVehicleQueryOneVM> VehicleList
        {
            get { return vehiclelist; }
            set
            {
                vehiclelist = value;
                this.RaisePropertyChanged("VehicleList");
            }
        }

    }
}
