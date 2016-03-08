using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VehicleGPS.Models;

namespace VehicleGPS.Services
{
    class VehicleStateUpdateServices
    {
        /// <summary>
        /// 更新指定单位下的所有车辆的任务状态
        /// </summary>
        /// <param name="unitId">单位的id</param>
        /// <param name="regionradius">区域的半径</param>
        /// <param name="reglng">区域中心经度</param>
        /// <param name="reglat">区域中心纬度</param>
        /// <param name="operateType">操作类型:1:删除；2：修改/增加</param>
        public static void UpdateVehicleState(string unitId, double regionradius, double reglng, double reglat, int operateType)
        {
            if (StaticTreeState.VehicleBasicInfo == LoadingState.LOADCOMPLETE)
            {
                StaticTreeState.VehicleBasicInfo = LoadingState.LOADING;
                #region update
                List<CVBasicInfo> vehicleInfoList = StaticBasicInfo.GetInstance().ListVehicleBasicInfo;
                foreach (CVBasicInfo vehicle in vehicleInfoList)
                {
                    //寻找该单位下的车辆
                    if (vehicle.ParentID == null || vehicle.TaskState == null)
                    {
                        continue;
                    }
                    if (!vehicle.ParentID.Equals(unitId))
                    {
                        continue;
                    }
                    if (operateType == 1 && vehicle.TaskState.Equals("3"))
                    {//删除了区域
                        vehicle.TaskState = "0";
                    }
                    else if (operateType == 2)
                    {//新建 或修改了区域
                        foreach (CVDetailInfo vehicleDetail in StaticDetailInfo.GetInstance().ListVehicleDetailInfo)
                        {
                            //寻找该车辆的gps信息
                            if (vehicle.SIM != vehicleDetail.SIM)
                            {
                                continue;
                            }
                            if (vehicleDetail.VehicleGPSInfo == null || vehicleDetail.VehicleGPSInfo.Latitude == null || vehicleDetail.VehicleGPSInfo.Longitude == null)
                            {
                                break;
                            }
                            //计算车辆位置和区域中心的距离
                            double distance = VehicleCommon.GetDistance(reglat, reglng,
                                     Convert.ToDouble(vehicleDetail.VehicleGPSInfo.Latitude), Convert.ToDouble(vehicleDetail.VehicleGPSInfo.Longitude));
                            if (distance < regionradius && vehicle.TaskState.Equals("3"))
                            {//本来为无任务离场 但是更改之后应该在区域内的 
                                vehicle.TaskState = "0";
                                break;
                            }
                            else if (distance >= regionradius && vehicle.TaskState.Equals("0"))
                            {//本来为空闲无任务 但是更改之后应该在区域外的 
                                vehicle.TaskState = "3";
                                break;
                            }
                        }
                    }
                }
                #endregion
                StaticTreeState.VehicleBasicInfo = LoadingState.LOADCOMPLETE;
            }
        }
    }
}
