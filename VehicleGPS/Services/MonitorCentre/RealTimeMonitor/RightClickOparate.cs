using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VehicleGPS.Models;
using System.Threading;
using VehicleGPS.ViewModels.MonitorCentre.RealTimeMonitor;
using System.Windows;
using VehicleGPS.Models.Login;
using System.Data;
using System.ComponentModel;
using Newtonsoft.Json;

namespace VehicleGPS.Services.MonitorCentre
{
    class RightClickOparate
    {

     

        /// <summary>
        /// 查询车辆信息
        /// </summary>
        public void getVehicleBaseInfo(string Sim)
        {
                VBaseInfo baseInfo = VBaseInfo.GetInstance();
                StaticLoginInfo loginInfo = StaticLoginInfo.GetInstance();
                string sql = "select v.VehicleId,v.FInnerId,v.VehicleType,v.SIM,v.GPSType,v.VehicleBrand,v.VehicleModel,v.VehicleColor,u.UNITNAME,u.ContactPeo,u.ContactTel,u.Address "
                 + "from InfoVehicle v,InfoUnit u "
                 + "where  v.ParentUnitId=u.UNITID and v.SIM='" + Sim + "'";
                string jsonStr = VehicleCommon.wcfDBHelper.BexcuteSql(loginInfo.UserName, sql);
                if (jsonStr.Equals("error"))
                    return ;
                DataTable retDt = JsonHelper.JsonToDataTable(jsonStr);
                if (retDt != null && retDt.Rows.Count == 1)
                {

                    baseInfo.VehicleId = retDt.Rows[0]["VehicleId"].ToString() == "null" ? "" : retDt.Rows[0]["VehicleId"].ToString();
                    baseInfo.FInnerId = retDt.Rows[0]["FInnerId"].ToString()== "null" ? "" : retDt.Rows[0]["FInnerId"].ToString() ;
                  //  baseInfo.Vehiclenum = retDt.Rows[0]["Vehiclenum"].ToString();
                    baseInfo.VehicleType= retDt.Rows[0]["VehicleType"].ToString()== "null" ? "" : retDt.Rows[0]["VehicleType"].ToString() ;
                    baseInfo.SIM = retDt.Rows[0]["SIM"].ToString()== "null" ? "" :retDt.Rows[0]["SIM"].ToString() ;
                    baseInfo.GPSType_id = retDt.Rows[0]["GPSType"].ToString()== "null" ? "" : retDt.Rows[0]["GPSType"].ToString();
                    baseInfo.VehicleBrand = retDt.Rows[0]["VehicleBrand"].ToString()== "null" ? "" :retDt.Rows[0]["VehicleBrand"].ToString() ;
                    baseInfo.VehicleModel = retDt.Rows[0]["VehicleModel"].ToString()== "null" ? "" : retDt.Rows[0]["VehicleModel"].ToString() ;
                    baseInfo.VehicleColor = retDt.Rows[0]["VehicleColor"].ToString()== "null" ? "" :retDt.Rows[0]["VehicleColor"].ToString() ;
                  //  baseInfo.GPSVersion_id = retDt.Rows[0]["GPSVersion"].ToString();
                    baseInfo.EUSERNAME = retDt.Rows[0]["UNITNAME"].ToString()== "null" ? "" :  retDt.Rows[0]["UNITNAME"].ToString();
                    baseInfo.ContactPeo = retDt.Rows[0]["ContactPeo"].ToString()== "null" ? "" : retDt.Rows[0]["ContactPeo"].ToString();
                    baseInfo.ContactTel = retDt.Rows[0]["ContactTel"].ToString() == "null" ? "" : retDt.Rows[0]["ContactTel"].ToString();
                    baseInfo.Address = retDt.Rows[0]["Address"].ToString() == "null" ? "" : retDt.Rows[0]["Address"].ToString();
                    baseInfo.init();
                }
        }
    }
}
