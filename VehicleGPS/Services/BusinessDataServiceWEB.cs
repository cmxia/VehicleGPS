
﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VehicleGPS.Models;
using System.Xml;
using System.IO;
using VehicleGPS.Models.Login;
using System.Net;
using System.Threading;
using VehicleGPS.ViewModels.MonitorCentre;
using System.Windows;
using VehicleGPS.ViewModels.MonitorCentre.TrackPlayBack;
using VehicleGPS.ViewModels.MonitorCentre.RealTimeMonitor;
using VehicleGPS.ViewModels.MonitorCentre.VehicleTrack;
using VehicleGPS.ViewModels.DispatchCentre.VehicleDispatch;
using VehicleGPS.Models.MonitorCentre;
using GpsNET;
using System.Data;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Windows.Threading;
using ZeroMQ;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using VehicleGPS.zmq;
using WpfApplication1.zmq;
using System.Globalization;
using System.Media;
using VehicleGPS.Views.Control.DispatchCentre.VehicleDispatch;
using VehicleGPS.ViewModels.DispatchCentre;

namespace VehicleGPS.Services
{
    class BusinessDataServiceWEB : IBusinessDataServiceWEB
    {

        private int RealTimeParseThreadNum = 20;
        private int RealTimeAllCount = 0;
        private object RealTimeCounterMonitor = new object();//地理和坐标解析计数互斥量
        private int RealTimeCounter = 0;//地理和坐标解析计数器

        ZmqContext testSubZmqContext;
        ZmqSocket testSubZmqSocket;
        //private string TestPubSubPoint = "tcp://61.183.9.107:4011";//发布版5554
        private string TestPubSubPoint = "tcp://" + VehicleConfig.GetInstance().BUSINESSIP + ":" + VehicleConfig.GetInstance().BUSINESSPORT;//发布版5554
        public System.Threading.Thread TestPubSubThread;
        public bool PushThreadRunning = false;

        ZmqContext WebGSZmqContext;
        ZmqSocket WebGSZmqSocket;
        private string TestWebGSPoint = "tcp://" + VehicleConfig.GetInstance().INSTRUCTIONIP + ":" + VehicleConfig.GetInstance().INSTRUCTIONPORT;//发布版5553
        System.Threading.Thread WebGSThread;

        System.Threading.Thread ss;
        MessageHander mh;
        public BusinessDataServiceWEB()
        {
            testSubZmqContext = ZmqContext.Create();
            testSubZmqSocket = testSubZmqContext.CreateSocket(SocketType.SUB);
            ss = new Thread(initMsgHander);

            WebGSZmqContext = ZmqContext.Create();
            WebGSZmqSocket = testSubZmqContext.CreateSocket(SocketType.REQ);
            ss.IsBackground = true;
            ss.Start();

        }
        //用于定义匿名对象
        public BusinessDataServiceWEB(int i)
        {

        }
        void initMsgHander()
        {
            mh = new MessageHander();
            //mh.SublishToPub += msgPub_Publish;
            mh.startListen();
        }
        void msgPub_Publish(object sender, PubEventArgs e)
        {
            string simStr = e.sim;
            string typeStr = e.type;
            string typeS = e.body;
            //
        }

        #region 获取实时GPS信息 列表
        private void Register()
        {
            //StaticTreeState.VehicleGPSInfo = LoadingState.LOADCOMPLETE;//车辆详细GPS信息加载完成
            using (var randCTX = ZmqContext.Create())
            {
                using (var randSockt = randCTX.CreateSocket(SocketType.REQ))
                {
                    randSockt.Connect(WpfApplication1.zmq.GlobalConfig.ReqRepRemoutPoint);
                    MsgHB mhb = new MsgHB();
                    mhb.GType = "Register";
                    mhb.GpsBasic = JsonConvert.SerializeObject(Subscriber.heartPack()).ToString();
                    mhb.GpsAttatch = "";
                    string tag = JsonConvert.SerializeObject(mhb);
                    ZmqMessage msg = new ZmqMessage();
                    msg.Append(Encoding.UTF8.GetBytes(tag));
                    randSockt.SendMessage(msg);
                    if (Monitor.TryEnter(StaticTreeState.VehicleGPSInfoMutex, 10000))
                    {
                        int sleepTimes = 0;
                        while (StaticTreeState.VehicleAllBasicInfo != LoadingState.LOADCOMPLETE
                               && StaticTreeState.VehicleAllBasicInfo != LoadingState.LOADDINGFAIL)
                        {//车辆的GPS信息依赖于车辆基础信息（拼接）
                            Thread.Sleep(500);
                            if (++sleepTimes == 100)
                            {//获取车辆的全部基础信息失败

                                StaticTreeState.VehicleGPSInfo = LoadingState.LOADDINGFAIL;//车辆详细GPS信息加载失败
                                return;
                            }
                        }
                        try
                        {
                            string mBody = randSockt.Receive(Encoding.UTF8);
                            if (StaticTreeState.VehicleGPSInfo != LoadingState.LOADCOMPLETE)
                            {
                                if (JsonDeserializeLastestGpsInfo(mBody))
                                    StaticTreeState.VehicleGPSInfo = LoadingState.LOADCOMPLETE;//车辆详细GPS信息加载完成
                                else
                                    StaticTreeState.VehicleGPSInfo = LoadingState.LOADDINGFAIL;//车辆详细GPS信息加载失败
                            }
                        }
                        catch
                        {
                            StaticTreeState.VehicleGPSInfo = LoadingState.LOADDINGFAIL;//车辆详细GPS信息加载失败
                        }
                        finally
                        {
                            Monitor.Exit(StaticTreeState.VehicleGPSInfoMutex);
                        }
                    }
                    else
                    {
                        return;
                    }

                }
            }

        }
        /// <summary>
        /// 获取最新的GPS数据
        /// </summary>
        public void GetLatestVehicleGPSInfoThread()
        {
            Thread nThread = new Thread(delegate() { this.Register(); });
            nThread.Start();
        }
        /// <summary>
        /// 客户端登陆心跳包
        /// </summary>
        public void ClientLogin()
        {
            //WebGSZmqContext = ZmqContext.Create();
            //WebGSZmqSocket = WebGSZmqContext.CreateSocket(SocketType.REQ);
            //WebGSZmqSocket.Connect(TestWebGSPoint);
            //WebGSZmqSocket.Identity = Encoding.UTF8.GetBytes("testSendTag1");
            //WebGSThread = new Thread(delegate() { this.ClientLoginHeart(); });
            //WebGSThread.Start();

        }
        public void ClientLoginHeart()
        {
            MessageToServiceType mhb = new MessageToServiceType();
            mhb.GType = "Heart";
            mhb.GpsBasic = JsonConvert.SerializeObject(Subscriber.heartPack()).ToString();
            mhb.GpsAttatch = "";
            string callBack = "";
            string tag = JsonConvert.SerializeObject(mhb);
            ZmqMessage msg = new ZmqMessage();
            msg.Append(Encoding.UTF8.GetBytes(tag));
            while (true)
            {

                WebGSZmqSocket.SendMessage(msg);
                callBack = WebGSZmqSocket.Receive(Encoding.UTF8);
                Thread.Sleep(100000);
            }
        }
        public void GetLatestVehicleGPSInfoThread_old()
        {
            Thread nThread = new Thread(delegate() { this.GetLatestVehicleGPSInfo(); });
            nThread.Start();
        }
        /// <summary>
        /// 获取最新的GPS数据（推送（弃））
        /// </summary>
        /// <param name="isRefresh"></param>
        public void GetLatestVehicleGPSInfo(bool isRefresh = false)
        {

            if (Monitor.TryEnter(StaticTreeState.VehicleGPSInfoMutex, 10000))
            {
                try
                {
                    if (!isRefresh)
                    {//如果是第一次加载数据，需要阻塞树形节点的勾选
                        StaticTreeState.VehicleGPSInfo = LoadingState.LOADING;//正在加载车辆详细GPS信息
                    }
                    StaticLoginInfo userInfo = StaticLoginInfo.GetInstance();

                    int sleepTimes = 0;
                    while (StaticTreeState.VehicleAllBasicInfo != LoadingState.LOADCOMPLETE
                        && StaticTreeState.VehicleAllBasicInfo != LoadingState.LOADDINGFAIL)
                    {//车辆的GPS信息依赖于车辆基础信息（拼接）
                        Thread.Sleep(500);
                        if (++sleepTimes == 10)
                        {//获取车辆的全部基础信息失败
                            if (!isRefresh)
                            {
                                StaticTreeState.VehicleGPSInfo = LoadingState.LOADDINGFAIL;//车辆详细GPS信息加载失败
                            }
                            return;
                        }
                    }
                    if (StaticTreeState.VehicleAllBasicInfo == LoadingState.LOADDINGFAIL)
                    {
                        return;
                    }
                    StaticDetailInfo detailInfo = StaticDetailInfo.GetInstance();
                    //foreach (CVDetailInfo info in detailInfo.ListVehicleDetailInfo)
                    //{
                    //    testSubZmqSocket.Subscribe(Encoding.UTF8.GetBytes(info.SIM));
                    //}
                    testSubZmqSocket.SubscribeAll();
                    try
                    {
                        string message = testSubZmqSocket.Receive(Encoding.UTF8);
                        string hb = testSubZmqSocket.Receive(Encoding.UTF8);
                        if (message != "")
                        {
                            if (hb != null && hb != "")
                            {
                                JsonDeserializeLastestGpsInfo(hb);
                            }
                        }
                    }
                    catch (Exception)
                    {
                        StaticTreeState.VehicleGPSInfo = LoadingState.LOADDINGFAIL;
                        return;
                    }
                    System.Threading.Thread.Sleep(50);


                    if (!isRefresh)
                    {
                        StaticTreeState.VehicleGPSInfo = LoadingState.LOADCOMPLETE;//车辆详细GPS信息加载完成
                    }

                }
                catch (Exception e)
                {
                    if (!isRefresh)
                    {
                        StaticTreeState.VehicleGPSInfo = LoadingState.LOADDINGFAIL;//车辆详细GPS信息加载失败
                    }
                }
                finally
                {
                    Monitor.Exit(StaticTreeState.VehicleGPSInfoMutex);
                }
            }
            else
            {
                return;
            }
        }
        /// <summary>
        /// 刷新调度条
        /// </summary>
        /// <param name="sim"></param>
        private void RefreshDispatchInfo(object sim)
        {
            string Sim = (string)sim;
            if (StaticTreeState.DispatchCenter != LoadingState.LOADCOMPLETE)
            {
                return;
            }
            foreach (CVDetailInfo vehicle in StaticDetailInfo.GetInstance().ListVehicleDetailInfo)
            {
                if (vehicle.SIM.Equals(Sim) && vehicle.VehicleState.Equals("1"))
                {//车辆有任务
                    VehicleDispatchViewModel instance = VehicleDispatchViewModel.GetInstance();
                    foreach (VehicleDispatchItemViewModel dispatchItem in instance.ListDispatchInfoCurrentPage)
                    {
                        if (dispatchItem.ListDispatchPoint != null && dispatchItem.ListDispatchPoint.Count > 0)
                        {

                            foreach (DispatchPointViewModel item in dispatchItem.ListDispatchPoint)
                            {
                                if (item.Sim.Equals(Sim))
                                {
                                    VehicleDispatchViewModel.GetInstance().InitDispatchInfo();
                                    break;
                                }
                            }
                        }
                        if (dispatchItem.ListInRegionPoint != null && dispatchItem.ListInRegionPoint.Count > 0)
                        {
                            foreach (DispatchPointViewModel item in dispatchItem.ListInRegionPoint)
                            {
                                if (item.Sim.Equals(Sim))
                                {
                                    VehicleDispatchViewModel.GetInstance().InitDispatchInfo();
                                    break;
                                }
                            }
                        }
                        if (dispatchItem.ListInSitePoint != null && dispatchItem.ListInSitePoint.Count > 0)
                        {
                            foreach (DispatchPointViewModel item in dispatchItem.ListInSitePoint)
                            {
                                if (item.Sim.Equals(Sim))
                                {
                                    VehicleDispatchViewModel.GetInstance().InitDispatchInfo();
                                    break;
                                }
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 将json格式的GPS数据解析成GPSInfo类型
        /// 2014-6-16
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        private GPSInfo DeserializeLastestGpsInfoToGPSInfo(string json)
        {
            GPSInfo gpsInfo = new GPSInfo();
            if (json == "")
                return null;

            JObject content = (JObject)JsonConvert.DeserializeObject(json);
            gpsInfo.Sim = content["SimId"] == null ? "" : content["SimId"].ToString();
            //刷新调度条
            //if (!string.IsNullOrEmpty(gpsInfo.Sim))
            //{
            //    Thread th = new Thread(new ParameterizedThreadStart(this.RefreshDispatchInfo));
            //    th.Start(gpsInfo.Sim);
            //}
            gpsInfo.Altitude = content["Altitude"] == null ? "" : content["Altitude"].ToString();
            gpsInfo.DevSpeed = content["DevSpeed"] == null ? "" : content["DevSpeed"].ToString();
            double speed = 0;
            bool result = double.TryParse(gpsInfo.DevSpeed, out speed);
            if (!result)
            {
                gpsInfo.DevSpeed = "";
            }
            else
            {
                if (speed < 0)
                {
                    gpsInfo.DevSpeed = "";
                }
                else
                {
                    gpsInfo.DevSpeed = Math.Round(speed, 2) + "";
                }
            }
            gpsInfo.Direction = content["Direction"] == null ? "" : translateDirection(content["Direction"].ToString());
            gpsInfo.GPSMileage = content["Distance"] == null ? "" : content["Distance"].ToString();
            result = double.TryParse(gpsInfo.GPSMileage, out speed);

            if (!result)
            {
                gpsInfo.GPSMileage = "";
            }
            else
            {
                if (speed < 0)
                {
                    gpsInfo.GPSMileage = "";
                }
                else
                {
                    gpsInfo.GPSMileage = Math.Round(speed, 2) + "";
                }
            }
            gpsInfo.Latitude = content["Nlatitude"] == null ? "" : content["Nlatitude"].ToString();
            gpsInfo.Longitude = content["Nlongitude"] == null ? "" : content["Nlongitude"].ToString();

            string oil = content["AttachOil"] == null ? "" : content["AttachOil"].ToString();
            gpsInfo.OilVolumn = content["AttachOil"] == null ? "" : content["AttachOil"].ToString();
            result = double.TryParse(gpsInfo.OilVolumn, out speed);
            if (!result)
            {
                gpsInfo.OilVolumn = "";
            }
            else
            {
                if (speed < 0)
                {
                    gpsInfo.OilVolumn = "";
                }
                else
                {
                    gpsInfo.OilVolumn = Math.Round(speed, 2) + "";
                }
            }
            gpsInfo.Speed = content["Speed"] == null ? "" : content["Speed"].ToString();
            gpsInfo.Datetime = content["UpTime"] == null ? "" : content["UpTime"].ToString();
            gpsInfo.CurLocation = content["Poi"] == null ? "" : content["Poi"].ToString();
            string WarnSign = content["WarnSign"] == null ? "" : content["WarnSign"].ToString();
            gpsInfo.isCheckedFlag = false;
            if (WarnSign != null && WarnSign.Length == 43)
            {
                gpsInfo.Soswarn = WarnSign[0] == 0 ? "正常" : "报警";//1:紧急报警触动报警开关后触发, 收到应答后清零
                gpsInfo.Overspeedwarn = WarnSign[1] == 0 ? "正常" : "报警";//1：超速报警, 标志维持至报警条件解除
                gpsInfo.Tiredwarn = WarnSign[2] == 0 ? "正常" : "报警";//1：疲劳驾驶, 标志维持至报警条件解除
                gpsInfo.Prewarn = WarnSign[3] == 0 ? "正常" : "报警";//1：危险预警, 收到应答后清零
                gpsInfo.Gnssfatal = WarnSign[4] == 0 ? "正常" : "报警";//1：GNSS模块发生故障, 标志维持至报警条件解除
                gpsInfo.Gnssantwarn = WarnSign[5] == 0 ? "正常" : "报警";//1：GNSS天线未接或被剪断, 标志维持至报警条件解除
                gpsInfo.Gnssshortwarn = WarnSign[6] == 0 ? "正常" : "报警";//1：GNSS天线短路, 标志维持至报警条件解除
                gpsInfo.Lowvolwarn = WarnSign[7] == 0 ? "正常" : "报警";//1：终端主电源欠压, 标志维持至报警条件解除
                gpsInfo.Outagewarn = WarnSign[8] == 0 ? "正常" : "报警";//1：终端主电源掉电, 标志维持至报警条件解除
                gpsInfo.Lcdfatalwarn = WarnSign[9] == 0 ? "正常" : "报警";//1：终端LCD或显示器故障, 标志维持至报警条件解除
                gpsInfo.Ttsfatalwarn = WarnSign[10] == 0 ? "正常" : "报警";//1：TTS模块故障, 标志维持至报警条件解除
                gpsInfo.Camerafatalwarn = WarnSign[11] == 0 ? "正常" : "报警";//1:摄像头故障, 标志维持至报警条件解除
                gpsInfo.Accumtimeout = WarnSign[12] == 0 ? "正常" : "报警";//1:当天累计驾驶超时, 标志维持至报警条件解除
                gpsInfo.Stoptimeout = WarnSign[13] == 0 ? "正常" : "报警";//1：超时停车, 标志维持至报警条件解除
                gpsInfo.Inoutareawarn = WarnSign[14] == 0 ? "正常" : "报警";//1：进出区域, 收到应答后清零///////////////////
                gpsInfo.Inoutlinewarn = WarnSign[15] == 0 ? "正常" : "报警";//1:进出路线, 收到应答后清零//////////////////////
                gpsInfo.Drivingtimewarn = WarnSign[16] == 0 ? "正常" : "报警";//1:路段行驶时间不足/过长, 收到应答后清零
                gpsInfo.Deviatewarn = WarnSign[17] == 0 ? "正常" : "报警";//1:路线偏离报警, 标志维持至报警条件解除
                gpsInfo.Vssfatalwarn = WarnSign[18] == 0 ? "正常" : "报警";//1：车辆VSS故障, 标志维持至报警条件解除
                gpsInfo.Oilexceptionwarn = WarnSign[19] == 0 ? "正常" : "报警";//1：车辆油量异常, 标志维持至报警条件解除
                gpsInfo.Vehiclestolenwarn = WarnSign[20] == 0 ? "正常" : "报警";//1：车辆被盗(通过车辆防盗器) , 标志维持至报警条件解除
                gpsInfo.Illignitewarn = WarnSign[21] == 0 ? "正常" : "报警";//1：车辆非法点火, 收到应答后清零
                gpsInfo.Illmovewarn = WarnSign[22] == 0 ? "正常" : "报警";//1：车辆非法位移, 收到应答后清零
                gpsInfo.Outagewarn = WarnSign[23] == 0 ? "正常" : "报警";//1：终端主电源高压，标志维持至报警条件解除
                gpsInfo.ICCardwarn = WarnSign[24] == 0 ? "正常" : "报警";//1：道路运输证IC卡模块故障，标志维持至报警条件解除
                gpsInfo.Overspeedprewarn = WarnSign[25] == 0 ? "正常" : "报警";//1：超速预警，标志维持至报警条件解除
                gpsInfo.Tiredprewarn = WarnSign[26] == 0 ? "正常" : "报警";//1：疲劳驾驶预警，标志维持至报警条件解除
                gpsInfo.Crashwarn = WarnSign[27] == 0 ? "正常" : "报警";//1：碰撞预警，标志维持至报警条件解除
                gpsInfo.Rolloverwarn = WarnSign[28] == 0 ? "正常" : "报警";//1：侧翻预警，标志维持至报警条件解除
                gpsInfo.Ilopendoorwarn = WarnSign[29] == 0 ? "正常" : "报警";//1：非法开门报警（终端未设置区域时，不判断非法开门），收到应答后清零
                gpsInfo.Vediolosewarn = WarnSign[30] == 0 ? "正常" : "报警";//1：视频丢失报警，标志维持至报警条件解除
                gpsInfo.Vedioshelterwarn = WarnSign[31] == 0 ? "正常" : "报警";//1：视频遮挡报警，标志维持至报警条件解除
                gpsInfo.Robwarn = WarnSign[32] == 0 ? "正常" : "报警";//1：劫警
                gpsInfo.Illtimedrivingwarn = WarnSign[33] == 0 ? "正常" : "报警";//1：非法时段行驶报警
                gpsInfo.Sleeptimewarn = WarnSign[34] == 0 ? "正常" : "报警";//1：停车休息时间不足报警
                gpsInfo.Overstationwarn = WarnSign[35] == 0 ? "正常" : "报警";//1：越站报警
                gpsInfo.Protectwarn = WarnSign[36] == 0 ? "正常" : "报警";//1：设防
                gpsInfo.Trimmingwarn = WarnSign[37] == 0 ? "正常" : "报警";//1：剪线报警
                gpsInfo.Lowbatterywarn = WarnSign[38] == 0 ? "正常" : "报警";//1：电瓶电压低报警
                gpsInfo.Passwdwarn = WarnSign[39] == 0 ? "正常" : "报警";//1：密码错误报警
                gpsInfo.Prohibitmovewarn = WarnSign[40] == 0 ? "正常" : "报警";//1：禁行报警
                gpsInfo.Illstopwarn = WarnSign[41] == 0 ? "正常" : "报警";//1：非法停车报警
                gpsInfo.Sdexceptionwarn = WarnSign[42] == 0 ? "正常" : "报警";//1：SD卡异常

            }
            string StatusSign = content["StatusSign"] == null ? "" : content["StatusSign"].ToString();
            if (StatusSign != null && StatusSign.Length == 48)
            {
                gpsInfo.Accstatus = StatusSign[0] == '3' ? "" : StatusSign[0] == '0' ? "关" : "开";//ACC状态
                // gpsInfo.Accstatus = StatusSign[1] == 0 ? "关" : "开";//定位信息，0未定位，1定位
                gpsInfo.Workstatus = StatusSign[1] == '3' ? "" : StatusSign[2] == '0' ? "停运" : "运行";//运营状态，0停运，1运营
                gpsInfo.Llsecret = StatusSign[3] == '3' ? "" : StatusSign[3] == '0' ? "未加密" : "加密";//使用加密插件状态，0未加密，1加密
                gpsInfo.Oilwaystatus = StatusSign[4] == '3' ? "" : StatusSign[4] == '0' ? "断开" : "正常";//车辆油路状态，0断开，1正常
                gpsInfo.Vcstatus = StatusSign[5] == '3' ? "" : StatusSign[5] == '0' ? "断开" : "正常";//车辆电路状态，0断开，1正常
                gpsInfo.Vdstatus = StatusSign[6] == '3' ? "" : StatusSign[6] == '0' ? "解锁" : "加锁";//车门加锁状态，0解锁，1加锁
                gpsInfo.Brakestatus = StatusSign[7] == '3' ? "" : StatusSign[7] == '0' ? "松开" : "踩下";//离合器状态，0松开，1踩下
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
                gpsInfo.Fdstatus = StatusSign[9] == '3' ? "" : StatusSign[9] == '0' ? "关" : "开";//前门开关状态，0关1开
                //gpsInfo.Accstatus = StatusSign[10] == 0 ? "关" : "开";//中门开关状态，0关1开
                gpsInfo.Bdstatus = StatusSign[11] == '3' ? "" : StatusSign[11] == '0' ? "关" : "开";//后门开关状态，0关1开
                //gpsInfo.Accstatus = StatusSign[12] == 0 ? "关" : "开";//驾驶席门开关状态，0关1开
                //gpsInfo.Accstatus = StatusSign[13] == 0 ? "关" : "开";//自定义门开关状态，0关1开
                gpsInfo.Gpsmode = "";
                if (StatusSign[14] == '1')//是否使用GPS卫星定位，0未使用1使用
                    gpsInfo.Gpsmode += "GPS卫星定位;";
                if (StatusSign[15] == '1')//是否使用北斗卫星定位，0未使用1使用
                    gpsInfo.Gpsmode += "北斗卫星定位;";
                if (StatusSign[16] == '1')//是否使用Glonass卫星定位，0未使用1使用
                    gpsInfo.Gpsmode += "Glonass卫星定位;";
                if (StatusSign[17] == '1')//是否使用Galileo卫星定位，0未使用1使用
                    gpsInfo.Gpsmode += "Galileo卫星定位";


                //gpsInfo.Accstatus = StatusSign[18] == 0 ? "关" : "开";//空挡信号，0非空挡，1空挡
                gpsInfo.Conditionerstatus = StatusSign[19] == '3' ? "" : (StatusSign[19] == '0' ? "关" : "开");//空调状态，1开，0关
                gpsInfo.Busstatus = StatusSign[20] == '3' ? "" : StatusSign[20] == '0' ? "无故障" : "故障";//总线故障，0无故障，1故障
                gpsInfo.Gsmstatus = StatusSign[21] == '3' ? "" : StatusSign[21] == '0' ? "无故障" : "故障";//GSM模块故障，0无故障，1故障
                gpsInfo.Gpsantstatus = StatusSign[22] == '3' ? "" : StatusSign[22] == '0' ? "无故障" : "故障";//GPS模块故障，0无故障，1故障
                gpsInfo.Lcstatus = StatusSign[23] == '3' ? "" : StatusSign[23] == '0' ? "无故障" : "故障";//锁车电路故障，0无故障，1故障
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
                gpsInfo.Ltstatus = StatusSign[28] == '3' ? "" : StatusSign[28] == '0' ? "关" : "开";//左转灯状态，0关1开
                gpsInfo.Rtstatus = StatusSign[29] == '3' ? "" : StatusSign[29] == '0' ? "关" : "开";//右转灯状态，0关1开
                gpsInfo.Farlstatus = StatusSign[30] == '3' ? "" : StatusSign[30] == '0' ? "关" : "开";//远光灯状态，0关1开
                gpsInfo.Nearlstatus = StatusSign[31] == '3' ? "" : StatusSign[31] == '0' ? "关" : "开";//近光灯状态，0关1开
                gpsInfo.Ffstatus = StatusSign[32] == '3' ? "" : StatusSign[32] == '0' ? "关" : "开";//前雾灯状态，0关1开
                gpsInfo.Bfstatus = StatusSign[33] == '3' ? "" : StatusSign[33] == '0' ? "关" : "开";//后雾灯状态，0关1开
                //gpsInfo.Accstatus = StatusSign[34] == 0 ? "关" : "开";//倒车灯状态，0关1开
                gpsInfo.Hornstatus = StatusSign[35] == '3' ? "" : StatusSign[35] == '0' ? "关" : "鸣";//喇叭状态，0关1鸣
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

            gpsInfo.OnlineStates = getVehicleOnlinestates(gpsInfo.Speed, gpsInfo.Datetime, gpsInfo);
            // }
            return gpsInfo;
        }
        /// <summary>
        /// 获取实时数据（首次）
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        public bool JsonDeserializeLastestGpsInfo(string json)
        {

            GPSInfo gpsInfo = new GPSInfo();
            JObject job = (JObject)JsonConvert.DeserializeObject(json);
            if (job["GType"].ToString().Equals("Register"))
            {
                string temp = job["GpsBasic"].ToString();
                JArray arry = (JArray)JsonConvert.DeserializeObject(temp);
                if (arry.Count == 0)
                    return false;

                StaticDetailInfo detailInfo = StaticDetailInfo.GetInstance();
                for (int i = 0; i < arry.Count; i++)
                {
                    gpsInfo = DeserializeLastestGpsInfoToGPSInfo(arry[i].ToString());
                    if (gpsInfo != null)
                    {
                        foreach (CVDetailInfo vdi in detailInfo.ListVehicleDetailInfo)
                        {
                            if (vdi.SIM == gpsInfo.Sim)
                            {
                                vdi.VehicleGPSInfo = gpsInfo;
                                break;
                            }
                        }
                    }
                }

                this.RealTimeAllCount = detailInfo.ListVehicleDetailInfo.Count;
            }
            return true;
        }

        /// <summary>
        /// socket通信，更新gps信息(推送）
        /// </summary>
        /// <param name="json">json字符串</param>
        /// <returns>车辆的详细信息</returns>
        public CVDetailInfo DeserializeSockGpsInfo(string json)
        {
            try
            {
                GPSInfo gpsInfo = new GPSInfo();
                StaticDetailInfo detailInfo = StaticDetailInfo.GetInstance();
                JObject job = new Newtonsoft.Json.Linq.JObject();

                job = (JObject)JsonConvert.DeserializeObject(json);
                if (job["gType"].ToString().Equals("gpsLocation"))
                {//GPS信息ok
                    if (job["gpsBasic"] == null)
                        return null;
                    string temp = job["gpsBasic"].ToString();
                    gpsInfo = DeserializeLastestGpsInfoToGPSInfo(temp);

                }
                else if (job["gType"].ToString().Equals("relieveWarn"))
                {//报警解除信息
                    if (job["gpsBasic"] == null)
                        return null;

                    Log.WriteLog(job["gpsBasic"].ToString(), "warnRelieve");
                }
                else if (job["gType"].ToString().Equals("gpsWenben"))
                {//短语
                    if (job["gpsBasic"] == null)
                        return null;
                    string temp = job["gpsBasic"].ToString();
                    DeserializeText(temp);
                }
                else if (job["gType"].ToString().Equals("gpsTaskState"))
                {//回单通知
                    if (job["gpsBasic"] == null)
                        return null;
                    string temp = job["gpsBasic"].ToString();
                    Log.WriteLog(temp, "taskListFinish");
                    if (temp != "")
                    {
                        int i = 0;
                        while (StaticTreeState.DispatchCenter != LoadingState.LOADCOMPLETE && i < 3)
                        {
                            Thread.Sleep(500);
                            i++;
                        }
                        if (StaticTreeState.DispatchCenter == LoadingState.LOADCOMPLETE)
                        {
                            StaticTreeState.DispatchCenter = LoadingState.LOADING;
                            VehicleDispatchViewModel.GetInstance().QueryCommandExecute();
                        }
                    }
                }
                else if (job["gType"].ToString().Equals("dispatchState"))
                {//区域或工地更新 
                    if (job["gpsBasic"] == null)
                        return null;
                    string temp = job["gpsBasic"].ToString();
                    Log.WriteLog(temp, "regionUpdate");
                    if (temp != "")
                    {
                        DeserializeRegionInfo(temp);
                    }
                }
                else if (job["gType"].ToString().Equals("ins"))
                {//指令回传    ok
                    if (job["gpsBasic"] == null)
                    {
                        return null;
                    }
                    string temp = job["gpsBasic"].ToString();
                    Log.WriteLog(temp, "insReturn");
                    DeserializeInsBack(temp);
                }
                else if (job["gType"].ToString().Equals("gpsVehicleState"))
                {//车辆任务状态更新    ok
                    if (job["gpsBasic"] == null)
                        return null;
                    string temp = job["gpsBasic"].ToString();
                    Log.WriteLog(temp, "vehicleStatusUpdate");
                    DeserializeVehicleTaskStatus(temp);
                }
                else if (job["gType"].ToString().Equals("gpsWarn"))
                {//报警或消息     ok
                    if (job["gpsBasic"] == null)
                        return null;
                    string temp = job["gpsBasic"].ToString();

                    Log.WriteLog(temp, "warnOrMessage");
                    WarnInfo gpsWarnInfo = new WarnInfo();
                    gpsWarnInfo = DeserializeLastestWarnToGPSInfo(temp);
                    if (!StaticWarnInfo.IsWarnOrMessage(gpsWarnInfo.AlarmType))
                    {//消息
                        if (StaticTreeState.MessageInfo == LoadingState.LOADCOMPLETE)
                        {
                            StaticTreeState.MessageInfo = LoadingState.LOADING;
                            foreach (CVDetailInfo vehicle in StaticDetailInfo.GetInstance().ListVehicleDetailInfo)
                            {
                                if (vehicle.VehicleId.Equals(gpsWarnInfo.VehicleNum))
                                {
                                    Log.WriteLog(gpsWarnInfo.AlarmContent + "-" + gpsWarnInfo.AlarmType + "-" + gpsWarnInfo.VehicleId + "-" + gpsWarnInfo.VehicleNum, "messageinfo");
                                    StaticWarnInfo.GetInstance().MessageList.Add(gpsWarnInfo);                                    
                                }
                            }
                            StaticTreeState.MessageInfo = LoadingState.LOADCOMPLETE;
                        }
                    }
                    else
                    {//报警
                        #region 报警

                        //获取选中的车辆
                        List<CVBasicInfo> selectedvehicle = RealTimeViewModel.GetInstance().listSelectedVehicleInfo;
                        StaticWarnInfo staticwarninfo = StaticWarnInfo.GetInstance();
                        if (selectedvehicle.Count > 0)
                        {//选中的车辆数大于零
                            if (StaticTreeState.WarnSettinInfo == LoadingState.LOADCOMPLETE)
                            {//报警配置完成
                                foreach (AlarmSettingInfo item in StaticWarnInfo.warnsetlist)
                                {
                                    //判断是否在要显示的报警类型里面
                                    if (item.WarnID != gpsWarnInfo.AlarmType)
                                    {
                                        continue;
                                    }
                                    //报警信息是要显示的
                                    WarnInfo gpsWarnInfoTemp = null;
                                    /*更新车辆报警信息*/
                                    if (gpsWarnInfo != null)
                                    {
                                        //判断是否是是要显示的车辆的报警信息
                                        foreach (CVBasicInfo vehicle in selectedvehicle)
                                        {
                                            if (vehicle.SIM == gpsWarnInfo.SimId)
                                            {//是选中车辆的报警信息
                                                //判断报警信息是否已经在内存里
                                                foreach (WarnInfo warninfo in StaticWarnInfo.GetInstance().WarnInfoList)
                                                {
                                                    //判断是不是同一辆车的同一种报警类型
                                                    if (warninfo.SimId == gpsWarnInfo.SimId && warninfo.AlarmType == gpsWarnInfo.AlarmType)
                                                    {
                                                        gpsWarnInfoTemp = warninfo;
                                                        break;
                                                    }
                                                }
                                                if (gpsWarnInfoTemp != null)
                                                {//已经在内存里，更新报警信息

                                                    if (StaticTreeState.WarnInfo == LoadingState.LOADCOMPLETE)
                                                    {
                                                        StaticTreeState.WarnInfo = LoadingState.LOADING;
                                                        List<WarnInfo> tmplist = new List<WarnInfo>();
                                                        tmplist = StaticWarnInfo.GetInstance().WarnInfoList;
                                                        tmplist.Remove(gpsWarnInfoTemp);
                                                        tmplist.Add(gpsWarnInfo);
                                                        StaticWarnInfo.GetInstance().WarnInfoList = tmplist;
                                                        StaticTreeState.WarnInfo = LoadingState.LOADCOMPLETE;

                                                    }
                                                }
                                                else
                                                {//不在,添加一条新的信息到内存，并刷新列表

                                                    if (StaticTreeState.WarnInfo == LoadingState.LOADCOMPLETE)
                                                    {
                                                        StaticTreeState.WarnInfo = LoadingState.LOADING;
                                                        List<WarnInfo> tmplist = new List<WarnInfo>();
                                                        tmplist = StaticWarnInfo.GetInstance().WarnInfoList;
                                                        tmplist.Add(gpsWarnInfo);
                                                        StaticWarnInfo.GetInstance().WarnInfoList = tmplist;
                                                        StaticTreeState.WarnInfo = LoadingState.LOADCOMPLETE;
                                                        //播放报警声音

                                                        string soundUrl = VehicleConfig.GetInstance().warnSoundPath;
                                                        if (!File.Exists(soundUrl))
                                                        {//如果声音文件不存在则播放默认的报警声音
                                                            soundUrl = VehicleConfig.GetInstance().warnSoundPathDefault;
                                                        }
                                                        SoundPlayer palyer = new SoundPlayer(soundUrl);
                                                        palyer.Play();

                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        #endregion
                    }
                }
                /*找到该车辆并记录最新GPS信息*/
                foreach (CVDetailInfo vdi in detailInfo.ListVehicleDetailInfo)
                {
                    //vdi.SIM = gpsInfo.Sim;

                    if (vdi.SIM == gpsInfo.Sim)
                    {
                        Log.WriteLog("1", "writeVehicleInfo");
                        vdi.VehicleGPSInfo = gpsInfo;
                        return vdi;
                    }

                }
                return null;
            }
            catch (Exception)
            {
                return null;
            }
        }

        //解析指令回传
        private void DeserializeInsBack(string json)
        {
            if (json == "")
                return;
            JObject content = (JObject)JsonConvert.DeserializeObject(json);
            string result = content["result"] == null ? "" : content["result"].ToString();
            if (string.IsNullOrEmpty(result))
            {
                return;
            }
            string sim = content["simId"] == null ? "" : content["simId"].ToString();
            int loopcount = 0;
            while (true)
            {
                Thread.Sleep(200);
                if (StaticTreeState.CmdStatus == LoadingState.LOADCOMPLETE)
                {
                    StaticTreeState.CmdStatus = LoadingState.LOADING;
                    foreach (CommandInfo cmd in StaticMessageInfo.GetInstance().CmdList)
                    {
                        if (cmd.cmdSim.Equals(sim) && string.IsNullOrEmpty(cmd.ReturnMsg))
                        {
                            string returnstatus = "发送失败！";
                            switch (result)
                            {
                                case "0":
                                    returnstatus = "发送成功！";
                                    break;
                                case "1":
                                    returnstatus = "发送失败！";
                                    break;
                                case "2":
                                    returnstatus = "消息有误！";
                                    break;
                                case "3":
                                    returnstatus = "终端不支持该指令！";
                                    break;
                                default:
                                    returnstatus = "发送失败！";
                                    break;
                            }
                            cmd.ReturnMsg = returnstatus;
                        }
                    }
                    StaticTreeState.CmdStatus = LoadingState.LOADCOMPLETE;
                    break;
                }
                if (loopcount++ > 10)
                {
                    break;
                }

            }
        }

        //解析区域的更新信息
        private void DeserializeRegionInfo(string json)
        {
            if (json == "")
                return;
            JObject content = (JObject)JsonConvert.DeserializeObject(json);
            string type = content["type"] == null ? "" : content["type"].ToString();
            if (!string.IsNullOrEmpty(type))
            {
                if (string.Equals(type, "TASK"))
                {//任务单的更新
                    int i = 0;
                    while (StaticTreeState.DispatchCenter != LoadingState.LOADCOMPLETE && i < 3)
                    {
                        Thread.Sleep(500);
                        i++;
                    }
                    if (StaticTreeState.DispatchCenter == LoadingState.LOADCOMPLETE)
                    {
                        StaticTreeState.DispatchCenter = LoadingState.LOADING;
                        VehicleDispatchViewModel.GetInstance().QueryCommandExecute();
                    }
                }
                else if (string.Equals(type, "REG") || string.Equals(type, "SITE"))
                {//区域或者工地的更新
                    string regionId = content["ID"] == null ? "" : content["ID"].ToString();
                    if (!string.IsNullOrEmpty(regionId))
                    {
                        string operateType = content["OPERATETYPE"] == null ? "" : content["OPERATETYPE"].ToString();
                        if (string.Equals(operateType, "2"))
                        {
                            VehicleDispatchViewModel.GetInstance().RefreshOneDispatchInfo(type, regionId);
                        }
                        RealTimeViewModel.GetInstance().RefreshRealtimeMap();
                    }
                }
            }

        }

        //解析车辆任务状态信息
        private void DeserializeVehicleTaskStatus(string json)
        {
            if (json == "")
                return;
            string simId = null;
            string vehicleStatus = null;
            JObject content = (JObject)JsonConvert.DeserializeObject(json);
            simId = content["simId"] == null ? "" : content["simId"].ToString().Trim();
            vehicleStatus = content["vehiclestate"] == null ? "0" : content["vehiclestate"].ToString().Trim();

            bool isVehicleStatus = false;
            int vehicleStatusInteger;
            isVehicleStatus = int.TryParse(vehicleStatus, out vehicleStatusInteger);
            if (!isVehicleStatus)
            {
                return;
            }

            bool shouldUpdate = false;
            int loop_count = 0;
            while (true && loop_count++ < 15)
            {
                if (StaticTreeState.DispatchTreeLoad == LoadingState.LOADCOMPLETE)
                {
                    StaticTreeState.DispatchTreeLoad = LoadingState.LOADING;
                    int loop_count2 = 0;
                    while (true && loop_count2++ < 15)
                    {
                        if (StaticTreeState.VehicleBasicInfo == LoadingState.LOADCOMPLETE && StaticTreeState.VehicleBasicInfo == LoadingState.LOADCOMPLETE)
                        {
                            StaticTreeState.VehicleBasicInfo = LoadingState.LOADING;
                            foreach (CVBasicInfo info in StaticBasicInfo.GetInstance().ListVehicleBasicInfo)
                            {//更新内存中车辆基本数据的任务状态
                                if (info.SIM.Equals(simId))
                                {
                                    string[] vehiclestatuses = { "空闲无任务", "进行任务中", "维修不可用", "无任务离场" };

                                    if (!info.TaskState.Equals(vehiclestatuses[vehicleStatusInteger]))
                                    {
                                        info.TaskState = vehiclestatuses[vehicleStatusInteger];
                                        shouldUpdate = true;
                                    }
                                    break;
                                }
                            }

                            foreach (CVDetailInfo vehicle_detail in StaticDetailInfo.GetInstance().ListVehicleDetailInfo)
                            {//更新内存中车辆详细数据的任务状态
                                if (vehicle_detail.SIM.Equals(simId))
                                {
                                    if (!vehicle_detail.VehicleState.Equals(vehicleStatus))
                                    {
                                        vehicle_detail.VehicleState = vehicleStatus;
                                        shouldUpdate = true;
                                    }
                                    break;
                                }
                            }
                            StaticTreeState.VehicleBasicInfo = LoadingState.LOADCOMPLETE;
                            break;
                        }
                    }
                    StaticTreeState.DispatchTreeLoad = LoadingState.LOADCOMPLETE;
                    break;
                }
            }
            if (shouldUpdate)
            {
                Log.WriteLog(simId + ":" + vehicleStatus, "vehicleStateUpdateExe");
                StaticTreeState.VehicleBasicInfo = LoadingState.NOLOADING;
                DispatchTreeViewModel.GetInstance().TreeOperate.RefreshTree();
            }
        }
        //解析文本信息
        public void DeserializeText(string json)
        {
            if (json == "")
                return;
            MessageInfo text = new MessageInfo();
            JObject content = (JObject)JsonConvert.DeserializeObject(json);
            text.SimID = content["simId"] == null ? "" : content["simId"].ToString();
            text.Time = content["upTime"] == null ? "" : content["upTime  "].ToString();
            text.Content = content["data"] == null ? "" : content["data"].ToString();
            text.VehicleID = content["fInnerId"] == null ? "" : content["fInnerId"].ToString();
            text.VehicleNum = content["vehicleId"] == null ? "" : content["vehicleId"].ToString();

            StaticTreeState.MessageInfo = LoadingState.NOLOADING;
            StaticRealTimeInfo.GetInstance().ReceiveMessageList.Add(text);
            StaticTreeState.MessageInfo = LoadingState.LOADCOMPLETE;
        }

        /*
         * 函数名称：DeserializeLatestGpsInfoList(string xml)
         * 函数功能：反序列化GpsInfoList
         * 函数参数： xml 字符串
         * 作者：wangyi
         *	时间：2013/09/25
         */
        public void DeserializeLatestGpsInfoList(string xml)
        {
            GPSInfo gpsInfo;//GPS信息
            StaticDetailInfo detailInfo = StaticDetailInfo.GetInstance();
            using (XmlReader xReader = XmlReader.Create(new StringReader(xml)))
            {
                while (xReader.ReadToFollowing("GpsInfo"))
                {
                    gpsInfo = null;
                    gpsInfo = new GPSInfo();
                    gpsInfo.Sim = (xReader.GetAttribute("a1") == null ? "" : xReader.GetAttribute("a1")).Trim();

                    gpsInfo.Longitude = chargeLngData(xReader.GetAttribute("a2"));
                    if (gpsInfo.Longitude.Equals("null"))
                    {
                        continue;
                    }
                    gpsInfo.Latitude = chargeLatData(xReader.GetAttribute("a3"));
                    if (gpsInfo.Latitude.Equals("null"))
                    {
                        continue;
                    }
                    gpsInfo.Altitude = xReader.GetAttribute("a4") == null ? "" : xReader.GetAttribute("a4");
                    gpsInfo.GpsStatus = xReader.GetAttribute("a5") == null ? "" : xReader.GetAttribute("a5");
                    gpsInfo.Speed = xReader.GetAttribute("a6") == null ? "" : xReader.GetAttribute("a6");
                    gpsInfo.DevSpeed = xReader.GetAttribute("a7") == null ? "" : xReader.GetAttribute("a7");
                    gpsInfo.Mileage = xReader.GetAttribute("a8") == null ? "" : xReader.GetAttribute("a8");
                    gpsInfo.OilVolumn = xReader.GetAttribute("a9") == null ? "" : xReader.GetAttribute("a9");
                    gpsInfo.Direction = xReader.GetAttribute("b0") == "正北" ? "" : translateDirection(xReader.GetAttribute("b0"));
                    gpsInfo.Datetime = xReader.GetAttribute("b1");
                    if (!string.IsNullOrEmpty(gpsInfo.Datetime))
                    {
                        gpsInfo.Datetime = Convert.ToDateTime(gpsInfo.Datetime).ToString("yyyy/MM/dd HH:mm:hh");
                    }
                    gpsInfo.OnlineStates = getVehicleOnlinestates(gpsInfo.Speed, gpsInfo.Datetime, gpsInfo);
                    gpsInfo.isCheckedFlag = false;

                    gpsInfo.GPSMileage = xReader.GetAttribute("b2") == null ? "" : xReader.GetAttribute("b2");
                    gpsInfo.Accstatus = xReader.GetAttribute("b3") == null ? "" : xReader.GetAttribute("b3");
                    gpsInfo.Workstatus = xReader.GetAttribute("b4") == null ? "" : xReader.GetAttribute("b4");
                    gpsInfo.Llsecret = xReader.GetAttribute("b5") == null ? "" : xReader.GetAttribute("b5");
                    gpsInfo.Gpsmode = xReader.GetAttribute("b6") == null ? "" : xReader.GetAttribute("b6");
                    gpsInfo.Oilwaystatus = xReader.GetAttribute("b7") == null ? "" : xReader.GetAttribute("b7");
                    gpsInfo.Vcstatus = xReader.GetAttribute("b8") == null ? "" : xReader.GetAttribute("b8");
                    gpsInfo.Vdstatus = xReader.GetAttribute("b9") == null ? "" : xReader.GetAttribute("b9");
                    gpsInfo.Fdstatus = xReader.GetAttribute("c0") == null ? "" : xReader.GetAttribute("c0");
                    gpsInfo.Bdstatus = xReader.GetAttribute("c1") == null ? "" : xReader.GetAttribute("c1");
                    gpsInfo.Enginestatus = xReader.GetAttribute("c2") == null ? "" : xReader.GetAttribute("c2");
                    gpsInfo.Conditionerstatus = xReader.GetAttribute("c3") == null ? "" : xReader.GetAttribute("c3");
                    gpsInfo.Brakestatus = xReader.GetAttribute("c4") == null ? "" : xReader.GetAttribute("c4");
                    gpsInfo.Ltstatus = xReader.GetAttribute("c5") == null ? "" : xReader.GetAttribute("c5");
                    gpsInfo.Rtstatus = xReader.GetAttribute("c6") == null ? "" : xReader.GetAttribute("c6");
                    gpsInfo.Farlstatus = xReader.GetAttribute("c7") == null ? "" : xReader.GetAttribute("c7");
                    gpsInfo.Nearlstatus = xReader.GetAttribute("c8") == null ? "" : xReader.GetAttribute("c8");
                    gpsInfo.Pnstatus = xReader.GetAttribute("c9") == null ? "" : xReader.GetAttribute("c9");
                    gpsInfo.Shakestatus = xReader.GetAttribute("d0") == null ? "" : xReader.GetAttribute("d0");
                    gpsInfo.Hornstatus = xReader.GetAttribute("d1") == null ? "" : xReader.GetAttribute("d1");
                    gpsInfo.Protectstatus = xReader.GetAttribute("d2") == null ? "" : xReader.GetAttribute("d2");
                    gpsInfo.Loadstatus = xReader.GetAttribute("d3") == null ? "" : xReader.GetAttribute("d3");
                    gpsInfo.Busstatus = xReader.GetAttribute("d4") == null ? "" : xReader.GetAttribute("d4");
                    gpsInfo.Gsmstatus = xReader.GetAttribute("d5") == null ? "" : xReader.GetAttribute("d5");
                    gpsInfo.Gpsstatus = xReader.GetAttribute("d6") == null ? "" : xReader.GetAttribute("d6");
                    gpsInfo.Lcstatus = xReader.GetAttribute("d7") == null ? "" : xReader.GetAttribute("d7");
                    gpsInfo.Ffstatus = xReader.GetAttribute("d8") == null ? "" : xReader.GetAttribute("d8");
                    gpsInfo.Bfstatus = xReader.GetAttribute("d9") == null ? "" : xReader.GetAttribute("d9");
                    gpsInfo.Gpsantstatus = xReader.GetAttribute("e0") == null ? "" : xReader.GetAttribute("e0");
                    gpsInfo.Soswarn = xReader.GetAttribute("e1") == null ? "" : xReader.GetAttribute("e1");
                    gpsInfo.Overspeedwarn = xReader.GetAttribute("e2") == null ? "" : xReader.GetAttribute("e2");
                    gpsInfo.Tiredwarn = xReader.GetAttribute("e3") == null ? "" : xReader.GetAttribute("e3");
                    gpsInfo.Prewarn = xReader.GetAttribute("e4") == null ? "" : xReader.GetAttribute("e4");
                    gpsInfo.Gnssfatal = xReader.GetAttribute("e5") == null ? "" : xReader.GetAttribute("e5");
                    gpsInfo.Gnssantwarn = xReader.GetAttribute("e6") == null ? "" : xReader.GetAttribute("e6");
                    gpsInfo.Gnssshortwarn = xReader.GetAttribute("e7") == null ? "" : xReader.GetAttribute("e7");
                    gpsInfo.Lowvolwarn = xReader.GetAttribute("e8") == null ? "" : xReader.GetAttribute("e8");
                    gpsInfo.Highvolwarn = xReader.GetAttribute("e9") == null ? "" : xReader.GetAttribute("e9");
                    gpsInfo.Outagewarn = xReader.GetAttribute("f0") == null ? "" : xReader.GetAttribute("f0");
                    gpsInfo.Lcdfatalwarn = xReader.GetAttribute("f1") == null ? "" : xReader.GetAttribute("f1");
                    gpsInfo.Ttsfatalwarn = xReader.GetAttribute("f1") == null ? "" : xReader.GetAttribute("f2");
                    gpsInfo.Camerafatalwarn = xReader.GetAttribute("f3") == null ? "" : xReader.GetAttribute("f3");
                    gpsInfo.Vediolosewarn = xReader.GetAttribute("f4") == null ? "" : xReader.GetAttribute("f4");
                    gpsInfo.Vedioshelterwarn = xReader.GetAttribute("f5") == null ? "" : xReader.GetAttribute("f5");
                    gpsInfo.Accumtimeout = xReader.GetAttribute("f6") == null ? "" : xReader.GetAttribute("f6");
                    gpsInfo.Stoptimeout = xReader.GetAttribute("f7") == null ? "" : xReader.GetAttribute("f7");
                    gpsInfo.Inoutareawarn = xReader.GetAttribute("f8") == null ? "" : xReader.GetAttribute("f8");
                    gpsInfo.Inoutlinewarn = xReader.GetAttribute("f9") == null ? "" : xReader.GetAttribute("f9");
                    gpsInfo.Drivingtimewarn = xReader.GetAttribute("g0") == null ? "" : xReader.GetAttribute("g0");
                    gpsInfo.Deviatewarn = xReader.GetAttribute("g1") == null ? "" : xReader.GetAttribute("g1");
                    gpsInfo.Vssfatalwarn = xReader.GetAttribute("g2") == null ? "" : xReader.GetAttribute("g2");
                    gpsInfo.Oilexceptionwarn = xReader.GetAttribute("g3") == null ? "" : xReader.GetAttribute("g3");
                    gpsInfo.Vehiclestolenwarn = xReader.GetAttribute("g4") == null ? "" : xReader.GetAttribute("g4");
                    gpsInfo.Illignitewarn = xReader.GetAttribute("g5") == null ? "" : xReader.GetAttribute("g5");
                    gpsInfo.Illmovewarn = xReader.GetAttribute("g6") == null ? "" : xReader.GetAttribute("g6");
                    gpsInfo.Crashwarn = xReader.GetAttribute("g7") == null ? "" : xReader.GetAttribute("g7");
                    gpsInfo.Sdexceptionwarn = xReader.GetAttribute("g8") == null ? "" : xReader.GetAttribute("g8");
                    gpsInfo.Robwarn = xReader.GetAttribute("g9") == null ? "" : xReader.GetAttribute("g9");
                    gpsInfo.Sleeptimewarn = xReader.GetAttribute("h0") == null ? "" : xReader.GetAttribute("h0");
                    gpsInfo.Illtimedrivingwarn = xReader.GetAttribute("h1") == null ? "" : xReader.GetAttribute("h1");
                    gpsInfo.Overstationwarn = xReader.GetAttribute("h2") == null ? "" : xReader.GetAttribute("h2");
                    gpsInfo.Ilopendoorwarn = xReader.GetAttribute("h3") == null ? "" : xReader.GetAttribute("h3");
                    gpsInfo.Protectwarn = xReader.GetAttribute("h4") == null ? "" : xReader.GetAttribute("h4");
                    gpsInfo.Trimmingwarn = xReader.GetAttribute("h5") == null ? "" : xReader.GetAttribute("h5");
                    gpsInfo.Passwdwarn = xReader.GetAttribute("h6") == null ? "" : xReader.GetAttribute("h6");
                    gpsInfo.Prohibitmovewarn = xReader.GetAttribute("h7") == null ? "" : xReader.GetAttribute("h7");
                    gpsInfo.Illstopwarn = xReader.GetAttribute("h8") == null ? "" : xReader.GetAttribute("h8");
                    gpsInfo.CurLocation = xReader.GetAttribute("p1") == null ? "" : xReader.GetAttribute("p1");

                    /*找到该车辆并记录最新GPS信息*/
                    foreach (CVDetailInfo vdi in detailInfo.ListVehicleDetailInfo)
                    {
                        if (vdi.SIM == gpsInfo.Sim)
                        {
                            vdi.VehicleGPSInfo = gpsInfo;
                            break;
                        }
                    }
                }
            }
            this.RealTimeAllCount = detailInfo.ListVehicleDetailInfo.Count;//总的车数
        }
        // 定时获取去的最新gps信息根据经纬度解析地址
        private void ParseAddressThread(int startIndex, int endIndex)
        {
            /*逆地址解析*/
            StaticDetailInfo infoInstance = StaticDetailInfo.GetInstance();
            WebClient webClient = new WebClient();
            webClient.Encoding = System.Text.Encoding.UTF8;
            BaiDuPoint bdPoint = new BaiDuPoint();
            BaiDuAddr bdAddr = new BaiDuAddr();
            Uri endpoint;
            for (int i = startIndex; i < endIndex; i++)
            {
                CVDetailInfo detailInfo = infoInstance.ListVehicleDetailInfo[i];
                GPSInfo gpsInfo = detailInfo.VehicleGPSInfo;
                try
                {
                    string lat = gpsInfo.Latitude;
                    string lng = gpsInfo.Longitude;
                    /*坐标转换*/
                    endpoint = new Uri(VehicleConfig.GetInstance().URL_POINTTRANS + "&x=" + lng + "&y=" + lat);
                    string pointStr = webClient.DownloadString(endpoint);
                    if (pointStr != "")
                    {
                        bdPoint = JsonConvert.DeserializeObject<BaiDuPoint>(pointStr);
                        if (bdPoint.error == "0")
                        {
                            byte[] buf;
                            buf = Convert.FromBase64String(bdPoint.x);
                            gpsInfo.Longitude = Encoding.UTF8.GetString(buf, 0, buf.Length);
                            buf = Convert.FromBase64String(bdPoint.y);
                            gpsInfo.Latitude = Encoding.UTF8.GetString(buf, 0, buf.Length);
                        }
                    }
                    /*地址解析*/
                    endpoint = new Uri(VehicleConfig.GetInstance().URL_PARSEADDRESS + "&location=" + lat + "," + lng + "&output=json");
                    string addrStr = webClient.DownloadString(endpoint);
                    if (addrStr != "")
                    {
                        bdAddr = JsonConvert.DeserializeObject<BaiDuAddr>(addrStr);
                        if (bdAddr.status == "0")
                        {
                            gpsInfo.CurLocation = bdAddr.result.formatted_address;
                        }
                    }
                }
                catch
                {

                }
                finally
                {

                }
            }
        }
        #endregion

        #region 车辆跟踪根据sim定时获取车辆信息

        /// <summary>
        /// 定时刷新最新GPS信息线程开启
        /// </summary>
        /// <param name="vehicleTrackViewModel"></param>
        public void GetLatestVehicleGPSInfoBySimThread(VehicleTrackViewModel vehicleTrackViewModel)
        {

            testSubZmqSocket.Connect(TestPubSubPoint);
            PushThreadRunning = true;
            Thread thread = new Thread(delegate() { this.GetLatestVehicleGPSInfoBySim(vehicleTrackViewModel); });
            thread.Start();
        }

        /// <summary>
        /// 根据SIM卡号获取车辆的最新
        /// </summary>
        /// <param name="vehicleTrackViewModel"></param>
        public void GetLatestVehicleGPSInfoBySim(VehicleTrackViewModel vehicleTrackViewModel)
        {
            try
            {
                ///需要改
                testSubZmqSocket.Subscribe(Encoding.UTF8.GetBytes(vehicleTrackViewModel.SelectedInfo.SIM));

                while (PushThreadRunning)
                {
                    try
                    {
                        string message = testSubZmqSocket.Receive(Encoding.UTF8);
                        string hb = testSubZmqSocket.Receive(Encoding.UTF8);

                        if (message != "")
                        {
                            if (hb != null && hb != "")
                            {
                                GPSInfo gpsInfo = JsonDeserializeLatestGpsInfoBySim(hb);
                                if (gpsInfo == null)
                                {
                                    testSubZmqSocket.Disconnect(TestPubSubPoint);
                                    return;
                                }
                                ////////if (vehicleTrackViewModel.ListVehicleInfo == null)
                                ////////{
                                ////////    vehicleTrackViewModel.ListVehicleInfo = new List<GPSInfo>();
                                ////////    gpsInfo.Sequence = 1;
                                ////////}
                                ////////else
                                gpsInfo.Sequence = vehicleTrackViewModel.ListVehicleInfo.Count + 1;
                                List<GPSInfo> tmpList = new List<GPSInfo>();
                                if (vehicleTrackViewModel.ListVehicleInfo.Count != 0)
                                {
                                    GPSInfo lastGpsInfo = vehicleTrackViewModel.ListVehicleInfo[vehicleTrackViewModel.ListVehicleInfo.Count - 1];
                                    ////测试
                                    //gpsInfo.Datetime = Convert.ToDateTime(gpsInfo.Datetime).AddSeconds(count++).ToString();
                                    //gpsInfo.Latitude = (Convert.ToDouble(gpsInfo.Latitude) + count * 0.001).ToString();
                                    ////测试
                                    if (Convert.ToDateTime(gpsInfo.Datetime).CompareTo(Convert.ToDateTime(lastGpsInfo.Datetime)) <= 0)
                                    {
                                        return;
                                    }
                                    else
                                    {
                                        foreach (GPSInfo info in vehicleTrackViewModel.ListVehicleInfo)
                                        {
                                            tmpList.Add(info);
                                        }

                                        tmpList.Add(gpsInfo);
                                        vehicleTrackViewModel.ListVehicleInfo = tmpList;

                                    }
                                }
                                else
                                {
                                    tmpList.Add(gpsInfo);
                                    vehicleTrackViewModel.ListVehicleInfo = tmpList;

                                }


                            }
                            else
                            {
                                testSubZmqSocket.Disconnect(TestPubSubPoint);
                                return;
                            }

                        }
                    }
                    catch (Exception)
                    {
                        MessageBox.Show("连接服务器错误");
                        // return;
                    }
                    System.Threading.Thread.Sleep(1000);
                }
                testSubZmqSocket.Disconnect(TestPubSubPoint);
                return;
            }
            catch (Exception e)
            {
                testSubZmqSocket.Disconnect(TestPubSubPoint);
                return;
            }
        }
        /// <summary>
        /// json实时信息解析层GPSInfo
        /// 2014/6/10
        /// </summary>
        /// <param name="json">json 数据</param>
        /// <returns>GPSInfo</returns>
        public GPSInfo JsonDeserializeLatestGpsInfoBySim(string json)
        {
            JObject job = (JObject)JsonConvert.DeserializeObject(json);
            if (job["gType"].ToString().Equals("gpsLocation"))
            {
                if (job["gpsBasic"] == null)
                    return null;
                string temp = job["gpsBasic"].ToString();
                return DeserializeLastestGpsInfoToGPSInfo(temp);
            }
            else
                return null;
        }

        /// <summary>
        /// 解除报警
        /// </summary>
        /// <param name="json"></param>
        private void DeserializeWarnRelieveInfo(string json)
        {
            if (json == "")
                return;
            WarnInfo WarnRelieveInfo = new WarnInfo();
            JObject content = (JObject)JsonConvert.DeserializeObject(json);
            WarnRelieveInfo.SimId = content["SimId"] == null ? "" : content["SimId"].ToString();
            WarnRelieveInfo.AlarmType = content["warntype"] == null ? "" : content["warntype"].ToString();
            WarnRelieveInfo.cmdId = content["flag"] == null ? "" : content["flag"].ToString();//获取解除报警的状态
            foreach (WarnInfo warninfo in StaticWarnInfo.GetInstance().WarnInfoList)
            {
                if (warninfo.SimId == WarnRelieveInfo.SimId && warninfo.AlarmType == WarnRelieveInfo.AlarmType)
                {
                    StaticWarnInfo.GetInstance().WarnInfoList.Remove(warninfo);
                    List<WarnInfo> tmp = StaticWarnInfo.GetInstance().WarnInfoList;
                    StaticWarnInfo.GetInstance().WarnInfoList = tmp;
                    break;
                }
            }

            return;
        }
        /// <summary>
        /// 获取报警信息
        /// </summary>
        /// <param name="vehicleTrackViewModel"></param>
        private WarnInfo DeserializeLastestWarnToGPSInfo(string json)
        {
            WarnInfo gpsWarnInfo = new WarnInfo();
            if (json == "")
                return null;

            JObject content = (JObject)JsonConvert.DeserializeObject(json);
            gpsWarnInfo.SimId = content["SimId"] == null ? "" : content["SimId"].ToString();
            if (gpsWarnInfo.SimId == null || gpsWarnInfo.SimId == "")
            {
                return null;
            }
            string sql = "select IV.FInnerId,IV.VehicleId,IV.ParentUnitId,IU.UNITNAME from InfoVehicle IV,InfoUnit IU where IU.UNITID=IV.ParentUnitId and IV.SIM='" + gpsWarnInfo.SimId + "'";
            string jsonStr = VehicleCommon.wcfDBHelper.BexcuteSql(StaticLoginInfo.GetInstance().UserName, sql);
            if (jsonStr == "error")
            {
                //MessageBox.Show("报警列表初始化失败！请重试！", "提示", MessageBoxButton.OK);
                //confirm.IsEnabled = false;
                return null;
            }
            DataTable dt = JsonHelper.JsonToDataTable(jsonStr);
            if (dt != null)
            {
                gpsWarnInfo.VehicleNum = dt.Rows[0]["VehicleId"].ToString();
                gpsWarnInfo.VehicleId = dt.Rows[0]["FInnerId"].ToString();
                gpsWarnInfo.UnitName = dt.Rows[0]["UNITNAME"].ToString();
                gpsWarnInfo.UnitId = dt.Rows[0]["ParentUnitId"].ToString();
            }
            List<CVDetailInfo> vehicleinfolist = StaticDetailInfo.GetInstance().ListVehicleDetailInfo;
            foreach (CVDetailInfo item in vehicleinfolist)
            {
                if (item.SIM == gpsWarnInfo.SimId)
                {
                    if (item.VehicleGPSInfo != null)
                    {
                        gpsWarnInfo.Speed = item.VehicleGPSInfo.Speed;
                    }
                    else
                    {
                        gpsWarnInfo.Speed = "0";
                    }
                }
            }
            gpsWarnInfo.cmdId = content["cmdId"] == null ? "" : content["cmdId"].ToString();
            gpsWarnInfo.serialNum = content["serialNum"] == null ? "" : content["serialNum"].ToString();
            gpsWarnInfo.Lat = content["Latitude"] == null ? "" : content["Latitude"].ToString();
            gpsWarnInfo.Long = content["Longitude"] == null ? "" : content["Longitude"].ToString();

            gpsWarnInfo.Place = this.ParseOneAddress(gpsWarnInfo.Long, gpsWarnInfo.Lat);

            //string WarnType = content["WarnType"] == null ? "" : content["WarnType"].ToString();
            gpsWarnInfo.AlarmType = content["WarnType"] == null ? "" : content["WarnType"].ToString();
            //gpsWarnInfo.Address = content["Poi"] == null ? "" : content["Poi"].ToString();
            gpsWarnInfo.Time = content["WarnStartTime"] == null ? "" : content["WarnStartTime"].ToString();
            string[] warntypes = StaticWarnInfo.getWarnType();
            try
            {
                gpsWarnInfo.AlarmContent = warntypes[int.Parse(gpsWarnInfo.AlarmType)];
            }
            catch (Exception)
            {

                throw;
            }
            return gpsWarnInfo;
        }

        #region OLD
        public void GetLatestVehicleGPSInfoBySimThread_old(VehicleTrackViewModel vehicleTrackViewModel)
        {
            Thread nThread = new Thread(delegate() { this.GetLatestVehicleGPSInfoBySimOld(vehicleTrackViewModel); });
            nThread.Start();
        }
        /*定时刷新最新GPS信息*/
        public void GetLatestVehicleGPSInfoBySimOld(VehicleTrackViewModel vehicleTrackViewModel)
        {
            try
            {
                StaticLoginInfo userInfo = StaticLoginInfo.GetInstance();
                long tick = DateTime.Now.Ticks;
                Random urlRandom = new Random((int)(tick & 0xffffffffL) | (int)(tick >> 32));
                string randomStr = urlRandom.Next().ToString() + DateTime.Now.ToString() + urlRandom.Next().ToString();
                Uri endpoint = new Uri(VehicleConfig.GetInstance().URL_REMOTE_VEHICLETRACKBYSIMWEB + "?Sim=" + vehicleTrackViewModel.SelectedInfo.SIM + "&mapType=baidu&apFlag=1" + "&time=" + randomStr);
                WebClient client = new WebClient();
                client.Encoding = System.Text.Encoding.UTF8;
                string xmlStr = client.DownloadString(endpoint);
                if ((xmlStr != null) && (!xmlStr.Equals("")))
                {
                    GPSInfo gpsInfo = DeserializeLatestGpsInfoBySim(xmlStr);

                    if (gpsInfo == null)
                    {
                        return;
                    }
                    gpsInfo.Sequence = vehicleTrackViewModel.ListVehicleInfo.Count + 1;
                    List<GPSInfo> tmpList = new List<GPSInfo>();
                    if (vehicleTrackViewModel.ListVehicleInfo.Count != 0)
                    {
                        GPSInfo lastGpsInfo = vehicleTrackViewModel.ListVehicleInfo[vehicleTrackViewModel.ListVehicleInfo.Count - 1];
                        if (Convert.ToDateTime(gpsInfo.Datetime).CompareTo(Convert.ToDateTime(lastGpsInfo.Datetime)) <= 0)
                        {
                            return;
                        }
                        else
                        {
                            foreach (GPSInfo info in vehicleTrackViewModel.ListVehicleInfo)
                            {
                                tmpList.Add(info);
                            }
                            tmpList.Add(gpsInfo);
                            vehicleTrackViewModel.ListVehicleInfo = tmpList;
                        }
                    }
                    else
                    {
                        tmpList.Add(gpsInfo);
                        vehicleTrackViewModel.ListVehicleInfo = tmpList;
                    }
                }
                else
                {
                    return;
                }
            }
            catch (Exception e)
            {
                return;
            }
        }

        /*GPS信息解析xml*/
        public GPSInfo DeserializeLatestGpsInfoBySim(string xml)
        {
            GPSInfo gpsInfo = new GPSInfo();
            using (XmlReader xReader = XmlReader.Create(new StringReader(xml)))
            {
                if (!xReader.ReadToFollowing("GpsInfo"))
                {
                    return null;
                }
                gpsInfo.Sim = xReader.GetAttribute("a1") == null ? "" : xReader.GetAttribute("a1");
                gpsInfo.Longitude = chargeLngData(xReader.GetAttribute("a2"));
                if (gpsInfo.Longitude.Equals("null"))
                {
                    return null;
                }
                gpsInfo.Latitude = chargeLatData(xReader.GetAttribute("a3"));
                if (gpsInfo.Latitude.Equals("null"))
                {
                    return null;
                }
                gpsInfo.Altitude = xReader.GetAttribute("a4") == null ? "" : xReader.GetAttribute("a4");
                gpsInfo.GpsStatus = xReader.GetAttribute("a5") == null ? "" : xReader.GetAttribute("a5");
                gpsInfo.Speed = xReader.GetAttribute("a6") == null ? "" : xReader.GetAttribute("a6");
                gpsInfo.DevSpeed = xReader.GetAttribute("a7") == null ? "" : xReader.GetAttribute("a7");
                gpsInfo.Mileage = xReader.GetAttribute("a8") == null ? "" : xReader.GetAttribute("a8");
                gpsInfo.OilVolumn = xReader.GetAttribute("a9") == null ? "" : xReader.GetAttribute("a9");
                gpsInfo.Direction = xReader.GetAttribute("b0") == "正北" ? "" : translateDirection(xReader.GetAttribute("b0"));
                gpsInfo.Datetime = xReader.GetAttribute("b1");
                gpsInfo.OnlineStates = getVehicleOnlinestates(gpsInfo.Speed, gpsInfo.Datetime, gpsInfo);
                gpsInfo.isCheckedFlag = false;

                gpsInfo.Idtime = xReader.GetAttribute("b2") == null ? "" : xReader.GetAttribute("b2");
                gpsInfo.Accstatus = xReader.GetAttribute("b3") == null ? "" : xReader.GetAttribute("b3");
                gpsInfo.Workstatus = xReader.GetAttribute("b4") == null ? "" : xReader.GetAttribute("b4");
                gpsInfo.Llsecret = xReader.GetAttribute("b5") == null ? "" : xReader.GetAttribute("b5");
                gpsInfo.Gpsmode = xReader.GetAttribute("b6") == null ? "" : xReader.GetAttribute("b6");
                gpsInfo.Oilwaystatus = xReader.GetAttribute("b7") == null ? "" : xReader.GetAttribute("b7");
                gpsInfo.Vcstatus = xReader.GetAttribute("b8") == null ? "" : xReader.GetAttribute("b8");
                gpsInfo.Vdstatus = xReader.GetAttribute("b9") == null ? "" : xReader.GetAttribute("b9");
                gpsInfo.Fdstatus = xReader.GetAttribute("c0") == null ? "" : xReader.GetAttribute("c0");
                gpsInfo.Bdstatus = xReader.GetAttribute("c1") == null ? "" : xReader.GetAttribute("c1");
                gpsInfo.Enginestatus = xReader.GetAttribute("c2") == null ? "" : xReader.GetAttribute("c2");
                gpsInfo.Conditionerstatus = xReader.GetAttribute("c3") == null ? "" : xReader.GetAttribute("c3");
                gpsInfo.Brakestatus = xReader.GetAttribute("c4") == null ? "" : xReader.GetAttribute("c4");
                gpsInfo.Ltstatus = xReader.GetAttribute("c5") == null ? "" : xReader.GetAttribute("c5");
                gpsInfo.Rtstatus = xReader.GetAttribute("c6") == null ? "" : xReader.GetAttribute("c6");
                gpsInfo.Farlstatus = xReader.GetAttribute("c7") == null ? "" : xReader.GetAttribute("c7");
                gpsInfo.Nearlstatus = xReader.GetAttribute("c8") == null ? "" : xReader.GetAttribute("c8");
                gpsInfo.Pnstatus = xReader.GetAttribute("c9") == null ? "" : xReader.GetAttribute("c9");
                gpsInfo.Shakestatus = xReader.GetAttribute("d0") == null ? "" : xReader.GetAttribute("d0");
                gpsInfo.Hornstatus = xReader.GetAttribute("d1") == null ? "" : xReader.GetAttribute("d1");
                gpsInfo.Protectstatus = xReader.GetAttribute("d2") == null ? "" : xReader.GetAttribute("d2");
                gpsInfo.Loadstatus = xReader.GetAttribute("d3") == null ? "" : xReader.GetAttribute("d3");
                gpsInfo.Busstatus = xReader.GetAttribute("d4") == null ? "" : xReader.GetAttribute("d4");
                gpsInfo.Gsmstatus = xReader.GetAttribute("d5") == null ? "" : xReader.GetAttribute("d5");
                gpsInfo.Gpsstatus = xReader.GetAttribute("d6") == null ? "" : xReader.GetAttribute("d6");
                gpsInfo.Lcstatus = xReader.GetAttribute("d7") == null ? "" : xReader.GetAttribute("d7");
                gpsInfo.Ffstatus = xReader.GetAttribute("d8") == null ? "" : xReader.GetAttribute("d8");
                gpsInfo.Bfstatus = xReader.GetAttribute("d9") == null ? "" : xReader.GetAttribute("d9");
                gpsInfo.Gpsantstatus = xReader.GetAttribute("e0") == null ? "" : xReader.GetAttribute("e0");
                gpsInfo.Soswarn = xReader.GetAttribute("e1") == null ? "" : xReader.GetAttribute("e1");
                gpsInfo.Overspeedwarn = xReader.GetAttribute("e2") == null ? "" : xReader.GetAttribute("e2");
                gpsInfo.Tiredwarn = xReader.GetAttribute("e3") == null ? "" : xReader.GetAttribute("e3");
                gpsInfo.Prewarn = xReader.GetAttribute("e4") == null ? "" : xReader.GetAttribute("e4");
                gpsInfo.Gnssfatal = xReader.GetAttribute("e5") == null ? "" : xReader.GetAttribute("e5");
                gpsInfo.Gnssantwarn = xReader.GetAttribute("e6") == null ? "" : xReader.GetAttribute("e6");
                gpsInfo.Gnssshortwarn = xReader.GetAttribute("e7") == null ? "" : xReader.GetAttribute("e7");
                gpsInfo.Lowvolwarn = xReader.GetAttribute("e8") == null ? "" : xReader.GetAttribute("e8");
                gpsInfo.Highvolwarn = xReader.GetAttribute("e9") == null ? "" : xReader.GetAttribute("e9");
                gpsInfo.Outagewarn = xReader.GetAttribute("f0") == null ? "" : xReader.GetAttribute("f0");
                gpsInfo.Lcdfatalwarn = xReader.GetAttribute("f1") == null ? "" : xReader.GetAttribute("f1");
                gpsInfo.Ttsfatalwarn = xReader.GetAttribute("f1") == null ? "" : xReader.GetAttribute("f2");
                gpsInfo.Camerafatalwarn = xReader.GetAttribute("f3") == null ? "" : xReader.GetAttribute("f3");
                gpsInfo.Vediolosewarn = xReader.GetAttribute("f4") == null ? "" : xReader.GetAttribute("f4");
                gpsInfo.Vedioshelterwarn = xReader.GetAttribute("f5") == null ? "" : xReader.GetAttribute("f5");
                gpsInfo.Accumtimeout = xReader.GetAttribute("f6") == null ? "" : xReader.GetAttribute("f6");
                gpsInfo.Stoptimeout = xReader.GetAttribute("f7") == null ? "" : xReader.GetAttribute("f7");
                gpsInfo.Inoutareawarn = xReader.GetAttribute("f8") == null ? "" : xReader.GetAttribute("f8");
                gpsInfo.Inoutlinewarn = xReader.GetAttribute("f9") == null ? "" : xReader.GetAttribute("f9");
                gpsInfo.Drivingtimewarn = xReader.GetAttribute("g0") == null ? "" : xReader.GetAttribute("g0");
                gpsInfo.Deviatewarn = xReader.GetAttribute("g1") == null ? "" : xReader.GetAttribute("g1");
                gpsInfo.Vssfatalwarn = xReader.GetAttribute("g2") == null ? "" : xReader.GetAttribute("g2");
                gpsInfo.Oilexceptionwarn = xReader.GetAttribute("g3") == null ? "" : xReader.GetAttribute("g3");
                gpsInfo.Vehiclestolenwarn = xReader.GetAttribute("g4") == null ? "" : xReader.GetAttribute("g4");
                gpsInfo.Illignitewarn = xReader.GetAttribute("g5") == null ? "" : xReader.GetAttribute("g5");
                gpsInfo.Illmovewarn = xReader.GetAttribute("g6") == null ? "" : xReader.GetAttribute("g6");
                gpsInfo.Crashwarn = xReader.GetAttribute("g7") == null ? "" : xReader.GetAttribute("g7");
                gpsInfo.Sdexceptionwarn = xReader.GetAttribute("g8") == null ? "" : xReader.GetAttribute("g8");
                gpsInfo.Robwarn = xReader.GetAttribute("g9") == null ? "" : xReader.GetAttribute("g9");
                gpsInfo.Sleeptimewarn = xReader.GetAttribute("h0") == null ? "" : xReader.GetAttribute("h0");
                gpsInfo.Illtimedrivingwarn = xReader.GetAttribute("h1") == null ? "" : xReader.GetAttribute("h1");
                gpsInfo.Overstationwarn = xReader.GetAttribute("h2") == null ? "" : xReader.GetAttribute("h2");
                gpsInfo.Ilopendoorwarn = xReader.GetAttribute("h3") == null ? "" : xReader.GetAttribute("h3");
                gpsInfo.Protectwarn = xReader.GetAttribute("h4") == null ? "" : xReader.GetAttribute("h4");
                gpsInfo.Trimmingwarn = xReader.GetAttribute("h5") == null ? "" : xReader.GetAttribute("h5");
                gpsInfo.Passwdwarn = xReader.GetAttribute("h6") == null ? "" : xReader.GetAttribute("h6");
                gpsInfo.Prohibitmovewarn = xReader.GetAttribute("h7") == null ? "" : xReader.GetAttribute("h7");
                gpsInfo.Illstopwarn = xReader.GetAttribute("h8") == null ? "" : xReader.GetAttribute("h8");
                gpsInfo.GPSMileage = xReader.GetAttribute("h9") == null ? "" : xReader.GetAttribute("h9");

                /*逆地址解析*/
                //this.ParseAddressPoint(gpsInfo);
                gpsInfo.CurLocation = xReader.GetAttribute("p1") == null ? "" : xReader.GetAttribute("p1");

            }
            return gpsInfo;
        }
        #endregion


        /*根据Sim卡获取的一部车的最新gps信息根据经纬度解析地址*/
        private void ParseAddressPoint(GPSInfo gpsInfo)
        {
            WebClient webClient = new WebClient();
            webClient.Encoding = System.Text.Encoding.UTF8;
            BaiDuPoint bdPoint = new BaiDuPoint();
            BaiDuAddr bdAddr = new BaiDuAddr();
            /*逆地址解析*/
            try
            {
                string lng = gpsInfo.Longitude;
                string lat = gpsInfo.Latitude;
                /*坐标转换*/
                Uri endpoint = new Uri(VehicleConfig.GetInstance().URL_POINTTRANS + "&x=" + lng + "&y=" + lat);
                string pointStr = webClient.DownloadString(endpoint);
                if (pointStr != "")
                {
                    bdPoint = JsonConvert.DeserializeObject<BaiDuPoint>(pointStr);
                    if (bdPoint.error == "0")
                    {
                        byte[] buf;
                        buf = Convert.FromBase64String(bdPoint.x);
                        gpsInfo.Longitude = Encoding.UTF8.GetString(buf, 0, buf.Length);
                        buf = Convert.FromBase64String(bdPoint.y);
                        gpsInfo.Latitude = Encoding.UTF8.GetString(buf, 0, buf.Length);
                    }
                }
                /*地址解析*/
                endpoint = new Uri(VehicleConfig.GetInstance().URL_PARSEADDRESS + "&location=" + lat + "," + lng + "&output=json");
                string addrStr = webClient.DownloadString(endpoint);
                if (addrStr != "")
                {
                    bdAddr = JsonConvert.DeserializeObject<BaiDuAddr>(addrStr);
                    if (bdAddr.status == "0")
                    {
                        gpsInfo.CurLocation = bdAddr.result.formatted_address;
                    }
                }
            }
            catch
            {
            }
        }
        #endregion

        #region 车辆调度信息 根据车牌号获取Gps信息
        public void GetLatestVehicleGPSInfoByVehicleID(VehicleDispatchItemViewModel dispatchItem)
        {
            foreach (DispatchVehicleViewModel vm in dispatchItem.ListDispatchVehicle)
            {
                if (vm.DetailInfo != null)
                {
                    GPSInfo gpsInfo = vm.DetailInfo.VehicleGPSInfo;
                    if (gpsInfo != null && !string.IsNullOrEmpty(gpsInfo.Latitude) && !string.IsNullOrEmpty(gpsInfo.Longitude))
                    {
                        double distance = 0.0;
                        if (vm.DispatchInfo.CarsStatus == VehicleCommon.TaskDriveTransIng)
                        {//去途中
                            vm.DistanceType = "离工地距离";
                            vm.RegionDistance = VehicleCommon.GetDistance(Convert.ToDouble(gpsInfo.Latitude), Convert.ToDouble(gpsInfo.Longitude),
                                Convert.ToDouble(dispatchItem.TaskNumberInfo.StartLat), Convert.ToDouble(dispatchItem.TaskNumberInfo.StartLng));
                            vm.SiteDistance = VehicleCommon.GetDistance(Convert.ToDouble(gpsInfo.Latitude), Convert.ToDouble(gpsInfo.Longitude),
                                Convert.ToDouble(dispatchItem.TaskNumberInfo.EndLat), Convert.ToDouble(dispatchItem.TaskNumberInfo.EndLng));
                            distance = vm.SiteDistance;
                        }
                        else
                        {//返途中
                            vm.DistanceType = "离区域距离";
                            vm.SiteDistance = VehicleCommon.GetDistance(Convert.ToDouble(gpsInfo.Latitude), Convert.ToDouble(gpsInfo.Longitude),
                                Convert.ToDouble(dispatchItem.TaskNumberInfo.EndLat), Convert.ToDouble(dispatchItem.TaskNumberInfo.EndLng));
                            vm.RegionDistance = VehicleCommon.GetDistance(Convert.ToDouble(gpsInfo.Latitude), Convert.ToDouble(gpsInfo.Longitude),
                                Convert.ToDouble(dispatchItem.TaskNumberInfo.StartLat), Convert.ToDouble(dispatchItem.TaskNumberInfo.StartLng));
                            distance = vm.RegionDistance;
                        }
                        double totalDistance = vm.RegionDistance + vm.SiteDistance;
                        vm.MarginDistance = dispatchItem.GetMarginDistance(vm.RegionDistance / totalDistance);
                        vm.RealDistance = distance;
                    }
                }
            }

            /*处理重叠的车辆*/
            //途中
            List<DispatchPointViewModel> pointList = new List<DispatchPointViewModel>();
            bool hasFind = false;
            foreach (DispatchVehicleViewModel dvvm in dispatchItem.ListDispatchVehicle)
            {
                //foreach (DispatchPointViewModel dpvm in pointList)
                //{
                //    if (dvvm.MarginDistance < dpvm.MarginDistance + 10 &&
                //        dvvm.MarginDistance > dpvm.MarginDistance - 10)
                //    {//在[D-10,D+10]区间
                //        dpvm.ListOverlapVehicle.Add(dvvm);
                //        hasFind = true;
                //        break;
                //    }
                //}
                if (!hasFind)
                {//如果没有找到，则以这部车辆的基准建立标注点
                    if (dvvm.DetailInfo == null)
                    {
                        continue;
                    }
                    DispatchPointViewModel newPoint = new DispatchPointViewModel();
                    newPoint.MarginDistance = dvvm.MarginDistance;
                    newPoint.TaskNumberInfo = dispatchItem.TaskNumberInfo;
                    newPoint.VehicleId = dvvm.DetailInfo.VehicleId;
                    newPoint.Sim = dvvm.DetailInfo.SIM;
                    newPoint.ListOverlapVehicle.Add(dvvm);//将自己加入该标注点
                    if (dvvm.DistanceType != null)
                    {

                        if (dvvm.DistanceType.Equals("离区域距离"))
                        {
                            newPoint.ImageMargin = new Thickness(dvvm.MarginDistance, 20, 0, 0);
                        }
                        else
                        {
                            newPoint.ImageMargin = new Thickness(dvvm.MarginDistance, 0, 0, 0);
                        }
                    }
                    pointList.Add(newPoint);
                }
                hasFind = false;
            }
            //计算每个标注点的重叠车辆总数
            foreach (DispatchPointViewModel dpvm in pointList)
            {
                dpvm.OverlapCount = dpvm.ListOverlapVehicle.Count;
            }
            dispatchItem.ListDispatchPoint = pointList;//赋值

            //区域内
            List<DispatchPointViewModel> inRegionList = new List<DispatchPointViewModel>();
            DispatchPointViewModel newInRegionPoint = null;
            if (dispatchItem.ListInRegionVehicle != null && dispatchItem.ListInRegionVehicle.Count < 5)
            {
                dispatchItem.ListViewVisibility = Visibility.Visible;
                dispatchItem.ComboBoxVisibility = Visibility.Collapsed;
            }
            else
            {
                dispatchItem.ComboBoxVisibility = Visibility.Visible;
                dispatchItem.ListViewVisibility = Visibility.Collapsed;
            }
            int i = 0;
            foreach (DispatchVehicleViewModel dvvm in dispatchItem.ListInRegionVehicle)
            {
                newInRegionPoint = new DispatchPointViewModel();
                newInRegionPoint.TaskNumberInfo = dispatchItem.TaskNumberInfo;
                dvvm.DistanceType = "离工地距离";
                dvvm.RealDistance = dispatchItem.TaskNumberInfo.RealTransDistance;
                newInRegionPoint.ListOverlapVehicle.Add(dvvm);//将自己加入该标注点
                if (dvvm.DetailInfo == null)
                {
                    continue;
                }
                newInRegionPoint.Sim = dvvm.DetailInfo.SIM;
                newInRegionPoint.VehicleNum = dvvm.DetailInfo.VehicleNum;
                newInRegionPoint.VehicleId = dvvm.DetailInfo.VehicleId;
                //newInRegionPoint.OverlapCount = newInRegionPoint.ListOverlapVehicle.Count;
                switch (i)
                {
                    case 0:
                        newInRegionPoint.ImageMargin = new Thickness(0, 0, 70, 25);
                        i++;
                        break;
                    case 1:
                        newInRegionPoint.ImageMargin = new Thickness(70, 0, 0, 25);
                        i++;
                        break;
                    case 2:
                        newInRegionPoint.ImageMargin = new Thickness(0, 25, 70, 0);
                        i++;
                        break;
                    case 3:
                        newInRegionPoint.ImageMargin = new Thickness(70, 25, 0, 0);
                        i++;
                        break;
                    default:
                        newInRegionPoint.ImageMargin = new Thickness(0, 40.0, 0, 0);
                        i++;
                        break;
                }
                inRegionList.Add(newInRegionPoint);
            }
            dispatchItem.ListInRegionPoint = inRegionList;//赋值

            //工地内
            List<DispatchPointViewModel> inSiteList = new List<DispatchPointViewModel>();
            DispatchPointViewModel newInSitePoint = null;

            if (dispatchItem.ListInRegionVehicle != null && dispatchItem.ListInRegionVehicle.Count < 5)
            {
                dispatchItem.ListViewVisibility = Visibility.Visible;
                dispatchItem.ComboBoxVisibility = Visibility.Collapsed;
            }
            else
            {
                dispatchItem.ComboBoxVisibility = Visibility.Visible;
                dispatchItem.ListViewVisibility = Visibility.Collapsed;
            }
            i = 0;
            foreach (DispatchVehicleViewModel dvvm in dispatchItem.ListInSiteVehicle)
            {
                if (dvvm.DetailInfo == null)
                {
                    continue;
                }
                newInSitePoint = new DispatchPointViewModel();
                newInSitePoint.TaskNumberInfo = dispatchItem.TaskNumberInfo;
                dvvm.DistanceType = "离区域距离";
                dvvm.RealDistance = dispatchItem.TaskNumberInfo.RealTransDistance;
                newInSitePoint.ListOverlapVehicle.Add(dvvm);//将自己加入该标注点
                newInSitePoint.Sim = dvvm.DetailInfo.SIM;
                newInSitePoint.VehicleId = dvvm.DetailInfo.VehicleId;
                newInSitePoint.VehicleNum = dvvm.DetailInfo.VehicleNum;

                switch (i)
                {
                    case 0:
                        newInSitePoint.ImageMargin = new Thickness(0, 0, 70, 25);
                        i++;
                        break;
                    case 1:
                        newInSitePoint.ImageMargin = new Thickness(70, 0, 0, 25);
                        i++;
                        break;
                    case 2:
                        newInSitePoint.ImageMargin = new Thickness(0, 25, 70, 0);
                        i++;
                        break;
                    case 3:
                        newInSitePoint.ImageMargin = new Thickness(70, 25, 0, 0);
                        i++;
                        break;
                    default:
                        newInSitePoint.ImageMargin = new Thickness(0, 50, 0, 0);
                        i++;
                        break;
                }
                inSiteList.Add(newInSitePoint);
            }
            //if (newInSitePoint.ListOverlapVehicle.Count > 0)
            //{
            //    newInSitePoint.OverlapCount = newInSitePoint.ListOverlapVehicle.Count;
            //    newInSitePoint.VehicleId = newInSitePoint.ListOverlapVehicle.Count.ToString();
            //    inSiteList.Add(newInSitePoint);
            //}
            dispatchItem.ListInSitePoint = inSiteList;//赋值
        }
        #endregion

        #region 辅助函数
        // 根据经纬度解析地址
        /// <summary>
        /// 根据经纬度解析地址
        /// </summary>
        /// <param name="lng">经度</param>
        /// <param name="lat">纬度</param>
        /// <returns>地址</returns>
        public string ParseOneAddress(string lng, string lat)
        {
            /*逆地址解析*/
            WebClient webClient = new WebClient();
            webClient.Encoding = System.Text.Encoding.UTF8;
            try
            {
                /*地址解析*/
                Uri endpoint = new Uri(VehicleConfig.GetInstance().URL_PARSEADDRESS + "&location=" + lat + "," + lng + "&output=json");
                string addrStr = webClient.DownloadString(endpoint);
                if (addrStr != "")
                {
                    BaiDuAddr bdAddr = JsonConvert.DeserializeObject<BaiDuAddr>(addrStr);
                    if (bdAddr.status == "0")
                    {
                        return bdAddr.result.formatted_address;
                    }
                }
            }
            catch
            {
                return "";
            }
            return "";
        }
        // 判断获取的gps格式，并且对不正确的进行修改
        public static string chargeLngData(string lng)
        {
            if (lng == null)
                return "null";

            double lngd = 0;
            if ((lng.Length > 3) && (!lng.Substring(3, 1).Equals(".")))
            {
                lng = lng.Replace(".", "");
                lng = lng.Trim();
                if (lng.Length > 3)
                {
                    lng = lng.Substring(0, 3) + "." + lng.Substring(3);
                }
            }

            if (double.TryParse(lng, out lngd))
            {
                if (lngd < -180 || lngd > 180)
                    return "null";
            }
            else
            {
                return "null";
            }

            return lng;
        }

        public static string chargeLatData(string lat)
        {
            if (lat == null)
                return "null";

            double latd = 0;

            if ((lat.Length > 2) && (!lat.Substring(2, 1).Equals(".")))
            {
                lat = lat.Replace(".", "");
                lat = lat.Trim();
                if (lat.Length > 2)
                {
                    lat = lat.Substring(0, 2) + "." + lat.Substring(2);
                }
            }

            if (double.TryParse(lat, out latd))
            {
                if (latd < -90 || latd > 90)
                    return "null";
            }
            else
            {
                return "null";
            }

            return lat;
        }
        /// <summary>
        /// 验证时间格式
        /// </summary>
        /// <param name="dateTime">时间字符串</param>
        /// <returns></returns>
        public string chargeDateTime(string dateTime)
        {
            if ((dateTime != null) && (dateTime.Length == 18))
            {
                dateTime = dateTime.Substring(0, 10) + " " + dateTime.Substring(10);
            }
            else if (dateTime.Length != 19)
            {
                dateTime = DateTime.Now.ToString();
            }
            return dateTime;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="direction"></param>
        /// <returns></returns>
        private string translateDirection(string direction)
        {
            string tdr = "正北"; //正北 正西 正南 正东 西北 西南  东南 东北
            double tdi = 0;
            if (double.TryParse(direction, out tdi))
            {
                if (tdi == 0)
                {
                    tdr = "正北";
                }
                else if (tdi < 90)
                {
                    tdr = "东北";
                }
                else if (tdi == 90)
                {
                    tdr = "正东";
                }
                else if (tdi < 180)
                {
                    tdr = "东南";
                }
                else if (tdi == 180)
                {
                    tdr = "正南";
                }
                else if (tdi < 270)
                {
                    tdr = "西南";
                }
                else if (tdi == 270)
                {
                    tdr = "正西";
                }
                else if (tdi < 360)
                {
                    tdr = "西北";
                }
                return tdr;
            }
            else
            {
                return direction;
            }


        }

        private bool getTrackVehicleWarnShow(GPSInfo gpsInfo)
        {
            string normal = "正常";
            if ((gpsInfo.Soswarn != null) && (!gpsInfo.Soswarn.Equals(normal)) && (!gpsInfo.Soswarn.Equals("")))
            {
                return true;
            }

            if ((gpsInfo.Overspeedwarn != null) && (!gpsInfo.Overspeedwarn.Equals(normal)) && (!gpsInfo.Overspeedwarn.Equals("")))
            {
                return true;
            }

            if ((gpsInfo.Tiredwarn != null) && (!gpsInfo.Tiredwarn.Equals(normal)) && (!gpsInfo.Tiredwarn.Equals("")))
            {
                return true;
            }

            if ((gpsInfo.Prewarn != null) && (!gpsInfo.Prewarn.Equals(normal)) && (!gpsInfo.Prewarn.Equals("")))
            {
                return true;
            }

            if ((gpsInfo.Gnssfatal != null) && (!gpsInfo.Gnssfatal.Equals(normal)) && (!gpsInfo.Gnssfatal.Equals("")))
            {
                return true;
            }

            if ((gpsInfo.Gnssantwarn != null) && (!gpsInfo.Gnssantwarn.Equals(normal)) && (!gpsInfo.Gnssantwarn.Equals("")))
            {
                return true;
            }


            if ((gpsInfo.Gnssshortwarn != null) && (!gpsInfo.Gnssshortwarn.Equals(normal)) && (!gpsInfo.Gnssshortwarn.Equals("")))
            {
                return true;
            }

            if ((gpsInfo.Lowvolwarn != null) && (!gpsInfo.Lowvolwarn.Equals(normal)) && (!gpsInfo.Lowvolwarn.Equals("")))
            {
                return true;
            }

            if ((gpsInfo.Highvolwarn != null) && (!gpsInfo.Highvolwarn.Equals(normal)) && (!gpsInfo.Highvolwarn.Equals("")))
            {
                return true;
            }


            if ((gpsInfo.Outagewarn != null) && (!gpsInfo.Outagewarn.Equals(normal)) && (!gpsInfo.Outagewarn.Equals("")))
            {
                return true;
            }

            if ((gpsInfo.Lcdfatalwarn != null) && (!gpsInfo.Lcdfatalwarn.Equals(normal)) && (!gpsInfo.Lcdfatalwarn.Equals("")))
            {
                return true;
            }

            if ((gpsInfo.Ttsfatalwarn != null) && (!gpsInfo.Ttsfatalwarn.Equals(normal)) && (!gpsInfo.Ttsfatalwarn.Equals("")))
            {
                return true;
            }

            if ((gpsInfo.Camerafatalwarn != null) && (!gpsInfo.Camerafatalwarn.Equals(normal)) && (!gpsInfo.Camerafatalwarn.Equals("")))
            {
                return true;
            }

            if ((gpsInfo.Vediolosewarn != null) && (!gpsInfo.Vediolosewarn.Equals(normal)) && (!gpsInfo.Vediolosewarn.Equals("")))
            {
                return true;
            }

            if ((gpsInfo.Vedioshelterwarn != null) && (!gpsInfo.Vedioshelterwarn.Equals(normal)) && (!gpsInfo.Vedioshelterwarn.Equals("")))
            {
                return true;
            }

            if ((gpsInfo.Accumtimeout != null) && (!gpsInfo.Accumtimeout.Equals(normal)) && (!gpsInfo.Accumtimeout.Equals("")))
            {
                return true;
            }

            if ((gpsInfo.Stoptimeout != null) && (!gpsInfo.Stoptimeout.Equals(normal)) && (!gpsInfo.Stoptimeout.Equals("")))
            {
                return true;
            }

            if ((gpsInfo.Inoutareawarn != null) && (!gpsInfo.Inoutareawarn.Equals(normal)) && (!gpsInfo.Inoutareawarn.Equals("")))
            {
                return true;
            }


            if ((gpsInfo.Inoutlinewarn != null) && (!gpsInfo.Inoutlinewarn.Equals(normal)) && (!gpsInfo.Inoutlinewarn.Equals("")))
            {
                return true;
            }


            if ((gpsInfo.Drivingtimewarn != null) && (!gpsInfo.Drivingtimewarn.Equals(normal)) && (!gpsInfo.Drivingtimewarn.Equals("")))
            {
                return true;
            }

            if ((gpsInfo.Deviatewarn != null) && (!gpsInfo.Deviatewarn.Equals(normal)) && (!gpsInfo.Deviatewarn.Equals("")))
            {
                return true;
            }


            if ((gpsInfo.Vssfatalwarn != null) && (!gpsInfo.Vssfatalwarn.Equals(normal)) && (!gpsInfo.Vssfatalwarn.Equals("")))
            {
                return true;
            }

            if ((gpsInfo.Oilexceptionwarn != null) && (!gpsInfo.Oilexceptionwarn.Equals(normal)) && (!gpsInfo.Oilexceptionwarn.Equals("")))
            {
                return true;
            }

            if ((gpsInfo.Vehiclestolenwarn != null) && (!gpsInfo.Vehiclestolenwarn.Equals(normal)) && (!gpsInfo.Vehiclestolenwarn.Equals("")))
            {
                return true;
            }


            if ((gpsInfo.Illignitewarn != null) && (!gpsInfo.Illignitewarn.Equals(normal)) && (!gpsInfo.Illignitewarn.Equals("")))
            {
                return true;
            }


            if ((gpsInfo.Illmovewarn != null) && (!gpsInfo.Illmovewarn.Equals(normal)) && (!gpsInfo.Illmovewarn.Equals("")))
            {
                return true;
            }
            if ((gpsInfo.Crashwarn != null) && (!gpsInfo.Crashwarn.Equals(normal)) && (!gpsInfo.Crashwarn.Equals("")))
            {
                return true;
            }

            if ((gpsInfo.Sdexceptionwarn != null) && (!gpsInfo.Sdexceptionwarn.Equals(normal)) && (!gpsInfo.Sdexceptionwarn.Equals("")))
            {
                return true;
            }


            if ((gpsInfo.Robwarn != null) && (!gpsInfo.Robwarn.Equals(normal)) && (!gpsInfo.Robwarn.Equals("")))
            {
                return true;
            }

            if ((gpsInfo.Sleeptimewarn != null) && (!gpsInfo.Sleeptimewarn.Equals(normal)) && (!gpsInfo.Sleeptimewarn.Equals("")))
            {
                return true;
            }

            if ((gpsInfo.Illtimedrivingwarn != null) && (!gpsInfo.Illtimedrivingwarn.Equals(normal)) && (!gpsInfo.Illtimedrivingwarn.Equals("")))
            {
                return true;
            }

            if ((gpsInfo.Overstationwarn != null) && (!gpsInfo.Overstationwarn.Equals(normal)) && (!gpsInfo.Overstationwarn.Equals("")))
            {
                return true;
            }

            if ((gpsInfo.Ilopendoorwarn != null) && (!gpsInfo.Ilopendoorwarn.Equals(normal)) && (!gpsInfo.Ilopendoorwarn.Equals("")))
            {
                return true;
            }
            if ((gpsInfo.Protectwarn != null) && (!gpsInfo.Protectwarn.Equals(normal)) && (!gpsInfo.Protectwarn.Equals("")))
            {
                return true;
            }

            if ((gpsInfo.Trimmingwarn != null) && (!gpsInfo.Trimmingwarn.Equals(normal)) && (!gpsInfo.Trimmingwarn.Equals("")))
            {
                return true;
            }
            if ((gpsInfo.Passwdwarn != null) && (!gpsInfo.Passwdwarn.Equals(normal)) && (!gpsInfo.Passwdwarn.Equals("")))
            {
                return true;
            }

            if ((gpsInfo.Prohibitmovewarn != null) && (!gpsInfo.Prohibitmovewarn.Equals(normal)) && (!gpsInfo.Prohibitmovewarn.Equals("")))
            {
                return true;
            }

            if ((gpsInfo.Illstopwarn != null) && (!gpsInfo.Illstopwarn.Equals(normal)) && (!gpsInfo.Illstopwarn.Equals("")))
            {
                return true;
            }
            return false;
        }

        private string getVehicleOnlinestates(string speed, string dtOldStr, GPSInfo gpsInfo)
        {
            DateTime dtNow = DateTime.Now;
            DateTime dtOld = new DateTime();

            if (DateTime.TryParse(dtOldStr, out dtOld))
            {
                TimeSpan ts = dtNow - dtOld;
                if (ts.TotalMinutes > VehicleConfig.GetInstance().LONG_TIME_OFF_ONLINE_SPAN_HOURS)
                {
                    return VehicleCommon.VSOnlineOff;
                }
                if (gpsInfo.Accstatus != null && gpsInfo.Accstatus.Equals("开"))
                {
                    return VehicleCommon.VSOnlineRun;
                }
                return VehicleCommon.VSOnlinePark;
            }
            return VehicleCommon.VSOnlineOff;
        }
        #endregion

        #region 获取历史轨迹信息
        private int TrackPlayAllCount = 0;
        private object TrackPlayCounterMonitor = new object();//地理和坐标解析计数互斥量
        public void GetTrackPlayVehicleGPSInfoThread(TrackPlayViewModel trackPlayVM)
        {
            Thread nThread = new Thread(delegate() { this.GetTrackPlayVehicleGPSInfo(trackPlayVM); });
            nThread.Start();
        }
        public bool CheckQueryCondition(TrackPlayViewModel trackPlayVM)
        {
            if (trackPlayVM.SelectedVehicle == null
                || trackPlayVM.BeginTime == null
                || trackPlayVM.EndTime == null
                || trackPlayVM.BeginTime > trackPlayVM.EndTime)
            {
                MessageBox.Show("查询条件格式出错，请重试", "提示", MessageBoxButton.OKCancel);
                return false;
            }
            return true;
        }
        /// <summary>
        /// 获取车辆轨迹数据
        /// </summary>
        /// <param name="trackPlayVM">车辆轨迹ViewModel实例</param>
        public void GetTrackPlayVehicleGPSInfo(TrackPlayViewModel trackPlayVM)
        {
            #region 测试数据
            //List<GPSInfo> gpsInfoList = new List<GPSInfo>();
            //List<TrackBackGpsInfo> tmpList = trackPlayVM.ListVehicleInfo;
            //TrackBackGpsInfo temp;
            //GPSInfo gpsInfo;
            //gpsInfo = new GPSInfo();
            //gpsInfo.Sequence = 1;
            //gpsInfo.Longitude = "112.969667";
            //gpsInfo.Latitude = "28.040105";
            //gpsInfo.Direction = "正西";
            //gpsInfoList.Add(gpsInfo);
            //temp = new TrackBackGpsInfo();
            //temp.GpsInfo = gpsInfo;
            //tmpList.Add(temp);

            //gpsInfo = new GPSInfo();
            //gpsInfo.Sequence = 2;
            //gpsInfo.Longitude = "112.9702667";
            //gpsInfo.Latitude = "28.040025";
            //gpsInfo.Direction = "正东";
            //gpsInfoList.Add(gpsInfo);
            //temp = new TrackBackGpsInfo();
            //temp.GpsInfo = gpsInfo;
            //tmpList.Add(temp);

            //gpsInfo = new GPSInfo();
            //gpsInfo.Sequence = 3;
            //gpsInfo.Longitude = "112.9702167";
            //gpsInfo.Latitude = "28.0411383";
            //gpsInfo.Direction = "西北";
            //gpsInfoList.Add(gpsInfo);
            //temp = new TrackBackGpsInfo();
            //temp.GpsInfo = gpsInfo;
            //tmpList.Add(temp);

            //gpsInfo = new GPSInfo();
            //gpsInfo.Sequence = 4;
            //gpsInfo.Longitude = "112.96985";
            //gpsInfo.Latitude = "28.042205";
            //gpsInfo.Direction = "正北";
            //gpsInfoList.Add(gpsInfo);
            //temp = new TrackBackGpsInfo();
            //temp.GpsInfo = gpsInfo;
            //tmpList.Add(temp);

            //gpsInfo = new GPSInfo();
            //gpsInfo.Sequence = 5;
            //gpsInfo.Longitude = "112.9693333";
            //gpsInfo.Latitude = "28.0437067";
            //gpsInfo.Direction = "正北";
            //gpsInfoList.Add(gpsInfo);
            //temp = new TrackBackGpsInfo();
            //temp.GpsInfo = gpsInfo;
            //tmpList.Add(temp);

            //gpsInfo = new GPSInfo();
            //gpsInfo.Sequence = 6;
            //gpsInfo.Longitude = "112.9682";
            //gpsInfo.Latitude = "28.0455667";
            //gpsInfo.Direction = "西北";
            //gpsInfoList.Add(gpsInfo);
            //temp = new TrackBackGpsInfo();
            //temp.GpsInfo = gpsInfo;
            //tmpList.Add(temp);

            //gpsInfo = new GPSInfo();
            //gpsInfo.Sequence = 7;
            //gpsInfo.Longitude = "112.9673167";
            //gpsInfo.Latitude = "28.0470233";
            //gpsInfo.Direction = "正北";
            //gpsInfoList.Add(gpsInfo);
            //temp = new TrackBackGpsInfo();
            //temp.GpsInfo = gpsInfo;
            //tmpList.Add(temp);

            //gpsInfo = new GPSInfo();
            //gpsInfo.Sequence = 8;
            //gpsInfo.Longitude = "112.9673333";
            //gpsInfo.Latitude = "28.0490667";
            //gpsInfo.Direction = "正北";
            //gpsInfoList.Add(gpsInfo);
            //temp = new TrackBackGpsInfo();
            //temp.GpsInfo = gpsInfo;
            //tmpList.Add(temp);

            //gpsInfo = new GPSInfo();
            //gpsInfo.Sequence = 9;
            //gpsInfo.Longitude = "112.9684333";
            //gpsInfo.Latitude = "28.0511217";
            //gpsInfo.Direction = "东北";
            //gpsInfoList.Add(gpsInfo);
            //temp = new TrackBackGpsInfo();
            //temp.GpsInfo = gpsInfo;
            //tmpList.Add(temp);

            //gpsInfo = new GPSInfo();
            //gpsInfo.Sequence = 10;
            //gpsInfo.Longitude = "112.969";
            //gpsInfo.Latitude = "28.0520867";
            //gpsInfo.Direction = "东北";
            //gpsInfoList.Add(gpsInfo);
            //temp = new TrackBackGpsInfo();
            //temp.GpsInfo = gpsInfo;
            //tmpList.Add(temp);

            //gpsInfo = new GPSInfo();
            //gpsInfo.Sequence = 11;
            //gpsInfo.Longitude = "112.96965";
            //gpsInfo.Latitude = "28.0531967";
            //gpsInfo.Direction = "东北";
            //gpsInfoList.Add(gpsInfo);
            //temp = new TrackBackGpsInfo();
            //temp.GpsInfo = gpsInfo;
            //tmpList.Add(temp);

            //gpsInfo = new GPSInfo();
            //gpsInfo.Sequence = 12;
            //gpsInfo.Longitude = "112.9711833";
            //gpsInfo.Latitude = "28.0557783";
            //gpsInfo.Direction = "东北";
            //gpsInfoList.Add(gpsInfo);
            //temp = new TrackBackGpsInfo();
            //temp.GpsInfo = gpsInfo;
            //tmpList.Add(temp);

            //gpsInfo = new GPSInfo();
            //gpsInfo.Sequence = 13;
            //gpsInfo.Longitude = "112.9726667";
            //gpsInfo.Latitude = "28.0582733";
            //gpsInfo.Direction = "东北";
            //gpsInfoList.Add(gpsInfo);
            //temp = new TrackBackGpsInfo();
            //temp.GpsInfo = gpsInfo;
            //tmpList.Add(temp);

            //gpsInfo = new GPSInfo();
            //gpsInfo.Sequence = 14;
            //gpsInfo.Longitude = "112.9740833";
            //gpsInfo.Latitude = "28.060665";
            //gpsInfo.Direction = "东北";
            //gpsInfoList.Add(gpsInfo);
            //temp = new TrackBackGpsInfo();
            //temp.GpsInfo = gpsInfo;
            //tmpList.Add(temp);

            //gpsInfo = new GPSInfo();
            //gpsInfo.Sequence = 15;
            //gpsInfo.Longitude = "112.9755667";
            //gpsInfo.Latitude = "28.0636133";
            //gpsInfo.Direction = "正北";
            //gpsInfoList.Add(gpsInfo);
            //temp = new TrackBackGpsInfo();
            //temp.GpsInfo = gpsInfo;
            //tmpList.Add(temp);

            //gpsInfo = new GPSInfo();
            //gpsInfo.Sequence = 16;
            //gpsInfo.Longitude = "112.97705";
            //gpsInfo.Latitude = "28.06679";
            //gpsInfo.Direction = "正北";
            //gpsInfoList.Add(gpsInfo);
            //temp = new TrackBackGpsInfo();
            //temp.GpsInfo = gpsInfo;
            //tmpList.Add(temp);

            //gpsInfo = new GPSInfo();
            //gpsInfo.Sequence = 17;
            //gpsInfo.Longitude = "112.9780833";
            //gpsInfo.Latitude = "28.069595";
            //gpsInfo.Direction = "正北";
            //gpsInfoList.Add(gpsInfo);
            //temp = new TrackBackGpsInfo();
            //temp.GpsInfo = gpsInfo;
            //tmpList.Add(temp);

            //gpsInfo = new GPSInfo();
            //gpsInfo.Sequence = 18;
            //gpsInfo.Longitude = "112.9781833";
            //gpsInfo.Latitude = "28.069845";
            //gpsInfo.Direction = "正北";
            //gpsInfoList.Add(gpsInfo);
            //temp = new TrackBackGpsInfo();
            //temp.GpsInfo = gpsInfo;
            //tmpList.Add(temp);

            //gpsInfo = new GPSInfo();
            //gpsInfo.Sequence = 19;
            //gpsInfo.Longitude = "112.9794833";
            //gpsInfo.Latitude = "28.0725383";
            //gpsInfo.Direction = "东北";
            //gpsInfoList.Add(gpsInfo);
            //temp = new TrackBackGpsInfo();
            //temp.GpsInfo = gpsInfo;
            //tmpList.Add(temp);

            //gpsInfo = new GPSInfo();
            //gpsInfo.Sequence = 20;
            //gpsInfo.Longitude = "112.9822667";
            //gpsInfo.Latitude = "28.07249";
            //gpsInfo.Direction = "正东";
            //gpsInfoList.Add(gpsInfo);
            //temp = new TrackBackGpsInfo();
            //temp.GpsInfo = gpsInfo;
            //tmpList.Add(temp);

            //gpsInfo = new GPSInfo();
            //gpsInfo.Sequence = 21;
            //gpsInfo.Longitude = "112.9841667";
            //gpsInfo.Latitude = "28.0724967";
            //gpsInfo.Direction = "正东";
            //gpsInfoList.Add(gpsInfo);
            //temp = new TrackBackGpsInfo();
            //temp.GpsInfo = gpsInfo;
            //tmpList.Add(temp);

            //gpsInfo = new GPSInfo();
            //gpsInfo.Sequence = 22;
            //gpsInfo.Longitude = "112.9850167";
            //gpsInfo.Latitude = "28.0726433";
            //gpsInfo.Direction = "正东";
            //gpsInfoList.Add(gpsInfo);
            //temp = new TrackBackGpsInfo();
            //temp.GpsInfo = gpsInfo;
            //tmpList.Add(temp);

            //gpsInfo = new GPSInfo();
            //gpsInfo.Sequence = 23;
            //gpsInfo.Longitude = "112.98915";
            //gpsInfo.Latitude = "28.0732383";
            //gpsInfo.Direction = "正东";
            //gpsInfoList.Add(gpsInfo);
            //temp = new TrackBackGpsInfo();
            //temp.GpsInfo = gpsInfo;
            //tmpList.Add(temp);

            //gpsInfo = new GPSInfo();
            //gpsInfo.Sequence = 24;
            //gpsInfo.Longitude = "112.9929";
            //gpsInfo.Latitude = "28.0739967";
            //gpsInfo.Direction = "正东";
            //gpsInfoList.Add(gpsInfo);
            //temp = new TrackBackGpsInfo();
            //temp.GpsInfo = gpsInfo;
            //tmpList.Add(temp);

            //gpsInfo = new GPSInfo();
            //gpsInfo.Sequence = 25;
            //gpsInfo.Longitude = "112.99705";
            //gpsInfo.Latitude = "28.0750033";
            //gpsInfo.Direction = "正东";
            //gpsInfoList.Add(gpsInfo);
            //temp = new TrackBackGpsInfo();
            //temp.GpsInfo = gpsInfo;
            //tmpList.Add(temp);

            //gpsInfo = new GPSInfo();
            //gpsInfo.Sequence = 26;
            //gpsInfo.Longitude = "113.0019";
            //gpsInfo.Latitude = "28.0760917";
            //gpsInfo.Direction = "正东";
            //gpsInfoList.Add(gpsInfo);
            //temp = new TrackBackGpsInfo();
            //temp.GpsInfo = gpsInfo;
            //tmpList.Add(temp);

            //gpsInfo = new GPSInfo();
            //gpsInfo.Sequence = 27;
            //gpsInfo.Longitude = "113.0069167";
            //gpsInfo.Latitude = "28.07709";
            //gpsInfo.Direction = "正东";
            //gpsInfoList.Add(gpsInfo);
            //temp = new TrackBackGpsInfo();
            //temp.GpsInfo = gpsInfo;
            //tmpList.Add(temp);

            //gpsInfo = new GPSInfo();
            //gpsInfo.Sequence = 28;
            //gpsInfo.Longitude = "113.0075";
            //gpsInfo.Latitude = "28.0787517";
            //gpsInfo.Direction = "正北";
            //gpsInfoList.Add(gpsInfo);
            //temp = new TrackBackGpsInfo();
            //temp.GpsInfo = gpsInfo;
            //tmpList.Add(temp);

            //gpsInfo = new GPSInfo();
            //gpsInfo.Sequence = 29;
            //gpsInfo.Longitude = "113.0067167";
            //gpsInfo.Latitude = "28.0830083";
            //gpsInfo.Direction = "正北";
            //gpsInfoList.Add(gpsInfo);
            //temp = new TrackBackGpsInfo();
            //temp.GpsInfo = gpsInfo;
            //tmpList.Add(temp);

            //gpsInfo = new GPSInfo();
            //gpsInfo.Sequence = 30;
            //gpsInfo.Longitude = "113.0063167";
            //gpsInfo.Latitude = "28.0861767";
            //gpsInfo.Direction = "正北";
            //gpsInfoList.Add(gpsInfo);
            //temp = new TrackBackGpsInfo();
            //temp.GpsInfo = gpsInfo;
            //tmpList.Add(temp);

            //gpsInfo = new GPSInfo();
            //gpsInfo.Sequence = 31;
            //gpsInfo.Longitude = "113.0065333";
            //gpsInfo.Latitude = "28.0901517";
            //gpsInfo.Direction = "正北";
            //gpsInfoList.Add(gpsInfo);
            //temp = new TrackBackGpsInfo();
            //temp.GpsInfo = gpsInfo;
            //tmpList.Add(temp);

            //gpsInfo = new GPSInfo();
            //gpsInfo.Sequence = 32;
            //gpsInfo.Longitude = "113.0067167";
            //gpsInfo.Latitude = "28.0940083";
            //gpsInfo.Direction = "正北";
            //gpsInfoList.Add(gpsInfo);
            //temp = new TrackBackGpsInfo();
            //temp.GpsInfo = gpsInfo;
            //tmpList.Add(temp);

            //gpsInfo = new GPSInfo();
            //gpsInfo.Sequence = 33;
            //gpsInfo.Longitude = "113.0064333";
            //gpsInfo.Latitude = "28.0926117";
            //gpsInfo.Direction = "正南";
            //gpsInfoList.Add(gpsInfo);
            //temp = new TrackBackGpsInfo();
            //temp.GpsInfo = gpsInfo;
            //tmpList.Add(temp);

            //gpsInfo = new GPSInfo();
            //gpsInfo.Sequence = 34;
            //gpsInfo.Longitude = "113.0062833";
            //gpsInfo.Latitude = "28.090505";
            //gpsInfo.Direction = "正南";
            //gpsInfoList.Add(gpsInfo);
            //temp = new TrackBackGpsInfo();
            //temp.GpsInfo = gpsInfo;
            //tmpList.Add(temp);

            //gpsInfo = new GPSInfo();
            //gpsInfo.Sequence = 35;
            //gpsInfo.Longitude = "113.0061333";
            //gpsInfo.Latitude = "28.087665";
            //gpsInfo.Direction = "正南";
            //gpsInfoList.Add(gpsInfo);
            //temp = new TrackBackGpsInfo();
            //temp.GpsInfo = gpsInfo;
            //tmpList.Add(temp);

            //gpsInfo = new GPSInfo();
            //gpsInfo.Sequence = 36;
            //gpsInfo.Longitude = "113.0062667";
            //gpsInfo.Latitude = "28.08389";
            //gpsInfo.Direction = "正南";
            //gpsInfoList.Add(gpsInfo);
            //temp = new TrackBackGpsInfo();
            //temp.GpsInfo = gpsInfo;
            //tmpList.Add(temp);

            //gpsInfo = new GPSInfo();
            //gpsInfo.Sequence = 37;
            //gpsInfo.Longitude = "113.0070167";
            //gpsInfo.Latitude = "28.0797733";
            //gpsInfo.Direction = "正南";
            //gpsInfoList.Add(gpsInfo);
            //temp = new TrackBackGpsInfo();
            //temp.GpsInfo = gpsInfo;
            //tmpList.Add(temp);

            //gpsInfo = new GPSInfo();
            //gpsInfo.Sequence = 38;
            //gpsInfo.Longitude = "113.0061333";
            //gpsInfo.Latitude = "28.0770067";
            //gpsInfo.Direction = "正西";
            //gpsInfoList.Add(gpsInfo);
            //temp = new TrackBackGpsInfo();
            //temp.GpsInfo = gpsInfo;
            //tmpList.Add(temp);

            //gpsInfo = new GPSInfo();
            //gpsInfo.Sequence = 39;
            //gpsInfo.Longitude = "113.00165";
            //gpsInfo.Latitude = "28.076095";
            //gpsInfo.Direction = "正西";
            //gpsInfoList.Add(gpsInfo);
            //temp = new TrackBackGpsInfo();
            //temp.GpsInfo = gpsInfo;
            //tmpList.Add(temp);

            //gpsInfo = new GPSInfo();
            //gpsInfo.Sequence = 40;
            //gpsInfo.Longitude = "112.9968";
            //gpsInfo.Latitude = "28.0750217";
            //gpsInfo.Direction = "正西";
            //gpsInfoList.Add(gpsInfo);
            //temp = new TrackBackGpsInfo();
            //temp.GpsInfo = gpsInfo;
            //tmpList.Add(temp);

            //gpsInfo = new GPSInfo();
            //gpsInfo.Sequence = 41;
            //gpsInfo.Longitude = "112.9924";
            //gpsInfo.Latitude = "28.0739967";
            //gpsInfo.Direction = "正西";
            //gpsInfoList.Add(gpsInfo);
            //temp = new TrackBackGpsInfo();
            //temp.GpsInfo = gpsInfo;
            //tmpList.Add(temp);

            //gpsInfo = new GPSInfo();
            //gpsInfo.Sequence = 42;
            //gpsInfo.Longitude = "112.9885667";
            //gpsInfo.Latitude = "28.0732667";
            //gpsInfo.Direction = "正西";
            //gpsInfoList.Add(gpsInfo);
            //temp = new TrackBackGpsInfo();
            //temp.GpsInfo = gpsInfo;
            //tmpList.Add(temp);

            //gpsInfo = new GPSInfo();
            //gpsInfo.Sequence = 43;
            //gpsInfo.Longitude = "112.9849333";
            //gpsInfo.Latitude = "28.072705";
            //gpsInfo.Direction = "正西";
            //gpsInfoList.Add(gpsInfo);
            //temp = new TrackBackGpsInfo();
            //temp.GpsInfo = gpsInfo;
            //tmpList.Add(temp);

            //gpsInfo = new GPSInfo();
            //gpsInfo.Sequence = 44;
            //gpsInfo.Longitude = "112.9847833";
            //gpsInfo.Latitude = "28.0726517";
            //gpsInfo.Direction = "西南";
            //gpsInfoList.Add(gpsInfo);
            //temp = new TrackBackGpsInfo();
            //temp.GpsInfo = gpsInfo;
            //tmpList.Add(temp);

            //gpsInfo = new GPSInfo();
            //gpsInfo.Sequence = 45;
            //gpsInfo.Longitude = "112.98505";
            //gpsInfo.Latitude = "28.0700567";
            //gpsInfo.Direction = "正南";
            //gpsInfoList.Add(gpsInfo);
            //temp = new TrackBackGpsInfo();
            //temp.GpsInfo = gpsInfo;
            //tmpList.Add(temp);

            //gpsInfo = new GPSInfo();
            //gpsInfo.Sequence = 46;
            //gpsInfo.Longitude = "112.9856167";
            //gpsInfo.Latitude = "28.0669967";
            //gpsInfo.Direction = "正西";
            //gpsInfoList.Add(gpsInfo);
            //temp = new TrackBackGpsInfo();
            //temp.GpsInfo = gpsInfo;
            //tmpList.Add(temp);

            //gpsInfo = new GPSInfo();
            //gpsInfo.Sequence = 47;
            //gpsInfo.Longitude = "112.9814667";
            //gpsInfo.Latitude = "28.06572";
            //gpsInfo.Direction = "正西";
            //gpsInfoList.Add(gpsInfo);
            //temp = new TrackBackGpsInfo();
            //temp.GpsInfo = gpsInfo;
            //tmpList.Add(temp);

            //gpsInfo = new GPSInfo();
            //gpsInfo.Sequence = 48;
            //gpsInfo.Longitude = "112.9773167";
            //gpsInfo.Latitude = "28.0652967";
            //gpsInfo.Direction = "正西";
            //gpsInfoList.Add(gpsInfo);
            //temp = new TrackBackGpsInfo();
            //temp.GpsInfo = gpsInfo;
            //tmpList.Add(temp);

            //gpsInfo = new GPSInfo();
            //gpsInfo.Sequence = 49;
            //gpsInfo.Longitude = "112.9752667";
            //gpsInfo.Latitude = "28.0635033";
            //gpsInfo.Direction = "西南";
            //gpsInfoList.Add(gpsInfo);
            //temp = new TrackBackGpsInfo();
            //temp.GpsInfo = gpsInfo;
            //tmpList.Add(temp);

            //gpsInfo = new GPSInfo();
            //gpsInfo.Sequence = 50;
            //gpsInfo.Longitude = "112.9737833";
            //gpsInfo.Latitude = "28.060325";
            //gpsInfo.Direction = "西南";
            //gpsInfoList.Add(gpsInfo);
            //temp = new TrackBackGpsInfo();
            //temp.GpsInfo = gpsInfo;
            //tmpList.Add(temp);

            //gpsInfo = new GPSInfo();
            //gpsInfo.Sequence = 51;
            //gpsInfo.Longitude = "112.9723333";
            //gpsInfo.Latitude = "28.0578233";
            //gpsInfo.Direction = "西南";
            //gpsInfoList.Add(gpsInfo);
            //temp = new TrackBackGpsInfo();
            //temp.GpsInfo = gpsInfo;
            //tmpList.Add(temp);

            //gpsInfo = new GPSInfo();
            //gpsInfo.Sequence = 52;
            //gpsInfo.Longitude = "112.97015";
            //gpsInfo.Latitude = "28.0542417";
            //gpsInfo.Direction = "西南";
            //gpsInfoList.Add(gpsInfo);
            //temp = new TrackBackGpsInfo();
            //temp.GpsInfo = gpsInfo;
            //tmpList.Add(temp);

            ////List<TrackBackGpsInfo> tmpList = trackPlayVM.ListVehicleInfo;
            //trackPlayVM.ListVehicleInfo = null;
            //trackPlayVM.ListVehicleInfo = tmpList;
            ////TrackPlayViewModel trackPlayVM = TrackPlayViewModel.GetInstance();
            ////trackPlayVM.ListVehicleInfo = gpsInfoList;
            #endregion

            if (!CheckQueryCondition(trackPlayVM))
            {
                return;
            }
            /*查询条件*/
            string vehicleId = trackPlayVM.SelectedVehicle.Name;
            string SIM = trackPlayVM.SelectedVehicle.SIM;//"013477294904"; //
            string startTime = trackPlayVM.BeginTime.ToString("yyyy-MM-dd HH:mm:ss");
            string endTime = trackPlayVM.EndTime.ToString("yyyy-MM-dd HH:mm:ss");

            try
            {

                StaticLoginInfo loginInfo = StaticLoginInfo.GetInstance();
                string sql = "select * from GpsBasic where simId='" + SIM + "' and recordtime between '" + startTime + "' and '" + endTime + "' order by recordtime asc";
                string jsonStr = "error";
                DateTimeFormatInfo dtFormat = new DateTimeFormatInfo();

                dtFormat.ShortDatePattern = "yyyy-MM-dd HH:mm:sss";
                DateTime dts = Convert.ToDateTime(startTime, dtFormat);
                DateTime dte = Convert.ToDateTime(endTime, dtFormat);
                TimeSpan ts = dte - dts;
                if (ts.Days > 7)
                {
                    MessageBox.Show("查询时间长度不能超过7天！");
                    return;
                }
                try
                {
                    jsonStr = VehicleCommon.wcfDBHelper.YExecuteReportSql(loginInfo.UserName, dts, dte, sql, "0");
                    int i = 0;
                }
                catch (Exception)
                {
                    MessageBox.Show("查询数据库错误！", "历史轨迹", MessageBoxButton.OKCancel);
                    trackPlayVM.ListVehicleInfo = null;
                    return;
                }
                if (jsonStr == "error")
                {
                    MessageBox.Show("查询数据库错误！", "历史轨迹", MessageBoxButton.OKCancel);
                    trackPlayVM.ListVehicleInfo = null;
                    return;
                }

                if (jsonStr.Length > 10)
                {
                    trackPlayVM.ListVehicleInfo = DeserializeTrackGpsInfoList(jsonStr);

                    List<TrackBackGpsInfo> tmpList = trackPlayVM.ListVehicleInfo;
                    trackPlayVM.ListVehicleInfo = null;
                    trackPlayVM.ListVehicleInfo = tmpList;
                }
                else
                {
                    MessageBox.Show("在该时间段没有该车历史数据！", "历史轨迹", MessageBoxButton.OKCancel);
                    trackPlayVM.ListVehicleInfo = null;
                }
            }
            catch (Exception e)
            {
                MessageBox.Show("查询历史轨迹信息失败，请重试", "历史轨迹", MessageBoxButton.OKCancel);
                trackPlayVM.ListVehicleInfo = null;
            }
        }
        public void GetTrackPlayVehicleGPSInfo_old(TrackPlayViewModel trackPlayVM)
        {
            #region 测试数据
            //List<GPSInfo> gpsInfoList = new List<GPSInfo>();
            //GPSInfo gpsInfo;
            //gpsInfo = new GPSInfo();
            //gpsInfo.Sequence = 1;
            //gpsInfo.Longitude = "112.969667";
            //gpsInfo.Latitude = "28.040105";
            //gpsInfo.Direction = "正西";
            //gpsInfoList.Add(gpsInfo);

            //gpsInfo = new GPSInfo();
            //gpsInfo.Sequence = 2;
            //gpsInfo.Longitude = "112.9702667";
            //gpsInfo.Latitude = "28.040025";
            //gpsInfo.Direction = "正东";
            //gpsInfoList.Add(gpsInfo);

            //gpsInfo = new GPSInfo();
            //gpsInfo.Sequence = 3;
            //gpsInfo.Longitude = "112.9702167";
            //gpsInfo.Latitude = "28.0411383";
            //gpsInfo.Direction = "西北";
            //gpsInfoList.Add(gpsInfo);

            //gpsInfo = new GPSInfo();
            //gpsInfo.Sequence = 4;
            //gpsInfo.Longitude = "112.96985";
            //gpsInfo.Latitude = "28.042205";
            //gpsInfo.Direction = "正北";
            //gpsInfoList.Add(gpsInfo);

            //gpsInfo = new GPSInfo();
            //gpsInfo.Sequence = 5;
            //gpsInfo.Longitude = "112.9693333";
            //gpsInfo.Latitude = "28.0437067";
            //gpsInfo.Direction = "正北";
            //gpsInfoList.Add(gpsInfo);

            //gpsInfo = new GPSInfo();
            //gpsInfo.Sequence = 6;
            //gpsInfo.Longitude = "112.9682";
            //gpsInfo.Latitude = "28.0455667";
            //gpsInfo.Direction = "西北";
            //gpsInfoList.Add(gpsInfo);

            //gpsInfo = new GPSInfo();
            //gpsInfo.Sequence = 7;
            //gpsInfo.Longitude = "112.9673167";
            //gpsInfo.Latitude = "28.0470233";
            //gpsInfo.Direction = "正北";
            //gpsInfoList.Add(gpsInfo);

            //gpsInfo = new GPSInfo();
            //gpsInfo.Sequence = 8;
            //gpsInfo.Longitude = "112.9673333";
            //gpsInfo.Latitude = "28.0490667";
            //gpsInfo.Direction = "正北";
            //gpsInfoList.Add(gpsInfo);

            //gpsInfo = new GPSInfo();
            //gpsInfo.Sequence = 9;
            //gpsInfo.Longitude = "112.9684333";
            //gpsInfo.Latitude = "28.0511217";
            //gpsInfo.Direction = "东北";
            //gpsInfoList.Add(gpsInfo);

            //gpsInfo = new GPSInfo();
            //gpsInfo.Sequence = 10;
            //gpsInfo.Longitude = "112.969";
            //gpsInfo.Latitude = "28.0520867";
            //gpsInfo.Direction = "东北";
            //gpsInfoList.Add(gpsInfo);

            //gpsInfo = new GPSInfo();
            //gpsInfo.Sequence = 11;
            //gpsInfo.Longitude = "112.96965";
            //gpsInfo.Latitude = "28.0531967";
            //gpsInfo.Direction = "东北";
            //gpsInfoList.Add(gpsInfo);

            //gpsInfo = new GPSInfo();
            //gpsInfo.Sequence = 12;
            //gpsInfo.Longitude = "112.9711833";
            //gpsInfo.Latitude = "28.0557783";
            //gpsInfo.Direction = "东北";
            //gpsInfoList.Add(gpsInfo);

            //gpsInfo = new GPSInfo();
            //gpsInfo.Sequence = 13;
            //gpsInfo.Longitude = "112.9726667";
            //gpsInfo.Latitude = "28.0582733";
            //gpsInfo.Direction = "东北";
            //gpsInfoList.Add(gpsInfo);

            //gpsInfo = new GPSInfo();
            //gpsInfo.Sequence = 14;
            //gpsInfo.Longitude = "112.9740833";
            //gpsInfo.Latitude = "28.060665";
            //gpsInfo.Direction = "东北";
            //gpsInfoList.Add(gpsInfo);

            //gpsInfo = new GPSInfo();
            //gpsInfo.Sequence = 15;
            //gpsInfo.Longitude = "112.9755667";
            //gpsInfo.Latitude = "28.0636133";
            //gpsInfo.Direction = "正北";
            //gpsInfoList.Add(gpsInfo);

            //gpsInfo = new GPSInfo();
            //gpsInfo.Sequence = 16;
            //gpsInfo.Longitude = "112.97705";
            //gpsInfo.Latitude = "28.06679";
            //gpsInfo.Direction = "正北";
            //gpsInfoList.Add(gpsInfo);

            //gpsInfo = new GPSInfo();
            //gpsInfo.Sequence = 17;
            //gpsInfo.Longitude = "112.9780833";
            //gpsInfo.Latitude = "28.069595";
            //gpsInfo.Direction = "正北";
            //gpsInfoList.Add(gpsInfo);

            //gpsInfo = new GPSInfo();
            //gpsInfo.Sequence = 18;
            //gpsInfo.Longitude = "112.9781833";
            //gpsInfo.Latitude = "28.069845";
            //gpsInfo.Direction = "正北";
            //gpsInfoList.Add(gpsInfo);

            //gpsInfo = new GPSInfo();
            //gpsInfo.Sequence = 19;
            //gpsInfo.Longitude = "112.9794833";
            //gpsInfo.Latitude = "28.0725383";
            //gpsInfo.Direction = "东北";
            //gpsInfoList.Add(gpsInfo);

            //gpsInfo = new GPSInfo();
            //gpsInfo.Sequence = 20;
            //gpsInfo.Longitude = "112.9822667";
            //gpsInfo.Latitude = "28.07249";
            //gpsInfo.Direction = "正东";
            //gpsInfoList.Add(gpsInfo);

            //gpsInfo = new GPSInfo();
            //gpsInfo.Sequence = 21;
            //gpsInfo.Longitude = "112.9841667";
            //gpsInfo.Latitude = "28.0724967";
            //gpsInfo.Direction = "正东";
            //gpsInfoList.Add(gpsInfo);

            //gpsInfo = new GPSInfo();
            //gpsInfo.Sequence = 22;
            //gpsInfo.Longitude = "112.9850167";
            //gpsInfo.Latitude = "28.0726433";
            //gpsInfo.Direction = "正东";
            //gpsInfoList.Add(gpsInfo);

            //gpsInfo = new GPSInfo();
            //gpsInfo.Sequence = 23;
            //gpsInfo.Longitude = "112.98915";
            //gpsInfo.Latitude = "28.0732383";
            //gpsInfo.Direction = "正东";
            //gpsInfoList.Add(gpsInfo);

            //gpsInfo = new GPSInfo();
            //gpsInfo.Sequence = 24;
            //gpsInfo.Longitude = "112.9929";
            //gpsInfo.Latitude = "28.0739967";
            //gpsInfo.Direction = "正东";
            //gpsInfoList.Add(gpsInfo);

            //gpsInfo = new GPSInfo();
            //gpsInfo.Sequence = 25;
            //gpsInfo.Longitude = "112.99705";
            //gpsInfo.Latitude = "28.0750033";
            //gpsInfo.Direction = "正东";
            //gpsInfoList.Add(gpsInfo);

            //gpsInfo = new GPSInfo();
            //gpsInfo.Sequence = 26;
            //gpsInfo.Longitude = "113.0019";
            //gpsInfo.Latitude = "28.0760917";
            //gpsInfo.Direction = "正东";
            //gpsInfoList.Add(gpsInfo);

            //gpsInfo = new GPSInfo();
            //gpsInfo.Sequence = 27;
            //gpsInfo.Longitude = "113.0069167";
            //gpsInfo.Latitude = "28.07709";
            //gpsInfo.Direction = "正东";
            //gpsInfoList.Add(gpsInfo);

            //gpsInfo = new GPSInfo();
            //gpsInfo.Sequence = 28;
            //gpsInfo.Longitude = "113.0075";
            //gpsInfo.Latitude = "28.0787517";
            //gpsInfo.Direction = "正北";
            //gpsInfoList.Add(gpsInfo);

            //gpsInfo = new GPSInfo();
            //gpsInfo.Sequence = 29;
            //gpsInfo.Longitude = "113.0067167";
            //gpsInfo.Latitude = "28.0830083";
            //gpsInfo.Direction = "正北";
            //gpsInfoList.Add(gpsInfo);

            //gpsInfo = new GPSInfo();
            //gpsInfo.Sequence = 30;
            //gpsInfo.Longitude = "113.0063167";
            //gpsInfo.Latitude = "28.0861767";
            //gpsInfo.Direction = "正北";
            //gpsInfoList.Add(gpsInfo);

            //gpsInfo = new GPSInfo();
            //gpsInfo.Sequence = 31;
            //gpsInfo.Longitude = "113.0065333";
            //gpsInfo.Latitude = "28.0901517";
            //gpsInfo.Direction = "正北";
            //gpsInfoList.Add(gpsInfo);

            //gpsInfo = new GPSInfo();
            //gpsInfo.Sequence = 32;
            //gpsInfo.Longitude = "113.0067167";
            //gpsInfo.Latitude = "28.0940083";
            //gpsInfo.Direction = "正北";
            //gpsInfoList.Add(gpsInfo);

            //gpsInfo = new GPSInfo();
            //gpsInfo.Sequence = 33;
            //gpsInfo.Longitude = "113.0064333";
            //gpsInfo.Latitude = "28.0926117";
            //gpsInfo.Direction = "正南";
            //gpsInfoList.Add(gpsInfo);

            //gpsInfo = new GPSInfo();
            //gpsInfo.Sequence = 34;
            //gpsInfo.Longitude = "113.0062833";
            //gpsInfo.Latitude = "28.090505";
            //gpsInfo.Direction = "正南";
            //gpsInfoList.Add(gpsInfo);

            //gpsInfo = new GPSInfo();
            //gpsInfo.Sequence = 35;
            //gpsInfo.Longitude = "113.0061333";
            //gpsInfo.Latitude = "28.087665";
            //gpsInfo.Direction = "正南";
            //gpsInfoList.Add(gpsInfo);

            //gpsInfo = new GPSInfo();
            //gpsInfo.Sequence = 36;
            //gpsInfo.Longitude = "113.0062667";
            //gpsInfo.Latitude = "28.08389";
            //gpsInfo.Direction = "正南";
            //gpsInfoList.Add(gpsInfo);

            //gpsInfo = new GPSInfo();
            //gpsInfo.Sequence = 37;
            //gpsInfo.Longitude = "113.0070167";
            //gpsInfo.Latitude = "28.0797733";
            //gpsInfo.Direction = "正南";
            //gpsInfoList.Add(gpsInfo);

            //gpsInfo = new GPSInfo();
            //gpsInfo.Sequence = 38;
            //gpsInfo.Longitude = "113.0061333";
            //gpsInfo.Latitude = "28.0770067";
            //gpsInfo.Direction = "正西";
            //gpsInfoList.Add(gpsInfo);

            //gpsInfo = new GPSInfo();
            //gpsInfo.Sequence = 39;
            //gpsInfo.Longitude = "113.00165";
            //gpsInfo.Latitude = "28.076095";
            //gpsInfo.Direction = "正西";
            //gpsInfoList.Add(gpsInfo);

            //gpsInfo = new GPSInfo();
            //gpsInfo.Sequence = 40;
            //gpsInfo.Longitude = "112.9968";
            //gpsInfo.Latitude = "28.0750217";
            //gpsInfo.Direction = "正西";
            //gpsInfoList.Add(gpsInfo);

            //gpsInfo = new GPSInfo();
            //gpsInfo.Sequence = 41;
            //gpsInfo.Longitude = "112.9924";
            //gpsInfo.Latitude = "28.0739967";
            //gpsInfo.Direction = "正西";
            //gpsInfoList.Add(gpsInfo);

            //gpsInfo = new GPSInfo();
            //gpsInfo.Sequence = 42;
            //gpsInfo.Longitude = "112.9885667";
            //gpsInfo.Latitude = "28.0732667";
            //gpsInfo.Direction = "正西";
            //gpsInfoList.Add(gpsInfo);

            //gpsInfo = new GPSInfo();
            //gpsInfo.Sequence = 43;
            //gpsInfo.Longitude = "112.9849333";
            //gpsInfo.Latitude = "28.072705";
            //gpsInfo.Direction = "正西";
            //gpsInfoList.Add(gpsInfo);

            //gpsInfo = new GPSInfo();
            //gpsInfo.Sequence = 44;
            //gpsInfo.Longitude = "112.9847833";
            //gpsInfo.Latitude = "28.0726517";
            //gpsInfo.Direction = "西南";
            //gpsInfoList.Add(gpsInfo);

            //gpsInfo = new GPSInfo();
            //gpsInfo.Sequence = 45;
            //gpsInfo.Longitude = "112.98505";
            //gpsInfo.Latitude = "28.0700567";
            //gpsInfo.Direction = "正南";
            //gpsInfoList.Add(gpsInfo);

            //gpsInfo = new GPSInfo();
            //gpsInfo.Sequence = 46;
            //gpsInfo.Longitude = "112.9856167";
            //gpsInfo.Latitude = "28.0669967";
            //gpsInfo.Direction = "正西";
            //gpsInfoList.Add(gpsInfo);

            //gpsInfo = new GPSInfo();
            //gpsInfo.Sequence = 47;
            //gpsInfo.Longitude = "112.9814667";
            //gpsInfo.Latitude = "28.06572";
            //gpsInfo.Direction = "正西";
            //gpsInfoList.Add(gpsInfo);

            //gpsInfo = new GPSInfo();
            //gpsInfo.Sequence = 48;
            //gpsInfo.Longitude = "112.9773167";
            //gpsInfo.Latitude = "28.0652967";
            //gpsInfo.Direction = "正西";
            //gpsInfoList.Add(gpsInfo);

            //gpsInfo = new GPSInfo();
            //gpsInfo.Sequence = 49;
            //gpsInfo.Longitude = "112.9752667";
            //gpsInfo.Latitude = "28.0635033";
            //gpsInfo.Direction = "西南";
            //gpsInfoList.Add(gpsInfo);

            //gpsInfo = new GPSInfo();
            //gpsInfo.Sequence = 50;
            //gpsInfo.Longitude = "112.9737833";
            //gpsInfo.Latitude = "28.060325";
            //gpsInfo.Direction = "西南";
            //gpsInfoList.Add(gpsInfo);

            //gpsInfo = new GPSInfo();
            //gpsInfo.Sequence = 51;
            //gpsInfo.Longitude = "112.9723333";
            //gpsInfo.Latitude = "28.0578233";
            //gpsInfo.Direction = "西南";
            //gpsInfoList.Add(gpsInfo);

            //gpsInfo = new GPSInfo();
            //gpsInfo.Sequence = 52;
            //gpsInfo.Longitude = "112.97015";
            //gpsInfo.Latitude = "28.0542417";
            //gpsInfo.Direction = "西南";
            //gpsInfoList.Add(gpsInfo);

            //TrackPlayViewModel trackPlayVM = TrackPlayViewModel.GetInstance();
            //trackPlayVM.ListVehicleInfo = gpsInfoList;
            #endregion
            if (!CheckQueryCondition(trackPlayVM))
            {
                return;
            }
            /*查询条件*/
            string vehicleNum = trackPlayVM.SelectedVehicle.ID;
            string startTime = trackPlayVM.BeginTime.ToString("yyyy-MM-dd HH:mm:ss");
            string endTime = trackPlayVM.EndTime.ToString("yyyy-MM-dd HH:mm:ss");

            try
            {
                Random urlRandom = new Random();
                string randomStr = urlRandom.Next().ToString() + DateTime.Now.ToString() + urlRandom.Next().ToString();
                Uri endpoint = new Uri(VehicleConfig.GetInstance().URL_REMOTE_LASTESTTRACKINFOWEB + "?vehicleNum=" + vehicleNum + "&startTime=" + startTime + "&endTime=" + endTime + "&time=" + randomStr + "&mapType=baidu&apFlag=1");
                WebClient client = new WebClient();
                client.Encoding = System.Text.Encoding.UTF8;
                string xmlStr = client.DownloadString(endpoint);
                if ((xmlStr != null) && (!xmlStr.Equals("")))
                {
                    trackPlayVM.ListVehicleInfo = DeserializeTrackGpsInfoList(xmlStr);
                    #region 由于web接口改变2014/3/8注释地理解析
                    ///*启动地理解析和坐标转换线程*/
                    //int oneThreadNum = 0;
                    //if (this.TrackPlayAllCount < this.TrackPlayParseThreadNum)
                    //{
                    //    this.TrackPlayParseThreadNum = 1;
                    //    oneThreadNum = this.TrackPlayAllCount;
                    //}
                    //else
                    //{
                    //    oneThreadNum = this.TrackPlayAllCount / this.TrackPlayParseThreadNum;
                    //}

                    //for (int i = 0; i < this.TrackPlayParseThreadNum; i++)
                    //{
                    //    int startIndex = i * oneThreadNum;
                    //    int endIndex = (i + 1) * oneThreadNum;
                    //    Thread parseThread = new Thread(delegate() { this.ParseTrackBackAddressPointThread(startIndex, endIndex); });
                    //    parseThread.Start();
                    //}
                    //if (this.TrackPlayAllCount % this.TrackPlayParseThreadNum != 0)
                    //{
                    //    int startIndex = this.TrackPlayParseThreadNum * oneThreadNum;
                    //    int endIndex = this.TrackPlayAllCount;
                    //    Thread parseThread = new Thread(delegate() { this.ParseTrackBackAddressPointThread(startIndex, endIndex); });
                    //    parseThread.Start();
                    //}

                    //int lastCounter = 0;
                    //int sameTimes = 0;
                    //while (this.TrackPlayCounter != this.TrackPlayAllCount)
                    //{
                    //    Thread.Sleep(1000);
                    //    if (lastCounter == this.TrackPlayCounter)
                    //    {
                    //        if (++sameTimes == 3)
                    //        {
                    //            break;
                    //        }
                    //    }
                    //    else
                    //    {
                    //        lastCounter = this.TrackPlayCounter;
                    //    }
                    //}
                    #endregion
                    List<TrackBackGpsInfo> tmpList = trackPlayVM.ListVehicleInfo;
                    trackPlayVM.ListVehicleInfo = null;
                    trackPlayVM.ListVehicleInfo = tmpList;
                }
                else if (xmlStr.Equals(""))
                {
                    MessageBox.Show("在该时间段没有该车历史数据！", "历史轨迹", MessageBoxButton.OKCancel);
                    trackPlayVM.ListVehicleInfo = null;
                }
            }
            catch (Exception e)
            {
                MessageBox.Show("查询历史轨迹信息失败，请重试", "历史轨迹", MessageBoxButton.OKCancel);
                trackPlayVM.ListVehicleInfo = null;
            }
        }
        /*
       * 函数名称：DeserializeTrackGpsInfoList(string xml)
       * 函数参数： xml 字符串
       * 作者：wangyi
       *	时间：2013/09/25
       */
        public List<TrackBackGpsInfo> DeserializeTrackGpsInfoList(string json)
        {
            List<TrackBackGpsInfo> trackBackGpsInfoList = new List<TrackBackGpsInfo>();
            GPSInfo gpsInfo;
            TrackBackGpsInfo trackBackGpsInfo;
            int sequence = 0;//序号
            //BMapLngLat bm = new BMapLngLat();//gps转百度
            JArray arry = (JArray)JsonConvert.DeserializeObject(json);
            if (arry.Count == 0)
            {
                this.TrackPlayAllCount = trackBackGpsInfoList.Count;//轨迹条数
                return null;
            }

            StaticDetailInfo detailInfo = StaticDetailInfo.GetInstance();
            for (int i = 0; i < arry.Count; i++)
            {
                trackBackGpsInfo = new TrackBackGpsInfo();
                gpsInfo = new GPSInfo();
                trackBackGpsInfo.GpsInfo = gpsInfo;

                JObject content = (JObject)JsonConvert.DeserializeObject(arry[i].ToString());
                gpsInfo.Altitude = content["altitude"] == null ? "" : content["altitude"].ToString();
                gpsInfo.DevSpeed = content["devSpeed"] == null ? "" : content["devSpeed"].ToString();
                double speed = 0;
                bool result = double.TryParse(gpsInfo.DevSpeed, out speed);
                if (!result)
                {
                    gpsInfo.DevSpeed = "";
                }
                else
                {
                    if (speed < 0)
                    {
                        gpsInfo.DevSpeed = "";
                    }
                }
                gpsInfo.Direction = content["direction"] == null ? "" : translateDirection(content["direction"].ToString());
                gpsInfo.GPSMileage = content["gpsmileage"] == null ? "" : content["gpsmileage"].ToString();
                result = double.TryParse(gpsInfo.GPSMileage, out speed);
                if (!result)
                {
                    gpsInfo.GPSMileage = "";
                }
                else
                {
                    if (speed < 0)
                    {
                        gpsInfo.GPSMileage = "";
                    }
                    else
                    {
                        gpsInfo.GPSMileage = Math.Round(speed, 2) + "";
                    }
                }
                gpsInfo.Latitude = content["latitudeoffset"] == null ? "" : content["latitudeoffset"].ToString();
                gpsInfo.Longitude = content["longitudeoffset"] == null ? "" : content["longitudeoffset"].ToString();
                gpsInfo.OilVolumn = content["oilVolumn"] == null ? "" : content["oilVolumn"].ToString();
                gpsInfo.Sim = content["simId"] == null ? "" : content["simId"].ToString();
                gpsInfo.Speed = content["speed"] == null ? "" : content["speed"].ToString();
                gpsInfo.Datetime = content["recordtime"] == null ? "" : content["recordtime"].ToString();
                gpsInfo.InsertTime = content["inserttime"] == null ? "" : content["inserttime"].ToString();
                gpsInfo.CurLocation = content["POI"] == null ? "" : content["POI"].ToString();
                string WarnSign = content["warninfo"] == null ? "" : content["warninfo"].ToString();
                if (!WarnSign.Equals(""))
                {//报警
                    ResolveGpsStatusWarn.resolveGpsInfoWarn(gpsInfo, WarnSign);
                }
                string StatusSign = content["statusinfo"] == null ? "" : content["statusinfo"].ToString();
                if (!StatusSign.Equals(""))
                {//状态
                    ResolveGpsStatusWarn.resolveGpsInfoStatus(gpsInfo, StatusSign);
                }
                gpsInfo.StopTrackTime = content["stopTime"] == null ? "" : content["stopTime"].ToString().Split('#')[0];
                gpsInfo.OnlineStates = getVehicleOnlinestates(gpsInfo.Speed, gpsInfo.Datetime, gpsInfo);
                gpsInfo.isCheckedFlag = false;
                gpsInfo.Sequence = ++sequence;
                trackBackGpsInfo.CurrentLocation = gpsInfo.CurLocation;
                trackBackGpsInfoList.Add(trackBackGpsInfo);

            }
            this.TrackPlayAllCount = trackBackGpsInfoList.Count;//轨迹条数
            return trackBackGpsInfoList;
        }
        /*根据停车开始时间和停车时长获得停车结束时间*/
        private string GetEndTimeFromStartTimeAndSpan(string startTime, string spanStr)
        {
            if (spanStr == "")
            {//如果没有停车，停车结束时间就是停车开始时间
                return startTime;
            }
            DateTime startDt = DateTime.Parse(startTime);
            int index = spanStr.IndexOf("小时");
            if (index > 0)
            {
                double hours = double.Parse(spanStr.Substring(0, index));
                startDt = startDt.AddHours(hours);
                spanStr = spanStr.Substring(index + 2);
            }
            index = spanStr.IndexOf("分");
            if (index > 0)
            {
                double minutes = double.Parse(spanStr.Substring(0, index));
                startDt = startDt.AddMinutes(minutes);
                spanStr = spanStr.Substring(index + 1);
            }
            index = spanStr.IndexOf("秒");
            if (index > 0)
            {
                double seconds = double.Parse(spanStr.Substring(0, index));
                startDt = startDt.AddSeconds(seconds);
            }
            return startDt.ToString();
        }

        #region 由于web接口改变2014/3/8注释地理解析
        ///*获取历史轨迹gps信息根据经纬度解析地址*/
        //private void ParseTrackBackAddressPointThread(int startIndex, int endIndex)
        //{
        //    WebClient webClient = new WebClient();
        //    webClient.Encoding = System.Text.Encoding.UTF8;
        //    BaiDuPoint bdPoint = new BaiDuPoint();
        //    BaiDuAddr bdAddr = new BaiDuAddr();
        //    Uri endpoint;
        //    /*逆地址解析*/
        //    TrackPlayViewModel trackPlayVM = TrackPlayViewModel.GetInstance();
        //    for (int i = startIndex; i < endIndex; i++)
        //    {
        //        TrackBackGpsInfo trackBackGpsInfo = trackPlayVM.ListVehicleInfo[i];
        //        GPSInfo gpsInfo = trackBackGpsInfo.GpsInfo;
        //        try
        //        {
        //            string lat = gpsInfo.Latitude;
        //            string lng = gpsInfo.Longitude;
        //            /*坐标转换*/
        //            endpoint = new Uri(VehicleConfig.GetInstance().URL_POINTTRANS + "&x=" + lng + "&y=" + lat);
        //            string pointStr = webClient.DownloadString(endpoint);
        //            if (pointStr != "")
        //            {
        //                bdPoint = JsonConvert.DeserializeObject<BaiDuPoint>(pointStr);
        //                if (bdPoint.error == "0")
        //                {
        //                    byte[] buf;
        //                    buf = Convert.FromBase64String(bdPoint.x);
        //                    gpsInfo.Longitude = Encoding.UTF8.GetString(buf, 0, buf.Length);
        //                    buf = Convert.FromBase64String(bdPoint.y);
        //                    gpsInfo.Latitude = Encoding.UTF8.GetString(buf, 0, buf.Length);
        //                }
        //            }
        //            /*地址解析*/
        //            endpoint = new Uri(VehicleConfig.GetInstance().URL_PARSEADDRESS + "&location=" + lat + "," + lng + "&output=json");
        //            string addrStr = webClient.DownloadString(endpoint);
        //            if (addrStr != "")
        //            {
        //                bdAddr = JsonConvert.DeserializeObject<BaiDuAddr>(addrStr);
        //                if (bdAddr.status == "0")
        //                {
        //                    trackBackGpsInfo.CurrentLocation = bdAddr.result.formatted_address;
        //                    gpsInfo.CurLocation = trackBackGpsInfo.CurrentLocation;
        //                }
        //            }
        //        }
        //        catch
        //        {

        //        }
        //        finally
        //        {
        //            Monitor.Enter(this.TrackPlayCounterMonitor);
        //            this.TrackPlayCounter++;
        //            Monitor.Exit(this.TrackPlayCounterMonitor);
        //        }
        //    }
        //}
        #endregion

        #endregion

        #region 百度坐标转换和地址解析结构
        public class BaiDuPoint
        {
            public string error { get; set; }
            public string x { get; set; }
            public string y { get; set; }
        }
        public class BaiDuAddr
        {
            public string status { get; set; }
            public BaiDuResult result { get; set; }
        }
        public class BaiDuResult
        {
            public BaiDuAddrLocation location { get; set; }
            public string formatted_address { get; set; }
            public string business { get; set; }
            public BaidDuAddrComponent addressComponent { get; set; }
        }
        public class BaiDuAddrLocation
        {
            public string lat { get; set; }
            public string lng { get; set; }
        }
        public class BaidDuAddrComponent
        {
            public string city { get; set; }
            public string district { get; set; }
            public string province { get; set; }
            public string street { get; set; }
            public string street_number { get; set; }
        }
        #endregion

        #region 获取离线信息（异常查询）
        public void GetOfflineVehicleThread(RealTimeTreeViewModel realTimeTreeViewModel)
        {
            //Thread nThread = new Thread(delegate() { this.GetVehicleInfoByVehicleID(realTimeTreeViewModel); });
            //nThread.Start();
        }
        #endregion
    }


}

