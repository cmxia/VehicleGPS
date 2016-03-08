using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VehicleGPS.Views.Control.ReportCentre.Reports.Common
{

        #region 运行分析

        /// <summary>
        /// by LYM
        /// 报表运行分析所需字段
        /// </summary>
        public class RunningAnalysisJson
        {
            #region //运行分析信息（LYM）

            private string vehicleID; //车牌号
            public string VehicleID
            {
                get { return vehicleID; }
                set { vehicleID = value; }
            }

            private string username; //所属单位名
            public string UserName
            {
                get { return username; }
                set { username = value; }
            }

            private string sim; //SIM卡号
            public string SIM
            {
                get { return sim; }
                set { sim = value; }
            }

            private string accstatus; //ACC状态1开，0关
            public string Accstatus
            {
                get { return accstatus; }
                set { accstatus = value; }
            }

            private string llsecret; //经纬度加密状态：0不加密，1加密
            public string Llsecret
            {
                get { return llsecret; }
                set { llsecret = value; }
            }

            private double slongitude; //经度 开始
            public double sLongituse
            {
                get { return slongitude; }
                set { slongitude = value; }
            }

            private double slatitude; //纬度
            public double sLatitude
            {
                get { return slatitude; }
                set { slatitude = value; }
            }

            private double elongitude; //经度 开始
            public double eLongituse
            {
                get { return elongitude; }
                set { elongitude = value; }
            }

            private double elatitude; //纬度
            public double eLatitude
            {
                get { return elatitude; }
                set { elatitude = value; }
            }

            private string starttime; //开始时间（上线时间)
            public string StartTime
            {
                get { return starttime; }
                set { starttime = value; }
            }

            private string endtime; //结束时间（下线时间)
            public string EndTime
            {
                get { return endtime; }
                set { endtime = value; }
            }

            private string timelength; //持续时间
            public string TimeLength
            {
                get { return timelength; }
                set { timelength = value; }
            }

            private string startport; //开始地点（停车地点）
            public string StartPort
            {
                get { return startport; }
                set { startport = value; }
            }

            private string endpoit; //结束地点
            public string EndPoit
            {
                get { return endpoit; }
                set { endpoit = value; }
            }

            private string regionname;//区域名称（客户兴趣点）
            public string RegionName
            {
                get { return regionname; }
                set { regionname = value; }

            }

            //private int onlineoffline;//上下线标识（0：下线；1：上线）
            //public int OnlineOffline
            //{
            //    get { return onlineoffline; }
            //    set { onlineoffline = value; }

            //}

            private string regiontype;//区域类型（工地，站点）
            public string RegionType
            {
                get { return regiontype; }
                set { regiontype = value; }
            }
            #endregion
        }

        //停车统计和未熄火停车
        public class ParkingInfo
        {
            public int sequence { get; set; }
            public string vehicleNum { get; set; }//车牌号
            public string sim { get; set; }
            public string parentDepart { get; set; }//所属单位
            public string startTime { get; set; }//开始时间
            public string endTime { get; set; }//结束时间
            public string interval { get; set; }//停车时长
            public string lng { get; set; }
            public string lat { get; set; }
            public string address { get; set; }

        }
        //Acc分析 运行统计
        public class ACCRunning
        {
            public int sequence { get; set; }
            public string vehicleNum { get; set; }//车牌号
            public string sim { get; set; }
            public string parentDepart { get; set; }//所属单位
            public string acc { get; set; }
            public string startTime { get; set; }//开始时间
            public string endTime { get; set; }//结束时间
            public string interval { get; set; }//持续时长
            public string sLng { get; set; }
            public string sLat { get; set; }
            public string startAddr { get; set; }
            public string eLng { get; set; }
            public string eLat { get; set; }
            public string endAddr { get; set; }
        }

        //上下线明细
        public class OnOffline
        {
            public int sequence { get; set; }
            public string vehicleNum { get; set; }//车牌号
            public string sim { get; set; }
            public string parentDepart { get; set; }//所属单位
            public string inTime { get; set; }//上线时间
            public string offTime { get; set; }//下线时间
            public string interval { get; set; }//持续时长
        }
        
    //进出区域
        public class RegionInfo
        {
            public int sequence { get; set; }
            public string vehicleNum { get; set; }//车牌号
            public string sim { get; set; }
            public string parentDepart { get; set; }//所属单位
            public string regionName { get; set; }//区域名称
            public string regionType { get; set; }//区域类型
            public string inTime { get; set; }//上线时间
            public string outTime { get; set; }//下线时间
            public string interval { get; set; }//持续时长
        }
    //历史轨迹
        public class InfoTrack
        {
            public int serial { get; set; }
            public string id { get; set; }
            public string sim { get; set; }
            public string vehicleNum { get; set; }//车牌号
            public string parentDepart { get; set; }//所属单位
            public double   longitude { get; set; }
            public double   latitude  { get; set; }
            public string   direction{ get; set; }
            public string   gpsStatus { get; set; }//GPS状态
            public double   altitude { get; set; }//海拔高度
            public double   speed{ get; set; }
            public double   devSpeed { get; set; }//行驶记录速度
            public double   mileage { get; set; }
            public double   oilVolumn { get; set; }//油量
            public string   recordtime { get; set; }
            public string   idtime { get; set; }
            public string   inserttime { get; set; }
            public string   accstatus { get; set; }//ACC状态
            public string   workstatus { get; set; }//运营状态
            public string   llsecret { get; set; }//经纬度加密状态
            public string   gpsmode { get; set; }//GPS模式
            public string   oilwaystatus { get; set; }//油路状态
            public string   vcstatus { get; set; }//车辆电路状态
            public string   vdstatus { get; set; }//车门状态
            public string   fdstatus { get; set; }//前车门
            public string   bdstatus { get; set; }//后车门
            public string   enginestatus { get; set; }//发动机状态
            public string   conditionerstatus { get; set; }//空调状态
            public string   brakestatus { get; set; }//车状态
            public string   ltstatus { get; set; }//左转向
            public string   rtstatus { get; set; }//右转向
            public string   farlstatus { get; set; }//远光灯状态
            public string   nearlstatus { get; set; }
            public string   pnstatus { get; set; }//正反转状态
            public string   shakestatus { get; set; }
            public string   hornstatus { get; set; }//喇叭
            public string   protectstatus { get; set; }
            public string   loadstatus { get; set; }
            public string   busstatus { get; set; }
            public string   gsmstatus { get; set; }
            public string   lcstatus { get; set; }//锁车电路状态
            public string   ffstatus { get; set; }//前雾灯
            public string   bfstatus { get; set; }
            public string   gpsantstatus { get; set; }//GPS天线
            public string   soswarn { get; set; }
            public string   overspeedwarn { get; set; }
            public string   tiredwarn { get; set; }
            public string   prewarn { get; set; }
            public string   gnssfatal { get; set; }//GNSS模块故障
            public string   gnssantwarn { get; set; }//GNSS天线故障
            public string   lowvolwarn { get; set; }//终端主电源断电
            public string   highvolwarn { get; set; }//终端主电源高压
            public string   outagewarn { get; set; }//终端主电源掉电
            public string   lcdfatalwarn { get; set; }//终端LCD或者显示器故障
            public string   ttsfatalwarn { get; set; }//TSS模块故障
            public string   camerafatalwarn { get; set; }
            public string   vediolosewarn { get; set; }
            public string   accumtimeout { get; set; }//当天累计驾驶超时
            public string   stoptimeout { get; set; }
            public string   inoutareawarn { get; set; }//进出区域报警
            public string   inoutlinewarn { get; set; }//进出路段报警 
            public string   drivingtimewarn { get; set; }//路段行驶时间不足/过长报警
            public string   deviatewarn { get; set; }//路线偏离报警
            public string   vssfatalwarn { get; set; }//车辆VSS故障
            public string   oilexceptionwarn { get; set; }
            public string   vehiclestolenwarn { get; set; }
            public string   illignitewarn { get; set; }//非法点火报警
            public string   illmovewarn { get; set; }
            public string   crashwarn { get; set; }//碰撞侧翻报警
            public string   sdexceptionwarn { get; set; }
            public string   robwarn { get; set; }//劫警
            public string   sleeptimewarn { get; set; }//司机停车休息时间不足报警
            public string   illtimedrivingwarn { get; set; }//非法时段行驶报警
            public string   overstationwarn { get; set; }
            public string   ilopendoorwarn { get; set; }//非法开车门报警
            public string   protectwarn { get; set; }//设防报警
            public string   trimmingwarn { get; set; }//剪线报警
            public string   passwdwarn { get; set; }
            public string   prohibitmovewarn { get; set; }//禁行报警
            public string   illstopwarn { get; set; }
            public string   gnssshortwarn { get; set; }//GNSS天线短路
            public string   vedioshelterwarn { get; set; }//视频遮挡报警
        }
        /// <summary>
        /// 离线分析
        /// 俞哲
        /// </summary>
        public class LastTimeJson
        {
            public string VehicleNum { get; set; }
            public string lastTime { get; set; }
            public string lng { get; set; }
            public string lat { get; set; }

        }

        public class InfoOffline
        {
            public int sequence { get; set; }
            public string vehicleNum { get; set; }
            public string sim { get; set; }
            public string parentDepart { get; set; }//所属单位
            public string lastTime { get; set; }//最后上报时间
            public string offTime { get; set; }//离线时间
            public string lng { get; set; }
            public string lat { get; set; }
            public string address { get; set; }

        }
        #endregion

        #region 油耗分析

        /// <summary>
        /// 油耗统计
        /// 匡丽
        /// </summary>
        public class oilstatis
        {
            //public string sim { get; set; }
            public string vehicleNum { get; set; }
            public List<double> oilVolume; //油量 
            public List<string> datetime;  //时间
            //kl
            public Dictionary<string, double> daysoil { get; set; }
        }

        /// <summary>
        /// 油耗明细 油表查看
        /// 夏创铭 武林林
        /// </summary>
        public class OilJson
        {
            public string sim { get; set; }
            public string vehicleNum { get; set; }
            public Dictionary<string, double> oil { get; set; }//key:时间 value：油量
            public Dictionary<double, double> location { get; set; } //key:经度  value:纬度

            public List<double> oilVolume { get; set;} //油量 
            public List<string> datetime { get; set; }  //时间
            public List<double> longitude { get; set; } //经度
            public List<double> latitude { get; set; }  //纬度
            //kl
            public Dictionary<string, double> daysoil { get; set; }
        }
        
        /*油耗明细*/
        public class Oil
        {
            public int sequence { get; set; }//序号
            public string vehicleNum { get; set; }//车牌号
            public string id { get; set; }//车辆编号
            public string sim { get; set; }//sim卡号
            public string parentDepart { get; set; }//所属单位
            public string time { get; set; }//上报时间
            public string oil { get; set; }//油量
            public string address { get; set; }//地址
            public string lat { get; set; }//纬度
            public string lng { get; set; }//经度
        }
        #endregion

        #region 告警分析

        #endregion

        #region 行车分析

        /// <summary>
        /// 速度明细
        /// 武林林
        /// </summary>
        public class speedDetail
        {
            //public int num { get; set; }//列表显示编号
            //public string SimId { get; set; }//SIM卡号
            public string VehicleId { get; set; }//车牌号
            //public string Vehiclenum { get; set; }//车辆编号  查询条件
            public List<string> StartTime { get; set; }//报警开始时间
            public List<string> EndTime { get; set; }//报警结束时间
            public List<string> LengthOfTime { get; set; }//超速时长
            public List<double> Speed { get; set; }//GPS时速
            //public string location { get; set; }//车辆位置
            public List<double> S_LONG { get; set; }//开始经度
            public List<double> S_LAT { get; set; }//开始纬度
            public List<double> E_LONG { get; set; }//结束经度
            public List<double> E_LAT { get; set; }//结束纬度

        }

        /// <summary>
        /// 单车区域报警，区域汇总
        /// 俞哲
        /// </summary>
        public class OverSpeedJson
        {
            public string startTime { get; set; }
            public string endTime { get; set; }
            public double mileage { get; set; }
            public string lng { get; set; }
            public string lat { get; set; }
            public string SIM { get; set; }
            public string VehicleNum { get; set; }
            public string maxSpeed { get; set; }
            public string continusTime { get; set; }
            public List<LngLat> listLngLat { get; set; }
        }

        /// <summary>
        /// 单车区域报警，区域汇总
        /// 俞哲
        /// </summary>
        public class LngLat
        {
            public string lng { get; set; }
            public string lat { get; set; }
            public string addr { get; set; }
        }

        public class InfoOverSpeed
        {
            public InfoOverSpeed()
            {
                this.overSpeedDetailList = new List<InfoOverSpeedDetail>();
            }
            public int sequence { get; set; }
            public string id { get; set; }
            public string vehicleNum { get; set; }//车辆名称
            public string parentDepart { get; set; }//所属单位
            public string sim { get; set; }
            public int overSpeedTimes { get; set; } //超速次数
            public string startTime { get; set; }
            public string endTime { get; set; }
            public string mileage { get; set; }
            public List<InfoOverSpeedDetail> overSpeedDetailList { get; set; }
        }

        public class InfoOverSpeedDetail
        {
            public int sequence { get; set; }
            public string id { get; set; }
            public string vehicleNum { get; set; }//车辆名称
            public string parentDepart { get; set; }//所属单位
            public string sim { get; set; }
            public int overSpeedRank { get; set; } //第几次超速
            public string startTime { get; set; }
            public string endTime { get; set; }
            public double mileage { get; set; }
            public string lng { get; set; }
            public string lat { get; set; }
            public string address { get; set; }
            public string maxSpeed { get; set; }
            public string continuedTime { get; set; }
            public List<LngLat> listLngLat { get; set; }

        }
        #endregion

        #region 里程分析

        /// <summary>
        /// 里程统计 里程明细 形式总里程
        /// 李浩文
        /// </summary>
        public class Mileage
        {
            //public string sim { get; set; }
            public int sequence { get; set; }//序号
            public string vehicleNum { get; set; }//车牌号
            public string id { get; set; }//车辆编号
            public string sim { get; set; }//sim卡号
            public string parentDepart { get; set; }//所属单位
            public string time { get; set; }//统计时间
            public string mileage { get; set; }//里程
        }

        public class MileageJson
        {
            public string vehicleNum { get; set; }//车牌号
            public Dictionary<string, double> mileages { get; set; }
        }
        #endregion

        #region 常用报表
        /*运输明细*/
        public class InfoTransCondetails
        {
            public int Sequence { get; set; }
            public string VehicleNum { get; set; }//车牌编号
            public string VehicleID { get; set; }//车牌号
            public string UserName { get; set; }//所属单位
            public string FPlanId { get; set; }//任务单号
            public string FPlanName { get; set; }//工地名称/项目名称
            public string FinsertTime { get; set; }//派单时间
            public float PlanDistence { get; set; }//运距
            public float TransCapPer { get; set; }//运输方量
            public string LeaveStartPointTime { get; set; }//离站时间
            public string EnterEndPointTime { get; set; }//进工地时间
            public string LeaveEndPointTime { get; set; }//离开工地时间
            public string EnterStartPointTime { get; set; }//回站时间
            public string TimeCost { get; set; } //运次耗时(分钟)
        }
    /*指令下发历史*/
        public class InstructionHistory
        {
            public int Sequence { get; set; }
            public string CustomerID { get; set; }
            public string Finsertdate { get; set; }
            public string FMemo { get; set; }
            public string InsDetails { get; set; }
            public string Instruction { get; set; }
            public string Sender { get; set; }
            public string SIM { get; set; }
            public string VehicleId { get; set; }
            public string VehicleNum { get; set; }
            public string InsDetailsStr { get; set; }
        }
    /*无任务离场*/
        public class InfoLeaveWithoutTask
        {
            public string VehicleId { get; set; }//车辆名称
            public string SIM { get; set; }
            public string VehicleNum { get; set; }//车辆编号
            public string Customer { get; set; }//所属单位
            private string leavetime;

            public string Leavetime
            {
                get { return leavetime; }
                set { leavetime = value; }
            }
            private string entertime;

            public string Entertime
            {
                get { return entertime; }
                set { entertime = value; }
            }
            private string warntype;

            public string Warntype
            {
                get
                {
                    switch (warntype.ToLower())
                    {
                        case "wt0001":
                            warntype = "无任务离场";
                            break;
                        case "wt0002":
                            warntype = "运输超时";
                            break;
                        case "wt0003":
                            warntype = "卸料超时";
                            break;
                        case "wt0004":
                            warntype = "未按时返回";
                            break;

                    };

                    return warntype;
                }
                set { warntype = value; }
            }
            private string inserttime;

            public string Inserttime
            {
                get { return inserttime; }
                set { inserttime = value; }
            }
            private string planId;

            public string PlanId
            {
                get { return planId; }
                set { planId = value; }
            }
            private string planName;

            public string PlanName
            {
                get { return planName; }
                set { planName = value; }
            }
        }
        #endregion
}
