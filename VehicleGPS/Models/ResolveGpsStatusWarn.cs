using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VehicleGPS.Models
{
    static class ResolveGpsStatusWarn
    {
        #region resolve States
        //0没有，1使用
        public static string stringgeGpsUseMod(string oc)
        {
            string ocStr = "";
            switch (oc)
            {
                case "0":
                    ocStr = "没有";
                    break;
                case "1":
                    ocStr = "使用";
                    break;
            }
            return ocStr;
        }

        //0关；1开
        public static string stringgeOC(string oc)
        {
            string ocStr = "";
            switch (oc)
            {
                case "0":
                    ocStr = "关";
                    break;
                case "1":
                    ocStr = "开";
                    break;
            }
            return ocStr;
        }

        //0运行；1停运
        public static string stringgeRunS(string oc)
        {
            string ocStr = "";
            switch (oc)
            {
                case "0":
                    ocStr = "运行";
                    break;
                case "1":
                    ocStr = "停运";
                    break;
            }
            return ocStr;
        }

        //0断开；1正常
        public static string stringgeOilWayS(string oc)
        {
            string ocStr = "";
            switch (oc)
            {
                case "0":
                    ocStr = "断开";
                    break;
                case "1":
                    ocStr = "正常";
                    break;
            }
            return ocStr;
        }

        //0不加密；1加密
        public static string stringgeMMS(string oc)
        {
            string ocStr = "";
            switch (oc)
            {
                case "0":
                    ocStr = "不加密";
                    break;
                case "1":
                    ocStr = "加密";
                    break;
            }
            return ocStr;
        }

        //0解防；1设防
        public static string stringgeSecort(string oc)
        {
            string ocStr = "";
            switch (oc)
            {
                case "0":
                    ocStr = "解防";
                    break;
                case "1":
                    ocStr = "设防";
                    break;
            }
            return ocStr;
        }

        //1加锁，0解锁
        public static string stringgeDoorLock(string oc)
        {
            string ocStr = "";
            switch (oc)
            {
                case "0":
                    ocStr = "解锁";
                    break;
                case "1":
                    ocStr = "加锁";
                    break;
            }
            return ocStr;
        }

        //0不转；1转
        public static string stringgeZhuan(string oc)
        {
            string ocStr = "";
            switch (oc)
            {
                case "0":
                    ocStr = "不转";
                    break;
                case "1":
                    ocStr = "转";
                    break;
            }
            return ocStr;
        }

        //0未刹车；1刹车
        public static string stringgeBrake(string oc)
        {
            string ocStr = "";
            switch (oc)
            {
                case "0":
                    ocStr = "未刹车";
                    break;
                case "1":
                    ocStr = "刹车";
                    break;
            }
            return ocStr;
        }

        //0正常；1故障
        public static string stringgeNormal(string oc)
        {
            string ocStr = "";
            switch (oc)
            {
                case "0":
                    ocStr = "正常";
                    break;
                case "1":
                    ocStr = "故障";
                    break;
            }
            return ocStr;
        }

        //1单北斗；2单GPS；3双模
        public static string stringgeGpsMod(string oc)
        {
            string ocStr = "";
            switch (oc)
            {
                case "1":
                    ocStr = "单北斗";
                    break;
                case "2":
                    ocStr = "单GPS";
                    break;
                case "3":
                    ocStr = "双模";
                    break;
            }
            return ocStr;
        }

        //0空载；1重载；2半载
        public static string stringgeLoadS(string oc)
        {
            string ocStr = "";
            switch (oc)
            {
                case "0":
                    ocStr = "空载";
                    break;
                case "1":
                    ocStr = "重载";
                    break;
                case "2":
                    ocStr = "半载";
                    break;
            }
            return ocStr;
        }

        //0正常；1短路；2断路；3未知
        public static string stringgeBusS(string oc)
        {
            string ocStr = "";
            switch (oc)
            {
                case "0":
                    ocStr = "正常";
                    break;
                case "1":
                    ocStr = "短路";
                    break;
                case "2":
                    ocStr = "断路";
                    break;
            }
            return ocStr;
        }
        #endregion

        #region resolve Warn
        //0正常,1报警
        public static string warnCharge(string c)
        {
            string warnStr = "";
            switch (c)
            {
                case "0":
                    warnStr = "正常";
                    break;
                case "1":
                    warnStr = "报警";
                    break;
            }
            return warnStr;
        }

        //-1不报警，0出报警，1进报警，2报警
        public static string warnInOutCharge(string c)
        {
            string warnStr = "";
            switch (c)
            {
                case "-1":
                    warnStr = "正常";
                    break;
                case "0":
                    warnStr = "进报警";
                    break;
                case "1":
                    warnStr = "出报警";
                    break;
                case "2":
                    warnStr = "进出报警";
                    break;
            }
            return warnStr;
        }

        //-1不报警，0低油量，1异常上升，2异常下降
        public static string warnOilCharge(string c)
        {
            string warnStr = "";
            switch (c)
            {
                case "-1":
                    warnStr = "正常";
                    break;
                case "0":
                    warnStr = "低油量";
                    break;
                case "1":
                    warnStr = "异常上升";
                    break;
                case "2":
                    warnStr = "异常下降";
                    break;
            }
            return warnStr;
        }
        #endregion

        #region 解析 历史轨迹回放中的 gpsInfo 状态和报警信息
        public static void resolveGpsInfoStatus_old(GPSInfo gpsInfo, string stwaStr)
        {
            string[] statesInfoArr = stwaStr.Split(';');
            gpsInfo.Workstatus = ResolveGpsStatusWarn.stringgeRunS(statesInfoArr[0]);  //运营状态：0运行，1停运   
            gpsInfo.Llsecret = ResolveGpsStatusWarn.stringgeMMS(statesInfoArr[1]);//经纬度加密状态：0不加密，1加密     
            gpsInfo.Gpsmode = ResolveGpsStatusWarn.stringgeGpsMod(statesInfoArr[2]);    //GPS模式：1单北斗，2单GPS，3双模       
            gpsInfo.Oilwaystatus = ResolveGpsStatusWarn.stringgeOilWayS(statesInfoArr[3]);//油路状态：1正常，0断开
            gpsInfo.Vcstatus = ResolveGpsStatusWarn.stringgeOilWayS(statesInfoArr[4]);//车辆电路状态:1正常，0断开
            gpsInfo.Vdstatus = ResolveGpsStatusWarn.stringgeDoorLock(statesInfoArr[5]);//车门状态:1加锁，0解锁
            gpsInfo.Enginestatus = ResolveGpsStatusWarn.stringgeOC(statesInfoArr[6]);//发动机状态：1开，0关
            gpsInfo.Conditionerstatus = ResolveGpsStatusWarn.stringgeOC(statesInfoArr[7]);	//空调状态：1开，0关
            gpsInfo.Brakestatus = ResolveGpsStatusWarn.stringgeBrake(statesInfoArr[8]);//刹车状态：1刹车，0未刹车
            gpsInfo.Ltstatus = ResolveGpsStatusWarn.stringgeOC(statesInfoArr[9]);//左转向状态：1开，0关
            gpsInfo.Rtstatus = ResolveGpsStatusWarn.stringgeOC(statesInfoArr[10]);	//右转向状态：1开，0关
            gpsInfo.Farlstatus = ResolveGpsStatusWarn.stringgeOC(statesInfoArr[11]);//远光灯状态：1开，0关
            gpsInfo.Nearlstatus = ResolveGpsStatusWarn.stringgeOC(statesInfoArr[12]);//近光灯状态：1开，0关
            gpsInfo.Pnstatus = ResolveGpsStatusWarn.stringgeZhuan(statesInfoArr[13]);//正反转状态：1转，0不转
            gpsInfo.Shakestatus = ResolveGpsStatusWarn.stringgeOC(statesInfoArr[14]);//震动状态：1开，0关
            gpsInfo.Hornstatus = ResolveGpsStatusWarn.stringgeOC(statesInfoArr[15]);//喇叭状态：1开，0关
            gpsInfo.Protectstatus = ResolveGpsStatusWarn.stringgeSecort(statesInfoArr[16]);//安全状态：1设防，0解防
            gpsInfo.Loadstatus = ResolveGpsStatusWarn.stringgeLoadS(statesInfoArr[17]);//负载状态：1重载，0空载
            gpsInfo.Busstatus = ResolveGpsStatusWarn.stringgeNormal(statesInfoArr[18]);//总线状态:0正常，1故障
            gpsInfo.Gsmstatus = ResolveGpsStatusWarn.stringgeNormal(statesInfoArr[19]);//GSM模块状态，0正常，1故障
            gpsInfo.Gpsstatus = ResolveGpsStatusWarn.stringgeNormal(statesInfoArr[20]);//GPS模块状态，0正常，1故障           
            gpsInfo.Lcstatus = ResolveGpsStatusWarn.stringgeNormal(statesInfoArr[21]);//锁车电路状态，0正常，1故障
            gpsInfo.Ffstatus = ResolveGpsStatusWarn.stringgeOC(statesInfoArr[22]);//前雾灯状态:1开，0关
            gpsInfo.Bfstatus = ResolveGpsStatusWarn.stringgeOC(statesInfoArr[23]);//后雾灯状态：1开，0关
            gpsInfo.Gpsantstatus = ResolveGpsStatusWarn.stringgeBusS(statesInfoArr[24]);//GPS天线状态：（0正常，1短路，2断路,3未知）          
            gpsInfo.Fdstatus = ResolveGpsStatusWarn.stringgeOC(statesInfoArr[25]);//前车门状态：1开，0关
            //gpsInfo.Mdstatus = ResolveGpsStatusWarn.stringgeOC(statesInfoArr[26]);//中车门状态
            gpsInfo.Bdstatus = ResolveGpsStatusWarn.stringgeOC(statesInfoArr[27]);//后车门状态：1开，0关
            //gpsInfo.Driveseatdoor = ResolveGpsStatusWarn.stringgeOC(statesInfoArr[28]); //驾驶席门状态，1开，0关
            //gpsInfo.Zdydoor = ResolveGpsStatusWarn.stringgeOC(statesInfoArr[29]);//（自定义）门状态，1开，0关
            //gpsInfo.Isusergps = stringgeGpsUseMod(statesInfoArr[30]);//是否使用GPS卫星定位，0没有，1使用
            //gpsInfo.Isbdgps = stringgeGpsUseMod(statesInfoArr[31]);//是否使用北斗卫星定位，0没有，1使用
            //gpsInfo.Isglonassgps = stringgeGpsUseMod(statesInfoArr[32]);//是否使用GLONASS卫星定位，0没有，1使用
            //gpsInfo.Isgalileogps = stringgeGpsUseMod(statesInfoArr[33]);//是否使用Galileo卫星定位，0没有，1使用          
        }

        public static void resolveGpsInfoStatus(GPSInfo gpsInfo, string StatusSign)
        {
            // string StatusSign = content["StatusSign"] == null ? "" : content["StatusSign"].ToString();
            if (StatusSign != null)// && StatusSign.Length == 48)
            {
                gpsInfo.Accstatus = StatusSign[0] == 3 ? "" : StatusSign[0] == 0 ? "关" : "开";//ACC状态
                // gpsInfo.Accstatus = StatusSign[1] == 0 ? "关" : "开";//定位信息，0未定位，1定位
                gpsInfo.Workstatus = StatusSign[1] == 3 ? "" : StatusSign[2] == 0 ? "停运" : "运行";//运营状态，0停运，1运营
                gpsInfo.Llsecret = StatusSign[3] == 3 ? "" : StatusSign[3] == 0 ? "未加密" : "加密";//使用加密插件状态，0未加密，1加密
                gpsInfo.Oilwaystatus = StatusSign[4] == 3 ? "" : StatusSign[4] == 0 ? "断开" : "正常";//车辆油路状态，0断开，1正常
                gpsInfo.Vcstatus = StatusSign[5] == 3 ? "" : StatusSign[5] == 0 ? "断开" : "正常";//车辆电路状态，0断开，1正常
                gpsInfo.Vdstatus = StatusSign[6] == 3 ? "" : StatusSign[6] == 0 ? "解锁" : "加锁";//车门加锁状态，0解锁，1加锁
                gpsInfo.Brakestatus = StatusSign[7] == 3 ? "" : StatusSign[7] == 0 ? "松开" : "踩下";//离合器状态，0松开，1踩下
                switch (StatusSign[8])
                {
                    case '0':
                        gpsInfo.Loadstatus = "空车";
                        break;
                    case '1':
                        gpsInfo.Loadstatus = "半载";
                        break;
                    case '2':
                        gpsInfo.Loadstatus = "满载";
                        break;
                    default:
                        gpsInfo.Loadstatus = "";
                        break;
                }//车辆载重状态，0空车，1半载，2满载
                gpsInfo.Fdstatus = StatusSign[9] == 3 ? "" : StatusSign[9] == 0 ? "关" : "开";//前门开关状态，0关1开
                //gpsInfo.Accstatus = StatusSign[10] == 0 ? "关" : "开";//中门开关状态，0关1开
                gpsInfo.Bdstatus = StatusSign[11] == 3 ? "" : StatusSign[11] == 0 ? "关" : "开";//后门开关状态，0关1开
                //gpsInfo.Accstatus = StatusSign[12] == 0 ? "关" : "开";//驾驶席门开关状态，0关1开
                //gpsInfo.Accstatus = StatusSign[13] == 0 ? "关" : "开";//自定义门开关状态，0关1开
                gpsInfo.Gpsmode = "";
                if (StatusSign[14] == 1)//是否使用GPS卫星定位，0未使用1使用
                    gpsInfo.Gpsmode += "GPS卫星定位;";
                if (StatusSign[15] == 1)//是否使用北斗卫星定位，0未使用1使用
                    gpsInfo.Gpsmode += "北斗卫星定位;";
                if (StatusSign[16] == 1)//是否使用Glonass卫星定位，0未使用1使用
                    gpsInfo.Gpsmode += "Glonass卫星定位;";
                if (StatusSign[17] == 1)//是否使用Galileo卫星定位，0未使用1使用
                    gpsInfo.Gpsmode += "Galileo卫星定位";


                //gpsInfo.Accstatus = StatusSign[18] == 0 ? "关" : "开";//空挡信号，0非空挡，1空挡
                gpsInfo.Conditionerstatus = StatusSign[19] == 3 ? "" : (StatusSign[19] == 0 ? "关" : "开");//空调状态，1开，0关
                gpsInfo.Busstatus = StatusSign[20] == 3 ? "" : StatusSign[20] == 0 ? "无故障" : "故障";//总线故障，0无故障，1故障
                gpsInfo.Gsmstatus = StatusSign[21] == 3 ? "" : StatusSign[21] == 0 ? "无故障" : "故障";//GSM模块故障，0无故障，1故障
                gpsInfo.Gpsantstatus = StatusSign[22] == 3 ? "" : StatusSign[22] == 0 ? "无故障" : "故障";//GPS模块故障，0无故障，1故障
                gpsInfo.Lcstatus = StatusSign[23] == 3 ? "" : StatusSign[23] == 0 ? "无故障" : "故障";//锁车电路故障，0无故障，1故障
                //gpsInfo.Accstatus = StatusSign[24] == 0 ? "关" : "开";//车门状态，0关，1开
                //gpsInfo.Accstatus = StatusSign[25] == 0 ? "关" : "开";//私密状态，0公开，1私密
                switch (StatusSign[26])
                {
                    case '0':
                        gpsInfo.Gpsantstatus = "正常";
                        break;
                    case '1':
                        gpsInfo.Gpsantstatus = "断路";
                        break;
                    case '2':
                        gpsInfo.Gpsantstatus = "短路";
                        break;
                    case '3':
                        gpsInfo.Gpsantstatus = "未知";
                        break;
                    default:
                        gpsInfo.Gpsantstatus = "";
                        break;
                }//GPS天线状态，0正常，1天线断路，2天线短路， 3天线未知状态
                //gpsInfo.Accstatus = StatusSign[27] == 0 ? "关" : "开";//刹车灯状态，0关1开
                gpsInfo.Ltstatus = StatusSign[28] == 3 ? "" : StatusSign[28] == 0 ? "关" : "开";//左转灯状态，0关1开
                gpsInfo.Rtstatus = StatusSign[29] == 3 ? "" : StatusSign[29] == 0 ? "关" : "开";//右转灯状态，0关1开
                gpsInfo.Farlstatus = StatusSign[30] == 3 ? "" : StatusSign[30] == 0 ? "关" : "开";//远光灯状态，0关1开
                gpsInfo.Nearlstatus = StatusSign[31] == 3 ? "" : StatusSign[31] == 0 ? "关" : "开";//近光灯状态，0关1开
                gpsInfo.Ffstatus = StatusSign[32] == 3 ? "" : StatusSign[32] == 0 ? "关" : "开";//前雾灯状态，0关1开
                gpsInfo.Bfstatus = StatusSign[33] == 3 ? "" : StatusSign[33] == 0 ? "关" : "开";//后雾灯状态，0关1开
                //gpsInfo.Accstatus = StatusSign[34] == 0 ? "关" : "开";//倒车灯状态，0关1开
                gpsInfo.Hornstatus = StatusSign[35] == 3 ? "" : StatusSign[35] == 0 ? "关" : "鸣";//喇叭状态，0关1鸣
                //gpsInfo.Accstatus = StatusSign[36] == 0 ? "关" : "开";//示廓灯状态，1开，0关
                //gpsInfo.Accstatus = StatusSign[37] == 0 ? "关" : "开";//缓冲器状态，1工作，0未工作
                //gpsInfo.Accstatus = StatusSign[38] == 0 ? "关" : "开";//ABS状态，1工作，0未工作
                //gpsInfo.Accstatus = StatusSign[39] == 0 ? "关" : "开";//加热器状态，1工作，0未工作
                //                    39	加热器状态，1工作，0未工作
                //40	定位类型，1单北斗，1单GPS，2双模
                //41	正反转状态，0不转，1正转，2反转
                //42	外部蜂鸣器状态，0无效，1有效
                //43	震动状态，0未震动，1震动
                //44	设防状态，0解防，1设防
                //45	发动机状态，0关，1开
            }
        }

        public static void resolveGpsInfoWarn_old(GPSInfo gpsInfo, string warnInfoStr)
        {
            string[] warnInfoArr = warnInfoStr.Split(';');
            gpsInfo.Soswarn = ResolveGpsStatusWarn.warnCharge(warnInfoArr[0]);//紧急报警，触动报警开关后触发
            gpsInfo.Overspeedwarn = ResolveGpsStatusWarn.warnCharge(warnInfoArr[1]);//超速报警
            gpsInfo.Tiredwarn = ResolveGpsStatusWarn.warnCharge(warnInfoArr[2]);//疲劳驾驶
            gpsInfo.Prewarn = ResolveGpsStatusWarn.warnCharge(warnInfoArr[3]);//预警
            gpsInfo.Gnssfatal = ResolveGpsStatusWarn.warnCharge(warnInfoArr[4]);//GNSS模块故障
            gpsInfo.Gnssantwarn = ResolveGpsStatusWarn.warnCharge(warnInfoArr[5]);//GNSS天线未接或被剪断
            gpsInfo.Lowvolwarn = ResolveGpsStatusWarn.warnCharge(warnInfoArr[6]);//终端主电源欠压
            gpsInfo.Highvolwarn = ResolveGpsStatusWarn.warnCharge(warnInfoArr[7]);//终端主电源高压
            gpsInfo.Outagewarn = ResolveGpsStatusWarn.warnCharge(warnInfoArr[8]);//终端主电源断电
            gpsInfo.Lcdfatalwarn = ResolveGpsStatusWarn.warnCharge(warnInfoArr[9]);//终端LCD或者显示器故障
            gpsInfo.Ttsfatalwarn = ResolveGpsStatusWarn.warnCharge(warnInfoArr[10]);//TTS模块故障
            gpsInfo.Camerafatalwarn = ResolveGpsStatusWarn.warnCharge(warnInfoArr[11]);//摄像头故障
            gpsInfo.Vediolosewarn = ResolveGpsStatusWarn.warnCharge(warnInfoArr[12]);//视频丢失报警
            gpsInfo.Accumtimeout = ResolveGpsStatusWarn.warnCharge(warnInfoArr[13]);//当天累计驾驶超时
            gpsInfo.Stoptimeout = ResolveGpsStatusWarn.warnCharge(warnInfoArr[14]);//超时停车
            gpsInfo.Inoutareawarn = ResolveGpsStatusWarn.warnInOutCharge(warnInfoArr[15]);//进出区域报警（-1不报警，0出报警，1进报警，2报警）
            gpsInfo.Inoutlinewarn = ResolveGpsStatusWarn.warnInOutCharge(warnInfoArr[16]);//进出路段报警（-1不报警，0出报警，1进报警，2报警）
            gpsInfo.Drivingtimewarn = ResolveGpsStatusWarn.warnCharge(warnInfoArr[17]);//路段行驶时间不足/过长报警
            gpsInfo.Deviatewarn = ResolveGpsStatusWarn.warnCharge(warnInfoArr[18]);//路线偏离报警
            gpsInfo.Vssfatalwarn = ResolveGpsStatusWarn.warnCharge(warnInfoArr[19]);//车辆VSS故障
            gpsInfo.Oilexceptionwarn = ResolveGpsStatusWarn.warnOilCharge(warnInfoArr[20]);//车辆油量异常报警（-1不报警，0低油量，1异常上升，2异常下降）
            gpsInfo.Vehiclestolenwarn = ResolveGpsStatusWarn.warnCharge(warnInfoArr[21]);//车辆被盗报警
            gpsInfo.Illignitewarn = ResolveGpsStatusWarn.warnCharge(warnInfoArr[22]);//非法点火报警
            gpsInfo.Illmovewarn = ResolveGpsStatusWarn.warnCharge(warnInfoArr[23]);//非法位移报警
            gpsInfo.Crashwarn = ResolveGpsStatusWarn.warnCharge(warnInfoArr[24]);//碰撞侧翻报警
            gpsInfo.Sdexceptionwarn = ResolveGpsStatusWarn.warnCharge(warnInfoArr[25]);//SD卡异常报警
            gpsInfo.Robwarn = ResolveGpsStatusWarn.warnCharge(warnInfoArr[26]);//劫警
            gpsInfo.Sleeptimewarn = ResolveGpsStatusWarn.warnCharge(warnInfoArr[27]);//司机停车休息时间不足报警
            gpsInfo.Illtimedrivingwarn = ResolveGpsStatusWarn.warnCharge(warnInfoArr[28]);//非法时段行驶报警
            gpsInfo.Overstationwarn = ResolveGpsStatusWarn.warnCharge(warnInfoArr[29]);//越站报警
            gpsInfo.Ilopendoorwarn = ResolveGpsStatusWarn.warnCharge(warnInfoArr[30]);//非法开车门报警
            gpsInfo.Protectwarn = ResolveGpsStatusWarn.warnCharge(warnInfoArr[31]);//设防报警
            gpsInfo.Trimmingwarn = ResolveGpsStatusWarn.warnCharge(warnInfoArr[32]);//剪线报警
            gpsInfo.Passwdwarn = ResolveGpsStatusWarn.warnCharge(warnInfoArr[33]);//密码错误报警
            gpsInfo.Prohibitmovewarn = ResolveGpsStatusWarn.warnCharge(warnInfoArr[34]);//禁行报警
            gpsInfo.Illstopwarn = ResolveGpsStatusWarn.warnCharge(warnInfoArr[35]);//非法停车报警
            gpsInfo.Gnssshortwarn = ResolveGpsStatusWarn.warnCharge(warnInfoArr[36]);//GNSS天线短路
            gpsInfo.Vedioshelterwarn = ResolveGpsStatusWarn.warnCharge(warnInfoArr[37]);//视频遮挡报警
            //gpsInfo.Icfatalwarn = ResolveGpsStatusWarn.warnCharge(warnInfoArr[38]);//道路运输证IC卡模块故障
            //gpsInfo.Overspeedprewarn = ResolveGpsStatusWarn.warnCharge(warnInfoArr[39]);//超速预警
            //gpsInfo.Crashprewarn = ResolveGpsStatusWarn.warnCharge(warnInfoArr[40]);//碰撞预警
            //gpsInfo.Patiguedprewarn = ResolveGpsStatusWarn.warnCharge(warnInfoArr[41]);//疲劳驾驶预警
            //gpsInfo.Rolloverprewarn = ResolveGpsStatusWarn.warnCharge(warnInfoArr[42]);//侧翻预警
        }

        public static void resolveGpsInfoWarn(GPSInfo gpsInfo, string WarnSign)
        { 
            if (WarnSign != null)// && WarnSign.Length == 43)
            {
                gpsInfo.Soswarn = WarnSign[0] == '0' ? "正常" : "报警";//1:紧急报警触动报警开关后触发, 收到应答后清零
                gpsInfo.Overspeedwarn = WarnSign[1] == '0' ? "正常" : "报警";//1：超速报警, 标志维持至报警条件解除
                gpsInfo.Tiredwarn = WarnSign[2] == '0' ? "正常" : "报警";//1：疲劳驾驶, 标志维持至报警条件解除
                gpsInfo.Prewarn = WarnSign[3] == '0' ? "正常" : "报警";//1：危险预警, 收到应答后清零
                gpsInfo.Gnssfatal = WarnSign[4] == '0' ? "正常" : "报警";//1：GNSS模块发生故障, 标志维持至报警条件解除
                gpsInfo.Gnssantwarn = WarnSign[5] == '0' ? "正常" : "报警";//1：GNSS天线未接或被剪断, 标志维持至报警条件解除
                gpsInfo.Gnssshortwarn = WarnSign[6] == '0' ? "正常" : "报警";//1：GNSS天线短路, 标志维持至报警条件解除
                gpsInfo.Lowvolwarn = WarnSign[7] == '0' ? "正常" : "报警";//1：终端主电源欠压, 标志维持至报警条件解除
                gpsInfo.Outagewarn = WarnSign[8] == '0' ? "正常" : "报警";//1：终端主电源掉电, 标志维持至报警条件解除
                gpsInfo.Lcdfatalwarn = WarnSign[9] == '0' ? "正常" : "报警";//1：终端LCD或显示器故障, 标志维持至报警条件解除
                gpsInfo.Ttsfatalwarn = WarnSign[10] == '0' ? "正常" : "报警";//1：TTS模块故障, 标志维持至报警条件解除
                gpsInfo.Camerafatalwarn = WarnSign[11] == '0' ? "正常" : "报警";//1:摄像头故障, 标志维持至报警条件解除
                gpsInfo.Accumtimeout = WarnSign[12] == '0' ? "正常" : "报警";//1:当天累计驾驶超时, 标志维持至报警条件解除
                gpsInfo.Stoptimeout = WarnSign[13] == '0' ? "正常" : "报警";//1：超时停车, 标志维持至报警条件解除
                gpsInfo.Inoutareawarn = WarnSign[14] == '0' ? "正常" : "报警";//1：进出区域, 收到应答后清零///////////////////
                gpsInfo.Inoutlinewarn = WarnSign[15] == '0'? "正常" : "报警";//1:进出路线, 收到应答后清零//////////////////////
                gpsInfo.Drivingtimewarn = WarnSign[16] == '0' ? "正常" : "报警";//1:路段行驶时间不足/过长, 收到应答后清零
                gpsInfo.Deviatewarn = WarnSign[17] == '0' ? "正常" : "报警";//1:路线偏离报警, 标志维持至报警条件解除
                gpsInfo.Vssfatalwarn = WarnSign[18] == '0' ? "正常" : "报警";//1：车辆VSS故障, 标志维持至报警条件解除
                gpsInfo.Oilexceptionwarn = WarnSign[19] == '0' ? "正常" : "报警";//1：车辆油量异常, 标志维持至报警条件解除
                gpsInfo.Vehiclestolenwarn = WarnSign[20] == '0' ? "正常" : "报警";//1：车辆被盗(通过车辆防盗器) , 标志维持至报警条件解除
                gpsInfo.Illignitewarn = WarnSign[21] == '0' ? "正常" : "报警";//1：车辆非法点火, 收到应答后清零
                gpsInfo.Illmovewarn = WarnSign[22] == '0' ? "正常" : "报警";//1：车辆非法位移, 收到应答后清零
                gpsInfo.Outagewarn = WarnSign[23] == '0' ? "正常" : "报警";//1：终端主电源高压，标志维持至报警条件解除
                gpsInfo.ICCardwarn = WarnSign[24] == '0' ? "正常" : "报警";//1：道路运输证IC卡模块故障，标志维持至报警条件解除
                gpsInfo.Overspeedprewarn = WarnSign[25] == '0' ? "正常" : "报警";//1：超速预警，标志维持至报警条件解除
                gpsInfo.Tiredprewarn = WarnSign[26] == '0' ? "正常" : "报警";//1：疲劳驾驶预警，标志维持至报警条件解除
                gpsInfo.Crashwarn = WarnSign[27] == '0' ? "正常" : "报警";//1：碰撞预警，标志维持至报警条件解除
                gpsInfo.Rolloverwarn = WarnSign[28] == '0' ? "正常" : "报警";//1：侧翻预警，标志维持至报警条件解除
                gpsInfo.Ilopendoorwarn = WarnSign[29] == '0' ? "正常" : "报警";//1：非法开门报警（终端未设置区域时，不判断非法开门），收到应答后清零
                gpsInfo.Vediolosewarn = WarnSign[30] == '0' ? "正常" : "报警";//1：视频丢失报警，标志维持至报警条件解除
                gpsInfo.Vedioshelterwarn = WarnSign[31] == '0' ? "正常" : "报警";//1：视频遮挡报警，标志维持至报警条件解除
                gpsInfo.Robwarn = WarnSign[32] == '0' ? "正常" : "报警";//1：劫警
                gpsInfo.Illtimedrivingwarn = WarnSign[33] == '0' ? "正常" : "报警";//1：非法时段行驶报警
                gpsInfo.Sleeptimewarn = WarnSign[34] == '0' ? "正常" : "报警";//1：停车休息时间不足报警
                gpsInfo.Overstationwarn = WarnSign[35] == '0' ? "正常" : "报警";//1：越站报警
                gpsInfo.Protectwarn = WarnSign[36] == '0' ? "正常" : "报警";//1：设防
                gpsInfo.Trimmingwarn = WarnSign[37] == '0' ? "正常" : "报警";//1：剪线报警
                gpsInfo.Lowbatterywarn = WarnSign[38] == '0' ? "正常" : "报警";//1：电瓶电压低报警
                gpsInfo.Passwdwarn = WarnSign[39] == '0' ? "正常" : "报警";//1：密码错误报警
                gpsInfo.Prohibitmovewarn = WarnSign[40] == '0' ? "正常" : "报警";//1：禁行报警
                gpsInfo.Illstopwarn = WarnSign[41] == '0' ? "正常" : "报警";//1：非法停车报警
                gpsInfo.Sdexceptionwarn = WarnSign[42] == '0' ? "正常" : "报警";//1：SD卡异常

            }
        }
        #endregion
    }
}