using Microsoft.Practices.Prism.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VehicleGPS.Models
{
    public class GPSWarnInfo : NotificationObject
    {

        private bool isSelected;
        public bool IsSelected
        {
            get { return isSelected; }
            set
            {
                isSelected = value;
                this.RaisePropertyChanged("IsSelected");
            }
        }
        public int Sequence { get; set; }//序号
        public string VehicleId { get; set; }//车牌号
        public string WarnTime { get; set; }//报警时间
        public string EndTime { get; set; }//报警结束时间
        public string LastTime { get; set; }//持续时间
        public string WarnType { get; set; }//报警类型
        public string WarnData { get; set; }//报警原由
        public string Address { get; set; }//报警地点
        public string WarnInfo { get; set; }//报警内容
        public string FInnerId { get; set; }//内部编号
        public string Speed { get; set; }//速度
        public string Unit { get; set; }//所属单位
        public string UnitId { get; set; }//所属单位ID

        public string SimId { get; set; } //sim卡(simId)
        public string Longitude { get; set; }//经度(纠偏后)
        public string Latitude { get; set; } //纬度(纠偏后)
        public string NLongitude { get; set; }//经度 
        public string NLatitude { get; set; } //纬度 
        public string Poi { get; set; } // 
        public string WarnStartTime { get; set; }//报警开始时间



        //推送过来的字段
        public string cmdId { get; set; }
        public string serialNum { get; set; }

        //#region 车辆报警状态(初始化为-1，表示报警状态不定,1 表示报警，0表示不报警)
        //public string Soswarn { get; set; }//紧急报警，触动报警开关后触发
        //public string Overspeedwarn { get; set; }//超速报警
        //public string Tiredwarn { get; set; }//疲劳驾驶
        //public string Prewarn { get; set; }//预警
        //public string Gnssfatal { get; set; }//GNSS模块故障
        //public string Gnssantwarn { get; set; }//GNSS天线未接或被剪断
        //public string Gnssshortwarn { get; set; } //GNSS天线短路
        //public string Lowvolwarn { get; set; }//终端主电源欠压
        //public string Highvolwarn { get; set; } //终端主电源高压
        //public string Outagewarn { get; set; }//终端主电源断电
        //public string Lcdfatalwarn { get; set; }//终端LCD或者显示器故障
        //public string Ttsfatalwarn { get; set; }//TTS模块故障
        //public string Camerafatalwarn { get; set; }//摄像头故障
        //public string Vediolosewarn { get; set; }//视频丢失报警
        //public string Vedioshelterwarn { get; set; } //视频遮挡报警
        //public string Accumtimeout { get; set; }//当天累计驾驶超时
        //public string Stoptimeout { get; set; }//超时停车
        //public string Inoutareawarn { get; set; }//进出区域报警（-1不报警，0出报警，1进报警，2报警）
        //public string Inoutlinewarn { get; set; }//进出录像报警（-1不报警，0出报警，1进报警，2报警）
        //public string Drivingtimewarn { get; set; }//路段行驶时间不足/过长报警
        //public string Deviatewarn { get; set; }//路线偏离报警
        //public string Vssfatalwarn { get; set; }//车辆VSS故障	
        //public string Oilexceptionwarn { get; set; }//车辆油量异常报警（0低油量，1异常上升，2异常下降）
        //public string Vehiclestolenwarn { get; set; } //车辆被盗报警
        //public string Illignitewarn { get; set; }//非法点火报警
        //public string Illmovewarn { get; set; } //非法位移报警
        //public string Crashwarn { get; set; }//碰撞侧翻报警
        //public string Sdexceptionwarn { get; set; }//SD卡异常报警
        //public string Robwarn { get; set; }//劫警
        //public string Sleeptimewarn { get; set; }//司机停车休息时间不足报警
        //public string Illtimedrivingwarn { get; set; }//非法时段行驶报警
        //public string Overstationwarn { get; set; }//越战报警
        //public string Ilopendoorwarn { get; set; } //非法开车门报警
        //public string Protectwarn { get; set; }//设防报警
        //public string Trimmingwarn { get; set; }//剪线报警
        //public string Passwdwarn { get; set; }//密码错误报警
        //public string Prohibitmovewarn { get; set; }//禁行报警
        //public string Illstopwarn { get; set; }//非法停车报警   
        ////6-16 lym
        //public string ICCardwarn { get; set; }//1：道路运输证IC卡模块故障，标志维持至报警条件解除
        //public string Overspeedprewarn { get; set; }//1：超速预警，标志维持至报警条件解除
        //public string Tiredprewarn { get; set; }//1：疲劳驾驶预警，标志维持至报警条件解除
        //public string Rolloverwarn { get; set; }//1：侧翻预警，标志维持至报警条件解除
        //public string Lowbatterywarn { get; set; }//1：电瓶电压低报警

        //#endregion
    }
}
