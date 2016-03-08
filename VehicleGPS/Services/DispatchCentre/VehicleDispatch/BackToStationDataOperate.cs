using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VehicleGPS.Models.DispatchCentre.VehicleDispatch;
using System.Data;
using VehicleGPS.Models;
using VehicleGPS.Models.Login;

namespace VehicleGPS.Services.DispatchCentre.VehicleDispatch
{
    class BackToStationDataOperate
    {
        private BasicDataServiceWCF WcfService { get; set; }
        public BackToStationDataOperate()
        {
            this.WcfService = new BasicDataServiceWCF();
        }
        public BackToStationInfo GetTransConDetailByVehicleNum(string vehicleNum)
        {
            string sql = "select td.id,td.tranTaskListId,td.driverId,td.driverName,td.transGoods,td.carStatus,td.unloadType,td.transPerCap,"
                + "tl.endRegName,tl.startRegName,tl.concretNum,"
                + "iv.VehicleType,iv.LoadAmount,iv.Vehiclenum,iv.VehicleId,iv.SIM"
                + " from TranTaskListDetail td,InfoVehicle iv,TranTaskList tl"
                + " where td.vehicleNum='"+vehicleNum+"' and td.tranTaskListId=tl.taskListId and iv.Vehiclenum=td.vehicleNum";
            DataTable dt = this.WcfService.ExecuteSql(sql);
            BackToStationInfo info = null;
            if (dt != null && dt.Rows.Count == 1)
            {
                info = new BackToStationInfo();
                info.fPlanId = dt.Rows[0]["tranTaskListId"].ToString() == "null" ? "" : dt.Rows[0]["tranTaskListId"].ToString();
                info.sim = dt.Rows[0]["SIM"].ToString() == "null" ? "" : dt.Rows[0]["SIM"].ToString();
                info.vehicleID = dt.Rows[0]["VehicleId"].ToString() == "null" ? "" : dt.Rows[0]["VehicleId"].ToString();
                string typeid = dt.Rows[0]["VehicleType"].ToString() == "null" ? "" : dt.Rows[0]["VehicleType"].ToString();
                info.startPoint = dt.Rows[0]["startRegName"].ToString() == "null" ? "" : dt.Rows[0]["startRegName"].ToString();
                info.endPoint = dt.Rows[0]["endRegName"].ToString() == "null" ? "" : dt.Rows[0]["endRegName"].ToString();
                info.transCapPer = dt.Rows[0]["transPerCap"].ToString() == "null" ? "" : dt.Rows[0]["transPerCap"].ToString();
                info.loadAmount = dt.Rows[0]["LoadAmount"].ToString() == "null" ? "" : dt.Rows[0]["LoadAmount"].ToString();
                string concretId = dt.Rows[0]["concretNum"].ToString() == "null" ? "" : dt.Rows[0]["concretNum"].ToString();
                info.concreteName = concretId;
                string carsStatus = dt.Rows[0]["carStatus"].ToString() == "null" ? "" : dt.Rows[0]["carStatus"].ToString();
                string offtype = dt.Rows[0]["unloadType"].ToString() == "null" ? "" : dt.Rows[0]["unloadType"].ToString();
                info.offTypeName = offtype;
                info.driverId = dt.Rows[0]["driverId"].ToString() == "null" ? "" : dt.Rows[0]["driverId"].ToString();
                info.driverName = dt.Rows[0]["driverName"].ToString() == "null" ? "" : dt.Rows[0]["driverName"].ToString();
                if (StaticTreeState.BasicTypeInfo == LoadingState.LOADCOMPLETE)
                {
                    foreach (BasicTypeInfo type in StaticBasicType.GetInstance().ListBasicTypeInfo)
                    {
                        if (type.TypeID == typeid)
                        {
                            info.vehicleTypeName = type.TypeName;
                        }
                        if (type.TypeID == concretId)
                        {
                            info.concreteName = type.TypeName;
                        }
                        if (type.TypeID == offtype)
                        {
                            info.offTypeName = type.TypeName;
                        }
                    }
                }
                else
                {
                    info.vehicleTypeName = typeid;
                    info.concreteName = concretId;
                    info.offTypeName = offtype;
                }
                info.carsStatus = VehicleCommon.GetCarsStatus(carsStatus);
            }
            return info;
        }
        /*确定*/
        public bool ConfirmBackToStation(string driverid, string fplanid, string vehicleid, bool isadd)
        {
            /*改变transcondetails表*/
            string sql = "";
            if (isadd)
            {
                sql = "update transcondetails set carsstatus=5 where zt=1 and fplanid='" + fplanid + "' and vehicleid='" + vehicleid + "'";
            }
            else
            {
                sql = "update transcondetails set transcapper=0,carsstatus=6 where zt=1 and fplanid='" + fplanid + "' and vehicleid='" + vehicleid + "'";
            }
            string jsonStr = VehicleCommon.wcfDBHelper.BexcuteSql(StaticLoginInfo.GetInstance().UserName, sql);
            if (jsonStr == "error")
            {
                return false;
            }

            /*改变infovehicle表*/
            sql = "update infovehicle set vehiclestate=0 where vehicleid='" + vehicleid + "'";
            jsonStr = VehicleCommon.wcfDBHelper.BexcuteSql(StaticLoginInfo.GetInstance().UserName, sql);
            if (jsonStr == "error")
            {
                return false;
            }
            /*如果有司机改变司机状态*/
            if (driverid != "")
            {
                sql = "update infoworkers set workstate=1 where zt=1 and workid='" + driverid + "'";
                jsonStr = VehicleCommon.wcfDBHelper.BexcuteSql(StaticLoginInfo.GetInstance().UserName, sql);
                if (jsonStr == "error")
                {
                    return false;
                }
            }
            return true;
        }
    }
}
