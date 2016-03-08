using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Objects;
using VehicleGPS.Services;

namespace VehicleGPS.Models
{
    /*该类用来获取所有类别ID以及类别名称*/
    /// <summary>
    /// 该类用于获取车辆、用户的类别信息
    /// </summary>
    class StaticBasicType
    {
        private static StaticBasicType instance = null;
        private StaticBasicType()
        {
            this.ListBasicTypeInfo = new List<BasicTypeInfo>();
        }
        public static StaticBasicType GetInstance()
        {
            if (instance == null)
            {
                instance = new StaticBasicType();
            }
            return instance;
        }
        public List<BasicTypeInfo> ListBasicTypeInfo;
        public void RefreshBasicInfo()
        {
            BasicDataServiceWCF wcfService = new BasicDataServiceWCF();
            wcfService.GetBasicTypeThread();
        }
    }
}
