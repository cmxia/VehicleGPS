using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VehicleGPS.Services;

namespace VehicleGPS.Models
{
    class StaticRight
    {
        private static StaticRight instance;
        private BasicDataServiceWCF wcfService;
        private StaticRight()
        {
            this.ListAlarmRight = new List<RightInfo>();
            this.ListMileageRight = new List<RightInfo>();
            this.ListOilRight = new List<RightInfo>();
            this.ListRecordRight = new List<RightInfo>();
            this.ListRunRight = new List<RightInfo>();
            this.ListCommonRight = new List<RightInfo>();
            this.ListMenuRight = new List<RightInfo>();
            this.wcfService = new BasicDataServiceWCF();
        }
        public static StaticRight GetInstance()
        {
            if (instance == null)
            {
                instance = new StaticRight();
            }
            return instance;
        }
        public void RefreshRightInfo()
        {
            this.wcfService.GetRightThread();
        }
        //菜单权限
        public List<RightInfo> ListMenuRight { get; set; }//菜单权限
        #region 报表权限
        public List<RightInfo> ListOilRight { get; set; }//油耗分析
        public List<RightInfo> ListMileageRight { get; set; }//里程分析
        public List<RightInfo> ListRunRight { get; set; }//运行分析
        public List<RightInfo> ListAlarmRight { get; set; }//告警分析
        public List<RightInfo> ListRecordRight { get; set; }//行车记录
        public List<RightInfo> ListCommonRight { get; set; }//常用报表
        #endregion
    }
}
