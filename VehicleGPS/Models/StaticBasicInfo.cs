using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Objects;
using VehicleGPS.Models.Login;
using System.Net;
using System.Xml;
using System.IO;
using System.Threading;
using VehicleGPS.Services;

namespace VehicleGPS.Models
{
    /*车辆和用户的权限基本信息*/
    /// <summary>
    /// 获得车辆和用户信息（查数据库）
    /// 包含车辆和用户基本信息列表
    /// 包含车辆详细信息的查询
    /// </summary>
    class StaticBasicInfo
    {
        private static StaticBasicInfo instance = null;
        private BasicDataServiceWCF wcfService;
        private StaticBasicInfo()
        {
            this.ListClientBasicInfo = new List<CVBasicInfo>();
            this.ListVehicleBasicInfo = new List<CVBasicInfo>();
            this.ListVehicleOfClientBaseInfo = new List<string>();
            this.wcfService = new BasicDataServiceWCF();
        }
        public static StaticBasicInfo GetInstance()
        {
            if (instance == null)
            {
                instance = new StaticBasicInfo();
            }
            return instance;
        }
        public List<CVBasicInfo> ListVehicleBasicInfo;
        public List<CVBasicInfo> ListClientBasicInfo;

        public List<string> ListVehicleOfClientBaseInfo;//包含车辆的单位列表
        /// <summary>
        /// 14-6-6 RefreshBasicInfo
        /// </summary>
        public void RefreshBasicInfo_old()
        {
            this.wcfService.GetClientRightThread();
            this.wcfService.GetVehicleRightThread();
            this.wcfService.GetVehicleDetailThread();
        }
        /// <summary>
        /// 获取单位车辆权限信息14-6-6（还要交车辆详细信息this.wcfService.GetVehicleDetailThread();）
        /// </summary>
        public void RefreshBasicInfo()
        {
            this.wcfService.GetClientRightThread();
            this.wcfService.GetVehicleDetailThread();
        }
        /// <summary>
        /// 获取单位权限
        /// </summary>
        public void RefreshClientBasicInfo()
        {
            this.wcfService.GetClientRightThread();
        }
        public void RefreshVehicleBasicInfo()
        {
            this.wcfService.GetVehicleRightThread();
        }
        /// <summary>
        /// 获取车辆详细信息（此处不包括GPS信息）
        /// </summary>
        public void RefreshVehicleDetailInfo()
        {
            this.wcfService.GetVehicleDetailThread();
        }
       
    }
}
