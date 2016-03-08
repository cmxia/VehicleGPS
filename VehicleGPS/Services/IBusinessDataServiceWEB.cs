using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VehicleGPS.Services
{
    interface IBusinessDataServiceWEB
    {
        void GetLatestVehicleGPSInfo(bool isRefresh=false);//获取最新的权限内所有车辆的GPS信息
    }
}
