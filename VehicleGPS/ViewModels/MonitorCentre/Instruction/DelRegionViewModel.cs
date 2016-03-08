using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.Prism.ViewModel;
using System.Windows;
using VehicleGPS.Models;
using VehicleGPS.Models.Login;
using Newtonsoft.Json;
using System.Data;
using VehicleGPS.Services;
using Microsoft.Practices.Prism.Commands;
using VehicleGPS.Models.MonitorCentre;
using Newtonsoft.Json.Linq;
using VehicleGPS.Views.Control.MonitorCentre.Instruction;

namespace VehicleGPS.ViewModels.MonitorCentre.Instruction
{
    class DelRegionViewModel: NotificationObject
    {
        public string InsType = "0";//删除区域类型
        List<string> idList = new List<string>();
        public DelRegionViewModel()
        {
            this.SendInstructionCommand = new DelegateCommand(this.SendInstructionCommandExecute);
            InitVehicleInfo();//初始化车辆信息
            InitCircleList();//初始化圆形区域ID
            InitRectList();//初始化矩形ID
            InitPolyList();//初始化多边形ID
            InitLineList();//初始化路线ID
        }

        #region 绑定数据
        private bool isradiocircle=true;

        public bool IsRadioCircle
        {
            get { return isradiocircle; }
            set
            {
                isradiocircle = value;
                this.RaisePropertyChanged("IsRadioCircle");
                if (IsRadioCircle==true)
                {
                    this.VisibleCircle = Visibility.Visible;
                    this.VisibleRect = Visibility.Collapsed;
                    this.VisiblePoly = Visibility.Collapsed;
                    this.VisibleLine = Visibility.Collapsed;
                    InsType = "0";
                }
            }
        }
        private bool isradiorec=false;

        public bool IsRadioRec
        {
            get { return isradiorec; }
            set
            {
                isradiorec = value;
                this.RaisePropertyChanged("IsRadioRec");
                if (IsRadioRec == true)
                {
                    this.VisibleCircle = Visibility.Collapsed;
                    this.VisibleRect = Visibility.Visible;
                    this.VisiblePoly = Visibility.Collapsed;
                    this.VisibleLine = Visibility.Collapsed;
                    InsType = "1";
                }
            }
        }
        private bool isradiopolygon = false;

        public bool IsRadioPolygon
        {
            get { return isradiopolygon; }
            set
            {
                isradiopolygon = value;
                this.RaisePropertyChanged("IsRadioPolygon");
                if (IsRadioPolygon == true)
                {
                    this.VisibleCircle = Visibility.Collapsed;
                    this.VisibleRect = Visibility.Collapsed;
                    this.VisiblePoly = Visibility.Visible;
                    this.VisibleLine = Visibility.Collapsed;
                    InsType = "2";
                }
            }
        }
        private bool isradioline = false;

        public bool IsRadioLine
        {
            get { return isradioline; }
            set
            {
                isradioline = value;
                this.RaisePropertyChanged("IsRadioLine");
                if (IsRadioLine == true)
                {
                    this.VisibleCircle = Visibility.Collapsed;
                    this.VisibleRect = Visibility.Collapsed;
                    this.VisiblePoly = Visibility.Collapsed;
                    this.VisibleLine = Visibility.Visible;
                    InsType = "3";
                }
            }
        }
        private Visibility visiblecircle=Visibility.Visible;

        public Visibility VisibleCircle
        {
            get { return visiblecircle; }
            set
            {
                visiblecircle = value;
                this.RaisePropertyChanged("VisibleCircle");
            }
        }
        private Visibility visiblerect = Visibility.Collapsed;

        public Visibility VisibleRect
        {
            get { return visiblerect; }
            set
            {
                visiblerect = value;
                this.RaisePropertyChanged("VisibleRect");
            }
        }
        private Visibility visiblepoly = Visibility.Collapsed;

        public Visibility VisiblePoly
        {
            get { return visiblepoly; }
            set
            {
                visiblepoly = value;
                this.RaisePropertyChanged("VisiblePoly");
            }
        }
        private Visibility visibleline=Visibility.Collapsed;

        public Visibility VisibleLine
        {
            get { return visibleline; }
            set
            {
                visibleline = value;
                this.RaisePropertyChanged("VisibleLine");
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

        #region 绑定操作
        public DelegateCommand SendInstructionCommand { get; set; }//发送指令

        private void SendInstructionCommandExecute()
        {
            string regionType="circle";
            switch (this.InsType)
            {
                case "0":
                    foreach(circleInfo item in CircleList)
                    {
                        if (item.IsCheckedCircle==true)
                        {
                            string circleId = item.CircleID;
                            idList.Add(circleId);
                        }  
                    }
                    break;
                case "1":
                    foreach (rectInfo item in RectList)
                    {
                        if (item.IsCheckedRect == true)
                        {
                            string rectId = item.RectID;
                            idList.Add(rectId);
                        }
                    }
                    regionType="rect";
                    break;
                case "2":
                    foreach (polyInfo item in PolyList)
                    {
                        if (item.IsCheckedPoly == true)
                        {
                            string polyId = item.PolyID;
                            idList.Add(polyId);
                        }
                    }
                    regionType="poly";
                    break;
                case "3":
                    foreach (lineInfo item in LineList)
                    {
                        if (item.IsCheckedLine == true)
                        {
                            string lineId = item.LineID;
                            idList.Add(lineId);
                        }
                    }
                    regionType="line";
                    break;
                default:
                    break;
            }
            Result = Texttest();
            if (Result == "指令已发出，正在处理！")
            {
                States = "已发送";
                Socket.ExcuteSql("删除区域或路线", StaticLoginInfo.GetInstance().UserName, JsonConvert.SerializeObject(idList), Result, VBaseInfo.GetInstance().SIM);
                Socket.ExcuteSqlDelRegion(idList,regionType,VBaseInfo.GetInstance().SIM);
                CommandInfo cmd = new CommandInfo();
                cmd.cmdId = SIM + "_DELREGIONLINECMD_TYPE";
                cmd.cmdContent = "删除区域或路线" + ":" + JsonConvert.SerializeObject(idList);
                cmd.cmdSim = SIM.ToString();
                cmd.cmdTime = DateTime.Now.TimeOfDay.ToString().Substring(0, 8);
                cmd.SendStatus = Result.ToString();
                cmd.VehicleNum = VehicleId.ToString();
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
        }

        private string Texttest() 
        {
            string rebool = "";
            JObject jo = new JObject();
            jo.Add("cmd", "DELREGIONLINECMD_TYPE");
            jo.Add("simId", SIM);
            jo.Add("type", InsType);
            jo.Add("idList", JsonConvert.SerializeObject(idList));
            jo.Add("cmdid", SIM + "_DELREGIONLINECMD_TYPE");
            rebool = Socket.zmqInstructionsPack(SIM, jo);
            return rebool;
        }
        #endregion

        #region 类声明
        public class circleInfo: NotificationObject
        {
            public string  CircleID { get; set; }
            private bool ischeckedcircle;
            public bool IsCheckedCircle
            {
                get { return ischeckedcircle; }
                set
                {
                    this.ischeckedcircle = value;
                    this.RaisePropertyChanged("IsCheckedCircle");
                }
            }
        }

        public class rectInfo : NotificationObject
        {
            public string RectID { get; set; }
            private bool ischeckedrect;
            public bool IsCheckedRect
            {
                get { return ischeckedrect; }
                set
                {
                    this.ischeckedrect = value;
                    this.RaisePropertyChanged("IsCheckedRect");
                }
            }
        }

        public class polyInfo : NotificationObject
        {
            public string PolyID { get; set; }
            private bool ischeckedpoly;
            public bool IsCheckedPoly
            {
                get { return ischeckedpoly; }
                set
                {
                    this.ischeckedpoly = value;
                    this.RaisePropertyChanged("IsCheckedPoly");
                }
            }
        }

        public class lineInfo : NotificationObject
        {
            public string LineID { get; set; }
            private bool ischeckedline;
            public bool IsCheckedLine
            {
                get { return ischeckedline; }
                set
                {
                    this.ischeckedline = value;
                    this.RaisePropertyChanged("IsCheckedLine");
                }
            }
        }

        private List<circleInfo> circleList = new List<circleInfo>();
        public List<circleInfo> CircleList
        {
            get { return circleList; }
            set
            {
                circleList = value;
                this.RaisePropertyChanged("CircleList");
            }
        }
        private List<rectInfo> rectList = new List<rectInfo>();
        public List<rectInfo> RectList
        {
            get { return rectList; }
            set
            {
                rectList = value;
                this.RaisePropertyChanged("RectList");
            }
        }
        private List<polyInfo> polyList = new List<polyInfo>();
        public List<polyInfo> PolyList
        {
            get { return polyList; }
            set
            {
                polyList = value;
                this.RaisePropertyChanged("PolyList");
            }
        }
        private List<lineInfo> lineList = new List<lineInfo>();
        public List<lineInfo> LineList
        {
            get { return lineList; }
            set
            {
                lineList = value;
                this.RaisePropertyChanged("LineList");
            }
        }
        #endregion

        #region 初始化表格
        void InitCircleList()//圆形
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
                    circleInfo cirInfo = new circleInfo();
                    cirInfo.CircleID = dt.Rows[i]["id"].ToString();
                    CircleList.Add(cirInfo);
                }
            }
        }
        void InitRectList()//矩形
        {
            string sql = "select id from RegionIDInfo where type='rect' and sim='"
                + VBaseInfo.GetInstance().SIM + "'";
            string jsonStr = VehicleCommon.wcfDBHelper.BexcuteSql(StaticLoginInfo.GetInstance().UserName, sql);
            if (string.Compare(jsonStr, "error") != 0)
            {
                DataTable dt = new DataTable();
                dt = JsonHelper.JsonToDataTable(jsonStr);
                int num = 0;
                if (dt!=null)
                {
                    num = dt.Rows.Count;
                }
                for (int i = 0; i < num; i++)
                {
                    rectInfo rectInfo = new rectInfo();
                    rectInfo.RectID = dt.Rows[i]["id"].ToString();
                    RectList.Add(rectInfo);
                }
            }
        }
        void InitPolyList()//多边形
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
                    polyInfo polyInfo = new polyInfo();
                    polyInfo.PolyID = dt.Rows[i]["id"].ToString();
                    PolyList.Add(polyInfo);
                }
            }
        }
        void InitLineList()//线路
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
                    lineInfo lineInfo = new lineInfo();
                    lineInfo.LineID = dt.Rows[i]["id"].ToString();
                    LineList.Add(lineInfo);
                }
            }
        }
        #endregion
    }
}
