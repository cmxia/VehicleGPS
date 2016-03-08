using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.Prism.ViewModel;
using VehicleGPS.Models;
using Microsoft.Practices.Prism.Commands;
using System.Windows;
using System.Windows.Controls;
using VehicleGPS.Services.MonitorCentre.Instruction;
using VehicleGPS.Services;
using VehicleGPS.Services.MonitorCentre;
using System.Threading;
using Newtonsoft.Json.Linq;
using VehicleGPS.Views.Control.MonitorCentre.Instruction;
using Newtonsoft.Json;
using VehicleGPS.Models.Login;
using VehicleGPS.Models.MonitorCentre;
using System.Data;

namespace VehicleGPS.ViewModels.MonitorCentre.Instruction
{
    class InstructionViewModel : NotificationObject
    {
        public string InsType = null;//指令类型
        #region 区域属性
        private string timeFlag = "0";
        private string speedFlag = "0";
        private string InAlarmFlag = "0";
        private string InPlatformFlag = "0";
        private string OutAlamFlag = "0";
        private string OutPlatformFlag = "0";
        private string latitudeFlag = "0";
        private string longitudeFlag = "0";
        private string doorFlag = "0";
        private string socketFlag = "0";
        private string GNSSFlag = "0";
        private string setType = "0";

        private string latitudeText;//圆形中心纬度
        private string longitudeText;//圆形中心经度
        private string radiusText;//圆形半径
        private string llatitudeText;//矩形左上点纬度
        private string llongitudeText;//左上点经度
        private string rlatitudeText;//右下点纬度
        private string rlongitudeText;//右下点经度
        #endregion

        #region 构造函数
        public InstructionViewModel(WebBrowser webMap, string type)
        {
            InsType = type;
            this.WebMap = webMap;
            this.MapService = new InstructionMapService(this);
            this.GetGeoCommand = new DelegateCommand(this.GetGeoCommandExecute);
            this.ClearMarkerCommand = new DelegateCommand(this.ClearMarkerCommandExecute);
            this.SendInstructionCommand = new DelegateCommand(this.SendInstructionCommandExecute);
            this.SetPointCommand = new DelegateCommand(this.SetPointCommandExecute);
            InitVehicleInfo();

        }
        //将区域信息显示在地图上
        public void InitRegionInBaidu()
        {
            if (StaticTreeState.RegionBasicInfo == LoadingState.LOADCOMPLETE)
            {
                foreach (CRegionInfo regionInfo in StaticRegionInfo.GetInstance().ListRegionBasicInfo)
                {
                    if (regionInfo.Long == null || regionInfo.Long == "" || regionInfo.lat == null || regionInfo.lat == "")
                        continue;
                    this.MapService.addRegionByOne(regionInfo.zoneName, regionInfo.Long, regionInfo.lat, regionInfo.radio, "#f00");
                }
            }
        }
        #endregion

        #region 绑定车辆信息

        private void InitVehicleInfo()
        {
            this.VehicleId = VBaseInfo.GetInstance().VehicleId;
            this.SIM = VBaseInfo.GetInstance().SIM;
            this.EUSERNAME = VBaseInfo.GetInstance().EUSERNAME;
            this.States = "未发送";
        }

        private string vehicleid;
        public string VehicleId
        {
            get { return vehicleid; }
            set
            {
                vehicleid = value;
                this.RaisePropertyChanged("VehicleId");
            }
        }

        private string sim;
        public string SIM
        {
            get { return sim; }
            set
            {
                sim = value;
                this.RaisePropertyChanged("SIM");
            }
        }

        private string eusername;
        public string EUSERNAME
        {
            get { return eusername; }
            set
            {
                eusername = value;
                this.RaisePropertyChanged("EUSERNAME");
            }
        }

        private string states;
        public string States
        {
            get { return states; }
            set
            {
                states = value;
                this.RaisePropertyChanged("States");
            }
        }

        private string result;
        public string Result
        {
            get { return result; }
            set
            {
                result = value;
                this.RaisePropertyChanged("Result");
            }
        }

        #endregion

        #region 绑定地理信息
        private string lineinfo;

        public string LineInfo
        {
            get { return lineinfo; }
            set
            {
                lineinfo = value;
                this.RaisePropertyChanged("LineInfo");
            }
        }

        private string linepoints;

        public string LinePointsInfo
        {
            get { return linepoints; }
            set
            {
                linepoints = value;
                this.RaisePropertyChanged("LinePointsInfo");
            }
        }

        private string circleinfo;

        public string CircleInfo
        {
            get { return circleinfo; }
            set
            {
                circleinfo = value;
                this.RaisePropertyChanged("CircleInfo");
            }
        }
        private string rectinfo;

        public string RectInfo
        {
            get { return rectinfo; }
            set
            {
                rectinfo = value;
                this.RaisePropertyChanged("RectInfo");
            }
        }
        private string polyinfo;

        public string PolyInfo
        {
            get { return polyinfo; }
            set
            {
                polyinfo = value;
                this.RaisePropertyChanged("PolyInfo");
            }
        }
        private string polypointsinfo;

        public string PolyPointInfo
        {
            get { return polypointsinfo; }
            set
            {
                polypointsinfo = value;
                this.RaisePropertyChanged("PolyPointInfo");
            }
        }

        #endregion

        #region 绑定指令信息
        private bool isupdateregion = true;

        public bool IsUpdateRegion
        {
            get { return isupdateregion; }
            set
            {
                isupdateregion = value;
                this.RaisePropertyChanged("IsUpdateRegion");
            }
        }
        private bool isaddregion;

        public bool IsAddRegion
        {
            get { return isaddregion; }
            set
            {
                isaddregion = value;
                this.RaisePropertyChanged("IsAddRegion");
                if (isaddregion == true)
                {
                    setType = "1";
                }
            }
        }
        private bool ismodifyregion;

        public bool IsModify
        {
            get { return ismodifyregion; }
            set
            {
                ismodifyregion = value;
                this.RaisePropertyChanged("IsModify");
                if (ismodifyregion == true)
                {
                    setType = "2";
                }
            }
        }

        private bool isbasetime;

        public bool IsBaseTime
        {
            get
            {
                return isbasetime;
            }
            set
            {
                isbasetime = value;
                this.RaisePropertyChanged("IsBaseTime");
                if (isbasetime == true)
                {
                    this.TimeVisible = Visibility.Visible;
                    timeFlag = "1";
                }
                else
                {
                    this.TimeVisible = Visibility.Collapsed;
                    timeFlag = "0";
                }
            }
        }

        private bool islimitspeed;

        public bool IsLimitSpeed
        {
            get
            {
                return islimitspeed;
            }
            set
            {
                islimitspeed = value;
                this.RaisePropertyChanged("IsLimitSpeed");
                if (islimitspeed == true)
                {
                    this.SpeedVisible = Visibility.Visible;
                    speedFlag = "1";
                }
                else
                {
                    this.SpeedVisible = Visibility.Collapsed;
                    speedFlag = "0";
                }
            }
        }

        private Visibility timevisible = Visibility.Collapsed;
        public Visibility TimeVisible
        {
            get { return timevisible; }
            set
            {
                timevisible = value;
                this.RaisePropertyChanged("TimeVisible");
            }
        }

        private Visibility speedvisible = Visibility.Collapsed;
        public Visibility SpeedVisible
        {
            get { return speedvisible; }
            set
            {
                speedvisible = value;
                this.RaisePropertyChanged("SpeedVisible");
            }
        }

        private Visibility setpoint = Visibility.Collapsed;
        public Visibility SetPoint
        {
            get { return setpoint; }
            set
            {
                setpoint = value;
                this.RaisePropertyChanged("SetPoint");
            }
        }

        //开始时间
        private string begintime = DateTime.Now.AddDays(-2).ToString("yyyy-MM-dd HH:mm:ss");//开始时间为两天前

        public string BeginTime
        {
            get { return begintime; }
            set
            {
                begintime = value;
                this.RaisePropertyChanged("BeginTime");
            }
        }

        //结束时间
        private string endtime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        public string EndTime
        {
            get { return endtime; }
            set
            {
                endtime = value;
                this.RaisePropertyChanged("EndTime");
            }
        }

        //进区域报警给驾驶员
        private bool inalarmdriver;
        public bool InAlarmDriver
        {
            get { return inalarmdriver; }
            set
            {
                inalarmdriver = value;
                this.RaisePropertyChanged("InAlarmDriver");
                if (inalarmdriver == true)
                {
                    InAlarmFlag = "1";
                }
            }
        }

        //进区域报警给平台
        private bool inalarmplat;
        public bool InAlarmPlat
        {
            get { return inalarmplat; }
            set
            {
                inalarmplat = value;
                this.RaisePropertyChanged("InAlarmPlat");
                if (inalarmplat == true)
                {
                    InPlatformFlag = "1";
                }
            }
        }

        //出区域报警给驾驶员
        private bool outalarmdriver;
        public bool OutAlarmDriver
        {
            get { return outalarmdriver; }
            set
            {
                outalarmdriver = value;
                this.RaisePropertyChanged("OutAlarmDriver");
                if (outalarmdriver == true)
                {
                    OutAlamFlag = "1";
                }
            }
        }

        //出区域报警给平台
        private bool outalarmplat;
        public bool OutAlarmPlat
        {
            get { return outalarmplat; }
            set
            {
                outalarmplat = value;
                this.RaisePropertyChanged("OutAlarmPlat");
                if (outalarmplat == true)
                {
                    OutPlatformFlag = "1";
                }
            }
        }

        //纬度
        private bool latitude = true;
        public bool Latitude
        {
            get { return latitude; }
            set
            {
                latitude = value;
                this.RaisePropertyChanged("Latitude");
                if (latitude == false)
                {
                    latitudeFlag = "1";
                }
            }
        }

        //经度
        private bool longitude = true;
        public bool Longitude
        {
            get { return longitude; }
            set
            {
                longitude = value;
                this.RaisePropertyChanged("Longitude");
                if (longitude == false)
                {
                    longitudeFlag = "1";
                }
            }
        }

        //开门
        private bool opendoor = true;
        public bool openDoor
        {
            get { return opendoor; }
            set
            {
                opendoor = value;
                this.RaisePropertyChanged("openDoor");
                if (opendoor == false)
                {
                    doorFlag = "1";
                }
            }
        }

        //进区域开启通信模块
        private bool ingprs = true;
        public bool InGPRS
        {
            get { return ingprs; }
            set
            {
                ingprs = value;
                this.RaisePropertyChanged("InGPRS");
                if (ingprs == false)
                {
                    socketFlag = "1";
                }
            }
        }

        //进区域GNSS详细定位数据
        private bool ingnss = true;
        public bool InGNSS
        {
            get { return ingnss; }
            set
            {
                ingnss = value;
                this.RaisePropertyChanged("InGNSS");
                if (ingnss == false)
                {
                    GNSSFlag = "1";
                }
            }
        }

        //最高速度
        private string maxspeed;
        public string MaxSpeed
        {
            get { return maxspeed; }
            set
            {
                maxspeed = value;
                this.RaisePropertyChanged("MaxSpeed");
            }
        }

        //超速持续时间
        private string maxtime;
        public string MaxTime
        {
            get { return maxtime; }
            set
            {
                maxtime = value;
                this.RaisePropertyChanged("MaxTime");
            }
        }
        //区域ID
        private string regionid;
        public string  RegionID
        {
            get { return regionid; }
            set
            {
                regionid = value;
                this.RaisePropertyChanged("RegionID");
            }
        }
        #endregion

        #region 绑定操作
        public DelegateCommand GetGeoCommand { get; set; }//获取地理信息
        public DelegateCommand ClearMarkerCommand { get; set; }//清除地图覆盖物
        public DelegateCommand SendInstructionCommand { get; set; }//发送指令
        public DelegateCommand SetPointCommand { get; set; }//设置点属性
        private void SetPointCommandExecute()
        {
            Window win = new SetPointsView(PointsOfLine);
            win.ShowDialog();
        }

        private void GetGeoCommandExecute()
        {
            switch (this.InsType)
            {
                case "circle":
                    this.GetCircleInfo();
                    break;
                case "line":
                    this.GetLineInfo();
                    break;
                case "rect":
                    this.GetRectInfo();
                    break;
                case "poly":
                    this.GetPolyInfo();
                    break;
                default:
                    break;
            }
        }
        private void GetCircleInfo()
        {
            string circleString = MapService.GetCircle();
            if (!string.IsNullOrEmpty(circleString))
            {
                string[] circle = circleString.Split(';');
                this.CircleInfo = "半径：" + circle[0] + " 圆心经度：" + circle[1] + " 圆心纬度：" + circle[2] + " 面积：" + circle[3];
                radiusText = circle[0];
                longitudeText = circle[1];
                latitudeText = circle[2];
            }
        }
        private void GetLineInfo()
        {
            string lineString = MapService.GetLine();
            if (!string.IsNullOrEmpty(lineString))
            {
                string[] line = lineString.Split(';');
                this.LineInfo = "点个数：" + line[line.Length - 2] + " 长度：" + line[line.Length - 1];
                this.LinePointsInfo = "";
                for (int i = 0; i < line.Length - 2; i++)
                {
                    this.LinePointsInfo += line[i] + " ";
                    string[] pointinfo = line[i].Split(',');
                    PointOfLine point = new PointOfLine();
                    point.Lng = pointinfo[0];
                    point.Lat = pointinfo[1];
                    this.PointsOfLine.Add(point);
                }
                SetPoint = Visibility.Visible;
            }
        }
        private void GetRectInfo()
        {
            string rectString = MapService.GetRect();
            if (!string.IsNullOrEmpty(rectString))
            {
                string[] rect = rectString.Split(';');
                this.RectInfo = "面积" + rect[2] + " 左上角：" + rect[0] + " 右下角：" + rect[1];
                string[] leftArr = rect[0].Split(',');
                llatitudeText = leftArr[1];
                llongitudeText = leftArr[0];
                string[] rightArr = rect[1].Split(',');
                rlatitudeText = rightArr[1];
                rlongitudeText = rightArr[0];
            }
        }
        private void GetPolyInfo()
        {
            string polyString = MapService.GetPoly();
            if (!string.IsNullOrEmpty(polyString))
            {
                string[] poly = polyString.Split(';');
                this.PolyInfo = "面积" + poly[poly.Length - 1] + " 点个数：" + poly[poly.Length - 2];
                this.PolyPointInfo = "";
                for (int i = 0; i < poly.Length - 2; i++)
                {
                    this.PolyPointInfo += poly[i] + " ";
                    pointInfo pointItem = new pointInfo();
                    string[] pointArr = poly[i].Split(',');
                    pointItem.latitude = pointArr[1];
                    pointItem.longitude = pointArr[0];
                    pointList.Add(pointItem);
                }
            }
        }
        private void ClearMarkerCommandExecute()
        {
            MapService.RemoveAllMarkers();
            this.InitRegionInBaidu();
            MapService.OpenDrawTool();
        }

        private void SendInstructionCommandExecute()
        {
            if (IsBaseTime == true)
            {
                if (string.IsNullOrEmpty(this.BeginTime))
                {
                    MessageBox.Show("请选择开始时间！");
                    return;
                }
                if (string.IsNullOrEmpty(this.EndTime))
                {
                    MessageBox.Show("请选择结束时间！");
                    return;
                }
                DateTime begintime = DateTime.Parse(BeginTime);
                DateTime endtime = DateTime.Parse(EndTime);
                if (DateTime.Compare(begintime, endtime) > 0)
                {
                    MessageBox.Show("开始时间不能大于结束时间！");
                    return;
                }
            }
            if (IsLimitSpeed == true)
            {
                if (string.IsNullOrEmpty(this.MaxSpeed))
                {
                    MessageBox.Show("请输入最高速度！");
                    return;
                }
                if (string.IsNullOrEmpty(this.MaxTime))
                {
                    MessageBox.Show("请输入超速持续时间！");
                    return;
                }
            }
            try
            {
                if ((uint.Parse(RegionID) > 0 && uint.Parse(RegionID) < 4294967295) == false)
                {
                    MessageBox.Show("区域ID应为0到4294967295之间的正整数！");
                    return;
                }
            }
            catch (System.Exception ex)
            {
                MessageBox.Show("区域ID应为0到4294967295之间的正整数！");
                return;
            }

            switch (this.InsType)
            {
                case "circle":
                    if (!IsCircleID())
                    {
                        MessageBox.Show("该车辆已设置此区域ID");
                        return;
                    }
                    else
                    {
                        circleRegion circlereg = new circleRegion();
                        circlereg.id = RegionID;
                        circlereg.property = (int.Parse(timeFlag) * 1 + int.Parse(speedFlag) * 2 + int.Parse(InAlarmFlag) * 4
                            + int.Parse(InPlatformFlag) * 8 + int.Parse(OutAlamFlag) * 16 + int.Parse(OutPlatformFlag) * 32
                            + int.Parse(latitudeFlag) * 64 + int.Parse(longitudeFlag) * 128 + int.Parse(doorFlag) * 256
                            + int.Parse(socketFlag) * 16384 + int.Parse(GNSSFlag) * 32768).ToString();
                        if (string.IsNullOrEmpty(latitudeText))
                        {
                            MessageBox.Show("请先获取地理信息！");
                            return;
                        }
                        circlereg.latitude = latitudeText;
                        circlereg.longitude = longitudeText;
                        circlereg.radius = radiusText;
                        circlereg.start = BeginTime;
                        circlereg.end = EndTime;
                        circlereg.maxSpeed = MaxSpeed;
                        circlereg.duration = MaxTime;
                        regionList.Add(circlereg);
                        Result=Texttest(this.InsType);
                        if (Result == "指令已发出，正在处理！")
                        {
                            States = "已发送";
                            Socket.ExcuteSql("设置圆形区域", StaticLoginInfo.GetInstance().UserName, RegionID, Result, VBaseInfo.GetInstance().SIM);
                            Socket.ExcuteSqlRegion(int.Parse(RegionID), "circle", StaticLoginInfo.GetInstance().UserName, VBaseInfo.GetInstance().SIM);
                            CommandInfo cmd = new CommandInfo();
                            cmd.cmdSim = SIM.ToString();
                            cmd.VehicleNum = VehicleId.ToString();
                            cmd.cmdTime = DateTime.Now.TimeOfDay.ToString().Substring(0, 8);
                            cmd.cmdContent = "设置圆形区域" + ":" + RegionID;
                            cmd.SendStatus = Result.ToString();
                            while (true)
                            {
                                if (StaticTreeState.CmdStatus == LoadingState.LOADCOMPLETE)
                                {
                                    StaticTreeState.CmdStatus = LoadingState.LOADING;
                                    StaticMessageInfo.GetInstance().CmdList.Add(cmd);
                                    StaticTreeState.CmdStatus = LoadingState.LOADCOMPLETE;
                                    break;
                                }
                            }
                        }
                        break;
                    }
                case "line":
                    if (!IsLineID())
                    {
                        MessageBox.Show("该车辆已设置此区域ID");
                        return;
                    }
                    else
                    {
                        if (PointsOfLine.Count==0)
                        {
                            MessageBox.Show("请先获取地理信息！");
                            return;
                        }
                        Result = LineText();
                        if (Result == "指令已发出，正在处理！")
                        {
                            States = "已发送";
                            Socket.ExcuteSql("设置路线", StaticLoginInfo.GetInstance().UserName, RegionID, Result, VBaseInfo.GetInstance().SIM);
                            Socket.ExcuteSqlRegion(int.Parse(RegionID), "line", StaticLoginInfo.GetInstance().UserName, VBaseInfo.GetInstance().SIM);
                            CommandInfo cmd = new CommandInfo();
                            cmd.cmdSim = SIM.ToString();
                            cmd.VehicleNum = VehicleId.ToString();
                            cmd.cmdTime = DateTime.Now.TimeOfDay.ToString().Substring(0, 8);
                            cmd.cmdContent = "设置路线" + ":" + RegionID;
                            cmd.SendStatus = Result.ToString();
                            while (true)
                            {
                                if (StaticTreeState.CmdStatus == LoadingState.LOADCOMPLETE)
                                {
                                    StaticTreeState.CmdStatus = LoadingState.LOADING;
                                    StaticMessageInfo.GetInstance().CmdList.Add(cmd);
                                    StaticTreeState.CmdStatus = LoadingState.LOADCOMPLETE;
                                    break;
                                }
                            }
                        }
                        break;
                    }
                    
                case "rect":
                    if (!IsRectID())
                    {
                        MessageBox.Show("该车辆已设置此区域ID");
                        return;
                    }
                    else
                    {
                        rectRegion rectreg = new rectRegion();
                        rectreg.id = RegionID;
                        rectreg.property = (int.Parse(timeFlag) * 1 + int.Parse(speedFlag) * 2 + int.Parse(InAlarmFlag) * 4
                            + int.Parse(InPlatformFlag) * 8 + int.Parse(OutAlamFlag) * 16 + int.Parse(OutPlatformFlag) * 32
                            + int.Parse(latitudeFlag) * 64 + int.Parse(longitudeFlag) * 128 + int.Parse(doorFlag) * 256
                            + int.Parse(socketFlag) * 16384 + int.Parse(GNSSFlag) * 32768).ToString();
                        if (string.IsNullOrEmpty(llatitudeText))
                        {
                            MessageBox.Show("请先获取地理信息！");
                            return;
                        }
                        rectreg.llatitude = llatitudeText;
                        rectreg.llongitude = longitudeText;
                        rectreg.rlatitude = rlatitudeText;
                        rectreg.rlongitude = rlongitudeText;
                        rectreg.start = BeginTime;
                        rectreg.end = EndTime;
                        rectreg.maxSpeed = MaxSpeed;
                        rectreg.duration = MaxTime;
                        rectList.Add(rectreg);
                        Result=Texttest(this.InsType);
                        if (Result == "指令已发出，正在处理！")
                        {
                            States = "已发送";
                            Socket.ExcuteSql("设置矩形区域", StaticLoginInfo.GetInstance().UserName, RegionID, Result, VBaseInfo.GetInstance().SIM);
                            Socket.ExcuteSqlRegion(int.Parse(RegionID), "rect", StaticLoginInfo.GetInstance().UserName, VBaseInfo.GetInstance().SIM);
                            CommandInfo cmd = new CommandInfo();
                            cmd.cmdSim = SIM.ToString();
                            cmd.VehicleNum = VehicleId.ToString();
                            cmd.cmdTime = DateTime.Now.TimeOfDay.ToString().Substring(0, 8);
                            cmd.cmdContent = "设置矩形区域" + ":" + RegionID;
                            cmd.SendStatus = Result.ToString();
                            while (true)
                            {
                                if (StaticTreeState.CmdStatus == LoadingState.LOADCOMPLETE)
                                {
                                    StaticTreeState.CmdStatus = LoadingState.LOADING;
                                    StaticMessageInfo.GetInstance().CmdList.Add(cmd);
                                    StaticTreeState.CmdStatus = LoadingState.LOADCOMPLETE;
                                    break;
                                }
                            }
                        }
                        break;
                    }
                case "poly":
                    if (!IsPolyID())
                    {
                        MessageBox.Show("该车辆已设置此区域ID");
                        return;
                    }
                    else
                    {
                        if (pointList.Count == 0)
                        {
                            MessageBox.Show("请先获取地理信息！");
                            return;
                        }
                        Result=PolyText();
                        if (Result == "指令已发出，正在处理！")
                        {
                            States = "已发送";
                            Socket.ExcuteSql("设置多边形区域", StaticLoginInfo.GetInstance().UserName, RegionID, Result, VBaseInfo.GetInstance().SIM);
                            Socket.ExcuteSqlRegion(int.Parse(RegionID), "poly", StaticLoginInfo.GetInstance().UserName, VBaseInfo.GetInstance().SIM);
                            CommandInfo cmd = new CommandInfo();
                            cmd.cmdSim = SIM.ToString();
                            cmd.VehicleNum = VehicleId.ToString();
                            cmd.cmdTime = DateTime.Now.TimeOfDay.ToString().Substring(0, 8);
                            cmd.cmdContent = "设置多边形区域" + ":" + RegionID;
                            cmd.SendStatus = Result.ToString();
                            while (true)
                            {
                                if (StaticTreeState.CmdStatus == LoadingState.LOADCOMPLETE)
                                {
                                    StaticTreeState.CmdStatus = LoadingState.LOADING;
                                    StaticMessageInfo.GetInstance().CmdList.Add(cmd);
                                    StaticTreeState.CmdStatus = LoadingState.LOADCOMPLETE;
                                    break;
                                }
                            }
                        }
                        break;
                    }
                default:
                    break;
            }
        }

        //判断ID是否存在
        bool IsCircleID()//圆形
        {
            string sql = "select id from RegionIDInfo where type='circle' and sim='"
                + VBaseInfo.GetInstance().SIM + "'";
            string jsonStr = VehicleCommon.wcfDBHelper.BexcuteSql(StaticLoginInfo.GetInstance().UserName, sql);
            if (string.Compare(jsonStr, "error") != 0)
            {
                DataTable dt = new DataTable();
                dt = JsonHelper.JsonToDataTable(jsonStr);
                int num = 0;
                if (dt != null)
                {
                    num = dt.Rows.Count;
                }
                for (int i = 0; i < num; i++)
                {
                    string id = dt.Rows[i]["id"].ToString();
                    if (RegionID==id)
                    {
                        return false;
                    }
                }
                return true;
            }
            else
            {
                MessageBox.Show("数据库操作失败！");
                return false;
            }
        }
        bool IsRectID()//矩形
        {
            string sql = "select id from RegionIDInfo where type='rect' and sim='"
                + VBaseInfo.GetInstance().SIM + "'";
            string jsonStr = VehicleCommon.wcfDBHelper.BexcuteSql(StaticLoginInfo.GetInstance().UserName, sql);
            if (string.Compare(jsonStr, "error") != 0)
            {
                DataTable dt = new DataTable();
                dt = JsonHelper.JsonToDataTable(jsonStr);
                int num = 0;
                if (dt != null)
                {
                    num = dt.Rows.Count;
                }
                for (int i = 0; i < num; i++)
                {
                    if (RegionID == dt.Rows[i]["id"].ToString())
                    {
                        return false;
                    }
                }
                return true;
            }
            else
            {
                MessageBox.Show("数据库操作失败！");
                return false;
            }
        }
        bool IsPolyID()//多边形
        {
            string sql = "select id from RegionIDInfo where type='poly' and sim='"
                + VBaseInfo.GetInstance().SIM + "'";
            string jsonStr = VehicleCommon.wcfDBHelper.BexcuteSql(StaticLoginInfo.GetInstance().UserName, sql);
            if (string.Compare(jsonStr, "error") != 0)
            {
                DataTable dt = new DataTable();
                dt = JsonHelper.JsonToDataTable(jsonStr);
                int num = 0;
                if (dt != null)
                {
                    num = dt.Rows.Count;
                }
                for (int i = 0; i < num; i++)
                {
                    if (RegionID == dt.Rows[i]["id"].ToString())
                    {
                        return false;
                    }
                }
                return true;
            }
            else
            {
                MessageBox.Show("数据库操作失败！");
                return false;
            }
        }
        bool IsLineID()//线路
        {
            string sql = "select id from RegionIDInfo where type='line' and sim='"
                + VBaseInfo.GetInstance().SIM + "'";
            string jsonStr = VehicleCommon.wcfDBHelper.BexcuteSql(StaticLoginInfo.GetInstance().UserName, sql);
            if (string.Compare(jsonStr, "error") != 0)
            {
                DataTable dt = new DataTable();
                dt = JsonHelper.JsonToDataTable(jsonStr);
                int num = 0;
                if (dt != null)
                {
                    num = dt.Rows.Count;
                }
                for (int i = 0; i < num; i++)
                {
                    if (RegionID == dt.Rows[i]["id"].ToString())
                    {
                        return false;
                    }
                }
                return true;
            }
            else
            {
                MessageBox.Show("数据库操作失败！");
                return false;
            }
        }
        #endregion

        private List<PointOfLine> pointsofline=new List<PointOfLine>();

        public List<PointOfLine> PointsOfLine
        {
            get { return pointsofline; }
            set
            {
                pointsofline = value;
                this.RaisePropertyChanged("PointsOfLine");
            }
        }


        #region 区域属性类
        //public class PointOfLine : NotificationObject//点
        //{
            
        //}
        public class circleRegion //圆形
        {
            public string id { get; set; }
            public string property { get; set; }
            public string latitude { get; set; }
            public string longitude { get; set; }
            public string radius { get; set; }
            public string start { get; set; }
            public string end { get; set; }
            public string maxSpeed { get; set; }
            public string duration { get; set; }
        }
        List<circleRegion> regionList = new List<circleRegion>();

        public class rectRegion //矩形
        {
            public string id { get; set; }
            public string property { get; set; }
            public string llatitude { get; set; }
            public string llongitude { get; set; }
            public string rlatitude { get; set; }
            public string rlongitude { get; set; }
            public string start { get; set; }
            public string end { get; set; }
            public string maxSpeed { get; set; }
            public string duration { get; set; }
        }
        public class cornerPoint
        {
            public string cornerId { get; set; }
            public string lineId { get; set; }
            public string latitude { get; set; }
            public string longitude { get; set; }
            public string width { get; set; }
            public string lineProperty { get; set; }
            public string overThreshold { get; set; }
            public string belowThreshold { get; set; }
            public string maxSpeed { get; set; }
            public string duration { get; set; }
        }
        List<rectRegion> rectList = new List<rectRegion>();

        public class pointInfo //多边形顶点
        {
            public string latitude;
            public string longitude;
        }
        List<pointInfo> pointList = new List<pointInfo>();

        private string Texttest(string InsType) //圆形，矩形
        {
            string rebool = "";
            JObject jo = new JObject();
            if (InsType == "circle")
            {
                jo.Add("cmd", "SETCIRCLEREGIONCMD_TYPE");
                jo.Add("cmdid", SIM + "_SETCIRCLEREGIONCMD_TYPE");
            }
            else
            {
                jo.Add("cmd", "SETRECTREGIONCMD_TYPE");
                jo.Add("cmdid", SIM + "_SETRECTREGIONCMD_TYPE");
            }
            jo.Add("simId", SIM);
            jo.Add("type", setType);
            jo.Add("regionList", JsonConvert.SerializeObject(regionList));
            rebool = Socket.zmqInstructionsPack(SIM, jo);
            return rebool;
        }

        private string PolyText()
        {
            string strProperty = (int.Parse(timeFlag) * 1 + int.Parse(speedFlag) * 2 + int.Parse(InAlarmFlag) * 4
                        + int.Parse(InPlatformFlag) * 8 + int.Parse(OutAlamFlag) * 16 + int.Parse(OutPlatformFlag) * 32
                        + int.Parse(latitudeFlag) * 64 + int.Parse(longitudeFlag) * 128 + int.Parse(doorFlag) * 256
                        + int.Parse(socketFlag) * 16384 + int.Parse(GNSSFlag) * 32768).ToString();
            string rebool = "";
            JObject jo = new JObject();
            jo.Add("cmd", "SETPOLYGONREGIONCMD_TYPE");
            jo.Add("simId", SIM);
            jo.Add("id", RegionID);
            jo.Add("property", strProperty);
            jo.Add("start", BeginTime);
            jo.Add("end", EndTime);
            jo.Add("maxSpeed", MaxSpeed);
            jo.Add("duration", MaxTime);
            jo.Add("pointList", JsonConvert.SerializeObject(pointList));
            jo.Add("cmdid", SIM + "_SETPOLYGONREGIONCMD_TYPE");
            rebool = Socket.zmqInstructionsPack(SIM, jo);
            return rebool;
        }

        private string LineText()
        {
            string strProperty = (int.Parse(timeFlag) * 1  + int.Parse(InAlarmFlag) * 4
                        + int.Parse(InPlatformFlag) * 8 + int.Parse(OutAlamFlag) * 16 + int.Parse(OutPlatformFlag) * 32).ToString();
            string rebool = "";
            JObject jo = new JObject();
            jo.Add("cmd", "SETLINECMD_TYPE");
            jo.Add("simId", SIM);
            jo.Add("id", RegionID);
            jo.Add("property", strProperty);
            jo.Add("start", BeginTime);
            jo.Add("end", EndTime);
            jo.Add("cmdid", SIM + "_SETLINECMD_TYPE");
            List<cornerPoint> cornerList = new List<cornerPoint>();
            foreach (PointOfLine item in PointsOfLine)
            {
                cornerPoint cp = new cornerPoint();
                int drivetimeFlag = (item.IsSelectedTime == true) ? 1 : 0;
                int speedlimitFlag = (item.IsSelectedSpeed == true) ? 1 : 0;
                string strlineproperty = (drivetimeFlag * 1 + speedlimitFlag * 2+ item.NSLatSelectedIndex * 4
                        + item.NSLngSelectedIndex * 8 ).ToString();
                cp.cornerId = item.InflectinID;
                cp.belowThreshold = item.LessLimit;
                cp.duration = item.HighTime;
                cp.latitude = item.Lat;
                cp.lineId = item.RoadID;
                cp.lineProperty = strlineproperty;
                cp.longitude = item.Lng;
                cp.maxSpeed = item.HighSpeed;
                cp.overThreshold = item.LongLimit;
                cp.width = item.RoadWidth;
                cornerList.Add(cp);
            }
            jo.Add("cornerList", JsonConvert.SerializeObject(cornerList));
            rebool = Socket.zmqInstructionsPack(SIM, jo);
            return rebool;
        }
        #endregion

        #region 百度地图
        public WebBrowser WebMap { get; set; }
        public InstructionMapService MapService { get; set; }


        #endregion
    }
}
