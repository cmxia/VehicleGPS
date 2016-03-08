using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;

namespace VehicleGPS.Models
{
    class VehicleConfig
    {
        private static VehicleConfig instance;
        private VehicleConfig()
        {
            InitResourceConfig();
            RealConfig();
            InitServerConfig();
        }
        public static VehicleConfig GetInstance()
        {
            if (instance == null)
            {
                instance = new VehicleConfig();
            }
            return instance;
        }
        #region 读取配置
        private void RealConfig()
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(this.configPath);    //加载Xml文件  
            XmlElement rootElem = doc.DocumentElement;   //获取根节点  
            XmlNodeList personNodes = rootElem.GetElementsByTagName("server"); //获取person子节点集合  
            foreach (XmlNode node in personNodes)
            {
                string strName = ((XmlElement)node).GetAttribute("name");   //获取name属性值  
                XmlNodeList ipNode = ((XmlElement)node).GetElementsByTagName("ip");  //获取子XmlElement集合  
                XmlNodeList portNode = ((XmlElement)node).GetElementsByTagName("port");  //获取子XmlElement集合  
                if (strName.Equals("web"))
                {
                    this.WEBIP = ipNode[0].InnerText;
                    this.WEBPORT = portNode[0].InnerText;
                }
                else if (strName.Equals("business"))
                {
                    this.BUSINESSIP = ipNode[0].InnerText;
                    this.BUSINESSPORT = portNode[0].InnerText;
                }
                else if (strName.Equals("instruction"))
                {
                    this.INSTRUCTIONIP = ipNode[0].InnerText;
                    this.INSTRUCTIONPORT = portNode[0].InnerText;
                }
            }
        }
        #endregion

        #region 写配置
        public void WriteConfig()
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(this.configPath);    //加载Xml文件  
            XmlElement rootElem = doc.DocumentElement;   //获取根节点  
            XmlNodeList personNodes = rootElem.GetElementsByTagName("server"); //获取person子节点集合  
            foreach (XmlNode node in personNodes)
            {
                string strName = ((XmlElement)node).GetAttribute("name");   //获取name属性值  
                XmlNodeList ipNode = ((XmlElement)node).GetElementsByTagName("ip");  //获取子XmlElement集合  
                XmlNodeList portNode = ((XmlElement)node).GetElementsByTagName("port");  //获取子XmlElement集合  
                if (strName.Equals("web"))
                {
                    ipNode[0].InnerText = this.WEBIP;
                    portNode[0].InnerText = this.WEBPORT;
                }
                else if (strName.Equals("business"))
                {
                    ipNode[0].InnerText = this.BUSINESSIP;
                    portNode[0].InnerText = this.BUSINESSPORT;
                }
                else if (strName.Equals("instruction"))
                {
                    ipNode[0].InnerText = this.INSTRUCTIONIP;
                    portNode[0].InnerText = this.INSTRUCTIONPORT;
                }
            }
            doc.Save(this.configPath);
        }
        #endregion

        private void InitServerConfig()
        {
            #region 服务器地址
            //WEBIP = "61.183.9.107";
            //WEBPORT = "81";
            //BUSINESSIP = "61.183.9.107";
            //BUSINESSPORT = "3331";
            //INSTRUCTIONIP = "61.183.9.107";
            //INSTRUCTIONPORT = "7779";
            string WEBIP_PORT_REMOTE = WEBIP + ":" + WEBPORT;
            string BUSINESSIP_PORT_REMOTE = BUSINESSIP + ":" + BUSINESSPORT;
            string INSTRUCTIONIP_PORT_REMOTE = INSTRUCTIONIP + ":" + INSTRUCTIONPORT;
            concrete_webServer_url = "http://" + WEBIP_PORT_REMOTE + "/vehiclegps/"; //web服务 
            concrete_businessServer_url = "http://" + BUSINESSIP_PORT_REMOTE + "/";//业务服务器地址
            CONCRETE_SEND_INSTRUCTION_WEB_URL = "http://" + INSTRUCTIONIP_PORT_REMOTE + "/fudum.aspx";//发送指令的访问服务地址
            CONCRETE_VEHICLEWARNINGINFO_WEB_URL = concrete_businessServer_url + "/Alarm/GetAlarmMsg.aspx";
            CONCRETE_VEHICLEWARNINGRELEASE_WEB_URL = concrete_businessServer_url + "/Alarm/ReleaseAlarm.aspx";
            URL_PARSEADDRESS = "http://api.map.baidu.com/geocoder/v2/?ak=6c497f51c06477544e5fa6e9bd68f7c3";
            URL_POINTTRANS = "http://api.map.baidu.com/ag/coord/convert?from=2&to=4";
            URL_REMOTE_RUNIMAGE = "http://" + WEBIP_PORT_REMOTE + "/VehicleGPS/Images";
            URL_REMOTE_LASTESTINFOWEB = "http://" + WEBIP_PORT_REMOTE + "/VehicleGPS/NavMonitorControl/ConcreteLatestVehicleInfoForSilverlight.aspx";
            URL_REMOTE_CLIENTBASICINFOWEB = "http://" + WEBIP_PORT_REMOTE + "/VehicleGPS/NavMonitorControl/GetVehicleCompanyForSLByUser.aspx";
            URL_REMOTE_VEHICLEBASICINFOWEB = "http://" + WEBIP_PORT_REMOTE + "/VehicleGPS/NavMonitorControl/GetVehicleInfoForSLByUser.aspx";
            URL_REMOTE_LASTESTTRACKINFOWEB = "http://" + WEBIP_PORT_REMOTE + "/VehicleGPS/NavMonitorControl/GetTrackVehicleInfoForSL.aspx";
            URL_REMOTE_VEHICLETRACKBYSIMWEB = "http://" + WEBIP_PORT_REMOTE + "/VehicleGPS/NavMonitorControl/GetLatestVehilceGpsBySIM.aspx";
            GETLATESTGPSINFOINTERVAL = 30;
            LONG_TIME_OFF_ONLINE_SPAN_HOURS = 30;
            VEHICLETRACKTIMEINTERVAL = 20;
            VEHICLEDISPATCHTIMEINTERVAL = 10000;
            VEHICLEWARNINTERVAL = 30;
            CONCRETE_SIM_ONLINE_WEB_URL = concrete_webServer_url + "NavMonitorControl/GetLatestVehilceGpsBySIM.aspx";
            POPUPWINDOWINTERVAL = 4;
            #endregion
        }
        private void InitResourceConfig()
        {
            #region 资源路径
#if DEBUG
            //string appPath = Environment.CurrentDirectory + "/../../";
            string appPath = Environment.CurrentDirectory;
            string appPath1 = AppDomain.CurrentDomain.BaseDirectory;
            passPath = appPath + "/MemoryLoginInfo.txt";
            directionImgPath = appPath + "/Map/Images/";
            showSettingPath = appPath + "/ShowSetting.xml";
            carImgPath = "pack://application:,,,/Images/Car/";
            warnSettingPath = appPath + "/WarnSetting.xml";
            helpChmPath = appPath + "/基于WPF的车辆监控与调度管理系统说明文档.chm";
            realTimeMapPath = appPath + "/Map/RealTimeMap.htm";
            instructionMapPath = appPath + "/Map/InstructionMap.htm";
            trackPlayMapPath = appPath + "/Map/TrackPlayMap.htm";
            vehicleTrackMapPath = appPath + "/Map/VehicleTrackMap.htm";
            siteMapPath = appPath + "/Map/SiteMap.htm";
            regionsearchMapPath = appPath + "/Map/RegionSearch.htm";
            regionMapPath = appPath + "/Map/regionMap.htm";
            reportMapPath = appPath + "/Map/reportMap.htm";
            configPath = appPath + "/config.xml";  //appPath + "config.xml";
            voicePath = appPath + "/Voice";
            warnSoundPathDefault = voicePath + "/alarmsound.wav";
            remindSoundPath = voicePath + "/remindsound.wav";
            warnsetPwdPath = appPath + "/conf/WarnPwd.xml";
            vehicleConfigPath = appPath + "/conf/VehicleConfig.xml";
            cachePath = appPath + "/cache";
            //configPath = appPath + "config.xml";  //appPath + "config.xml";
#else
            //string appPath = Environment.CurrentDirectory + "/../../";
            string appPath = Environment.CurrentDirectory;
            string appPath1 = AppDomain.CurrentDomain.BaseDirectory;
            passPath = appPath + "/MemoryLoginInfo.txt";
            directionImgPath = appPath + "/Map/Images/";
            showSettingPath = appPath + "/ShowSetting.xml";
            carImgPath = "pack://application:,,,/Images/Car/";
            warnSettingPath = appPath + "/WarnSetting.xml";
            helpChmPath = appPath + "/基于WPF的车辆监控与调度管理系统说明文档.chm";
            realTimeMapPath = appPath + "/Map/RealTimeMap.htm";
            instructionMapPath = appPath + "/Map/InstructionMap.htm";
            trackPlayMapPath = appPath + "/Map/TrackPlayMap.htm";
            vehicleTrackMapPath = appPath + "/Map/VehicleTrackMap.htm";
            siteMapPath = appPath + "/Map/SiteMap.htm";
            regionsearchMapPath = appPath + "/Map/RegionSearch.htm";
            regionMapPath = appPath + "/Map/regionMap.htm";
            reportMapPath = appPath + "/Map/reportMap.htm";
            configPath = appPath + "/config.xml";  //appPath + "config.xml";
            voicePath = appPath + "/Voice";
            warnSoundPathDefault = voicePath + "/alarmsound.wav";
            warnsetPwdPath = appPath + "/conf/WarnPwd.xml";
            //configPath = appPath + "config.xml";  //appPath + "config.xml";
#endif
            #endregion
            // string appPath = Environment.CurrentDirectory;
            // string str = appPath + "/Map/Images/ss.png";
            // passPath = appPath + "/Map/MemoryLoginInfo.txt";
            // directionImgPath = appPath + "/Map/Images/";
            // showSettingPath = appPath + "/ShowSetting.xml";
            // carImgPath = "pack://application:,,,/Images/Car/";
            //// carImgPath = appPath + "/Map/Images/";
            // warnSettingPath = appPath + "WarnSetting.xml";
            // warnSoundPath = appPath + "alarmsound.wav";
            // helpChmPath = appPath + "/基于WPF的车辆监控与调度管理系统说明文档.chm";
            // realTimeMapPath = appPath + "/Map/RealTimeMap.htm";
            // trackPlayMapPath = appPath + "/Map/TrackPlayMap.htm";
            // vehicleTrackMapPath = appPath + "/Map/VehicleTrackMap.htm";
            // regionMapPath = appPath + "/Map/regionMap.htm";
            // reportMapPath = appPath + "/Map/reportMap.htm";
            // configPath = appPath + "/Map/config.xml"; //appPath + "config.xml";
        }
        public void RefreshConfig()
        {
            InitServerConfig();
        }
        //web ip地址和端口号
        public string WEBIP;
        public string WEBPORT;
        public string concrete_webServer_url; //web服务  
        public string BUSINESSIP;
        public string BUSINESSPORT;
        public string concrete_businessServer_url;//业务服务器地址
        public string INSTRUCTIONIP;
        public string INSTRUCTIONPORT;
        public string CONCRETE_SEND_INSTRUCTION_WEB_URL;//发送指令的访问服务地址
        /*报警数据*/
        public string CONCRETE_VEHICLEWARNINGINFO_WEB_URL;
        /*解除报警*/
        public string CONCRETE_VEHICLEWARNINGRELEASE_WEB_URL;
        /*逆地址解析*/
        public string URL_PARSEADDRESS;
        /*坐标转换*/
        public string URL_POINTTRANS;
        /*获取车辆在线运行方向图片*/
        public string URL_REMOTE_RUNIMAGE;
        //获取登陆用户所有车辆最新gps信息
        public string URL_REMOTE_LASTESTINFOWEB;
        //获取用户关键信息
        public string URL_REMOTE_CLIENTBASICINFOWEB;
        //获取车辆关键信息
        public string URL_REMOTE_VEHICLEBASICINFOWEB;
        //获取车辆轨迹回放信息的服务地址
        public string URL_REMOTE_LASTESTTRACKINFOWEB;
        //根据Sim卡获取车辆的最新gps信息
        public string URL_REMOTE_VEHICLETRACKBYSIMWEB; //获取指定sim卡号在线车辆实时信息的服务地址"
        //刷新最新GPS信息时间间隔(秒)
        public int GETLATESTGPSINFOINTERVAL;
        //车辆长时间未上传自动的时间间隔判断标准(以小时来计算)ss
        public int LONG_TIME_OFF_ONLINE_SPAN_HOURS;
        //车辆跟踪获取车辆数据的事件间隔(秒)
        public int VEHICLETRACKTIMEINTERVAL;
        //车辆调度获取车辆调度信息的事件间隔(秒)
        public int VEHICLEDISPATCHTIMEINTERVAL;
        //定时获取报警信息的时间间隔（秒）
        public int VEHICLEWARNINTERVAL;
        //获取指定sim卡号在线车辆实时信息的服务地址
        public string CONCRETE_SIM_ONLINE_WEB_URL;
        /*冒泡窗口停留时间(秒)*/
        public int POPUPWINDOWINTERVAL;


        #region 资源路径
        public string passPath;
        public string directionImgPath;
        public string showSettingPath;
        public string carImgPath;
        public string warnSettingPath;
        public string warnSoundPath;
        public string remindSoundPath;
        public string warnSoundPathDefault;//默认的报警声音路径
        public string helpChmPath;
        public string realTimeMapPath;
        public string instructionMapPath;
        public string siteMapPath;
        public string regionsearchMapPath;
        public string trackPlayMapPath;
        public string vehicleTrackMapPath;
        public string regionMapPath;
        public string reportMapPath;
        public string configPath;
        public string warnsetPwdPath;
        public string vehicleConfigPath;
        public string voicePath;
        public string cachePath;

        #endregion
    }
}
