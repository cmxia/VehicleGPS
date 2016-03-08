using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.Prism.ViewModel;

namespace VehicleGPS.Models
{
    public class CVDetailInfo : NotificationObject
    {
        /*车辆基本信息*/
        public int Sequence { get; set; }//序号
        public string FInnerId { get; set; }//内部编号
        public string VehicleNum { get; set; }//车辆编号
        public string VehicleId { get; set; }//车牌号码
        public string SIM { get; set; }//SIM卡号
        public string VehiclecurState { get; set; }//车辆状态
        ////////public string CustomerID { get; set; }
        ////////public string CustomerName { get; set; }//所属单位
        public string ParentUnitId { get; set; }//所属单位ID
        public string ParentUnitName { get; set; }//所属单位
        public string VehicleType { get; set; }//车辆类别
        public string VehicleTypeName { get; set; }//车辆类别（不需要了）
        public string BrandModel { get; set; }//厂牌型号
        public string OilType { get; set; }//燃油类型
        public string VIN { get; set; }//车架号
        public string EngineId { get; set; }//发动机号
        public string VehicleModel { get; set; }//车辆型号
        public string PurchaseNum { get; set; }//购置证号
        public string OperatNum { get; set; }//营运证号
        public string VehicleColor { get; set; }//车辆颜色
        public string Tonnage { get; set; }//吨位
        public string tiresNum { get; set; }//轮胎数
        public string EmptyCost { get; set; }//百公里油耗（空载）
        public string FullCost { get; set; }//百公里油耗（满载
        public string LoadAmount { get; set; }//核定承载方量
        public string GPSID { get; set; }//GPS车载终端编号
        public string GPSType { get; set; }
        public string GPSVersion { get; set; }
        public string seatsNum { get; set; }//座位数
        public string CarDealers { get; set; }//汽车销售商
        public string purchaseDate { get; set; }//购买日期
        public string cardDate { get; set; }//上牌日期
        public string debetOrNot { get; set; }//是否贷保
        private string loadCapacity;//承载容量
        public string LoadCapacity
        {
            get
            {
                return loadCapacity;
            }
            set
            {
                if (string.IsNullOrEmpty(loadCapacity))
                {
                    loadCapacity = value;
                }
                else
                {
                    loadCapacity = Convert.ToDouble(value).ToString("0.00");
                }
            }
        }//承载容量
        private string fuelCapacity;
        public string FuelCapacity
        {
            get
            {
                return fuelCapacity;
            }
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    fuelCapacity = value;
                }
                else
                {
                    fuelCapacity = Convert.ToDouble(value).ToString("0.00");
                }
            }
        }//油箱容量
        public string LoansOrNot { get; set; }//是否贷款
        public string VehicleCosts { get; set; }//车辆成本
        public string Life { get; set; }//使用年限
        public string SalvageValue { get; set; }//预计残值
        public string VehicleState { get; set; }//任务状况

        //public GPSInfo VehicleGPSInfo { get; set; }//车辆最新GPS信息
        private GPSInfo vehicleGPSInfo;//车辆最新GPS信息
        public GPSInfo VehicleGPSInfo
        {
            get { return vehicleGPSInfo; }
            set
            {
                this.vehicleGPSInfo = value;
                this.RaisePropertyChanged("VehicleGPSInfo");
            }
        }

        ///*GPS信息*/
        //public string CurrentLocation { get; set; }//当前位置
        //public string GPSState { get; set; }//GPS状态
        //public string Altitude { get; set; }//海拔高原
        //public string GPSSpeed { get; set; }//GPS速度
        //public string GPSSpeedRecord { get; set; }//GPS速度（行驶记录功能获取）
        //public string Direction { get; set; }//方向
        //public string Mileage { get; set; }//里程（车上里程表读数）
        //public string Oil { get; set; }//油量（车上油量表读数）
        //public string UploadTime { get; set; }//上传时间

        ///*车辆状态信息*/
        //public string ACCState { get; set; }//ACC状态
        //public string OperatingState { get; set; }//运营状态
        //public string GPSModel { get; set; }//GPS模式
        //public string OilWayState { get; set; }//油路状态
        //public string ShakeState { get; set; }//震动状态
        //public string HornState { get; set; }//喇叭状态
        //public string AirConditionState { get; set; }//空调状态
        //public string DoorState { get; set; }//车门状态
        //public string FrontDoorState { get; set; }//前车门状态
        //public string BackDoorState { get; set; }//后车门状态
        //public string TurnLeftState { get; set; }//左转向状态
        //public string TurnRightState { get; set; }//右转向状态
        //public string HighBeamState { get; set; }//远光灯状态
        //public string HeadLightState { get; set; }//近光灯状态
        //public string CircuitState { get; set; }//车辆电路状态
        //public string EncriptionState { get; set; }//经纬度加密状态
        //public string TurnState { get; set; }//正反转状态
        //public string EngineState { get; set; }//发动机状态
        //public string BrakeState { get; set; }//刹车状态
        //public string SafeState { get; set; }//安全状态
        //public string BearState { get; set; }//负载状态
        //public string BusState { get; set; }//总线状态
        //public string GSMState { get; set; }//GSM模块状态
        //public string LockCircuitState { get; set; }//锁车电路状态
        //public string FrontFogLightState { get; set; }//前雾灯状态
        //public string BackFogLightState { get; set; }//后雾灯状态
        //public string GPSAntennaState { get; set; }//GPS天线状态

        ///*报警状态信息*/
        //public string UrgencyAlarm { get; set; }//紧急报警
        //public string OverspeedAlarm { get; set; }//超速报警
        //public string TiredDriving { get; set; }//疲劳驾驶
        //public string EarlyWarning { get; set; }//预警
        //public string GNSSFault { get; set; }//GNSS模块故障
        //public string GNSSAntennaBreak { get; set; }//GNSS天线未接或被剪断
        //public string GNSSAntennaShotCircuit { get; set; }//GNSS天线短路
        //public string UnderVoltage { get; set; }//终端主电源欠压
        //public string OverVoltage { get; set; }//终端主电源高压
        //public string PowerBreakOut { get; set; }//终端主电源断电
        //public string LCDFault { get; set; }//终端LCD或者显示器故障
        //public string TTSFault { get; set; }//TTS模块故障
        //public string CameraFault { get; set; }//摄像头故障
        //public string VideoLostAlarm { get; set; }//视频丢失报警
        //public string VideoShelterAlarm { get; set; }//视频遮挡报警
        //public string OvertimeDrive { get; set; }//当天累计驾驶超时
        //public string OvertimePark { get; set; }//超时停车
        //public string RoadDriveTime { get; set; }//路段行驶时间不足/过长报警
        //public string RouteDiverge { get; set; }//路线偏离报警
        //public string VSSFault { get; set; }//车辆VSS故障
        //public string OilAbnormalAlarm { get; set; }//车辆油量异常报警
        //public string StolenAlarmAlarm { get; set; }//车辆被盗报警
        //public string IllegalIgnitionAlarm { get; set; }//非法点火报警
        //public string IllegalMoveAlarm { get; set; }//非法位移报警
        //public string CollisionAlarm { get; set; }//碰撞侧翻报警
        //public string SDAbnormalAlarm { get; set; }//SD卡异常报警
        //public string RobAlarm { get; set; }//劫警
        //public string RestTimeAlarm { get; set; }//司机停车休息时间不足报警
        //public string IllegalDriveAlarm { get; set; }//非法时段行驶报警
        //public string OverSiteAlarm { get; set; }//越战报警
        //public string IlleageOpenDoorAlarm { get; set; }//非法开车门报警
        //public string FortifiedAlarm { get; set; }//设防报警
        //public string CutLineAlarm { get; set; }//剪线报警
        //public string PasswordWrongAlarm { get; set; }//密码错误报警
    }
}
