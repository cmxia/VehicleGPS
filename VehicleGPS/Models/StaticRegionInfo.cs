using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VehicleGPS.Services;

namespace VehicleGPS.Models
{
    class StaticRegionInfo
    {
        private static StaticRegionInfo instance = null;
        private BasicDataServiceWCF wcfService;
        private StaticRegionInfo()
        {
            this.ListRegionBasicInfo = new List<CRegionInfo>();
            this.wcfService = new BasicDataServiceWCF();
        }
        public static StaticRegionInfo GetInstance()
        {
            if (instance == null)
            {
                instance = new StaticRegionInfo();
            }
            return instance;
        }
        public List<CRegionInfo> ListRegionBasicInfo;

        public void RefreshRegionInfo()
        {
            this.wcfService.GetRegionRightThread();
        }
    }
}
