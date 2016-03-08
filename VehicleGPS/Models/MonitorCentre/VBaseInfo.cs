using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VehicleGPS.Models
{
    /// <summary>
    /// 车辆基本信息表
    /// </summary>
    class VBaseInfo
    {
        public static VBaseInfo instance = null;
        private VBaseInfo()
        {
        }
        public static VBaseInfo GetInstance()
        {
            if (instance == null)
            {
                instance = new VBaseInfo();
            }
            return instance;
        }
        public string VehicleId { get; set; }//车牌号
        public string Vehiclenum { get; set; }//车辆编号
        public string VehicleType_id { get; set; }//车辆类型
        public string SIM { get; set; }//SIM卡号
        public string FInnerId { get; set; }//内部编号
        public string GPSType_id { get; set; }//终端类型
        public string GPSVersion_id { get; set; }//终端版本号
        public string EUSERNAME { get; set; }//所属单位
        public string ContactPeo { get; set; }//联系人
        public string ContactTel { get; set; }//联系电话
        public string VehicleType { get; set; }//车辆类型
        public string GPSType { get; set; }//终端类型
        public string GPSVersion { get; set; }//终端版本号
        public string VehicleModel { get; set; }//车辆型号
        public string VehicleColor { get; set; }//车辆颜色
        public string Address { get; set; }//联系地址
        public string VehicleBrand { get; set; }//车辆品牌

        public void init()
        {
            ////车辆类型
            //if (this.VehicleType_id != null)
            //{
            //    switch (VehicleType_id)
            //    {
            //        case "cl00001": this.VehicleType = "混凝土搅拌车"; break;
            //        case "cl00002": this.VehicleType = "泵车"; break;
            //        case "cl00003": this.VehicleType = "工具车"; break;
            //        case "cl00004": this.VehicleType = "服务车"; break;
            //    }
            //}

            //终端型号和类型
            if (this.GPSType_id != null)
            {
                foreach (GpsTYPE type in gps_Type())
                {
                    if (Convert.ToInt32(this.GPSType_id)==type.ID)
                    {
                        this.GPSType = type.gpsType;
                        if (this.GPSVersion_id != null)
                        {
                            listVersion = type.list;
                            foreach (GpsVERSION version in listVersion)
                            {
                                if (Convert.ToInt32(GPSVersion_id)==version.ID)
                                {
                                    this.GPSVersion = version.gpsVersion;
                                    break;
                                }
                            }
                        }
                        break;
                    }
                }
            }
        }

        #region 自定义项
        List<GpsTYPE> listType = new List<GpsTYPE>();
        List<GpsVERSION> listVersion = new List<GpsVERSION>();
        List<State> state = new List<State>();
        public class GpsTYPE
        {
            public int ID { get; set; }
            public string gpsType { get; set; }
            public List<GpsVERSION> list { get; set; }
        }
        public class GpsVERSION
        {
            public int ID { get; set; }
            public string gpsVersion { get; set; }
        }
        List<GpsTYPE> gps_Type()
        {
            List<GpsTYPE> g_type = new List<GpsTYPE>();
            g_type.Add(new GpsTYPE { ID = 1, gpsType = "华强协议", list = gps_Version(1) });
            g_type.Add(new GpsTYPE { ID = 2, gpsType = "部标协议", list = gps_Version(2) });
            g_type.Add(new GpsTYPE { ID = 3, gpsType = "移为协议", list = gps_Version(3) });
            return g_type;
        }
        public List<GpsVERSION> gps_Version(int Id)
        {
            List<GpsVERSION> g_type = new List<GpsVERSION>();
            if (Id == 1)
            {
                g_type.Add(new GpsVERSION { ID = 0, gpsVersion = "V3" });
            }
            else if (Id == 2)
            {
                g_type.Add(new GpsVERSION { ID = 0, gpsVersion = "标准部标" });
                g_type.Add(new GpsVERSION { ID = 1, gpsVersion = "世通协议" });
                g_type.Add(new GpsVERSION { ID = 2, gpsVersion = "博世杰" });
                g_type.Add(new GpsVERSION { ID = 3, gpsVersion = "北斗" });
            }
            else
            {
                g_type.Add(new GpsVERSION { ID = 1, gpsVersion = "V_100" });
                g_type.Add(new GpsVERSION { ID = 2, gpsVersion = "V_200" });
                g_type.Add(new GpsVERSION { ID = 3, gpsVersion = "V_300" });
            }
            return g_type;
        }

        public class State
        {
            public short ID { get; set; }
            public string Name { get; set; }
        }
        List<State> getState()
        {
            List<State> g_type = new List<State>();
            g_type.Add(new State { ID = 0, Name = "正常" });
            g_type.Add(new State { ID = 1, Name = "临时" });
            g_type.Add(new State { ID = 2, Name = "维修" });
            return g_type;
        }
        #endregion
        
    }
}
