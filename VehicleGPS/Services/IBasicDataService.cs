using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VehicleGPS.Models;

namespace VehicleGPS.Services
{
    interface IBasicDataService
    {
        void GetVehicleRightInfo();//获取权限所有车辆关键信息
        void GetClientRightInfo();//获取权限所有单位关键信息
        void GetVehicleDetailInfo();//获取权限车辆所有基础信息
    }
}
