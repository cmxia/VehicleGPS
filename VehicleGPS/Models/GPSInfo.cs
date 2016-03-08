using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.Prism.ViewModel;

namespace VehicleGPS.Models
{
    public class GPSInfo : NotificationObject
    {
        public int Sequence { get; set; }
        #region gps基本信息
        public string Sim { get; set; } //sim卡(simId)
        //当前位置（Poi地理位置）
        private string curLocation;

        public string CurLocation
        {
            get { return curLocation; }
            set
            {
                curLocation = value;
                this.RaisePropertyChanged("CurLocation");
            }
        }

        public string Longitude { get; set; }//经度(纠偏后)
        public string Latitude { get; set; } //纬度(纠偏后)
        public string Direction { get; set; }//表示方向
        public string GpsStatus { get; set; }//GPS状态
        public string Altitude { get; set; }//高度
        public string Speed { get; set; }//速度
        public string DevSpeed { get; set; }//速度，行驶记录功能获取的速度
        public string Mileage { get; set; } //里程（对应车上里程表读数） 
        public string GPSMileage { get; set; } //里程（GPS读数） 
        public string OilVolumn { get; set; }//油量（对应车上油量表读数 oil）
        public string Datetime { get; set; }//当前时间：YYYY-MM-DD HH-mm-ss（上传时间 upTime）
        public string InsertTime { get; set; }//入库时间
        // 新增
        public string SatelliteNum { get; set; }//GNSS卫星数
        #endregion

        #region 车辆状态信息(初始化为-1，表示状态不定)
        public string Accstatus { get; set; } //ACC状态1开，0关
        public string Workstatus { get; set; }//运营状态：0运行，1停运
        public string Llsecret { get; set; }//经纬度加密状态：0不加密，1加密
        public string Gpsmode { get; set; }//GPS模式：1单北斗，2单GPS，3双模
        public string Oilwaystatus { get; set; }//油路状态：1正常，0断开
        public string Vcstatus { get; set; }//车辆电路状态:1正常，0断开
        public string Vdstatus { get; set; }//车门状态:1加锁，0解锁
        public string Fdstatus { get; set; }//前车门状态：1开，0关
        public string Bdstatus { get; set; }//后车门状态：1开，0关
        public string Enginestatus { get; set; }//发动机状态：1开，0关
        public string Conditionerstatus { get; set; }//空调状态：1开，0关
        public string Brakestatus { get; set; }//刹车状态：1刹车，0未刹车
        public string Ltstatus { get; set; }//左转向状态：1开，0关
        public string Rtstatus { get; set; }//右转向状态：1开，0关
        public string Farlstatus { get; set; }//远光灯状态：1开，0关
        public string Nearlstatus { get; set; } //近光灯状态：1开，0关
        public string Pnstatus { get; set; }//正反转状态：1转，0不转
        public string Shakestatus { get; set; }//震动状态：1开，0关
        public string Hornstatus { get; set; }//喇叭状态：1开，0关
        public string Protectstatus { get; set; }//安全状态：1设防，0解防
        public string Loadstatus { get; set; }//负载状态：1重载，0空载
        public string Busstatus { get; set; }//总线状态:0正常，1故障
        public string Gsmstatus { get; set; }//GSM模块状态，0正常，1故障
        public string Gpsstatus { get; set; }//GPS模块状态，0正常，1故障
        public string Lcstatus { get; set; }//锁车电路状态，0正常，1故障
        public string Ffstatus { get; set; }//前雾灯状态:1开，0关
        public string Bfstatus { get; set; }//后雾灯状态：1开，0关
        public string Gpsantstatus { get; set; }//GPS天线状态：（0正常，1短路，2断路,3未知）
        #endregion

        #region 车辆报警状态(初始化为-1，表示报警状态不定,1 表示报警，0表示不报警)
        public string Soswarn { get; set; }//紧急报警，触动报警开关后触发
        public string Overspeedwarn { get; set; }//超速报警
        public string Tiredwarn { get; set; }//疲劳驾驶
        public string Prewarn { get; set; }//预警
        public string Gnssfatal { get; set; }//GNSS模块故障
        public string Gnssantwarn { get; set; }//GNSS天线未接或被剪断
        public string Gnssshortwarn { get; set; } //GNSS天线短路
        public string Lowvolwarn { get; set; }//终端主电源欠压
        public string Highvolwarn { get; set; } //终端主电源高压
        public string Outagewarn { get; set; }//终端主电源断电
        public string Lcdfatalwarn { get; set; }//终端LCD或者显示器故障
        public string Ttsfatalwarn { get; set; }//TTS模块故障
        public string Camerafatalwarn { get; set; }//摄像头故障
        public string Vediolosewarn { get; set; }//视频丢失报警
        public string Vedioshelterwarn { get; set; } //视频遮挡报警
        public string Accumtimeout { get; set; }//当天累计驾驶超时
        public string Stoptimeout { get; set; }//超时停车
        public string Inoutareawarn { get; set; }//进出区域报警（-1不报警，0出报警，1进报警，2报警）
        public string Inoutlinewarn { get; set; }//进出录像报警（-1不报警，0出报警，1进报警，2报警）
        public string Drivingtimewarn { get; set; }//路段行驶时间不足/过长报警
        public string Deviatewarn { get; set; }//路线偏离报警
        public string Vssfatalwarn { get; set; }//车辆VSS故障	
        public string Oilexceptionwarn { get; set; }//车辆油量异常报警（0低油量，1异常上升，2异常下降）
        public string Vehiclestolenwarn { get; set; } //车辆被盗报警
        public string Illignitewarn { get; set; }//非法点火报警
        public string Illmovewarn { get; set; } //非法位移报警
        public string Crashwarn { get; set; }//碰撞侧翻报警
        public string Sdexceptionwarn { get; set; }//SD卡异常报警
        public string Robwarn { get; set; }//劫警
        public string Sleeptimewarn { get; set; }//司机停车休息时间不足报警
        public string Illtimedrivingwarn { get; set; }//非法时段行驶报警
        public string Overstationwarn { get; set; }//越战报警
        public string Ilopendoorwarn { get; set; } //非法开车门报警
        public string Protectwarn { get; set; }//设防报警
        public string Trimmingwarn { get; set; }//剪线报警
        public string Passwdwarn { get; set; }//密码错误报警
        public string Prohibitmovewarn { get; set; }//禁行报警
        public string Illstopwarn { get; set; }//非法停车报警   
        //6-16 lym
        public string ICCardwarn { get; set; }//1：道路运输证IC卡模块故障，标志维持至报警条件解除
        public string Overspeedprewarn { get; set; }//1：超速预警，标志维持至报警条件解除
        public string Tiredprewarn { get; set; }//1：疲劳驾驶预警，标志维持至报警条件解除
        public string Rolloverwarn { get; set; }//1：侧翻预警，标志维持至报警条件解除
        public string Lowbatterywarn { get; set; }//1：电瓶电压低报警

        #endregion

        #region 扩展信息
        public string Idtime { get; set; }
        public string OnlineStates { set; get; } //在线状态

        public string StopTrackTime { get; set; }//停车时长

        public bool isCheckedFlag { set; get; }
        #endregion
    }
}
